using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IPolicyRepository<TDbContext> : ICUDRepository<Role, TDbContext, Guid>, IQueryRepository<Policy, Guid>
        where TDbContext : DbContext
    {
    }
}