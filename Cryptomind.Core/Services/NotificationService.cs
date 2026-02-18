using Cryptomind.Core.Contracts;
using Cryptomind.Core.Hubs;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class NotificationService (
		IRepository<Notification, int> notificationRepo,
		IHubContext<NotificationHub> hubContext): INotificationService
	{
		private const int NotificationCount = 20;
		public async Task CreateAndSendNotification(string userId, NotificationType type, string message, int? relatedEntityId, string link)
		{
			var notification = new Notification
			{
				UserId = userId,
				Type = type,
				Message = message,
				RelatedEntityId = relatedEntityId,
				Link = link
			};

			await notificationRepo.AddAsync(notification);
			await hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
			{
				notification.Id,
				notification.Type,
				notification.Message,
				notification.RelatedEntityId,
				notification.Link,
				notification.CreatedAt
			});
		}
		public async Task<int> GetUnreadCount(string userId)
		{
			return await notificationRepo.GetAllAttached()
				.CountAsync(x => x.UserId == userId && !x.IsRead);
		}
		public async Task<List<Notification>> GetUserNotifications(string userId)
		{
			return await notificationRepo.GetAllAttached()
				.Where(x => x.UserId == userId)
				.OrderByDescending(x => x.CreatedAt)
				.Take(NotificationCount)
				.ToListAsync();
		}
		public async Task MarkAsRead(int notificationId, string userId)
		{
			var notification = await notificationRepo.GetAllAttached()
				.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);

			if (notification == null)
				throw new InvalidOperationException("Notification not found");

			notification.IsRead = true;
			await notificationRepo.UpdateAsync(notification);
		}
	}
}
