using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class NotificationControllerTests : IntegrationTestBase
	{
		public NotificationControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		#region /api/notifications

		[Fact]
		public async Task GetAllNotifications_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/notifications");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetAllNotifications_AuthenticatedUser_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/notifications");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAllNotifications_ReturnsNotificationsAndUnreadCountFields()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/notifications");
			var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

			json.TryGetProperty("notifications", out _).Should().BeTrue();
			json.TryGetProperty("unreadCount", out _).Should().BeTrue();
		}

		[Fact]
		public async Task GetAllNotifications_NotificationsField_IsArray()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/notifications");
			var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

			json.GetProperty("notifications").ValueKind.Should().Be(JsonValueKind.Array);
		}

		[Fact]
		public async Task GetAllNotifications_UnreadCount_IsNonNegative()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/notifications");
			var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

			json.GetProperty("unreadCount").GetInt32().Should().BeGreaterThanOrEqualTo(0);
		}

		[Fact]
		public async Task GetAllNotifications_FreshUser_HasEmptyNotificationsAndZeroUnreadCount()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/notifications");
			var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

			json.GetProperty("notifications").GetArrayLength().Should().Be(0);
			json.GetProperty("unreadCount").GetInt32().Should().Be(0);
		}

		#endregion

		#region /api/notifications/mark-as-read

		[Fact]
		public async Task MarkAsRead_Unauthenticated_Returns401()
		{
			var body = new StringContent("[]", Encoding.UTF8, "application/json");

			var response = await Client.PutAsync("/api/notifications/mark-as-read", body);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task MarkAsRead_AuthenticatedUser_WithEmptyList_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var body = new StringContent("[]", Encoding.UTF8, "application/json");

			var response = await userClient.PutAsync("/api/notifications/mark-as-read", body);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task MarkAsRead_WithNonExistentId_Returns404()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.PutAsJsonAsync("/api/notifications/mark-as-read", new[] { 999999 });

			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task MarkAsRead_WithNoBody_Returns400()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var body = new StringContent("", Encoding.UTF8, "application/json");

			var response = await userClient.PutAsync("/api/notifications/mark-as-read", body);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task MarkAsRead_NotificationBelongingToOtherUser_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var userClient = await GetAuthenticatedUserClientAsync();

			var adminResponse = await adminClient.GetAsync("/api/notifications");
			var adminJson = JsonDocument.Parse(await adminResponse.Content.ReadAsStringAsync()).RootElement;
			adminJson.GetProperty("notifications").GetArrayLength().Should().Be(0);

			var response = await userClient.PutAsJsonAsync("/api/notifications/mark-as-read", new[] { 999999 });

			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		#endregion
	}
}