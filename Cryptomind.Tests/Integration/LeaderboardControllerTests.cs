using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class LeaderboardControllerTests : IntegrationTestBase
	{
		public LeaderboardControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		[Fact]
		public async Task GetLeaderboard_Unauthenticated_Returns200()
		{
			var response = await Client.GetAsync("/api/leaderboard");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetLeaderboard_AuthenticatedUser_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/leaderboard");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetLeaderboard_ReturnsValidJsonBody()
		{
			var response = await Client.GetAsync("/api/leaderboard");
			var content = await response.Content.ReadAsStringAsync();

			response.StatusCode.Should().Be(HttpStatusCode.OK);
			content.Should().NotBeNullOrEmpty();
		}
	}
}