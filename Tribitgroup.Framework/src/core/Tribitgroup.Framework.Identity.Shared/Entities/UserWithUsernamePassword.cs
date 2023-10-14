using Tribitgroup.Framework.Identity.Shared.Entities.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Entities
{
    public class UserWithUsernamePassword : BaseUser, IUsername, IPassword
    {
        public string Username { get; } = string.Empty;
        public string Password { get; } = string.Empty;
    }
}