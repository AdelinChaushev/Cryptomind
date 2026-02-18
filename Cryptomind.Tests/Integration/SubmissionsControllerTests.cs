using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class SubmissionControllerTests : IntegrationTestBase
	{
		public SubmissionControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		[Fact]
		public async Task GetAllSubmissions_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/submissions");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetAllSubmissions_AuthenticatedUser_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/submissions");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAllSubmissions_ReturnsBodyWithCiphersAndAnswers()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/submissions");
			var content = await response.Content.ReadAsStringAsync();

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			content.Should().Contain("ciphers");
			content.Should().Contain("answers");
		}

		[Fact]
		public async Task GetAllSubmissions_NewUser_ReturnsEmptyCollections()
		{
			var freshClient = await RegisterAndGetClientAsync();

			var response = await freshClient.GetAsync("/api/submissions");
			var content = await response.Content.ReadAsStringAsync();

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			// A brand new user has no submissions yet
			content.Should().Contain("[]");
		}

		[Fact]
		public async Task GetAllSubmissions_AdminUser_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();

			var response = await adminClient.GetAsync("/api/submissions");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}
	}
}