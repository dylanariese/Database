using Database.Data;
using Database.Models.Dtos;
using Database.Models.Entities;
using Database.Models.Interfaces;
using Database.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Core.Users
{
    public class UserService : Service<User>, IUserService
    {
        private readonly DatabaseDbContext dbContext;

        public UserService(DatabaseDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<(List<UserDto> Users, int Count)> GetAll(SearchPagingViewModel model)
        {
            var users = GetAll(user => user.Include(user => user.UserRoles).ThenInclude(user => user.Role));

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                users = users.Where(user => user.FirstName.Contains(model.SearchTerm) ||
                                            user.LastName.Contains(model.SearchTerm) ||
                                            user.Email.Contains(model.SearchTerm) ||
                                            user.UserName.Contains(model.SearchTerm));
            }

            var all = users.OrderBy(x => x.LastName);

            var filtered = await all.Skip((model.Page - 1) * model.Size).Take(model.Size).ToListAsync();
            var count = await all.CountAsync();

            var dtos = filtered.Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                Prefix = user.Prefix,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.UserRoles.Select(userRole => new RoleDto
                {
                    Id = userRole.Role.Id,
                    Name = userRole.Role.Name
                }).ToList(),
            }).ToList();

            return (Users: dtos, Count: count);
        }
    }
}