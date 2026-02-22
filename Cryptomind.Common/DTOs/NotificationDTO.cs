using Cryptomind.Data.Enums;

public class NotificationDTO
{
	public int Id { get; set; }
	public NotificationType Type { get; set; }
	public string Message { get; set; }
	public string Link { get; set; }
	public bool IsRead { get; set; }
	public TimeSpan CreatedSince { get; set; }
}