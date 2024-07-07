using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Identity.Shared.Entities.User;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IUserActivityLogRepository<TDbContext> : ICUDRepository<Role, TDbContext, Guid>, IQueryRepository<UserActivityLog, Guid>
        where TDbContext : DbContext
    {
    }
}