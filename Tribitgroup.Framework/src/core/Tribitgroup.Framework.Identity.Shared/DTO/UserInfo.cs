using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Identity.Shared.Entities.User;

namespace Tribitgroup.Framework.Identity.Shared.DTO
{
    public class UserInfo
    {
        public Guid UserId { get; set; }
        public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();
        public IEnumerable<Tenant> Tenants { get; set; } = Enumerable.Empty<Tenant>();
        public IEnumerable<Permission> Permissions { get; set; } = Enumerable.Empty<Permission>();
        public IEnumerable<Policy> Policies { get; set; } = Enumerable.Empty<Policy>();

    }
}