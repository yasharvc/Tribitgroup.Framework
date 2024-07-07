using Tribitgroup.Framework.Identity.Shared.Enums;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public class UserActivityLog : AuditedAggregateRoot
    {
        public UserActivityEnum Activity { get; protected set; }
        public string? Metadata { get; protected set; } = string.Empty;
        public string Description { get; protected set; } = string.Empty;
        public Guid UserId { get; protected set; }

        public override Task SetCreatedAtAsync(DateTime createdAt)
        {
            throw new NotImplementedException();
        }
    }
}