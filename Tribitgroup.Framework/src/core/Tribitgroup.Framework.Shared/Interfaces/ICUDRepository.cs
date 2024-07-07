using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface ICUDRepository<T, TDbContext, U>
        where T : class, IEntity<U> where U : notnull
        where TDbContext : DbContext
    {
        Task<T> InsertOneAsync(T entity, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> InsertManyAsync(IEnumerable<T> entities, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default);
        Task<T> UpdateOneAsync(T entity, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<T, object>>? includes = null);
        Task<IEnumerable<T>> UpdateManyAsync(IEnumerable<T> entities, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<T, object>>? includes = null);
        Task DeleteOneAsync(U id, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default);
        Task DeleteManyAsync(IEnumerable<U> ids, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default);
    }
}