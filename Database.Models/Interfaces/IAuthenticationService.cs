using Database.Models.Dtos;
using Database.Models.Models;
using System.Threading.Tasks;

namespace Database.Models.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UserDto> SignInAsync(string username, string password);

        Task<bool> SignOut(SignOutViewModel email);
    }
}