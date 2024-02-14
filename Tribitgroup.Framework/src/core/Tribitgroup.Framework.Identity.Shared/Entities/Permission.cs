using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities
{
    public class Permission : AggregateRoot
    {
        public string Name { get; set; } = string.Empty;
        private Permission() { }
        public Permission(string name)
        {
            Name = name;
        }
    }
}