using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tribitgroup.Framework.Persistance.Tests.DbContext;
using Tribitgroup.Framework.Test.Helper;

namespace Tribitgroup.Framework.Dapper.Tests
{

    public class DapperCUDRepositoryTests : BaseTestClass<SampleDbContext>
    {
        DapperCUDConnectionProvider<User> DapperRepoProvider { get; }
        DapperCUDRepository<User, SampleDbContext> DapperUserCUDRepo { get; }
        public DapperCUDRepositoryTests() : base(nameof(SampleDbContext))
        {
            DapperRepoProvider = new DapperCUDConnectionProvider<User>(GetDbContext());
            DapperUserCUDRepo = new DapperCUDRepository<User, SampleDbContext>(DapperRepoProvider);
        }


        [Fact]
        public async Task InsertMany_Should_Add_Users()
        {
            var user1 = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-20),
                Password = "password",
                Username = "User 1",
            };
            var user2 = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-15),
                Password = "password",
                Username = "User 2",
            };

            await DapperUserCUDRepo.InsertManyAsync(new List<User> { user1, user2 });

            var lst = await GetDbContext().Users.ToListAsync();

            lst.Count.ShouldBe(2);
            Assert.NotNull(lst.SingleOrDefault(x => x.Id == user1.Id));
            Assert.NotNull(lst.SingleOrDefault(x => x.Id == user2.Id));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_The_Data()
        {
            var users = await InsertTestUsersAsync();

            users.First().Username = "yashar";

            await DapperUserCUDRepo.UpdateOneAsync(users.First());

            var lst = await GetDbContext().Users.ToListAsync();

            lst.Count.ShouldBe(2);
            Assert.NotNull(lst.SingleOrDefault(x => x.Id == users.First().Id && x.Username == "yashar"));
        }

        [Fact]
        public async Task UpdateManyAsync_Should_Update_The_Data()
        {
            var users = await InsertTestUsersAsync();

            users.First().Username = "yashar";
            users.Last().Username = "Update user";

            await DapperUserCUDRepo.UpdateManyAsync(users);

            var lst = await GetDbContext().Users.ToListAsync();

            lst.Count.ShouldBe(2);
            Assert.NotNull(lst.SingleOrDefault(x => x.Id == users.First().Id && x.Username == "yashar"));
            Assert.NotNull(lst.SingleOrDefault(x => x.Id == users.Last().Id && x.Username == "Update user"));
        }

        private async Task<IEnumerable<User>> InsertTestUsersAsync()
        {
            var user1 = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-20),
                Password = "password",
                Username = "User 1",
            };
            var user2 = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-15),
                Password = "password",
                Username = "User 2",
            };

            var res = new List<User> { user1, user2 };

            await DapperUserCUDRepo.InsertManyAsync(res);
            return res;
        }
    }
}
