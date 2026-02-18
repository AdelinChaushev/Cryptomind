using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class UserControllerTests : IntegrationTestBase
	{
		public UserControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		#region Get Roles

		[Fact]
		public async Task GetUserRoles_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/user/get-roles");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetUserRoles_AuthenticatedUser_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/user/get-roles");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetUserRoles_RegularUser_ReturnsUserRole()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/user/get-roles");
			var content = await response.Content.ReadAsStringAsync();

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			content.Should().Contain("User");
		}

		[Fact]
		public async Task GetUserRoles_AdminUser_ReturnsAdminRole()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/user/get-roles");
			var content = await response.Content.ReadAsStringAsync();

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			content.Should().Contain("Admin");
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
		public async Task GetAccountInfo_AuthenticatedUser_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/user/get-account-info");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAccountInfo_ReturnsNonEmptyBody()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/user/get-account-info");
			var content = await response.Content.ReadAsStringAsync();

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			content.Should().NotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task GetAccountInfo_AdminUser_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/user/get-account-info");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAccountInfo_NewRegisteredUser_Returns200()
		{
			var freshClient = await RegisterAndGetClientAsync();

			var response = await freshClient.GetAsync("/api/user/get-account-info");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		#endregion
	}
}