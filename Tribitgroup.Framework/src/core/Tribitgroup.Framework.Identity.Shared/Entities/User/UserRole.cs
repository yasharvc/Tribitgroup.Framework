using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public class UserRole : Entity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}