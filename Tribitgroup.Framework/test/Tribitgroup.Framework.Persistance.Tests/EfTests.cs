using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tribitgroup.Framework.Identity.EF.Repositories;
using Tribitgroup.Framework.Persistance.Tests.DbContext;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Test.Helper;

namespace Tribitgroup.Framework.Dapper.Tests
{
    public class EfTests : BaseTestClass<SampleDbContext>
    {
        EfCUDRepository<User,SampleDbContext,Guid> repository;
        public EfTests() : base(nameof(EfTests))
        {
            repository = GetService<EfCUDRepository<User,SampleDbContext,Guid>>();
        }

        protected override void AddServices(ServiceCollection services)
        {
            base.AddServices(services);
            services.AddSingleton<EfCUDRepository<User, SampleDbContext, Guid>>();
        }

        [Fact]
        public async Task InsertOne_Should_Add_User_Into_DbContext()
        {
            var user = new User
            {
                DateOfBirth = DateTime.Now,
                Password = "password",
                Username = "username",
            };

            await repository.InsertOneAsync(user);

            GetDbContext().Users.Count().ShouldBe(1);
            GetDbContext().Users.First().Password.ShouldBe(user.Password);
            GetDbContext().Users.First().Id.ShouldBe(user.Id);
            GetDbContext().Users.First().DateOfBirth.ShouldBe(user.DateOfBirth);
        }

        [Fact]
        public async Task UOW_Should_Rollback_Updated_User()
        {
            User user = await AddUserAsync("User 1","Pass 1");
            var ctx = GetDbContext();

            var tran = await ctx.Database.BeginTransactionAsync();

            await repository.DeleteOneAsync(user.Id, unitOfWorkHost: new UnitOfWorkHostInterface<SampleDbContext>(ctx));

            ctx.Users.Count().ShouldBe(0);

            await tran.RollbackAsync();

            ctx.Users.Count().ShouldBe(1);
            ctx.Users.First().Password.ShouldBe(user.Password);
            ctx.Users.First().Id.ShouldBe(user.Id);
            ctx.Users.First().DateOfBirth.ShouldBe(user.DateOfBirth);
        }

        private async Task<User> AddUserAsync(string username, string pass)
        {
            var user = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-10),
                Password = pass,
                Username = username,
            };

            await repository.InsertOneAsync(user);
            return user;
        }
    }
}
