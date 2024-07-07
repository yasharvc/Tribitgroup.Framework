using Test.API.Authentication;
using Test.API.Authentication.Builder.Contracts;
using Test.API.Authentication.Contracts;
using Test.API.Authentication.Extensions;
using Test.API.Test;

namespace Test.API
{
    public class ScopedClass
    {
        private readonly ScoppedClassCounter classCounter;
        public string MyStat { get; set; } = string.Empty;

        public ScopedClass(ScoppedClassCounter classCounter)
        {
            this.classCounter = classCounter;
            classCounter.Next();
        }
        public void Print()
        {
            //Write current datetime in console
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(DateTime.Now);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(classCounter.GetCount());
            Console.WriteLine($"{MyStat} is mystat");
        }
    }

    public class ScoppedClassCounter
    {
        private int _counter = 0;
        public ScoppedClassCounter()
        {
            _counter++;
        }
        public void Next()
        {
            _counter++;
        }
        public int GetCount()
        {
            return _counter;
        }
    }
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            AddAuth(builder);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ScopedClass>();
            builder.Services.AddScoped<ScoppedClassCounter>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();

            app.Use((context, next) =>
            {
                //Get IServiceScopeFactory 
                var scopeFactory = context.RequestServices.GetRequiredService<IServiceProvider>();
                //Create a new scope
                //using var scope = scopeFactory.CreateScope();
                //var scopedClass = scope.ServiceProvider.GetRequiredService<ScopedClass>();
                var scopedClass = context.RequestServices.GetRequiredService<ScopedClass>();
                scopedClass.MyStat = "Inside app.Use";
                scopedClass.Print();
                return next();
            });

            app.MapPost("/", () =>
            {
                return new { controller = "Asda" };
            });

            app.MapControllers();

            app.Run();
        }

        

        private static void AddAuth(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<HttpCommandSetter>();
            builder.Services.AddScoped<DeviceTypeSetter>();
            builder.Services.AddScoped<TokenSetter>();
            builder.Services.AddScoped<IPSetter>();
            builder.Services.AddScoped<TokenHasher>();
        }
    }
}
