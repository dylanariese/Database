using Database.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Database.Data
{
    public class DatabaseDbContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public DatabaseDbContext(DbContextOptions<DatabaseDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(builder =>
            {
                // Each User can have many UserClaims
                builder.HasMany(e => e.Claims)
                       .WithOne(e => e.User)
                       .HasForeignKey(uc => uc.UserId)
                       .IsRequired();

                // Each User can have many UserLogins
                builder.HasMany(e => e.Logins)
                       .WithOne(e => e.User)
                       .HasForeignKey(ul => ul.UserId)
                       .IsRequired();

                // Each User can have many UserTokens
                builder.HasMany(e => e.Tokens)
                       .WithOne(e => e.User)
                       .HasForeignKey(ut => ut.UserId)
                       .IsRequired();

                // Each User can have many entries in the UserRole join table
                builder.HasMany(e => e.UserRoles)
                       .WithOne(e => e.User)
                       .HasForeignKey(ur => ur.UserId)
                       .IsRequired();

                builder.ToTable("Users");
            });

            modelBuilder.Entity<Role>(builder =>
            {
                // Each Role can have many entries in the UserRole join table
                builder.HasMany(e => e.UserRoles)
                       .WithOne(e => e.Role)
                       .HasForeignKey(ur => ur.RoleId)
                       .IsRequired();

                // Each Role can have many associated RoleClaims
                builder.HasMany(e => e.RoleClaims)
                       .WithOne(e => e.Role)
                       .HasForeignKey(rc => rc.RoleId)
                       .IsRequired();

                builder.ToTable("Roles");
            });

            modelBuilder.Entity<RoleClaim>()
                        .ToTable("RoleClaims");

            modelBuilder.Entity<UserClaim>()
                        .ToTable("UserClaims");

            modelBuilder.Entity<UserLogin>()
                        .ToTable("UserLogins");

            modelBuilder.Entity<UserRole>()
                        .ToTable("UserRoles");

            modelBuilder.Entity<UserToken>()
                        .ToTable("UserTokens");
        }
    }
}