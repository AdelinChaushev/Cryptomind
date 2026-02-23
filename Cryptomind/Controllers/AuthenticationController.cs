using Cryptomind.Common.ViewModels.AuthenticationViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthenticationController(IAuthService authService , IUserService userService) : ControllerBase
	{
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
		{
			ApplicationUser? user = await authService.CreateUser(
				model.Username,
				model.Email,
				model.Password);

			if (user == null)
			{
				return BadRequest("User creation failed");
			}

			string token = await authService.GenerateJSONWebToken(user);
			AddCookie(token);
			return Ok(new { token });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel model)
		{
			ApplicationUser user = await authService.Authenticate(model.Email, model.Password);
			string token = await authService.GenerateJSONWebToken(user);
			AddCookie(token);
			return Ok(new { token });
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
				Expires = DateTimeOffset.UtcNow.AddDays(-1)
			});

			return Ok("Logged out successfully");
		}

		[HttpPost("deactivate")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> DeactivateAccount()
		{
			string? userId = GetUserId();

			await authService.DeactivateAccount(userId);

			Response.Cookies.Delete("token", new CookieOptions
			{
				HttpOnly = true,
				IsEssential = true,
				SameSite = SameSiteMode.None,
				Secure = true,
				Expires = DateTimeOffset.UtcNow.AddDays(-1)
			});

			return Ok("Account deactivated");
		}

		#region Private methods
		private void AddCookie(string token)
		{
			HttpContext.Response.Cookies.Append("token", token, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				IsEssential = true,
				Expires = DateTimeOffset.UtcNow.AddHours(3),
			});
		}

		private string? GetUserId()
			=> User.FindFirstValue(ClaimTypes.NameIdentifier);
		#endregion
	}
}