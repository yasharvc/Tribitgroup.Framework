

using Test.API.Authentication.Contracts;

namespace Test.API.Test
{
    public abstract class HttpPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
        : IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        public async Task<IClient<Tenant, Policy, Role, Permission>> ExecuteAsync(IClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration) => await ExecuteAsync((HttpClient<Tenant, Policy, Role, Permission>)client, configuration);

        protected Task<string> GetHeaderValueAsync(HttpContext httpContext, string key) => Task.FromResult(httpContext.Request.Headers[key].FirstOrDefault() ?? "");
        protected Task<IEnumerable<string>> GetHeaderValuesAsync(HttpContext httpContext, string key) => Task.FromResult(httpContext.Request.Headers[key].AsEnumerable<string>() ?? new List<string>());
        public abstract Task<HttpClient<Tenant, Policy, Role, Permission>> ExecuteAsync(HttpClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration);
    }
}
