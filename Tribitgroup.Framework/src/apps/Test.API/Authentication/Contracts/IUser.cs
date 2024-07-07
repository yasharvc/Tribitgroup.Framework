namespace Test.API.Authentication.Contracts
{
    public interface IUser<Tenant, Policy, Role, Permission> where Tenant : ITenant where Policy : IPolicy where Role : IRole where Permission : IPermission
    {
        Guid Id { get; }
        IEnumerable<Permission> Permissions { get; }
        IEnumerable<Policy> Policies { get; }
        IEnumerable<Tenant> Tenants { get; }
        IEnumerable<Role> Roles { get; }

        Task<bool> HasPermissionAsync(Permission permission);
        Task<bool> HasPermissionAsync(string permissionName);
        Task<bool> HasPolicyAsync(Policy policy);
        Task<bool> HasPolicyAsync(string policyName);
        Task<bool> HasRoleAsync(Role role);
        Task<bool> HasRoleAsync(string roleName);
        Task<bool> HasTenantAsync(Tenant tenant);
        Task<bool> HasTenantAsync(string tenantName);
    }
}
