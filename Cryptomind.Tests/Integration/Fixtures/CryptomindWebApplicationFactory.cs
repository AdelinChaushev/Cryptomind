using Cryptomind.Core.Contracts;
using Cryptomind.Core.Hubs;
using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptomind.Tests.Integration.Fixtures
{
	public class CryptomindWebApplicationFactory : WebApplicationFactory<Program>
	{
		private readonly string _dbName = "CryptomindTestDb_" + Guid.NewGuid();
		private bool _seeded = false;

		private const string JwtSecret = "test-secret-key-long-enough-for-hmac-sha256-algorithm";
		private const string JwtIssuer = "test-issuer";
		private const string JwtAudience = "test-audience";

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			Environment.SetEnvironmentVariable("JWT_SECRET", JwtSecret);
			Environment.SetEnvironmentVariable("JWT_ISSUER", JwtIssuer);
			Environment.SetEnvironmentVariable("JWT_AUDIENCE", JwtAudience);
			Environment.SetEnvironmentVariable("OPENAI_API_URL", "https://api.openai.com/v1");
			Environment.SetEnvironmentVariable("OPENAI_API_KEY", "test-api-key");
			Environment.SetEnvironmentVariable("VALIDATION_MODEL", "gpt-4o-mini");
			Environment.SetEnvironmentVariable("EDUCATIONAL_MODEL", "gpt-4o");
			Environment.SetEnvironmentVariable("ML_URL", "http://localhost:5002");
			Environment.SetEnvironmentVariable("OCR_URL", "http://localhost:5001");

			builder.ConfigureAppConfiguration((context, config) =>
			{
				config.AddInMemoryCollection(new Dictionary<string, string?>
				{
					["ConnectionStrings:DefaultConnection"] = "ignored-replaced-by-inmemory",
				});
			});

			builder.ConfigureServices((context, services) =>
			{
				var mockBadge = new Mock<IBadgeService>();
				mockBadge
					.Setup(b => b.CheckBadgesByCategory(It.IsAny<string>(), It.IsAny<BadgeCategory>()))
					.Returns(Task.CompletedTask);

				services.RemoveAll<IBadgeService>();
				services.AddScoped(_ => mockBadge.Object);

				services.RemoveAll<DbContextOptions<CryptomindDbContext>>();
				services.RemoveAll<CryptomindDbContext>();

				services.AddDbContext<CryptomindDbContext>(options =>
				{
					options.UseInMemoryDatabase(_dbName);
				});

				services.Configure<CookiePolicyOptions>(options =>
				{
					options.MinimumSameSitePolicy = SameSiteMode.None;
				});

				var mockLLM = new Mock<ILLMService>();
				mockLLM
					.Setup(l => l.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()))
					.ReturnsAsync("This is a test hint.");

				services.RemoveAll<ILLMService>();
				services.AddScoped(_ => mockLLM.Object);

				services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
				{
					options.Cookie.SecurePolicy = CookieSecurePolicy.None;
				});

				var mockClients = new Mock<IHubClients>();
				var mockClientProxy = new Mock<IClientProxy>();
				mockClients
					.Setup(c => c.Group(It.IsAny<string>()))
					.Returns(mockClientProxy.Object);
				mockClientProxy
					.Setup(c => c.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);
				var mockHub = new Mock<IHubContext<NotificationHub>>();
				mockHub.Setup(h => h.Clients).Returns(mockClients.Object);
				services.RemoveAll<IHubContext<NotificationHub>>();
				services.AddSingleton(mockHub.Object);

				services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
				{
					var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
					options.TokenValidationParameters.IssuerSigningKey = key;
					options.TokenValidationParameters.ValidIssuer = JwtIssuer;
					options.TokenValidationParameters.ValidAudience = JwtAudience;

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

			builder.UseEnvironment("Development");
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

		public IServiceScope CreateScope() => Services.CreateScope();

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