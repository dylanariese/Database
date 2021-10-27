using Database.Models.Interfaces;
using Database.Models.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Database.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("signin"), AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] AuthenticateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Error: { ModelState }");
            }

            var user = await authenticationService.SignInAsync(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        [HttpPost("signout")]
        public async Task<IActionResult> Signout([FromBody] SignOutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Error: { ModelState }");
            }

            var success = await authenticationService.SignOut(model);

            if (!success)
            {
                return BadRequest("error!");
            }

            return Ok();
        }
    }
}