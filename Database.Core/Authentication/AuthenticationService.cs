using Database.Core.Exceptions;
using Database.Models.Dtos;
using Database.Models.Entities;
using Database.Models.Interfaces;
using Database.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Database.Core.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IService<User> userService;
        private readonly IConfiguration configuration;

        public AuthenticationService(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager, IService<User> userService)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userService = userService;
        }

        public async Task<bool> SignOut(SignOutViewModel model)
        {
            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                return false;
            }

            await signInManager.SignOutAsync();

            return true;
        }

        public async Task<UserDto> SignInAsync(string username, string password)
        {
            var user = await userService.GetSingleByAsync(user => user.UserName.ToLower() == username.ToLower(),
                                                          user => user.Include(user => user.UserRoles).ThenInclude(user => user.Role));

            if (user == null)
            {
                return null;
            }

            var verified = await userManager.IsEmailConfirmedAsync(user);

            if (!verified)
            {
                throw new BadRequestException("User is not verified!");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
            {
                return null;
            }

            var roles = await userManager.GetRolesAsync(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var descriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.Now.AddMinutes(600),
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(descriptor);
            var serialized = handler.WriteToken(token);

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                Prefix = user.Prefix,
                LastName = user.LastName,
                Token = serialized,
                ValidTo = token.ValidTo,
                Roles = user.UserRoles.Select(x => new RoleDto
                {
                    Id = x.Role.Id,
                    Name = x.Role.Name
                }).ToList()
            };

            return dto;
        }
    }
}