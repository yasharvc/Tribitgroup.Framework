﻿using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Models;

namespace Tribitgroup.Framework.Identity
{
    public interface IIdentityDbContext<TUser, TRole, TPermission> 
        where TUser: ApplicationUser 
        where TRole : ApplicationRole
        where TPermission : ApplicationPermission
    {
        DbSet<UserRefreshToken> RefreshTokens { get; }
        DbSet<TPermission> Permissions { get; }
        DbSet<UserPermission<TUser, TPermission>> UserPermissions { get; }
        DbSet<TUser> GetUserDbSet();
    }
}
