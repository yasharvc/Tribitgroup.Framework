using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public class UserToken : Entity, IHasActiveStatus
    {
        public Guid DeviceId { get; private set; }
        public UserDevice Device { get; private set; } = new UserDevice();
        public string TokenHash { get; private set; } = string.Empty;
        public DateTime ValidUntil { get; private set; }
        public bool IsActive { get; private set; }

        private UserToken() { }

        public UserToken(Guid deviceId, UserDevice device, string token, DateTime validUntil, bool isActive, Guid? id = null)
        {
            SetDeviceId(deviceId);
            SetDevice(device);
            SetTokenHash(token);
            SetValidUntil(validUntil);
            SetIsActive(isActive);
            SetId(id);
        }

        internal void SetId(Guid? id)
        {
            if (id == null)
                return;
            Id = id.Value;
        }

        internal void SetIsActive(bool isActive)
        {
            IsActive = isActive;
        }

        internal void SetValidUntil(DateTime validUntil)
        {
            ValidUntil = validUntil;
        }

        internal void SetTokenHash(string token)
        {
            TokenHash = GetTokenHash(token);
        }

        private string GetTokenHash(string token) => token.ToSHA512();

        internal void SetDevice(UserDevice device)
        {
            Device = device;
        }

        internal void SetDeviceId(Guid deviceId)
        {
            DeviceId = deviceId;
        }

        public Task ActivateAsync()
        {
            SetIsActive(true);
            return Task.CompletedTask;
        }

        public Task DeactivateAsync()
        {
            SetIsActive(false);
            return Task.CompletedTask;
        }
    }
}