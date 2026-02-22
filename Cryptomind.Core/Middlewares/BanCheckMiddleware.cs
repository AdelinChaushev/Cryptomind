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
			var path = context.Request.Path.Value?.ToLower();

			if (path.StartsWith("/api/auth/logout"))
			{
				await _next(context);
				return;
			}

			var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

			if (authResult.Succeeded && authResult.Principal != null)
			{
				var userId = authResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (userId != null)
				{
					var user = await userManager.FindByIdAsync(userId);

					if (user != null && user.IsBanned)
					{
						context.Response.StatusCode = 403;
						context.Response.ContentType = "application/json";
						var json = System.Text.Json.JsonSerializer.Serialize(new { message = "Your account is banned, reason: " + user.BanReason });
						await context.Response.WriteAsync(json);
						return;
					}
				}
			}

			await _next(context);
		}
	}
}