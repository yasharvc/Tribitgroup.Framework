using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public abstract class BaseUser : AuditedAggregateRoot, IHasActiveStatus, ILogicalDelete
    {
        public bool IsActive { get; protected set; } = false;
        public bool IsLocked { get; protected set; } = false;
        public bool Deleted { get; protected set; } = false;
        public DateTime? LastLoginDateTime { get; protected set; }
        public virtual ICollection<UserToken> Tokens { get; set; } = new List<UserToken>();
        public virtual ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
        public virtual ICollection<UserTenant> Tenants { get; set; } = new List<UserTenant>();
        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
        public virtual ICollection<UserPolicy> Policies { get; set; } = new List<UserPolicy>();
        public virtual ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();
        public virtual ICollection<UserActivityLog> ActivityLogs { get; set; } = new List<UserActivityLog>();

        public Task ActivateAsync()
        {
            IsActive = true;
            return Task.CompletedTask;
        }

        public Task DeactivateAsync()
        {
            IsActive = false;
            return Task.CompletedTask;
        }

        public Task DeleteAsync()
        {
            Deleted = true;
            return Task.CompletedTask;
        }

        public Task Undelete()
        {
            Deleted = false;
            return Task.CompletedTask;
        }

        public Task UndeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}