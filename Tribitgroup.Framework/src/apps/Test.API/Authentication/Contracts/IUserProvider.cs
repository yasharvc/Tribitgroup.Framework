namespace Test.API.Authentication.Contracts
{
    public interface IUserProvider<Tenant, Policy, Role, Permission, User>
        where Tenant : ITenant
        where Policy : IPolicy
        where Role : IRole
        where Permission : IPermission
        where User : IUser<Tenant, Policy, Role, Permission>
    {
        Task<IUser<Tenant, Policy, Role, Permission>?> GetByIdAsync(Guid id);
        Task<IEnumerable<IUser<Tenant, Policy, Role, Permission>>> GetAllAsync();
        Task InsertAsync(IUser<Tenant, Policy, Role, Permission> user);
        Task UpdateAsync(IUser<Tenant, Policy, Role, Permission> user);
        Task DeleteAsync(IUser<Tenant, Policy, Role, Permission> user);

        Task AddToRoleAsync(Guid userId, params Role[] roles);
        Task RemoveFromRoleAsync(Guid userId, params Role[] roles);
        Task RemoveFromRoleAsync(Guid userId, params string[] roleNames);

        Task AddToPolicyAsync(Guid userId, params Policy[] policies);
        Task RemoveFromPolicyAsync(Guid userId, params Policy[] policies);
        Task RemoveFromPolicyAsync(Guid userId, params string[] policyNames);

        Task AddToPermissionAsync(Guid userId, params Permission[] permissions);
        Task RemoveFromPermissionAsync(Guid userId, params Permission[] permissions);
        Task RemoveFromPermissionAsync(Guid userId, params string[] permissions);

        Task AddToTenantAsync(Guid userId, params Tenant[] tenants);
        Task RemoveFromTenantAsync(Guid userId, params Tenant[] tenants);
        Task RemoveFromTenantAsync(Guid userId, params string[] tenantNames);
    }
}
