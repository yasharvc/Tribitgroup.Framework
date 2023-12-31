﻿using System.Linq.Expressions;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface ICUDRepository<T, U> where T : class, IEntity<U>, IAggregateRoot where U : notnull
    {
        Task<T> InsertOneAsync(T entity, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> InsertManyAsync(IEnumerable<T> entities, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default);
        Task<T> UpdateOneAsync(T entity, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<T, object>>? includes = null);
        Task<IEnumerable<T>> UpdateManyAsync(IEnumerable<T> entities, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<T, object>>? includes = null);
        Task DeleteOneAsync(U id, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default);
        Task DeleteManyAsync(IEnumerable<T> entities, IUnitOfWorkHostInterface? unitOfWorkHost = null, CancellationToken cancellationToken = default);
    }
}