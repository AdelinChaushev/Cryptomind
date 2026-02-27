using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Contracts
{
	public interface INotificationService
	{
		Task CreateAndSendNotification(string userId, NotificationType type, string message, string link);
		Task<List<NotificationDTO>> GetUserNotifications(string userId);
		Task MarkAsRead(string userId);

		Task MarkAsReadSingle(string userId, int notificationId);
		Task<int> GetUnreadCount(string userId);
	}
}
