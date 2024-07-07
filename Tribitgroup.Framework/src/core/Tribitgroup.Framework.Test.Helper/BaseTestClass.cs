using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tribitgroup.Framework.Shared.Exceptions;

namespace Tribitgroup.Framework.Test.Helper
{
    public abstract class BaseTestClass<TDbContext> where TDbContext: DbContext
    {
        IServiceProvider ServiceProvider { get; }
        TDbContext Db { get; set; }
        IDictionary<Type, Faker> Fakers { get; } = new Dictionary<Type, Faker>();

        public BaseTestClass(string databaseName)
        {
            var services = new ServiceCollection();
            AddServices(services);
            services.AddDbContext<TDbContext>(options =>
                options.UseSqlite($"Data Source={databaseName};")
            );

            ServiceProvider = services.BuildServiceProvider();

            PostServiceCollecting();
        }

        protected void CreateDb()
        {
            Db = GetService<TDbContext>() ?? throw new ServiceNotInjectedException();
            Db.Database.EnsureDeleted();
            Db.Database.EnsureCreated();
        }

        protected TDbContext GetDbContext() => Db;

        protected virtual void AddServices(ServiceCollection services) { }
        protected T GetService<T>() => ServiceProvider.GetService<T>() ?? throw new Exception();

        ~BaseTestClass()
        {
            var db = GetDbContext();
            db.Database.EnsureDeleted();
            db.SaveChanges();
        }

        private void PostServiceCollecting()
        {
            CreateDb();
        }
    }
}