using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public class UserPermission : Entity
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}