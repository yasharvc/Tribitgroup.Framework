using Microsoft.EntityFrameworkCore;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IUnitOfWorkHostInterface<TDbContext> : IUnitOfWorkHostEvents
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }
    }

    public class UnitOfWorkHostInterface<TDbContext> : IUnitOfWorkHostInterface<TDbContext>, IUnitOfWorkHostEvents
        where TDbContext : DbContext
    {
        public TDbContext DbContext { get; init; }
        public event EventHandler Committed;
        public event EventHandler RollBacked;

        public UnitOfWorkHostInterface(TDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
