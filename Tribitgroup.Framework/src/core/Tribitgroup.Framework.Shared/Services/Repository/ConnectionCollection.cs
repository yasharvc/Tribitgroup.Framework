using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Interfaces.Repository;

namespace Tribitgroup.Framework.Shared.Services.Repository
{
    public sealed class ConnectionCollection : IConnectionCollection
    {
        IDictionary<string, Tuple<DbContext?, IDbConnection>> Connections { get; } = new Dictionary<string, Tuple<DbContext?, IDbConnection>>();
        public IConnectionProvider BuildConnectionProvider()
        {
            throw new NotImplementedException();
        }

        public void FromConnectionString<TDbContext>(string name, DatabaseEngineEnum databaseEngine, string connectionString) where TDbContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
            AddUsingPartToOptions(optionsBuilder, databaseEngine, connectionString);
            var ctx = Activator.CreateInstance(typeof(TDbContext), optionsBuilder) as TDbContext ?? throw new InvalidCastException();
            Connections[name] = new Tuple<DbContext?, IDbConnection>(ctx, ctx.Database.GetDbConnection());
        }

        private static void AddUsingPartToOptions<TDbContext>(DbContextOptionsBuilder<TDbContext> optionsBuilder, DatabaseEngineEnum databaseEngine, string connectionString) where TDbContext : DbContext
        {
            if(databaseEngine == DatabaseEngineEnum.SqlServer)
                optionsBuilder.UseSqlServer(connectionString);
            else if(databaseEngine == DatabaseEngineEnum.Sqlite)
                optionsBuilder.UseSqlite(connectionString);
            else
                throw new NotImplementedException();
        }

        public void FromDbConnection(string name, IDbConnection dbConnection) 
            => Connections[name] = new Tuple<DbContext?, IDbConnection>(null, dbConnection);

        public void FromDbContext<TDbContext>(string name, TDbContext ctx) where TDbContext : DbContext 
            => Connections[name] = new Tuple<DbContext?, IDbConnection>(ctx, ctx.Database.GetDbConnection());
    }
}
