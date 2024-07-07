using Microsoft.EntityFrameworkCore;
using System.Data;
using Tribitgroup.Framework.Shared.Enums;

namespace Tribitgroup.Framework.Shared.Interfaces.Repository
{
    public interface IConnectionCollection
    {
        IConnectionProvider BuildConnectionProvider();
        void FromConnectionString<TDbContext>(string name, DatabaseEngineEnum databaseEngine, string connectionString) where TDbContext : DbContext;
        void FromDbContext<TDbContext>(string name, TDbContext dbContext) where TDbContext : DbContext;
        void FromDbConnection(string name, IDbConnection dbConnection);
    }
}