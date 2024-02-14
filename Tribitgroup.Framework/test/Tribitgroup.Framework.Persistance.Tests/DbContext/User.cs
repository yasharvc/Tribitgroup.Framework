using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Persistance.Tests.DbContext
{
    public class User : AggregateRoot
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
