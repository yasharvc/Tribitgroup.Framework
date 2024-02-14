namespace Test.API.Authentication.Contracts
{
    public interface IAuthenticatePipeStep<Tenant, Policy, Role, Permission>
        where Role : IRole
        where Permission : IPermission
        where Tenant : ITenant
        where Policy : IPolicy
    {
        Task<IClient<Tenant, Policy, Role, Permission>> ExecuteAsync(IClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration);
    }
}