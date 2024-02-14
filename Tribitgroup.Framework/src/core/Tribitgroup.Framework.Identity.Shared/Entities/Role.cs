using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities
{
    public class Role : AggregateRoot
    {
        public string Name { get; set; } = string.Empty;

        private Role() { }
        public Role(string name) => Name = name;
    }
}