using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
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
	public class NotificationControllerTests : IntegrationTestBase
	{
		private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

		public NotificationControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		// -------------------------------------------------------------------------
		// Seed helpers
		// -------------------------------------------------------------------------

		private async Task<Notification> SeedNotificationAsync(string userId)
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();

			var notification = new Notification
			{
				UserId = userId,
				Type = NotificationType.CipherApproved,
				Message = "Test notification",
				Link = "test-link",
				IsRead = false,
				CreatedAt = DateTime.UtcNow
			};

			db.Notifications.Add(notification);
			await db.SaveChangesAsync();
			return notification;
		}

		// =========================================================================
		// GET ALL NOTIFICATIONS
		// =========================================================================

		[Fact]
		public async Task GetAllNotifications_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/notifications");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetAllNotifications_Authenticated_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/notifications");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAllNotifications_ResponseContainsExpectedFields()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/notifications");
			var body = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);

			json.TryGetProperty("notifications", out _).Should().BeTrue();
			json.TryGetProperty("unreadCount", out _).Should().BeTrue();
		}

		[Fact]
		public async Task GetAllNotifications_WithSeededNotification_ReturnsIt()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			await SeedNotificationAsync(user.Id);

			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/notifications");
			var body = await response.Content.ReadAsStringAsync();

			body.Should().Contain("Test notification");
		}

		[Fact]
		public async Task GetAllNotifications_UnreadCountIsCorrect()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			await SeedNotificationAsync(user.Id);

			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/notifications");
			var body = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);

			json.GetProperty("unreadCount").GetInt32().Should().BeGreaterThan(0);
		}

		// =========================================================================
		// MARK ALL AS READ
		// =========================================================================

		[Fact]
		public async Task MarkAsRead_Unauthenticated_Returns401()
		{
			var response = await Client.PutAsync("/api/notifications/mark-as-read", null);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task MarkAsRead_Authenticated_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.PutAsync("/api/notifications/mark-as-read", null);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task MarkAsRead_Authenticated_NotificationsMarkedAsRead()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var notification = await SeedNotificationAsync(user.Id);

			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.PutAsync("/api/notifications/mark-as-read", null);
			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var verifyScope = Factory.CreateScope();
			var verifyDb = verifyScope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var updated = await verifyDb.Notifications.FindAsync(notification.Id);
			updated!.IsRead.Should().BeTrue();
		}

		// =========================================================================
		// MARK SINGLE AS READ
		// =========================================================================

		[Fact]
		public async Task MarkAsReadSingle_Unauthenticated_Returns401()
		{
			var response = await Client.PutAsync("/api/notifications/mark-as-read/1", null);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task MarkAsReadSingle_NonExistentId_Returns404()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.PutAsync("/api/notifications/mark-as-read/99999", null);

			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task MarkAsReadSingle_ExistingNotification_Returns200()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var notification = await SeedNotificationAsync(user.Id);

			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.PutAsync($"/api/notifications/mark-as-read/{notification.Id}", null);
			var body = await response.Content.ReadAsStringAsync();

			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
		}

		[Fact]
		public async Task MarkAsReadSingle_ExistingNotification_IsReadBecomesTrue()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var notification = await SeedNotificationAsync(user.Id);

			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.PutAsync($"/api/notifications/mark-as-read/{notification.Id}", null);
			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var verifyScope = Factory.CreateScope();
			var verifyDb = verifyScope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var updated = await verifyDb.Notifications.FindAsync(notification.Id);
			updated!.IsRead.Should().BeTrue();
		}

		[Fact]
		public async Task MarkAsReadSingle_OtherUsersNotification_Returns404()
		{
			// Seed a notification belonging to the admin
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var admin = await db.Users.FirstAsync(u => u.Email == "admin@cryptomind.com");

			var notification = await SeedNotificationAsync(admin.Id);

			// Try to mark it as read with the regular user
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.PutAsync($"/api/notifications/mark-as-read/{notification.Id}", null);

			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}
	}
}