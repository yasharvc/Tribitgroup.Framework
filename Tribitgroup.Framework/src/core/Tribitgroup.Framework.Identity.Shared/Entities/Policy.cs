using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities
{
    public class Policy : AggregateRoot
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Role> Roles { get; set; } = new HashSet<Role>();
        public virtual ICollection<Permission> Permissions { get; set; } = new HashSet<Permission>();
        public virtual ICollection<Tenant> Tenants { get; set; } = new HashSet<Tenant>();

        public Policy(string name)
        {
            Name = name;
        }
    }
}