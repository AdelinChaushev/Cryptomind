using Cryptomind.Common.AuthenticationViewModels;
using Cryptomind.Data.Entities;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
    {
        private IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                ApplicationUser? user = await userService.CreateUser(model.Username, model.Email, model.Password);
                if (user == null)
                {
                    return BadRequest();
                }
                string token = userService.GenerateJSONWebToken(user);
                AddCookie(token);
                return Ok(token);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
          

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid) 
            { 
                return BadRequest("Invalid Credentials");
            }
            ApplicationUser? user =  await userService.Authenticate(model.Email,model.Password);
            if (user == null)
            {
                return BadRequest("Invalid Credentials");
            }
            string token = userService.GenerateJSONWebToken(user);
            
            AddCookie(token);
            return Ok(token);

        }
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("token", new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Secure = true, // Set Secure if your cookie is Secure
                Expires = DateTime.Now.AddDays(-1) // Set expiration to a past date
            });
            return Ok();
        }

        [HttpPost("deactivate")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeactivateAccount()
        {
            await userService.DeactivateAccount(GetUserId());
            return Ok();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetUserRoles()
        {
            var roles = await userService.GetRolesUsers(GetUserId());
            return Ok(roles);
        }
        //Common methods
        private void AddCookie(string token)
        {
            HttpContext.Response.Cookies.Append("token", token, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Expires = DateTime.Now.AddHours(3),
            });
        }
        private string GetUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier); 
    }
}
