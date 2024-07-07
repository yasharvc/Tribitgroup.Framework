using Microsoft.EntityFrameworkCore;

namespace Tribitgroup.Framework.Persistance.Tests.DbContext
{
    public class SampleDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();


        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>()
                .HasMany(m => m.OrderDetails)
                .WithOne(m => m.Order);

            builder.Entity<Order>()
                .HasOne(m => m.User);
        }

    }
}
