using Tribitgroup.Framework.Identity.Shared.Entities.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public class UserWithUsernamePassword : BaseUser, IUsername, IPassword
    {
        public string Username { get; } = string.Empty;
        public string Password { get; } = string.Empty;

        public override Task SetCreatedAtAsync(DateTime createdAt)
        {
            CreatedAt = createdAt;
            return Task.CompletedTask;
        }
    }
}