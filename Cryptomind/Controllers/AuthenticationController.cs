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
	public class AuthenticationController(IAuthService authService) : ControllerBase
	{
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
		{
			try
			{
				ApplicationUser? user = await authService.CreateUser(
					model.Username,
					model.Email,
					model.Password);

				if (user == null)
				{
					return BadRequest(new { error = "User creation failed" });
				}

				string token = await authService.GenerateJSONWebToken(user);
				AddCookie(token);
				return Ok(new { token });
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel model)
		{
			try
			{
				ApplicationUser? user = await authService.Authenticate(model.Email, model.Password);
				if (user == null)
				{
					return BadRequest(new { error = "Invalid Credentials" });
				}

				string token = await authService.GenerateJSONWebToken(user);
				AddCookie(token);
				return Ok(new { token });
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
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
				Expires = DateTimeOffset.Now.AddDays(-1)
			});

			return Ok(new { message = "Logged out successfully" });
		}

		[HttpPost("deactivate")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> DeactivateAccount()
		{
			try
			{
				string? userId = GetUserId();
				if (string.IsNullOrEmpty(userId))
				{
					return BadRequest(new { error = "User ID not found in token" });
				}

				await authService.DeactivateAccount(userId);
				return Ok(new { message = "Account deactivated" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
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
				Expires = DateTimeOffset.Now.AddHours(3),
			});
		}

		private string? GetUserId()
			=> User.FindFirstValue(ClaimTypes.NameIdentifier);
		#endregion
	}
}