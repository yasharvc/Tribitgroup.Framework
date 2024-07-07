using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.EF.Repositories
{
    public class EfCUDRepository<TEntity, TDbContext, U>
        : ICUDRepository<TEntity, TDbContext, U>
        where TEntity : class, IEntity<U>
        where U : notnull
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; set; }
        DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

        public EfCUDRepository(TDbContext dbContext) => DbContext = dbContext;

        public async Task DeleteManyAsync(
            IEnumerable<U> entities,
            IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null,
            CancellationToken cancellationToken = default)
                => await DoItWithUOWAsync(async (dbContext) =>
                    {
                        DbSet.RemoveRange(await GetItemsByIdsAsync(entities));
                    }, unitOfWorkHost, cancellationToken);

        public async Task DeleteOneAsync(U id, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default) 
            => await DeleteManyAsync(new List<U> { id }, unitOfWorkHost, cancellationToken);

        public async Task<IEnumerable<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            await DoItWithUOWAsync(async (ctx) =>
            {
                await DbContext.AddRangeAsync(entities, cancellationToken);
            }, unitOfWorkHost, cancellationToken);
            return entities;
        }

        public async Task<TEntity> InsertOneAsync(
            TEntity entity, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, 
            CancellationToken cancellationToken = default) 
            => (await InsertManyAsync(new List<TEntity> { entity }, unitOfWorkHost, cancellationToken)).First();

        public Task<IEnumerable<TEntity>> UpdateManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<TEntity, object>>? includes = null)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> UpdateOneAsync(TEntity entity, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<TEntity, object>>? includes = null)
            => (await UpdateManyAsync(new List<TEntity> { entity }, unitOfWorkHost, cancellationToken)).First();

        private async Task DoItWithUOWAsync(
            Func<TDbContext, Task> action,
            IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost, 
            CancellationToken cancellationToken = default)
        {
            await ApplyUOW(unitOfWorkHost);
            await action(DbContext);
            await CommitAsync(unitOfWorkHost, cancellationToken);
        }

        private async Task CommitAsync(IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost, CancellationToken cancellationToken = default)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        private Task ApplyUOW(IUnitOfWorkHostInterface<TDbContext>? uow)
        {
            if (uow == null) return Task.CompletedTask;
            DbContext = uow.DbContext;
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<TEntity>> GetItemsByIdsAsync(IEnumerable<U> entities)
        {
            var res = DbContext.Set<TEntity>().Where(new Condition
            {
                Operator = ConditionOperatorEnum.In,
                PropertyName = nameof(Entity.Id),
                Values = entities.Select(m => m.ToString() ?? "").ToList(),
            });

            return await res.ToListAsync();
        }
    }
}
