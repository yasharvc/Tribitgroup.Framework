using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public class UserPolicy : Entity
    {
        public Guid UserId { get; set; }
        public Guid PolicyId { get; set; }
        public Policy Policy { get; set; }
    }
}