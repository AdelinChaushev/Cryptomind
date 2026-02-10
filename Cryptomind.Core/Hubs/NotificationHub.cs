using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Cryptomind.Core.Hubs
{
	[Authorize]
	public class NotificationHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			var userId = Context.User?.FindFirst("userId")?.Value;

			if (!string.IsNullOrEmpty(userId))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
			}

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var userId = Context.User?.FindFirst("userId")?.Value;

			if (!string.IsNullOrEmpty(userId))
			{
				await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
			}

			await base.OnDisconnectedAsync(exception);
		}
	}
}