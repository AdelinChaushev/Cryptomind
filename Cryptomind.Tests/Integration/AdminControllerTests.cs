using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class AdminControllerTests : IntegrationTestBase
	{
		public AdminControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		#region Authorization

		[Fact]
		public async Task Dashboard_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/admin/dashboard");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Dashboard_RegularUser_Returns403()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/admin/dashboard");

			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task Dashboard_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/dashboard");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		// This pattern (unauthenticated → 401, user → 403, admin → 200) applies to every admin endpoint.
		// The tests below focus on business logic and assume the role guard works the same way.

		#endregion

		#region Pending Ciphers

		[Fact]
		public async Task GetPendingCiphers_Admin_Returns200WithList()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/pending-ciphers");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetPendingCiphers_RegularUser_Returns403()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/admin/pending-ciphers");

			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		#endregion

		#region Approved Ciphers

		[Fact]
		public async Task GetApprovedCiphers_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/approved-ciphers");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetApprovedCiphers_WithFilters_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/approved-ciphers?sortBy=date&order=desc");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		#endregion

		#region Get Single Cipher

		[Fact]
		public async Task GetCipher_NonExistentId_Returns400()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/cipher/99999");

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Approve Cipher

		[Fact]
		public async Task ApproveCipher_NonExistentId_Returns400()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var model = new
			{
				challengeType = "Standard",
				title = "Test Cipher",
				description = "A test cipher"
			};

			var response = await adminClient.PutAsJsonAsync("/api/admin/cipher/99999/approve", model);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Reject Cipher

		[Fact]
		public async Task RejectCipher_NonExistentId_Returns400()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.PutAsJsonAsync("/api/admin/cipher/99999/reject", "Not valid content");

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Restore Cipher

		[Fact]
		public async Task RestoreCipher_NonExistentId_Returns400()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.PutAsync("/api/admin/cipher/99999/restore", null);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Users

		[Fact]
		public async Task GetAllUsers_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/users");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAllUsers_RegularUser_Returns403()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/admin/users");

			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task GetUser_NonExistentId_Returns400()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/user/nonexistent-id-000");

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task BanUser_NonExistentId_Returns400()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("reason", "Test ban reason")
			});

			var response = await adminClient.PutAsync("/api/admin/user/nonexistent-user-id/ban", content);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task UnbanUser_NonExistentId_Returns400()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.PutAsync("/api/admin/user/nonexistent-user-id/unban", null);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Pending Answers

		[Fact]
		public async Task GetPendingAnswers_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/pending-answer-suggestions");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAnswer_NonExistentId_Returns400()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/admin/answer/99999");

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion
	}
}