using Microsoft.EntityFrameworkCore;
using System.Data;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Dapper
{
    public sealed class DapperCUDConnectionProvider<T> where T: class, IEntity<Guid>
    {
        IDbConnection? Connection { get; init; }
        public DbContext? DbContext { get; init; } = null;
        bool HasDirectConnection => Connection != null;

        public DapperCUDConnectionProvider(IDbConnection connection)
        {
            Connection = connection;
        }

        public DapperCUDConnectionProvider(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IDbConnection GetConnection()
        {
            if (HasDirectConnection)
                return Connection ?? throw new NullReferenceException();
            return DbContext?.Database.GetDbConnection() ?? throw new NullReferenceException();
        }
    }
}