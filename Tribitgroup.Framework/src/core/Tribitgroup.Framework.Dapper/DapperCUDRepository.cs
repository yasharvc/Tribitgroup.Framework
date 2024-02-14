using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Dapper
{
    public class DapperCUDRepository<TEntity, TDbContext> : ICUDRepository<TEntity, TDbContext, Guid>
        where TEntity : class, IAggregateRoot, IEntity<Guid>, new()
        where TDbContext : DbContext
    {
        protected bool IsLogicalDelete { get; }
        DapperCUDConnectionProvider<TEntity> ConnectionProvider { get; }
        TEntity Sample { get; } = new TEntity();

        public DapperCUDRepository(DapperCUDConnectionProvider<TEntity> connectionProvider)
        {
            ConnectionProvider = connectionProvider;
            IsLogicalDelete = typeof(TEntity)!.GetInterface(typeof(ILogicalDelete)!.FullName ?? "", ignoreCase: true) != null;
        }


        public async Task DeleteManyAsync(IEnumerable<Guid> ids, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            var query = "";
            var tableName = Sample.GetTableName(unitOfWorkHost?.DbContext);

            if (!IsLogicalDelete)
                query = $"delete from {tableName} where Id in (@Id)";
            else
                query = $"update {tableName} set ${nameof(ILogicalDelete.Deleted)} = @value where Id in (@Id)";
            await ExecuteAsync(unitOfWorkHost, query, new { id = ids, value = true }, cancellationToken);

        }

        public async Task DeleteOneAsync(Guid id, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            var query = "";
            var tableName = Sample.GetTableName(unitOfWorkHost?.DbContext);

            if (!IsLogicalDelete)
                query = $"delete from {tableName} where Id in (@Id)";
            else
                query = $"update {tableName} set ${nameof(ILogicalDelete.Deleted)} = @value where Id in (@Id)";
            await ExecuteAsync(unitOfWorkHost, $"delete from {tableName} where Id in (@Id)", new { id, value = true }, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> InsertManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            var lst = new List<string>();
            var param = new Dictionary<string, object>();
            var cols = Sample.GetColumnNames();
            var tableName = Sample.GetTableName(unitOfWorkHost?.DbContext ?? ConnectionProvider.DbContext);
            var columnNames = string.Join(", ", cols.Select(p => p));
            int index = 0;

            foreach (var entity in entities)
            {
                var valueParameters = string.Join(", ", cols.Select(p => $"@{p}{index}"));
                var dict = cols.Select(m => m).ToDictionary(n => $"{n}{index}", m => entity.GetValue(m));
                foreach (var item in dict)
                {
                    param[item.Key] = item.Value;
                }

                var query = $"INSERT INTO {tableName} ({columnNames}) VALUES ({valueParameters});";
                lst.Add(query);
                index++;
            }
            await ExecuteAsync(unitOfWorkHost, string.Join("\r\n", lst), param, cancellationToken);
            return entities;
        }

        public async Task<TEntity> InsertOneAsync(TEntity entity, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default)
        {
            await InsertManyAsync(new List<TEntity> { entity }, unitOfWorkHost, cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> UpdateManyAsync(IEnumerable<TEntity> entities, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<TEntity, object>>? includes = null)
        {
            var lst = new List<string>();
            var param = new Dictionary<string, object>();
            var cols = Sample.GetColumnNames();
            var tableName = Sample.GetTableName(unitOfWorkHost?.DbContext ?? ConnectionProvider.DbContext);

            int index = 0;

            foreach (var entity in entities)
            {
                var setParameters = string.Join(", ", cols.Where(p => p != nameof(entity.Id)).Select(p => $"{p}=@{p}{index}"));
                var dict = cols.Select(m => m).ToDictionary(n => $"{n}{index}", m => entity.GetValue(m));
                foreach (var item in dict)
                {
                    param[item.Key] = item.Value;
                }

                var query = $"UPDATE {tableName} SET {setParameters} WHERE {nameof(entity.Id)} = @{nameof(entity.Id)}{index};";
                lst.Add(query);
                index++;
            }
            await ExecuteAsync(unitOfWorkHost, string.Join("\r\n", lst), param, cancellationToken);
            return entities;
        }

        public async Task<TEntity> UpdateOneAsync(TEntity entity, IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost = null, CancellationToken cancellationToken = default, Expression<Func<TEntity, object>>? includes = null)
        {
            await UpdateManyAsync(new List<TEntity> { entity }, unitOfWorkHost, cancellationToken);
            return entity;
        }

        private Tuple<IDbConnection, IDbTransaction?> GetConnection(IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost)
        {
            var conn = ConnectionProvider.GetConnection();
            IDbTransaction? tran = null;
            if (unitOfWorkHost != null)
            {
                var ctx = unitOfWorkHost.DbContext;
                tran = ctx.Database.CurrentTransaction?.GetDbTransaction();
            }
            return new Tuple<IDbConnection, IDbTransaction?>(conn, tran);
        }

        private async Task ExecuteAsync(IUnitOfWorkHostInterface<TDbContext>? unitOfWorkHost, string query, object? @params, CancellationToken cancellationToken)
        {
            IDbTransaction? tran;
            IDbConnection conn;
            (conn, tran) = GetConnection(unitOfWorkHost);
            var cmd = new CommandDefinition(query, @params, tran, null, null, CommandFlags.Buffered, cancellationToken);

            await conn.ExecuteAsync(cmd);
        }
    }
}