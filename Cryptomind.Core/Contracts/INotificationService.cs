using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface INotificationService
	{
		Task CreateAndSendNotification(string userId, NotificationType type, string message, int? relatedEntityId, string link);
		Task<List<Notification>> GetUserNotifications(string userId);
		Task MarkAsRead(int notificationId, string userId);
		Task<int> GetUnreadCount(string userId);
	}
}
