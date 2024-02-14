using Test.API.Authentication.Contracts;

namespace Test.API.Authentication.Providers.InMemory;
public class UserProvider<Tenant, Policy, Role, Permission, User> :
    IUserProvider<Tenant, Policy, Role, Permission, User>
    where Tenant : ITenant where Policy : IPolicy where Role : IRole where Permission : IPermission
    where User : IUser<Tenant, Policy, Role, Permission>
{
    List<IUser<Tenant, Policy, Role, Permission>> Users = [];


    public Task AddToPermissionAsync(Guid userId, params Permission[] permissions)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Permission>)user.Permissions).AddRange(permissions);
        return Task.CompletedTask;
    }

    public Task AddToPolicyAsync(Guid userId, params Policy[] policies)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Policy>)user.Policies).AddRange(policies);
        return Task.CompletedTask;
    }

    public Task AddToRoleAsync(Guid userId, params Role[] roles)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Role>)user.Permissions).AddRange(roles);
        return Task.CompletedTask;
    }

    public Task AddToTenantAsync(Guid userId, params Tenant[] tenants)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Tenant>)user.Permissions).AddRange(tenants);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IUser<Tenant, Policy, Role, Permission> user)
    {
        Users.RemoveAll(u => u.Id == user.Id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<IUser<Tenant, Policy, Role, Permission>>> GetAllAsync() => Task.FromResult(Users.AsEnumerable());

    public Task<IUser<Tenant, Policy, Role, Permission>?> GetByIdAsync(Guid id) => Task.FromResult(Users.FirstOrDefault(u => u.Id == id));

    public Task InsertAsync(IUser<Tenant, Policy, Role, Permission> user)
    {
        Users.Add(user);
        return Task.CompletedTask;
    }

    public Task RemoveFromPermissionAsync(Guid userId, params Permission[] permissions)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Permission>)user.Permissions).RemoveAll(p => permissions.Select(m=>m.Name).Contains(p.Name));
        return Task.CompletedTask;
    }

    public Task RemoveFromPermissionAsync(Guid userId, params string[] permissions)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Permission>)user.Permissions).RemoveAll(p => permissions.Contains(p.Name));
        return Task.CompletedTask;
    }

    public Task RemoveFromPolicyAsync(Guid userId, params Policy[] policies)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Policy>)user.Policies).RemoveAll(p => policies.Select(m=>m.Name).Contains(p.Name));
        return Task.CompletedTask;
    }

    public Task RemoveFromPolicyAsync(Guid userId, params string[] policyNames)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Policy>)user.Policies).RemoveAll(p => policyNames.Contains(p.Name));
        return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(Guid userId, params Role[] roles)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Role>)user.Roles).RemoveAll(p => roles.Select(m=>m.Name).Contains(p.Name));
        return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(Guid userId, params string[] roleNames)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Role>)user.Roles).RemoveAll(p => roleNames.Contains(p.Name));
        return Task.CompletedTask;
    }

    public Task RemoveFromTenantAsync(Guid userId, params Tenant[] tenants)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Tenant>)user.Tenants).RemoveAll(p => tenants.Select(m => m.Name).Contains(p.Name));
        return Task.CompletedTask;
    }

    public Task RemoveFromTenantAsync(Guid userId, params string[] tenantNames)
    {
        var user = Users.FirstOrDefault(u => u.Id == userId) ?? throw new EntryPointNotFoundException();
        ((List<Tenant>)user.Tenants).RemoveAll(p => tenantNames.Contains(p.Name));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(IUser<Tenant, Policy, Role, Permission> user)
    {
        Users.RemoveAll(u => u.Id == user.Id);
        Users.Add(user);
        return Task.CompletedTask;
    }
}
