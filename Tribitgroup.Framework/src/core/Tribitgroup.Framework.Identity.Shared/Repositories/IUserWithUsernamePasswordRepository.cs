using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Entities.User;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IUserWithUsernamePasswordRepository<TDbContext> : 
        IBaseUserRepository<UserWithUsernamePassword, TDbContext>,
        ICUDRepository<UserWithUsernamePassword, TDbContext, Guid>, IQueryRepository<UserWithUsernamePassword, Guid>
        where TDbContext: DbContext
    {
        Task GetByUsernamePasswordAsync(string username, string password);
    }
}