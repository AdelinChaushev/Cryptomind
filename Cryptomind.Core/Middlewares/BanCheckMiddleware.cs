using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Cryptomind.Core.Middlewares
{
	public class BanCheckMiddleware
	{
		private readonly RequestDelegate _next;

		public BanCheckMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
		{
			var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

			Console.WriteLine("=== MIDDLEWARE HIT ===");
			Console.WriteLine($"AuthResult succeeded: {authResult.Succeeded}");

			if (authResult.Succeeded && authResult.Principal != null)
			{
				var userId = authResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				Console.WriteLine($"UserId: {userId}");

				if (userId != null)
				{
					var user = await userManager.FindByIdAsync(userId);

					if (user != null && user.IsBanned)
					{
						context.Response.StatusCode = 403;
						context.Response.ContentType = "application/json";
						var json = System.Text.Json.JsonSerializer.Serialize(new { message = "Your account has been banned." });
						await context.Response.WriteAsync(json);
						return;
					}
				}
			}

			await _next(context);
		}
	}
}