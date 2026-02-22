using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Contracts
{
	public interface INotificationService
	{
		Task CreateAndSendNotification(string userId, NotificationType type, string message, string link);
		Task<List<NotificationDTO>> GetUserNotifications(string userId);
		Task MarkAsRead(List<int> notificationIds, string userId);
		Task<int> GetUnreadCount(string userId);
	}
}
