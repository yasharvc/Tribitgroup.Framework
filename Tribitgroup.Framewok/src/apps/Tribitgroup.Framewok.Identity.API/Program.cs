
using Tribitgroup.Framewok.Shared.Interfaces;

namespace Tribitgroup.Framewok.Identity.API
{
    public class Program
    {

        //Add Middlewarefor dumming roles
        //Add authorized route
        //Add 
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IScheduler>(sp => new TaskDelayScheduler(sp));

            builder.InjectIdentityDependencies();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthenticationAndAutherization();

            app.Use(async (ctx, next) =>
            {
                await Console.Out.WriteLineAsync(ctx.User.Claims.First(m=>m.Type == "test").Value);
                await next(ctx);
            });

            app.MapControllers();

            app.Run();
        }
    }
}