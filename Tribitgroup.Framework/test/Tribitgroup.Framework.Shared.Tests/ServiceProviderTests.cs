using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Tests
{
    public class ServiceProviderTests
    {
        [Fact]
        public void DbConnectionProvider_Should_Return_ConnectionString_By_Name()
        {
            var services = new ServiceCollection();
            services.AddSingleton(sp => 
            {
                var res = new DbConnectionProvider(sp);

                res.AddConnectionString("test", "test://dbconnection");

                return res;
            });

            var provider = services.BuildServiceProvider().GetService<DbConnectionProvider>() ?? throw new NullReferenceException();
            var str = provider.GetConnectionString("test");
            

            str.ShouldBe("test://dbconnection");
        }
    }
}