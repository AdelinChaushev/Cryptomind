using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/user")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ApiController]
	public class UserController(IUserService userService) : ControllerBase
    {
		
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetUserRoles()
        {
			var roles = await userService.GetRolesUsers(GetUserId());
			return Ok(roles);

        }

        [HttpGet("get-account-info")]
        public async Task<IActionResult> GetAccountInfo()
        {
			var user = await userService.GetUserAccountInfo(GetUserId());
			return Ok(user);
		}
        private string GetUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
