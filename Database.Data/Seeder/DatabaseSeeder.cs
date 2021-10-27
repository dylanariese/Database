using Database.Data;
using Database.Models.Entities;
using Database.Models.Statics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Data.Seeder
{
    public class DatabaseSeeder
    {
        private readonly DatabaseDbContext context;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;

        public DatabaseSeeder(DatabaseDbContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            this.context = context;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task Seed()
        {
            await context.Database.MigrateAsync();

            if (!context.Roles.Any())
            {
                var roleStore = new RoleStore<Role>(context);

                var userRole = new Role { Name = Roles.User, NormalizedName = Roles.User };

                await roleStore.CreateAsync(userRole);

                await context.SaveChangesAsync();
            }

            var user = await userManager.FindByEmailAsync("user@test.nl");

            if (user == null)
            {
                var created = new User
                {
                    UserName = "user@test.nl",
                    Email = "user@test.nl",
                    FirstName = "Test",
                    LastName = "Test",
                };

                var result = await userManager.CreateAsync(created, configuration["Password"]);

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Failed to create user!");
                }

                var roleResult = await userManager.AddToRoleAsync(created, Roles.User);

                if (roleResult != IdentityResult.Success)
                {
                    throw new InvalidOperationException($"Failed to add user to role!");
                }

                await context.SaveChangesAsync();

                var token = await userManager.GenerateEmailConfirmationTokenAsync(created);
                var confirmation = await userManager.ConfirmEmailAsync(created, token);

                if (confirmation != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Failed to verify email!");
                }

                await context.SaveChangesAsync();
            }                        
        }
    }
}