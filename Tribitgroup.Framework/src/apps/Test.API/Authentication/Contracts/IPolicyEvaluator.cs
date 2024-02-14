namespace Test.API.Authentication.Contracts;
public delegate Task<bool> PolicyEvaluatorDelegate<Tenant, Policy, Role, Permission>(IClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration authenticationConfiguration)
    where Role : IRole
    where Permission : IPermission
    where Tenant : ITenant
    where Policy : IPolicy;
public interface IPolicyEvaluator<Tenant, Policy, Role, Permission>
    where Role : IRole
    where Permission : IPermission
    where Tenant : ITenant
    where Policy : IPolicy
{
    Task<bool> EvaluateAsync(IClient<Tenant, Policy, Role, Permission> client, IAuthenticationConfiguration authenticationConfiguration);
}