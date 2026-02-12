using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class NotificationService (
		IRepository<ApplicationUser, string> userRepo,
		IRepository<Notification, int> notificationRepo,
		IRepository<Cipher, int> cipherRepo,
		IRepository<AnswerSuggestion, int> answerRepo,
		IRepository<Badge, int> badgeRepo
		): INotificationService
	{
		public async Task CreateAndSendNotification(string userId, NotificationType type, string message, int? relatedEntityId, string link)
		{
			var user = userRepo.GetAllAttached().FirstOrDefault(x => x.Id == userId);

			if (user == null)
				throw new InvalidOperationException("User not found");

			var notification = new Notification
			{
				UserId = userId,
				Type = type,
				Message = message,
				RelatedEntityId = relatedEntityId,
				Link = link
			};
			user.Notifications.Add(notification);
			await userRepo.UpdateAsync(user);
		}
		public async Task<int> GetUnreadCount(string userId)
		{
			var user = userRepo.GetAllAttached().FirstOrDefault(x => x.Id == userId);

			if (user == null)
				throw new InvalidOperationException("User not found");

			return user.Notifications.Count(x => !x.IsRead);
		}
		public async Task<List<Notification>> GetUserNotifications(string userId, int limit = 20)
		{
			var user = userRepo.GetAllAttached().FirstOrDefault(x => x.Id == userId);

			if (user == null)
				throw new InvalidOperationException("User not found");

			return user.Notifications.Take(20).ToList();
		}
		public async Task MarkAsRead(int notificationId, string userId)
		{
			var user = userRepo.GetAllAttached().FirstOrDefault(x => x.Id == userId);

			if (user == null)
				throw new InvalidOperationException("User not found");

			var notification = user.Notifications.FirstOrDefault(x => x.Id == notificationId);

			if (notification == null)
				throw new InvalidOperationException("Notification not found");

			notification.IsRead = true;
			await notificationRepo.UpdateAsync(notification);
		}
	}
}
