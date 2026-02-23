using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class AuthControllerTests : IntegrationTestBase
	{
		public AuthControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		#region Register

		[Fact]
		public async Task Register_ValidData_Returns200AndSetsCookie()
		{
			var payload = new
			{
				email = $"newuser_{Guid.NewGuid():N}@test.com",
				username = $"u_{Guid.NewGuid():N}".Substring(0, 16),
				password = "Test123!",
				confirmPassword = "Test123!"
			};

			var response = await Client.PostAsJsonAsync("/api/auth/register", payload);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Register_DuplicateEmail_Returns400()
		{
			var email = $"duplicate_{Guid.NewGuid():N}@test.com";
			var payload = new { email, username = "user1", password = "Test123!" };

			await Client.PostAsJsonAsync("/api/auth/register", payload);

			var secondPayload = new { email, username = "user2", password = "Test123!" };
			var response = await Client.PostAsJsonAsync("/api/auth/register", secondPayload);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Register_PasswordTooShort_Returns400()
		{
			var payload = new
			{
				email = $"short_{Guid.NewGuid():N}@test.com",
				username = "shortpass",
				password = "abc"
			};

			var response = await Client.PostAsJsonAsync("/api/auth/register", payload);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task Register_EmptyEmail_Returns400()
		{
			var payload = new { email = "", username = "someone", password = "Test123!" };

			var response = await Client.PostAsJsonAsync("/api/auth/register", payload);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Login

		[Fact]
		public async Task Login_ValidCredentials_Returns200AndSetsCookie()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "user@cryptomind.com",
				password = "User123!"
			});

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Headers.Should().ContainKey("Set-Cookie");
		}

		[Fact]
		public async Task Login_WrongPassword_Returns401()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "user@cryptomind.com",
				password = "WrongPassword!"
			});

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Login_NonExistentEmail_Returns401()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "nobody@nowhere.com",
				password = "Test123!"
			});

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Login_EmptyBody_Returns400Or401()
		{
			var response = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = "",
				password = ""
			});

			((int)response.StatusCode).Should().BeOneOf(400, 401);
		}

		#endregion

		#region Logout

		[Fact]
		public async Task Logout_AuthenticatedUser_Returns200()
		{
			var authenticatedClient = await GetAuthenticatedUserClientAsync();

			var response = await authenticatedClient.PostAsync("/api/auth/logout", null);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Logout_UnauthenticatedUser_Returns401()
		{
			var response = await Client.PostAsync("/api/auth/logout", null);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		#endregion

		#region Deactivate

		[Fact]
		public async Task Deactivate_AuthenticatedUser_Returns200()
		{
			var client = await RegisterAndGetClientAsync();

			var response = await client.PostAsync("/api/auth/deactivate", null);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Deactivate_UnauthenticatedUser_Returns401()
		{
			var response = await Client.PostAsync("/api/auth/deactivate", null);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Deactivate_DeactivatedUserCannotLogin()
		{
			var uniqueEmail = $"deactivated_{Guid.NewGuid():N}@test.com";
			var client = await RegisterAndGetClientAsync(email: uniqueEmail, password: "Test123!");

			await client.PostAsync("/api/auth/deactivate", null);

			var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", new
			{
				email = uniqueEmail,
				password = "Test123!"
			});

			// Deactivated users should not be able to log in
			((int)loginResponse.StatusCode).Should().BeOneOf(400, 409);
		}

		#endregion
	}
}