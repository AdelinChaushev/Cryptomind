using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class UserControllerTests : IntegrationTestBase
	{
		private static readonly JsonSerializerOptions JsonOpts = new()
		{
			PropertyNameCaseInsensitive = true
		};

		public UserControllerTests(CryptomindWebApplicationFactory factory) : base(factory)
		{
		}

		#region Get Roles

		[Fact]
		public async Task GetUserRoles_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/user/get-roles");
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetUserRoles_RegularUser_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/user/get-roles");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetUserRoles_AdminUser_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/user/get-roles");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetUserRoles_RegularUser_ContainsUserRole()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/user/get-roles");
			var body = await response.Content.ReadAsStringAsync();
			body.Should().Contain("User");
		}

		[Fact]
		public async Task GetUserRoles_AdminUser_ContainsAdminRole()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/user/get-roles");
			var body = await response.Content.ReadAsStringAsync();
			body.Should().Contain("Admin");
		}

		[Fact]
		public async Task GetUserRoles_FreshRegisteredUser_ContainsUserRole()
		{
			var freshClient = await RegisterAndGetClientAsync();
			var response = await freshClient.GetAsync("/api/user/get-roles");
			var body = await response.Content.ReadAsStringAsync();
			body.Should().Contain("User");
		}

		#endregion

		#region Get Account Info

		[Fact]
		public async Task GetAccountInfo_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/user/get-account-info");
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetAccountInfo_RegularUser_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/user/get-account-info");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAccountInfo_AdminUser_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/user/get-account-info");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAccountInfo_RegularUser_ResponseContainsEmail()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/user/get-account-info");
			var body = await response.Content.ReadAsStringAsync();
			body.Should().Contain("user@cryptomind.com");
		}

		[Fact]
		public async Task GetAccountInfo_FreshUser_ResponseContainsTheirEmail()
		{
			var email = $"fresh_{System.Guid.NewGuid():N}@test.com";
			var freshClient = await RegisterAndGetClientAsync(email: email);
			var response = await freshClient.GetAsync("/api/user/get-account-info");
			var body = await response.Content.ReadAsStringAsync();
			body.Should().Contain(email);
		}

		#endregion
	}
}