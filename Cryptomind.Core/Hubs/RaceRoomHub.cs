using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Cryptomind.Core.Hubs
{
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class RaceRoomHub(IRoomService roomService, IHubContext<RaceRoomHub> hubContext) : Hub
	{
		private static ConcurrentDictionary<string, string> connectionToRoom = new();
		private static ConcurrentDictionary<string, string> userToConnection = new();
		private static ConcurrentDictionary<string, byte> intentionalLeaves = new();

		public override async Task OnConnectedAsync()
		{
			var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!string.IsNullOrEmpty(userId))
			{
				userToConnection[userId] = Context.ConnectionId;

				var existingRoom = connectionToRoom.Values
					.GroupBy(x => x)
					.Select(x => x.Key)
					.FirstOrDefault(roomCode => roomService.IsPlayerInRoom(roomCode, userId));

				if (existingRoom != null)
				{
					connectionToRoom.TryAdd(Context.ConnectionId, existingRoom);
					await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{existingRoom}");
				}
			}

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			if (connectionToRoom.TryRemove(Context.ConnectionId, out var roomCode))
			{
				var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

				if (!string.IsNullOrEmpty(userId) && intentionalLeaves.TryRemove(userId, out _))
				{
					await base.OnDisconnectedAsync(exception);
					return;
				}

				await Task.Delay(TimeSpan.FromSeconds(8));

				bool playerReconnected = !string.IsNullOrEmpty(userId) &&
					userToConnection.TryGetValue(userId, out var currentConnectionId) &&
					currentConnectionId != Context.ConnectionId &&
					connectionToRoom.ContainsKey(currentConnectionId) &&
					connectionToRoom[currentConnectionId] == roomCode;

				if (!playerReconnected)
				{
					if (!string.IsNullOrEmpty(userId))
						userToConnection.TryRemove(userId, out _);

					var staleConnections = connectionToRoom
						.Where(x => x.Value == roomCode)
						.Select(x => x.Key)
						.ToList();

					foreach (var connectionId in staleConnections)
						connectionToRoom.TryRemove(connectionId, out _);

					await hubContext.Clients.Group($"room_{roomCode}").SendAsync("PlayerDisconnected");
					roomService.CancelRoundTimer(roomCode);
					roomService.RemoveRoom(roomCode);
				}
			}

			await base.OnDisconnectedAsync(exception);
		}

		public async Task CreateRoom()
		{
			try
			{
				var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId)) return;

				var code = await roomService.CreateRoom(userId);
				connectionToRoom.TryAdd(Context.ConnectionId, code);
				await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{code}");
				await Clients.Caller.SendAsync("RoomCreated", code);
			}
			catch (NotFoundException ex)
			{
				await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (Exception)
			{
				await Clients.Caller.SendAsync("Error", "An unexpected error occurred");
			}
		}
		public async Task JoinRoom(string roomCode)
		{
			try
			{
				var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId)) return;

				var didJoin = await roomService.JoinRoom(roomCode, userId);

				if (!didJoin)
				{
					await Clients.Caller.SendAsync("RoomJoined", false);
					return;
				}

				connectionToRoom.TryAdd(Context.ConnectionId, roomCode);
				await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomCode}");
				await Clients.Group($"room_{roomCode}").SendAsync("RoomJoined", true);

				var (player1Username, player2Username) = await roomService.GetPlayerUsernames(roomCode);

				await Clients.Caller.SendAsync("PlayersInfo", player2Username, player1Username);
				await Clients.OthersInGroup($"room_{roomCode}").SendAsync("PlayersInfo", player1Username, player2Username);
			}
			catch (NotFoundException ex)
			{
				await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (ConflictException ex)
			{
				await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (Exception)
			{
				await Clients.Caller.SendAsync("Error", "An unexpected error occurred");
			}
		}
		public async Task SetReady(string roomCode)
		{
			try
			{
				var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId)) return;

				var areBothReady = await roomService.SetReady(roomCode, userId);

				if (areBothReady)
				{
					var encryptedText = roomService.StartRoom(roomCode);
					await Clients.Group($"room_{roomCode}").SendAsync("GameIsStarting", encryptedText);
					_ = StartRoundTimer(roomCode);
				}
				else
					await Clients.Group($"room_{roomCode}").SendAsync("PlayerReady", userId);
			}
			catch (NotFoundException ex)
			{
				await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (Exception)
			{
				await Clients.Caller.SendAsync("Error", "An unexpected error occurred");
			}
		}
		public async Task SubmitAnswer(string roomCode, CipherType answer)
		{
			try
			{
				var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId)) return;

				var result = await roomService.SubmitAnwer(roomCode, userId, answer);

				if (result.DidBothSubmit)
				{
					roomService.CancelRoundTimer(roomCode);
					await Clients.Group($"room_{roomCode}").SendAsync("RoundEnded", result.WinnerUsername);

					if (result.WasLastRound == true)
					{
						var gameResult = await roomService.EndGame(roomCode);
						await Clients.Group($"room_{roomCode}").SendAsync("GameEnded", gameResult);
					}
					else
					{
						var nextCipher = roomService.NextRound(roomCode);
						await Clients.Group($"room_{roomCode}").SendAsync("NextRoundStarting", nextCipher);
						_ = StartRoundTimer(roomCode);
					}
				}
				else
				{
					await Clients.Caller.SendAsync("AnswerSubmitted");
					await Clients.OthersInGroup($"room_{roomCode}").SendAsync("OtherUserHasSubmitted");
				}
			}
			catch (NotFoundException ex)
			{
				await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (ConflictException ex)
			{
				await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (Exception)
			{
				await Clients.Caller.SendAsync("Error", "An unexpected error occurred");
			}
		}
		public async Task VerifyRoom(string roomCode)
		{
			var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return;

			if (!roomService.IsPlayerInRoom(roomCode, userId))
				await Clients.Caller.SendAsync("RoomNoLongerExists");
		}
		public async Task LeaveRoom(string roomCode)
		{
			var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return;

			if (!roomService.IsPlayerInRoom(roomCode, userId)) return;

			intentionalLeaves.TryAdd(userId, 0);

			connectionToRoom.TryRemove(Context.ConnectionId, out _);
			userToConnection.TryRemove(userId, out _);

			var staleConnections = connectionToRoom
				.Where(x => x.Value == roomCode)
				.Select(x => x.Key)
				.ToList();

			foreach (var connectionId in staleConnections)
				connectionToRoom.TryRemove(connectionId, out _);

			await Clients.Group($"room_{roomCode}").SendAsync("PlayerDisconnected");
			roomService.CancelRoundTimer(roomCode);
			roomService.RemoveRoom(roomCode);
		}

		#region Private Methods
		private async Task StartRoundTimer(string roomCode)
		{
			var cts = new CancellationTokenSource();
			roomService.SetRoundTimer(roomCode, cts);

			try
			{
				await Task.Delay(TimeSpan.FromSeconds(RoomConstants.RoundDurationSeconds), cts.Token);

				var result = await roomService.EndRound(roomCode, true);
				if (result == null) return;

				await hubContext.Clients.Group($"room_{roomCode}").SendAsync("RoundEnded", null);

				if (result.WasLastRound)
				{
					var gameResult = await roomService.EndGame(roomCode);
					await hubContext.Clients.Group($"room_{roomCode}").SendAsync("GameEnded", gameResult);
				}
				else
				{
					var nextCipher = roomService.NextRound(roomCode);
					await hubContext.Clients.Group($"room_{roomCode}").SendAsync("NextRoundStarting", nextCipher);
					_ = StartRoundTimer(roomCode);
				}
			}
			catch (TaskCanceledException) { }
			catch (Exception)
			{
				await hubContext.Clients.Group($"room_{roomCode}").SendAsync("Error", "An unexpected error occurred");
			}
		}
		#endregion
	}
}