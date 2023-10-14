using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities
{
    public class UserDevice : Entity, ILogicalDelete, IHasActiveStatus
    {
        public bool Deleted { get; set; }
        public bool IsActive { get; set; }
        public string IP { get; set; } = string.Empty;
        public DateTime? ValidUntil { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}