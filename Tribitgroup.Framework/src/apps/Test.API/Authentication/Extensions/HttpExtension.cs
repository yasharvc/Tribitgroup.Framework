using Test.API.Authentication.Builder.Contracts;
using Test.API.Authentication.Contracts;
using Test.API.Test;

namespace Test.API.Authentication.Extensions
{
    public static class HttpExtension
    {
        public static void UseAuthentication(this WebApplication app)
        {
            app.Use(async (ctx, next) => {
                var client = new HttpClient<Tenant, Policy, Role, Permission>
                {
                    HttpContext = ctx
                };
                try
                {
                    await(await AddAuthForApp(app)).ProcessAsync(client);
                }
                catch (Exception ex)
                {
                    ctx.Response.StatusCode = 401;
                    return;
                }
                await next(ctx);
            });
        }
        private static async Task<Authenticator<Tenant, Policy, Role, Permission>> AddAuthForApp(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var builder = new AuthenticatorBuilder<Tenant, Policy, Role, Permission>(scope.ServiceProvider);
            
            builder.AddPreauthenticateStep<HttpCommandSetter>()
            .AddPreauthenticateStep<DeviceTypeSetter>()
            .AddPreauthenticateStep<TokenSetter>()
            .AddPreauthenticateStep<IPSetter>()
            .AddPreauthenticateStep<TokenHasher>();

            return await builder.BuildAsync();
        }
    }
}
