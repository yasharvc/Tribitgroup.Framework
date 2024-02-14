using Test.API.Authentication.Contracts;

namespace Test.API.Test
{
    public class TokenSetter : HttpPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
    {
        public override async Task<HttpClient<Tenant, Policy, Role, Permission>> ExecuteAsync(HttpClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration)
        {
            client.ClientToken = await GetHeaderValueAsync(client.HttpContext, "Authorization");
            client.ApplicationToken = await GetHeaderValueAsync(client.HttpContext, "ApplicationAuthorization");
            return client;
        }
    }
}
