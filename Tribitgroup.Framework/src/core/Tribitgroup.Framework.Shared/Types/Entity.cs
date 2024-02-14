using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Interfaces.Entity.Validation;

namespace Tribitgroup.Framework.Shared.Types
{
    public class Entity : Entity<Guid>
    {
        public Entity()
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }
    }
    public class Entity<T> : IEntity<T> where T : notnull
    {
        static IDictionary<Type,string> TableNames { get; set; } = new Dictionary<Type,string>();
        static IDictionary<Type, bool> IsFromDbContext { get; set; } = new Dictionary<Type,bool>();
        public T Id { get; set; }

        public IEnumerable<string> GetColumnNames()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsBasicType());

            return properties.Select(p => p.Name);
        }

        public object? GetValue(string propName)
        {
            var prop = GetType().GetProperties().Where(p => p.Name == propName).SingleOrDefault();
            if(prop == null)
                throw new EntryPointNotFoundException();
            return prop.GetValue(this);
        }

        public string GetTableName(DbContext? context = null)
        {
            var type = GetType();

            var flag = IsFromDbContext.ContainsKey(type) && !IsFromDbContext[type] && context != null;

            if (TableNames.TryGetValue(type, out string? value) && !flag)
                return value;
            dynamic? tableAttr = GetType().GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == nameof(TableAttribute));

            if (context != null)
            {
                var dbSetProperties = context.GetType()
                    .GetProperties()
                    .Where(prop => prop.PropertyType.IsGenericType &&
                                   prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                   type == prop.PropertyType.GenericTypeArguments.First());

                var dbSetProperty = dbSetProperties.SingleOrDefault() ?? throw new EntryPointNotFoundException();
                var res = TableNames[type] = tableAttr is null ? dbSetProperty.Name : tableAttr.Name;
                IsFromDbContext[type] = true;
                return res;
            }
            else
            {
                var res = TableNames[type] = tableAttr is null ? GetType().Name : tableAttr.Name;
                IsFromDbContext[type] = false;
                return res;
            }
        }

        public async Task ValidateAsync()
        {
            var entityType = GetType();
            Type interfaceType = typeof(IValidator);
            Type hasValidatorType = typeof(IHasValidator);
            var validatorInterfaces = entityType.GetInterfaces();
            var lst = validatorInterfaces.Where(m => hasValidatorType.IsAssignableFrom(m) && m != hasValidatorType && m != entityType).ToList();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);


            foreach (var item in lst)
            {
                var interfaceProps = item.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                foreach (var prop in interfaceProps)
                {
                    var allAttrs = prop.GetCustomAttributes().Where(attr =>
                    attr.GetType().GetInterfaces().Select(m => m.Name).Contains(typeof(IValidator<>).Name)
                    ).Select(attr => attr as object);

                    foreach (var attr in allAttrs)
                    {
                        var method = attr.GetType().GetMethod(nameof(IValidator<string>.ValidateAsync));
                        try
                        {
                            if (method?.Invoke(attr, new object[] { this }) is Task task)
                            {
                                await task;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex?.InnerException ?? throw ex ?? throw new Exception();
                        }
                    }
                }

            }
        }
    }
}