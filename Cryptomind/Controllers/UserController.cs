using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/user")]
	[ApiController]
	public class UserController(IUserService userService) : ControllerBase
    {
		[Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetUserRoles()
        {
            try
            {
				var roles = await userService.GetRolesUsers(GetUserId());
				return Ok(roles);
			}
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("get-account-info")]
        public async Task<IActionResult> GetAccountInfo()
        {
			try
			{
				var user = await userService.GetUserAccountInfo(GetUserId());
				return Ok(user);
			}
            catch (Exception ex)
            {
				return BadRequest(ex.Message);
			}
		}
        private string GetUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
