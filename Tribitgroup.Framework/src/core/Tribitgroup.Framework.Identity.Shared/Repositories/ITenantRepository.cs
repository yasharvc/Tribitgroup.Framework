using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;
namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface ITenantRepository<TDbContext> : ICUDRepository<Role, TDbContext, Guid>, IQueryRepository<Tenant, Guid> where TDbContext : DbContext
    {
    }
}