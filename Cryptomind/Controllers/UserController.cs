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
                string token = await userService.GenerateJSONWebToken(user);
                AddCookie(token);
                return Ok(token);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel model)
		{
			//REMOVE THIS IN PRODUCTION!!!
			//model.Email = "admin@cryptomind.com";
			//model.Password = "Admin123!";

			if (!ModelState.IsValid)
			{
				return BadRequest("Invalid Credentials");
			}

			ApplicationUser? user = await userService.Authenticate(model.Email, model.Password);
			if (user == null)
			{
				return BadRequest("Invalid Credentials");
			}

			string token = await userService.GenerateJSONWebToken(user); // Added await

			AddCookie(token);
			return Ok(token);
		}

		[HttpPost("logout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
		public IActionResult Logout()
		{
			Response.Cookies.Delete("token", new CookieOptions
			{
				HttpOnly = true,
				IsEssential = true,
				SameSite = SameSiteMode.None,
				Secure = true,
				Expires = DateTime.Now.AddDays(-1)
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

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("get-account-info")]
        public async Task<IActionResult> GetAccountInfo()
        {
            var user = await userService.GetUserAccountInfo(GetUserId());
            return Ok(user);
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
