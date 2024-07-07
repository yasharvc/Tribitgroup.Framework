using NextGen.Backbone.Backbone;
using NextGen.Backbone.Backbone.Contracts;
using NextGen.Backbone.ServiceProvider.Contracts;

namespace NextGen.Backbone.ServiceProvider
{
    public static class BackboneProviderBuilder
    {
        public static IServiceCollection AddBackboneProvider<T>(this IServiceCollection services) where T : class, IApplicationUser
        {
            services.AddSingleton<ApplicationModeProvider>();
            services.AddHttpContextAccessor();
            services.AddScoped<IBackboneProvider>(sp =>
            {
                var ctx = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
                var serviceProvider = ctx.RequestServices;
                var res = new BackboneProvider
                (
                    //ctx.RequestServices.GetService<T>(),
                    //sp.CreateScope().ServiceProvider.GetRequiredService<T>(),
                    ctx.RequestServices.GetRequiredService<T>(),
                    serviceProvider,
                    serviceProvider.GetRequiredService<ApplicationModeProvider>()
                    //sp.GetRequiredService<ApplicationModeProvider>()
                );

                return res;
            });
            return services;
        }

        public static WebApplication UseBackbone(this WebApplication app)
        {
            app.MapGet("/backbone", async (IBackboneProvider backbone) =>
            {
                await backbone.OnApplicationModeChanged(ApplicationModeEnum.Stopping);
                return Results.Ok();
            });

            return app;
        }
    }
}
