using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Numpy;
using Python.Included;
using Python.Runtime;
using Tribitgroup.Framework.Shared.Interfaces.Entity.Validation;
using Tribitgroup.Framework.Shared.Services;
using Tribitgroup.Framework.Shared.Types;

namespace ConsoleTestApp
{
    interface IFullName : IHasValidator
    {
        [FullNameValidator]
        [MaxLengthOf(50)]
        string FullName { get; }
    }
    class MaxLengthOfAttribute : Attribute, IValidator<IFullName>
    {
        int maxLength = 50;
        public MaxLengthOfAttribute(int max)
        {
            maxLength = max;
        }
        public Task ValidateAsync(IFullName fullName)
        {
            var value = fullName.FullName;
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(IFullName.FullName));
            if (value.Length > maxLength) throw new ArgumentOutOfRangeException(nameof(IFullName.FullName));
            return Task.CompletedTask;
        }
    }

    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    class FullNameValidatorAttribute : Attribute, IValidator<IFullName>
    {
        public Task ValidateAsync(IFullName fullName)
        {
            var value = fullName.FullName;
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(IFullName.FullName));
            return Task.CompletedTask;
        }
    }


    public class TestEntity : Entity, IFullName
        {
            public string FullName { get; set; } = string.Empty;
        }
    internal class Program
    {
        
        static async Task Main(string[] args)
        {
            var entityType = typeof(TestEntity);
            var ff = new TestEntity { FullName = new string('a', 200) };

            Type interfaceType = typeof(IValidator);
            Type hasValidatorType = typeof(IHasValidator);
            var validatorInterfaces = entityType.GetInterfaces();
            var lst = validatorInterfaces.Where(m=>hasValidatorType.IsAssignableFrom(m) && m != hasValidatorType && m != entityType).ToList();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);


            //foreach ( var item in lst) 
            //{
            //    var interfaceProps = item.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            //    foreach (var prop in interfaceProps)
            //    {
            //        var allAttrs = prop.GetCustomAttributes().Where(attr =>
            //        attr.GetType().GetInterfaces().Select(m => m.Name).Contains(typeof(IValidator<>).Name)
            //        ).Select(attr => attr as dynamic);

            //        foreach (var attr in allAttrs)
            //        {
            //            if(attr != null)
            //            await attr?.ValidateAsync(ff);
            //        }
            //    }
                        
            //}
            await ff.ValidateAsync();

            

            //if (req != null)
            //{
            //    await req.ValidateAsync(ff.FullName);
            //    await Console.Out.WriteLineAsync(prop.GetValue(ff)?.ToString() ?? "Error!");
            //}
            await InMemoryCacheTestAsync();
        }

        private static async Task RunCSharpAtRunTimeAsync()
        {
            var res = CSharpCodeRunner.RunLibraryCodeFromFile(await File.ReadAllTextAsync("D:/1.cs"), "LibClass", "LibMethod");
            if (res != null && res.GetType() == typeof(string))
                await Console.Out.WriteLineAsync(res as string);
        }

        private static async Task InMemoryCacheTestAsync()
        {
            var cache = new InMemoryEntityCache<TestEntity>(1024);

            var test1 = new TestEntity { FullName = "a" };
            var test2 = new TestEntity { FullName = "b" };

            await cache.AddOrUpdateAsync(test1, test2);

            test1.FullName = "Yashar";

            await cache.AddOrUpdateAsync(test1, TimeSpan.FromSeconds(3));
            var x = await cache.GetAsync(test1.Id);
            await Console.Out.WriteLineAsync(x.FullName);
            await Task.Delay(TimeSpan.FromSeconds(5));

            x = await cache.GetAsync(test1.Id);
            await Console.Out.WriteLineAsync(x?.FullName ?? "NULL");

            cache = new InMemoryEntityCache<TestEntity>(1024);
            x = await cache.GetAsync(test2.Id);

            await Console.Out.WriteLineAsync(x?.FullName ?? "NULL");
        }

        private async static Task Method1()
        {
            await Installer.SetupPython();
            await Installer.TryInstallPip();
            await Installer.PipInstallModule("spacy");
            PythonEngine.Initialize();
            dynamic spacy = Py.Import("spacy");
            Console.WriteLine("Spacy version: " + spacy.__version__);
            var a = np.arange(1000);
            var b = np.arange(1000);

            // https://github.com/pythonnet/pythonnet/issues/109
            PythonEngine.BeginAllowThreads();

            Task.Run(() => {
                // when running on different threads you must lock!
                using (Py.GIL())
                {
                    np.matmul(a, b);
                }
            }).Wait();
        }
    }
}