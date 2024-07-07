using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IPermissionRepository<TDbContext> : ICUDRepository<Role, TDbContext, Guid>, IQueryRepository<Permission, Guid>
        where TDbContext : DbContext
    {
    }
}