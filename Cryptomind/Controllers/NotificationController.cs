using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/notifications")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ApiController]
	public class NotificationController (INotificationService notificationService) : ControllerBase
	{
		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetAllNotifications()
		{
			string userId = GetUserId();

			var notifications = (await notificationService.GetUserNotifications(userId));
			var unreadCount = await notificationService.GetUnreadCount(userId);

			return Ok( new { notifications, unreadCount });
		}

		[HttpPut]
		[Route("mark-as-read")]
		public async Task<IActionResult> MarkAsRead([FromBody] int[] notificationIds)
		{
			string userId = GetUserId();

			await notificationService.MarkAsRead(notificationIds.ToList(), userId);
			return Ok();
		}

		private string GetUserId()
		   => User.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
