using Cryptomind.Data;
using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class AuthenticationControllerTests : IntegrationTestBase
	{
		private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

		public AuthenticationControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		// =========================================================================
		// REGISTER
		// =========================================================================

		[Fact]
		public async Task Register_ValidModel_Returns200()
		{
			var model = new
			{
				email = $"test_{Guid.NewGuid():N}@test.com",
				username = $"user_{Guid.NewGuid():N}".Substring(0, 16),
				password = "Test123!",
				confirmPassword = "Test123!"
			};

			var response = await Client.PostAsJsonAsync("/api/auth/register", model);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Register_ValidModel_ReturnsToken()
		{
			var model = new
			{
				email = $"test_{Guid.NewGuid():N}@test.com",
				username = $"user_{Guid.NewGuid():N}".Substring(0, 16),
				password = "Test123!",
				confirmPassword = "Test123!"
			};

			var response = await Client.PostAsJsonAsync("/api/auth/register", model);
			var body = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);

			json.TryGetProperty("token", out var token).Should().BeTrue();
			token.GetString().Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task Register_ValidModel_UserExistsInDatabase()
		{
			var email = $"test_{Guid.NewGuid():N}@test.com";
			var model = new
			{
				email,
				username = $"user_{Guid.NewGuid():N}".Substring(0, 16),
				password = "Test123!",
				confirmPassword = "Test123!"
			};

			await Client.PostAsJsonAsync("/api/auth/register", model);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
			user.Should().NotBeNull();
		}

		[Fact]
		public async Task Register_DuplicateEmail_ReturnsBadRequest()
		{
			var model = new
			{
				email = "user@cryptomind.com", // already seeded
				username = $"user_{Guid.NewGuid():N}".Substring(0, 16),
				password = "Test123!",
				confirmPassword = "Test123!"
			};

			var response = await Client.PostAsJsonAsync("/api/auth/register", model);

			response.StatusCode.Should().Be(HttpStatusCode.Conflict);
		}

		[Fact]
		public async Task Register_InvalidModel_ReturnsBadRequest()
		{
			var model = new
			{
				email = "not-an-email",
				username = "",
				password = "weak",
				confirmPassword = "weak"
			};

			var response = await Client.PostAsJsonAsync("/api/auth/register", model);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Register_MissingFields_ReturnsBadRequest()
		{
			var model = new { email = "" };

			var response = await Client.PostAsJsonAsync("/api/auth/register", model);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		// =========================================================================
		// LOGIN
		// =========================================================================

		[Fact]
		public async Task Login_ValidCredentials_Returns200()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "user@cryptomind.com",
				password = "User123!"
			});

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Login_ValidCredentials_ReturnsUserInfo()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "user@cryptomind.com",
				password = "User123!"
			});

			var body = await response.Content.ReadAsStringAsync();
			body.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task Login_AdminCredentials_Returns200()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "admin@cryptomind.com",
				password = "Admin123!"
			});

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Login_WrongPassword_ReturnsError()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "user@cryptomind.com",
				password = "WrongPassword!"
			});

			response.StatusCode.Should().NotBe(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Login_NonExistentEmail_ReturnsError()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "nonexistent@test.com",
				password = "Test123!"
			});

			response.StatusCode.Should().NotBe(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Login_BannedUser_Returns403()
		{
			// Register a fresh user then ban them
			var email = $"banned_{Guid.NewGuid():N}@test.com";
			var username = $"banned_{Guid.NewGuid():N}".Substring(0, 16);
			await Client.PostAsJsonAsync("/api/auth/register", new
			{
				email,
				username,
				password = "Test123!",
				confirmPassword = "Test123!"
			});

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == email);
			user.IsBanned = true;
			user.BanReason = "Test ban";
			await db.SaveChangesAsync();

			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email,
				password = "Test123!"
			});

			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		// =========================================================================
		// LOGOUT
		// =========================================================================

		[Fact]
		public async Task Logout_Authenticated_Returns200()
		{
			var authClient = await GetAuthenticatedUserClientAsync();

			var response = await authClient.PostAsync("/api/auth/logout", null);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Logout_Unauthenticated_Returns401()
		{
			var response = await Client.PostAsync("/api/auth/logout", null);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		// =========================================================================
		// DEACTIVATE
		// =========================================================================

		[Fact]
		public async Task DeactivateAccount_Authenticated_Returns200()
		{
			var freshClient = await RegisterAndGetClientAsync();

			var response = await freshClient.PostAsync("/api/auth/deactivate", null);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task DeactivateAccount_Unauthenticated_Returns401()
		{
			var response = await Client.PostAsync("/api/auth/deactivate", null);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task DeactivateAccount_Authenticated_UserIsDeactivatedInDatabase()
		{
			var email = $"deactivate_{Guid.NewGuid():N}@test.com";
			var freshClient = await RegisterAndGetClientAsync(email: email);

			var response = await freshClient.PostAsync("/api/auth/deactivate", null);
			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == email);
			user.IsDeactivated.Should().BeTrue();
		}
	}
}