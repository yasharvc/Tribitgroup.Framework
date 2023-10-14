using Tribitgroup.Framework.Identity.Shared.Enums;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities
{
    public class UserActivityLog : AuditedAggregateRoot
    {
        public UserActivityEnum Activity { get; set; }
        public string? Metadata { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}