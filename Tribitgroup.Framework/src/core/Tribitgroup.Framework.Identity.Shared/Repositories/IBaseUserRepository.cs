using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Identity.Shared.Entities.User;
using Tribitgroup.Framework.Identity.Shared.Enums;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IBaseUserRepository<TUser, TDbContext> : ICUDRepository<TUser, TDbContext, Guid>, IQueryRepository<TUser, Guid>
        where TUser : BaseUser, IAggregateRoot
        where TDbContext : DbContext
    {
        Task AddActivityLogAsync(TUser user, UserActivityEnum userActivity, string description, string? metadata = null);

        Task AddDeviceAsync(TUser user, UserDevice device);
        Task DeactivateDeviceAsync(TUser user, UserDevice device);

        Task LockAsync(TUser user);
        Task UnlockAsync(TUser user);
        Task DeleteAsync(TUser user);
        Task ActivateAsync(TUser user);
        Task DeactivateAsync(TUser user);

        Task AddPermissionAsync(TUser user, params Permission[] permissions);
        Task AddPermissionAsync(Guid userId, params Guid[] permissionIds);
        Task RemovePermissionAsync(TUser user, params Permission[] permissions);
        Task RemovePermissionAsync(Guid userId, params Guid[] permissionIds);

        Task AddRoleAsync(TUser user, params Role[] roles);
        Task AddRoleAsync(Guid userId, params Guid[] roleIds);
        Task RemoveRoleAsync(TUser user, params Role[] roles);
        Task RemoveRoleAsync(Guid userId, params Guid[] roleIds);

        Task AddTenantAsync(TUser user, params Tenant[] tenants);
        Task AddTenantAsync(Guid userId, params Guid[] tenantIds);
        Task RemoveTenantAsync(TUser user, params Tenant[] tenants);
        Task RemoveTenantAsync(Guid userId, params Guid[] tenantIds);

        Task AddPolicyAsync(TUser user, params Policy[] policies);
        Task AddPolicyAsync(Guid userId, params Guid[] policies);
        Task RemovePolicyAsync(TUser user, params Policy[] policies);
        Task RemovePolicyAsync(Guid userId, params Guid[] policies);

        Task AddTokenAsync(TUser user, Guid deviceId, string token, DateTime validUntil);
        Task DeactivateTokenAsync(Guid tokenId);

        Task UpdateLastLoginDateTimeAsynv(TUser user);
    }
}