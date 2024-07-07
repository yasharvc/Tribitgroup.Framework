using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IRepository<T, TDbContext, U> : IQueryRepository<T, U>, ICUDRepository<T, TDbContext, U> 
        where T : class, IEntity<U> where U : notnull
        where TDbContext : DbContext
    {
    }
}