using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Persistance.Tests.DbContext
{
    public class Order : AggregateRoot
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
