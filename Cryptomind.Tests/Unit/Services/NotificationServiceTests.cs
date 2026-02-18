using Cryptomind.Core.Hubs;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.SignalR;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Unit.Services
{
	public class NotificationServiceTests
	{
		private readonly Mock<IRepository<Notification, int>> _notificationRepoMock = new();
		private readonly Mock<IHubContext<NotificationHub>> _hubContextMock = new();
		private readonly NotificationService _service;

		public NotificationServiceTests()
		{
			// Setup SignalR hub context mock chain
			var clientProxyMock = new Mock<IClientProxy>();
			var hubClientsMock = new Mock<IHubClients>();

			hubClientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(clientProxyMock.Object);
			hubClientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
			_hubContextMock.Setup(h => h.Clients).Returns(hubClientsMock.Object);

			_service = new NotificationService(
				_notificationRepoMock.Object,
				_hubContextMock.Object);

			_notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
				.Returns(Task.CompletedTask);
			_notificationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Notification>()))
				.ReturnsAsync(true);
		}

		private static Notification MakeNotification(int id, string userId,
			NotificationType type = NotificationType.CipherApproved,
			bool isRead = false,
			DateTime? createdAt = null) => new()
			{
				Id = id,
				UserId = userId,
				Type = type,
				Message = $"Notification {id}",
				IsRead = isRead,
				CreatedAt = createdAt ?? DateTime.UtcNow,
			};

		private void SetupAttachedNotifications(params Notification[] notifications)
		{
			var mock = new List<Notification>(notifications).AsQueryable().BuildMock();
			_notificationRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region CreateAndSendNotification

		[Fact]
		public async Task CreateAndSendNotification_CreatesNotification_WithCorrectFields()
		{
			var hubContextMock = new Mock<IHubContext<NotificationHub>>();
			var clientsMock = new Mock<IHubClients>();
			var clientProxyMock = new Mock<IClientProxy>();

			Notification captured = null;
			_notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
				.Callback<Notification>(n => captured = n)
				.Returns(Task.CompletedTask);

			await _service.CreateAndSendNotification(
				"u1",
				NotificationType.CipherApproved,
				"Your cipher was approved",
				123,
				"/cipher/123");

			Assert.NotNull(captured);
			Assert.Equal("u1", captured.UserId);
			Assert.Equal(NotificationType.CipherApproved, captured.Type);
			Assert.Equal("Your cipher was approved", captured.Message);
			Assert.Equal(123, captured.RelatedEntityId);
			Assert.Equal("/cipher/123", captured.Link);
		}

		#endregion

		#region GetUnreadCount

		[Fact]
		public async Task GetUnreadCount_ReturnsZero_WhenNoNotifications()
		{
			SetupAttachedNotifications();

			var result = await _service.GetUnreadCount("u1");

			Assert.Equal(0, result);
		}

		[Fact]
		public async Task GetUnreadCount_CountsOnlyUnread()
		{
			SetupAttachedNotifications(
				MakeNotification(1, "u1", isRead: false),
				MakeNotification(2, "u1", isRead: false),
				MakeNotification(3, "u1", isRead: true));

			var result = await _service.GetUnreadCount("u1");

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetUnreadCount_CountsOnlyForGivenUser()
		{
			SetupAttachedNotifications(
				MakeNotification(1, "u1", isRead: false),
				MakeNotification(2, "u2", isRead: false));

			var result = await _service.GetUnreadCount("u1");

			Assert.Equal(1, result);
		}

		#endregion

		#region GetUserNotifications

		[Fact]
		public async Task GetUserNotifications_ReturnsEmpty_WhenNoNotifications()
		{
			SetupAttachedNotifications();

			var result = await _service.GetUserNotifications("u1");

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetUserNotifications_ReturnsOnlyForGivenUser()
		{
			SetupAttachedNotifications(
				MakeNotification(1, "u1"),
				MakeNotification(2, "u2"));

			var result = await _service.GetUserNotifications("u1");

			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task GetUserNotifications_OrdersByCreatedAt_Descending()
		{
			var old = MakeNotification(1, "u1", createdAt: DateTime.UtcNow.AddDays(-2));
			var recent = MakeNotification(2, "u1", createdAt: DateTime.UtcNow.AddDays(-1));
			var newest = MakeNotification(3, "u1", createdAt: DateTime.UtcNow);
			SetupAttachedNotifications(old, recent, newest);

			var result = await _service.GetUserNotifications("u1");

			Assert.Equal(3, result[0].Id);
			Assert.Equal(2, result[1].Id);
			Assert.Equal(1, result[2].Id);
		}

		[Fact]
		public async Task GetUserNotifications_ReturnsMaximum20Notifications()
		{
			var notifications = Enumerable.Range(1, 25)
				.Select(i => MakeNotification(i, "u1"))
				.ToArray();
			SetupAttachedNotifications(notifications);

			var result = await _service.GetUserNotifications("u1");

			Assert.Equal(20, result.Count);
		}

		[Fact]
		public async Task GetUserNotifications_IncludesReadAndUnread()
		{
			SetupAttachedNotifications(
				MakeNotification(1, "u1", isRead: true),
				MakeNotification(2, "u1", isRead: false));

			var result = await _service.GetUserNotifications("u1");

			Assert.Equal(2, result.Count);
		}

		#endregion

		#region MarkAsRead

		[Fact]
		public async Task MarkAsRead_Throws_WhenNotificationNotFound()
		{
			SetupAttachedNotifications();

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.MarkAsRead(99, "u1"));
		}

		[Fact]
		public async Task MarkAsRead_Throws_WhenNotificationBelongsToOtherUser()
		{
			SetupAttachedNotifications(MakeNotification(1, "u2"));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.MarkAsRead(1, "u1"));
		}

		[Fact]
		public async Task MarkAsRead_SetsIsReadToTrue()
		{
			var notification = MakeNotification(1, "u1", isRead: false);
			SetupAttachedNotifications(notification);

			await _service.MarkAsRead(1, "u1");

			Assert.True(notification.IsRead);
		}

		[Fact]
		public async Task MarkAsRead_UpdatesNotification()
		{
			var notification = MakeNotification(1, "u1", isRead: false);
			SetupAttachedNotifications(notification);

			await _service.MarkAsRead(1, "u1");

			_notificationRepoMock.Verify(r => r.UpdateAsync(notification), Times.Once);
		}

		[Fact]
		public async Task MarkAsRead_WorksWhenAlreadyRead()
		{
			var notification = MakeNotification(1, "u1", isRead: true);
			SetupAttachedNotifications(notification);

			await _service.MarkAsRead(1, "u1");

			Assert.True(notification.IsRead);
		}

		#endregion
	}
}