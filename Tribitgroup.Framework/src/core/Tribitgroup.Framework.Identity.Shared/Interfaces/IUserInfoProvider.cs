using Tribitgroup.Framework.Identity.Shared.DTO;
using Tribitgroup.Framework.Identity.Shared.Entities;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IUserInfoProvider
    {
        Task<UserInfo> GetCurrentUserAsync();
        Task<Tenant> GetCurrentTenant();
    }
}