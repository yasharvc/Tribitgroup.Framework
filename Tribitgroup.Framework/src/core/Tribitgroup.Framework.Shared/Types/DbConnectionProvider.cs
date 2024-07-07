using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Tribitgroup.Framework.Shared.Types
{
    public sealed class DbConnectionProvider
    {
        IDictionary<string, string> ConnectionStrings { get; } = new Dictionary<string, string>();
        IDictionary<string, IDbConnection> DbConnection { get; } = new Dictionary<string, IDbConnection>();
        IServiceProvider ServiceProvider { get; }

        IDbConnection? CurrentConnection { get; set; }
        DbContext? CurrentDbContext { get; set; }

        public DbConnectionProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

        public void SetCurrentConnection(IDbConnection connection) => CurrentConnection = connection;
        public IDbConnection? GetCurrentConnection() => CurrentConnection;
        public void SetCurrentDbContext<T>(T dbContext) where T : DbContext => CurrentDbContext = dbContext;
        public T GetCurrentDbContext<T>() where T : DbContext => CurrentDbContext as T ?? throw new InvalidCastException();

        public T GetDbContext<T>() where T : DbContext => ServiceProvider.GetService<T>() ?? throw new KeyNotFoundException();

        public void AddConnectionString(string name, string connectionString) => ConnectionStrings[name] = connectionString;

        public void AddDbConnection(string name, IDbConnection connection) => DbConnection[name] = connection;

        public string GetConnectionString(string name) => ConnectionStrings[name];

        public IDbConnection GetDbConnection(string name) => DbConnection[name];
    }
}