using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IQueryRepository<T, U> where T : class, IEntity<U> where U : notnull
    {
        void SetMaxSelectCount(int count);
        Task<PagedResult<T>> GetAllAsync(Pagination? pagination = default, List<Sort>? sorts = default, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<PagedResult<T>> WhereAsync(Expression<Func<T, bool>> selector, bool includeAll = false, Pagination? pagination = default, List<Sort>? sorts = default, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<PagedResult<T>> WhereAsync(IEnumerable<Condition> conditions, bool includeAll = false, Pagination? pagination = default, List<Sort>? sorts = default, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(U id, bool includeAll = false, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? selector = null, bool includeAll = false, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<T?> LastOrDefaultAsync(Expression<Func<T, bool>>? selector = null, bool includeAll = false, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>>? selector = null, bool includeAll = false, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<T> SingleAsync(Expression<Func<T, bool>>? selector = null, bool includeAll = false, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? selector = null, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>>? selector = null, bool includeLogicalDeleted = false, CancellationToken cancellationToken = default);
    }
}