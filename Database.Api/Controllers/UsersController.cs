using Database.Data;
using Database.Models.Entities;
using Database.Models.Interfaces;
using Database.Models.Models;
using Database.Models.Statics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Database.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseDbContext dbContext;
        private readonly IUserService service;

        public UsersController(DatabaseDbContext dbContext, IUserService service)
        {
            this.dbContext = dbContext;
            this.service = service;
        }

        [HttpGet]
        [Authorize(Roles = Roles.User)]
        public async Task<IActionResult> Get([FromQuery] SearchPagingViewModel model)
        {
            var (users, count) = await service.GetAll(model);

            return Ok(new { users, count });
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Error: { ModelState }");
            }

            var user = new User
            {
                FirstName = model.FirstName,
                Prefix = model.Prefix,
                LastName = model.LastName,
                UserName = model.Email,
                Email = model.Email
            };

            await service.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}