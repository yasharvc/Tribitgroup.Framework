

using Test.API.Authentication;
using Test.API.Authentication.Builder.Contracts;
using Test.API.Authentication.Contracts;
using Test.API.Authentication.Extensions;
using Test.API.Test;

namespace Test.API
{
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();

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
