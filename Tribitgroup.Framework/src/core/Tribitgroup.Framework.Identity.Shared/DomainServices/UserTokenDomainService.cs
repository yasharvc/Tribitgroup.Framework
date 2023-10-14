using Tribitgroup.Framework.Identity.Shared.Entities;

namespace Tribitgroup.Framework.Identity.Shared.DomainServices
{
    public sealed class UserTokenDomainService
    {
        public Task<UserToken> CreateUserTokenAsync(
            Guid deviceId, 
            UserDevice device, 
            string token, 
            DateTime validUntil, 
            bool isActive)
        {
            var res = new UserToken(deviceId, device, token, validUntil, isActive);
            return Task.FromResult(res);
        }
    }
}
