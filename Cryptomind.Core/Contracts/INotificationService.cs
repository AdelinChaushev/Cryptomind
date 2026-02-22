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
		Task CreateAndSendNotification(string userId, NotificationType type, string message, string link);
		Task<List<Notification>> GetUserNotifications(string userId);
		Task MarkAsRead(List<int> notificationIds, string userId);
		Task<int> GetUnreadCount(string userId);
	}
}
