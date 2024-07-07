using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public class UserTenant : Entity
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}