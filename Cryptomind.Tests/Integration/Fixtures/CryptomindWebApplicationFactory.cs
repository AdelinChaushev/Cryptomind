using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Tests.Integration.Fixtures
{
	public class CryptomindWebApplicationFactory : WebApplicationFactory<Program>
	{
		private readonly string _dbName = "CryptomindTestDb_" + Guid.NewGuid();
		private bool _seeded = false;
		private string? _jwtSecret;

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureAppConfiguration((context, config) =>
			{
				config.AddInMemoryCollection(new Dictionary<string, string?>
				{
					["JWT:Secret"] = "test-secret-key-long-enough-for-hmac-sha256-algorithm",
					["JWT:ValidAudience"] = "test-audience",
					["JWT:ValidIssuer"] = "test-issuer",
					["ConnectionStrings:DefaultConnection"] = "ignored-replaced-by-inmemory",
					["LLMService:ApiKey"] = "test-api-key"
				});
			});

			builder.ConfigureServices((context, services) =>
			{
				// Read whatever secret the app ended up with so we can sync validation to it
				_jwtSecret = context.Configuration["JWT:Secret"]
					?? "test-secret-key-long-enough-for-hmac-sha256-algorithm";

				services.RemoveAll<DbContextOptions<CryptomindDbContext>>();
				services.RemoveAll<CryptomindDbContext>();

				services.AddDbContext<CryptomindDbContext>(options =>
				{
					options.UseInMemoryDatabase(_dbName);
				});

				// Override the validation key to match whatever key the app is actually using
				// to sign tokens at runtime — this ensures generation and validation always match
				services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
				{
					var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret));
					options.TokenValidationParameters.IssuerSigningKey = key;

					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var cookie = context.Request.Cookies["token"];
							if (!string.IsNullOrEmpty(cookie))
								context.Token = cookie;
							return Task.CompletedTask;
						}
					};
				});
			});

			builder.UseEnvironment("Testing");
		}

		public HttpClient CreateSeededClient(WebApplicationFactoryClientOptions options)
		{
			var client = CreateClient(options);

			if (!_seeded)
			{
				using var scope = Services.CreateScope();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				SeedAsync(userManager, roleManager).GetAwaiter().GetResult();
				_seeded = true;
			}

			return client;
		}

		private static async Task SeedAsync(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			foreach (var role in new[] { "Admin", "User" })
			{
				if (!await roleManager.RoleExistsAsync(role))
					await roleManager.CreateAsync(new IdentityRole(role));
			}

			await CreateUserAsync(userManager, "admin@cryptomind.com", "Admin123!", "Admin");
			await CreateUserAsync(userManager, "user@cryptomind.com", "User123!", "User");
		}

		private static async Task CreateUserAsync(
			UserManager<ApplicationUser> userManager,
			string email,
			string password,
			string role)
		{
			if (await userManager.FindByEmailAsync(email) != null)
				return;

			var user = new ApplicationUser
			{
				UserName = email,
				Email = email,
				EmailConfirmed = true
			};

			var result = await userManager.CreateAsync(user, password);
			if (result.Succeeded)
				await userManager.AddToRoleAsync(user, role);
		}
	}
}