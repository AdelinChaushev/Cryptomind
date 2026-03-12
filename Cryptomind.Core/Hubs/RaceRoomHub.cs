using Cryptomind.Common.DTOs;
using Cryptomind.Common.Helpers;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Hubs
{
	public class RaceRoomHub (IRoomService roomService) : Hub
	{
		public override Task OnConnectedAsync()
		{
			return base.OnConnectedAsync();
		}
		public override Task OnDisconnectedAsync(Exception? exception)
		{
			return base.OnDisconnectedAsync(exception);
		}

		public async Task CreateRoom()
		{
			var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return;

			var code = roomService.CreateRoom(userId);
			await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{code}");
			await Clients.Caller.SendAsync("RoomCreated", code);
		}
		public async Task JoinRoom(string roomCode)
		{
			var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return;

			var didJoin = roomService.JoinRoom(roomCode, userId);

			if (!didJoin)
			{
				await Clients.Caller.SendAsync("RoomJoined", false);
				return;
			}

			await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomCode}");
			await Clients.Group($"room_{roomCode}").SendAsync("RoomJoined", true);
		}
		public async Task SetReady (string roomCode)
		{
			var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return;

			var areBothReady = roomService.SetReady(roomCode, userId);
			
			if (areBothReady)
			{
				var encryptedText = roomService.StartRoom(roomCode);

				await Clients.Group($"room_{roomCode}").SendAsync("GameIsStarting", encryptedText);
			}
			else
				await Clients.Group($"room_{roomCode}").SendAsync("PlayerReady", userId);
		}
		public async Task SubmitAnswer (string roomCode)
		{
			throw new NotImplementedException();
		}
	}
}
