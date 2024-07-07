using Tribitgroup.Framework.Shared.Interfaces.Entity;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public class UserDevice : Entity, ILogicalDelete, IHasActiveStatus
    {
        public bool Deleted { get; protected set; }
        public bool IsActive { get; protected set; }
        public string IP { get; protected set; } = string.Empty;
        public DateTime? ValidUntil { get; protected set; }
        public string Name { get; protected set; } = string.Empty;
        public Task ActivateAsync()
        {
            IsActive = true;
            return Task.CompletedTask;
        }

        public Task DeactivateAsync()
        {
            IsActive = false;
            return Task.CompletedTask;
        }
        public Task DeleteAsync()
        {
            Deleted = true;
            return Task.CompletedTask;
        }

        public Task Undelete()
        {
            Deleted = false;
            return Task.CompletedTask;
        }

        public Task UndeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}