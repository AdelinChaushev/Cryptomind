using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/notifications")]
	[ApiController]
	public class NotificationController (INotificationService notificationService) : ControllerBase
	{
		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetAllNotifications()
		{
			string userId = GetUserId();
			var notifications = await notificationService.GetUserNotifications(userId);
			var unreadCount = await notificationService.GetUnreadCount(userId);

			return Ok( new { notifications, unreadCount });
		}

		[HttpPut]
		[Route("mark-as-read")]
		public async Task<IActionResult> MarkAsRead([FromBody] int[] notificationIds)
		{
			try
			{
				await notificationService.MarkAsRead(notificationIds.ToList(), GetUserId());
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		private string GetUserId()
		   => User.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
