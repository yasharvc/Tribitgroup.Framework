using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Consts;
using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Identity.Shared.Entities.User;

namespace Tribitgroup.Framework.Identity.EF
{
    public class UserWithUsernamePasswordDbContext : DbContext
    {
        public DbSet<UserWithUsernamePassword> Users => Set<UserWithUsernamePassword>();
        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
        public DbSet<UserActivityLog> UserActivities => Set<UserActivityLog>();
        public DbSet<UserDevice> UserDevices => Set<UserDevice>();
        public DbSet<UserPolicy> UserPolicies => Set<UserPolicy>();
        public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserTenant> UserTenants => Set<UserTenant>();
        public DbSet<UserToken> UserTokens => Set<UserToken>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Policy> Policies => Set<Policy>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public UserWithUsernamePasswordDbContext(DbContextOptions<UserWithUsernamePasswordDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder
                .Entity<UserWithUsernamePassword>()
                .HasMany(m => m.ActivityLogs);
            builder
                .Entity<UserWithUsernamePassword>()
                .HasMany(m => m.Permissions);
            builder
                .Entity<UserWithUsernamePassword>()
                .HasMany(m => m.Policies);
            builder
                .Entity<UserWithUsernamePassword>()
                .HasMany(m => m.RefreshTokens);
            builder
                .Entity<UserWithUsernamePassword>()
                .HasMany(m => m.Roles);
            builder
                .Entity<UserWithUsernamePassword>()
                .HasMany(m => m.Tenants);
            builder
                .Entity<UserWithUsernamePassword>()
                .HasMany(m => m.Tokens);

            builder.Entity<UserWithUsernamePassword>()
                .HasData(new Role(DefaultRoles.UserAdmin));
        }
    }
}