using Dapper;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Tribitgroup.Framework.Dapper;
using Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder;
using Tribitgroup.Framework.Identity.Tests.DbContextTest;
using Tribitgroup.Framework.Shared.Types;
using Tribitgroup.Framework.Test.Helper;
using Tribitgroup.Framework.DB.Relational.Helper.Extensions;
using Tribitgroup.Framework.Identity.Tests.DbContextTest.DB;
using Tribitgroup.Framework.Shared.Extensions;
using System.Linq.Expressions;

namespace Tribitgroup.Framework.Identity.Tests.GeneralTests
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Productasdasds { get; set; }
        public DbSet<DbContextTest.Order> Orders => Set<DbContextTest.Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

        public MyDbContext()
        {
            
        }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DbContextTest.Order>()
                .HasMany(m => m.OrderDetails)
                .WithOne(m => m.Order);

            builder.Entity<DbContextTest.Order>()
                .HasOne(m => m.User);
        }
    }

    public class User : Entity
    {
        public string Name { get; set; }
    }

    public class Product : AggregateRoot
    {
        public string Name { get; set; }
    }

    public class EntityTests : BaseTestClass<MyDbContext>
    {
        public EntityTests() : base("test")
        {
        }

        class Test
        {
            public string Name { get; set; } = string.Empty;
            public List<string> List { get; set; }
            public Book Book { get; set; }
            public virtual ICollection<Book> Books { get; set; }
        }

        [Fact]
        public void GetMemberName_Should_return_name_of_selected_propeerty()
        {
            var testObject = new Test();

            testObject.SetMemberValue(x => x.Name, "Yashar");
            testObject.AddValueToListMember(x => x.List, "1", "2");
            testObject.AddValueToListMember(x => x.List, "4", "3");
            testObject.SetMemberValue(x => x.Book, new Book { Name = "Book 1" });
            testObject.SetMemberValue(x => x.Books, new List<Book> { 
                new Book { Name = "Book 2" },
                new Book { Name = "Book 4" }
            });

            testObject.Name.ShouldBe("Yashar");
            testObject.List.ShouldNotBeEmpty();
            testObject.Book.ShouldNotBeNull();
        }

        [Fact]
        public void GetTableName_With_DbContext_Should_return_Property_Name_Of_Product_DbSet()
        {
            var name = new Product().GetTableName(new MyDbContext());

            name.ShouldBe(nameof(MyDbContext.Productasdasds));
        }

        [Fact]
        public async Task Test1()
        {
            var p1 = new Product { Name = "P1" };
            var p2 = new Product { Name = "P2" };
            var repo = new DapperCUDRepository<Product, MyDbContext>(new DapperCUDConnectionProvider<Product>(GetDbContext()));

            await repo.InsertManyAsync(new List<Product> { p1, p2 });

            var res = await GetDbContext().Productasdasds.ToListAsync();

            res.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task JoinQueryTest()
        {
            var user = new User { Name = "yashar" };
            var order = new DbContextTest.Order
            {
                UserId = user.Id,
                User = user,
                OrderDetails = new List<OrderDetail>(),
            };

            var detail = new List<OrderDetail>{
                    new OrderDetail
                    {
                        Count = 5,
                        ItemName = "item 1",
                        Order = order,
                        OrderId = order.Id,
                        Price = 1000
                    },
                    new OrderDetail
                    {
                        Count = 10,
                        ItemName = "item 2",
                        Order = order,
                        OrderId = order.Id,
                        Price = 250
                    }
                };

            order.OrderDetails = detail;

            var ctx = GetDbContext();
            var conn = ctx.Database.GetDbConnection();

            await ctx.AddAsync(order);
            await ctx.SaveChangesAsync();

            var first = await ctx.Orders.FirstOrDefaultAsync();

            var userDb = await ctx.Users.FirstOrDefaultAsync() ?? throw new EntryPointNotFoundException();

            var query = @"SELECT Orders.*, OrderDetails.* FROM Orders INNER JOIN OrderDetails ON OrderDetails.OrderId = Orders.Id";

            var lst = await conn.QueryAsync(query);
            lst.ShouldNotBeEmpty();

            var countCol = Column.From<OrderDetail>(m => m.Count);
            var priceCol = Column.From<OrderDetail>(m => m.Price);
            var userIdCol = Column.From<DbContextTest.Order>(m => m.UserId);
            var itemNameCol = Column.From<OrderDetail>(m => m.ItemName);
            var detailIdCol = Column.From<OrderDetail>(m => m.Id,"Detail_Id");
            var orderIdCol = Column.From<DbContextTest.Order>(m => m.Id, "Order_Id");
            var userNameCol = Column.From<User>(m => m.Name, "User_Name");

            var qry = Query.From<DbContextTest.Order>(GetDbContext())
                .InnerJoin<OrderDetail, DbContextTest.Order>(b=>b.OrderId)
                .InnerJoin<DbContextTest.Order, User>(b=>b.UserId)
                .SelectColumns(() =>
                {
                    return new List<Column>
                    {
                        orderIdCol,
                        countCol,
                        priceCol,
                        itemNameCol,
                        userIdCol,
                        userNameCol,
                        detailIdCol
                    };
                });

            var ttt = await qry.RunAsync(conn,
                orderIdCol,
                QueryMapper<TestDTO>
                    .For(dto => dto.Id, orderIdCol)
                    .ForList(dto => dto.Items, item => item.Count, countCol)
                    .ForList(dto => dto.Items, item => item.Price, priceCol)
                    .ForList(dto => dto.Items, item => item.Name, itemNameCol)
                    .ForList(dto => dto.Items, item => item.Id, detailIdCol)
                    .ForObjectMember(dto=>dto.User,d=>d.Name,userNameCol)
                    .ForObjectMember(dto=>dto.User,d=>d.Id,userIdCol)
                );

            var res2 = await conn.QueryAsync(qry.ToString());

            Assert.NotNull(first);
            first.OrderDetails.ShouldNotBeEmpty();
        }

        [Fact]
        public void ExperssionTest()
        {
            Expression<Func<TestDTO, object>> propertySelector
                = dto => dto.User.Name;

            var x = propertySelector.GetMemberName();

            Expression<Func<TestDTO, object>> propertySelector2
                = dto => dto.User;
            var y = propertySelector2.GetMemberName();
            //We shoud get Detail.Count
            //var x = ((UnaryExpression)propertySelector.Body).Operand.ToString();
            //x = x[(x.IndexOf(".")+1)..];

            //var y = ((UnaryExpression)propertySelector2.Body).Operand.ToString();
            

            x.ShouldEndWith("Detail.Count");
        }
    }
    class TestDetailDTO : Entity
    {
        public int Count { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
    }

    class TestDTO :Entity 
    {
        public User User { get; set; }
        public List<TestDetailDTO> Items { get; set; } = new List<TestDetailDTO>();
    }
}
