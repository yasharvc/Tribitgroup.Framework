using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Persistance.Tests.DbContext
{
    public class OrderDetail : Entity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Count { get; set; } = 1;
        public decimal Price { get; set; } = 0;
    }
}
