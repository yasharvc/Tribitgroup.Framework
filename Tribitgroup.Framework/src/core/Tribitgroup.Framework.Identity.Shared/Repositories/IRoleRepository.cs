using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;
namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IRoleRepository<TDbContext> : ICUDRepository<Role, TDbContext, Guid>, IQueryRepository<Role, Guid> where TDbContext : DbContext
    {
    }
}