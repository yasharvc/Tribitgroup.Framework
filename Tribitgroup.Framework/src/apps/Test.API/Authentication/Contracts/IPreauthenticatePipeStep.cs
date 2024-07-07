namespace Test.API.Authentication.Contracts
{
    public interface IPreauthenticatePipeStep<Tenant, Policy, Role, Permission>
        where Tenant : ITenant
        where Policy : IPolicy
        where Role : IRole
        where Permission : IPermission
    {
        Task<IClient<Tenant, Policy, Role, Permission>> ExecuteAsync(IClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration configuration);
    }
}