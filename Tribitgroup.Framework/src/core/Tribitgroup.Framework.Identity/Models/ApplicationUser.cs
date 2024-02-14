using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Identity.Models
{
    public class ApplicationUser : IdentityUser<Guid>, IEntity<Guid>
    {
        public virtual ICollection<UserRefreshToken> RefreshTokens { get; set; } = new Collection<UserRefreshToken>();
        public virtual ICollection<UserTenant<ApplicationUser>> Tenants { get; set; } = new Collection<UserTenant<ApplicationUser>>();
        public virtual ICollection<UserPermission<ApplicationUser, ApplicationPermission>> Permissions { get; set; } = new Collection<UserPermission<ApplicationUser, ApplicationPermission>>();

        public ApplicationUser()
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }

        public string GetTableName(DbContext? dbContext = null)
        {
            return "AspNetUsers";
        }

        public object? GetValue(string propName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetColumnNames()
        {
            return new List<string>();
        }

        public Task ValidateAsync()
        {
            throw new NotImplementedException();
        }
    }
}