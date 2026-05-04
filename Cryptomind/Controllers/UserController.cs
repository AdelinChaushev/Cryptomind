using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/user")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ApiController]
	public class UserController(
		IUserService userService,
		IBadgeService badgeService) : ControllerBase
	{
		[HttpGet("get-roles")]
		public async Task<IActionResult> GetUserRoles()
		{
			var roles = await userService.GetRolesUsers(GetUserId());
			var email = await userService.GetEmail(GetUserId());
			return Ok(new
			{
				Roles = roles,
				Email = email
			});

		}

		[HttpGet("get-account-info")]
		public async Task<IActionResult> GetAccountInfo()
		{
			var user = await userService.GetUserAccountInfo(GetUserId());
			return Ok(user);
		}
		
		[HttpPut("7f1a3b82-9e4d-4c5a-b2f1-6d8e9a0c3f4b")] //This endpoint is for when a user reveals the hidden secret on the notebook
		public async Task<IActionResult> RevealSecret()
		{
			string userId = GetUserId();
			var result = await userService.RevealSecret(userId);
			await badgeService.CheckBadgesByCategory(userId, Data.Enums.BadgeCategory.OnSecretRevealing);
			
			return Ok(result);
		}
		private string GetUserId()
			=> User.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
