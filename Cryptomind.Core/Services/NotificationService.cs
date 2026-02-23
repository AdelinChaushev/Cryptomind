using Cryptomind.Common.Exceptions;
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
		public async Task CreateAndSendNotification(string userId, NotificationType type, string message, string link)
		{
			var notification = new Notification
			{
				UserId = userId,
				Type = type,
				Message = message,
				Link = link
			};
            // Clients.Group($"user_{userId}")
            Console.WriteLine($"[Hub] OnConnectedAsync userId={userId}");
            Console.WriteLine($"[SignalR] Targeting group: user_{userId}");
            await notificationRepo.AddAsync(notification);
			await hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
			{
				notification.Id,
				notification.Type,
				notification.Message,
				notification.Link,
				TimeSpan.Zero,
			});
		}
		public async Task<List<NotificationDTO>> GetUserNotifications(string userId)
		{
			return await notificationRepo.GetAllAttached()
				.Where(x => x.UserId == userId)
				.OrderByDescending(x => x.CreatedAt)
				.Take(NotificationCount)
				.Select(n => new NotificationDTO
				{
					Id = n.Id,
					Type = n.Type,
					Message = n.Message,
					Link = n.Link,
					IsRead = n.IsRead,
					CreatedSince = GetTimeSpan(n.CreatedAt)
				}).ToListAsync();
		}
		public async Task<int> GetUnreadCount(string userId)
		{
			return await notificationRepo.GetAllAttached()
				.CountAsync(x => x.UserId == userId && !x.IsRead);
		}
		public async Task MarkAsRead(string userId)
		{
			var userNotifications = await notificationRepo.GetAllAttached()
				.Where(c => c.UserId == userId)
				.ToListAsync();
			foreach (var notification in userNotifications)
			{
				

				if (notification == null)
					throw new NotFoundException("Notification not found");

				notification.IsRead = true;
				await notificationRepo.UpdateAsync(notification);
			}		
		}
		private static TimeSpan GetTimeSpan(DateTime createdAt)
		{
			return DateTime.UtcNow - createdAt;
		}
	}
}
