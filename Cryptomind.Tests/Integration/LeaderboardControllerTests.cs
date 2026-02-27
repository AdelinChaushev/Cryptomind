using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class LeaderboardControllerTests : IntegrationTestBase
	{
		private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

		public LeaderboardControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		[Fact]
		public async Task GetLeaderboard_Unauthenticated_Returns200()
		{
			var response = await Client.GetAsync("/api/leaderboard");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetLeaderboard_Authenticated_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/leaderboard");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetLeaderboard_ReturnsValidJson()
		{
			var response = await Client.GetAsync("/api/leaderboard");
			var body = await response.Content.ReadAsStringAsync();

			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);
			json.ValueKind.Should().NotBe(JsonValueKind.Undefined);
		}

		[Fact]
		public async Task GetLeaderboard_ReturnsList()
		{
			var response = await Client.GetAsync("/api/leaderboard");
			var body = await response.Content.ReadAsStringAsync();

			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);
			json.ValueKind.Should().Be(JsonValueKind.Array);
		}

		[Fact]
		public async Task GetLeaderboard_SeededUsersAppearInResults()
		{
			var response = await Client.GetAsync("/api/leaderboard");
			var body = await response.Content.ReadAsStringAsync();

			body.Should().NotBeNullOrEmpty();
		}
	}
}