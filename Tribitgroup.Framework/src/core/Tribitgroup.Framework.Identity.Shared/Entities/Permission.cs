using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities
{
    public class Permission : Entity
    {
        public string Name { get; set; } = string.Empty;
        private Permission() { }
        public Permission(string name)
        {
            Name = name;
        }
    }
}