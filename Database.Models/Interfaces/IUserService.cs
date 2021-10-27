using Database.Models.Dtos;
using Database.Models.Entities;
using Database.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database.Models.Interfaces
{
    public interface IUserService : IService<User>
    {
        Task<(List<UserDto> Users, int Count)> GetAll(SearchPagingViewModel model);
    }
}