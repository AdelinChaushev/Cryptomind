using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Cryptomind.Core.Hubs
{
	[Authorize(AuthenticationSchemes = "Bearer")]
	public class RaceRoomHub(
		IRoomService roomService,
		IHubContext<RaceRoomHub> hubContext,
		IServiceScopeFactory scopeFactory) : Hub
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

				var existingRoom = roomService.GetRoomCodeForPlayer(userId);
				if (existingRoom != null)
				{
					connectionToRoom[Context.ConnectionId] = existingRoom;
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

				var capturedConnectionId = Context.ConnectionId;
				var capturedRoomCode = roomCode;
				var capturedUserId = userId;

				_ = Task.Run(async () =>
				{
					await Task.Delay(TimeSpan.FromSeconds(8));

					bool playerReconnected =
						!string.IsNullOrEmpty(capturedUserId) &&
						userToConnection.TryGetValue(capturedUserId, out var currentConnectionId) &&
						currentConnectionId != capturedConnectionId &&
						connectionToRoom.ContainsKey(currentConnectionId) &&
						connectionToRoom[currentConnectionId] == capturedRoomCode;

					if (playerReconnected) return;

					if (!string.IsNullOrEmpty(capturedUserId))
						userToConnection.TryRemove(capturedUserId, out _);

					var staleConnections = connectionToRoom
						.Where(x => x.Value == capturedRoomCode)
						.Select(x => x.Key)
						.ToList();

					foreach (var connectionId in staleConnections)
						connectionToRoom.TryRemove(connectionId, out _);

					await using var scope = scopeFactory.CreateAsyncScope();
					var scoped = scope.ServiceProvider.GetRequiredService<IRoomService>();

					if (!scoped.RoomExists(capturedRoomCode)) return;

					var playerIds = scoped.GetPlayerIds(capturedRoomCode);
					if (playerIds.HasValue && !string.IsNullOrEmpty(playerIds.Value.player2Id))
					{
						await hubContext.Clients.Group($"room_{capturedRoomCode}").SendAsync("PlayerDisconnected");
					}

					scoped.CancelRoundTimer(capturedRoomCode);
					await scoped.RefundWagers(capturedRoomCode);
					scoped.RemoveRoom(capturedRoomCode);
				});
			}

			await base.OnDisconnectedAsync(exception);
		}
		public async Task RequestCurrentState()
		{
			var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(userId)) return;

			var state = roomService.GetPlayerGameState(userId);
			if (state != null)
			{
				var playerIds = roomService.GetPlayerIds(state.RoomCode);
				if (playerIds.HasValue)
				{
					var (p1Username, p2Username) = await roomService.GetPlayerUsernames(state.RoomCode);
					state.MyUsername       = playerIds.Value.player1Id == userId ? p1Username : p2Username;
					state.OpponentUsername = playerIds.Value.player1Id == userId ? p2Username : p1Username;
				}
				state.RoundHistory = await roomService.GetCompletedRounds(state.RoomCode);
				await Clients.Caller.SendAsync("GameStateRestored", state);
				return;
			}

			var roomCode = roomService.GetRoomCodeForPlayer(userId);
			if (roomCode != null && !roomService.IsRoomInProgress(roomCode))
			{
				var playerIds = roomService.GetPlayerIds(roomCode);
				if (playerIds.HasValue && !string.IsNullOrEmpty(playerIds.Value.player2Id))
				{
					await Clients.OthersInGroup($"room_{roomCode}").SendAsync("PlayerDisconnected");
				}
				roomService.CancelRoundTimer(roomCode);
				await roomService.RefundWagers(roomCode);
				roomService.RemoveRoom(roomCode);
				connectionToRoom.TryRemove(Context.ConnectionId, out _);
			}

			await Clients.Caller.SendAsync("NoActiveRoom");
		}
		public async Task CreateRoom(int wagerAmount = 0)
		{
			try
			{
				var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId)) return;

				var code = await roomService.CreateRoom(userId, wagerAmount);
				connectionToRoom[Context.ConnectionId] = code;
				await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{code}");
				await Clients.Caller.SendAsync("RoomCreated", code, wagerAmount);
			}
			catch (ConflictException ex)
			{
				await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (NotFoundException ex)
			{
				await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (Exception)
			{
				await Clients.Caller.SendAsync("Error", RoomConstants.AnUnexpectedErrorOccured);
			}
		}
		public async Task GetWagerInfo(string roomCode)
		{
			try
			{
				var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId)) return;

				var info = await roomService.GetWagerInfo(roomCode, userId);
				await Clients.Caller.SendAsync("WagerInfo", info);
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
				await Clients.Caller.SendAsync("Error", RoomConstants.AnUnexpectedErrorOccured);
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

				connectionToRoom[Context.ConnectionId] = roomCode;
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
				await Clients.Caller.SendAsync("Error", RoomConstants.AnUnexpectedErrorOccured);
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
					try
					{
						await roomService.LockWagers(roomCode);
					}
					catch (ConflictException ex)
					{
						var playerIds = roomService.GetPlayerIds(roomCode);
						if (playerIds.HasValue)
						{
							userToConnection.TryRemove(playerIds.Value.player1Id, out _);
							if (!string.IsNullOrEmpty(playerIds.Value.player2Id))
								userToConnection.TryRemove(playerIds.Value.player2Id!, out _);
						}

						var staleConnections = connectionToRoom
							.Where(x => x.Value == roomCode)
							.Select(x => x.Key)
							.ToList();
						foreach (var connectionId in staleConnections)
							connectionToRoom.TryRemove(connectionId, out _);

						await Clients.Group($"room_{roomCode}").SendAsync("RoomCancelled", ex.Message);
						roomService.RemoveRoom(roomCode);
						return;
					}

					var encryptedText = roomService.StartRoom(roomCode);
					await Clients.Group($"room_{roomCode}").SendAsync("GameIsStarting", encryptedText);
					_ = StartRoundTimer(roomCode, RoomConstants.PreRoundSeconds);
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
				await Clients.Caller.SendAsync("Error", RoomConstants.AnUnexpectedErrorOccured);
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
						await EndGameAndCleanup(roomCode, roomService);
					}
					else
					{
						var nextCipher = roomService.NextRound(roomCode);
						await Clients.Group($"room_{roomCode}").SendAsync("NextRoundStarting", nextCipher);
						_ = StartRoundTimer(roomCode, RoomConstants.PreRoundSeconds * 2);
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
				if (ex.Message != RoomConstants.RoundNotStarted)
					await Clients.Caller.SendAsync("Error", ex.Message);
			}
			catch (Exception)
			{
				await Clients.Caller.SendAsync("Error", RoomConstants.AnUnexpectedErrorOccured);
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
			await roomService.RefundWagers(roomCode);
			roomService.RemoveRoom(roomCode);
		}

		#region Private Methods
		private async Task StartRoundTimer(string roomCode, int extraDelaySeconds = 0)
		{
			var cts = new CancellationTokenSource();
			roomService.SetRoundTimer(roomCode, cts);

			try
			{
				await Task.Delay(TimeSpan.FromSeconds(RoomConstants.RoundDurationSeconds + extraDelaySeconds), cts.Token);

				await using var scope = scopeFactory.CreateAsyncScope();
				var scoped = scope.ServiceProvider.GetRequiredService<IRoomService>();

				if (!scoped.RoomExists(roomCode)) return;

				var result = await scoped.EndRound(roomCode, true);
				if (result == null) return;

				await hubContext.Clients.Group($"room_{roomCode}").SendAsync("RoundEnded", null);

				if (result.WasLastRound)
				{
					await EndGameAndCleanup(roomCode, scoped);
				}
				else
				{
					var nextCipher = scoped.NextRound(roomCode);
					await hubContext.Clients.Group($"room_{roomCode}").SendAsync("NextRoundStarting", nextCipher);
					_ = StartRoundTimer(roomCode, RoomConstants.PreRoundSeconds * 2);
				}
			}
			catch (TaskCanceledException) { }
			catch (Exception)
			{
				await hubContext.Clients.Group($"room_{roomCode}").SendAsync("Error", RoomConstants.AnUnexpectedErrorOccured);
			}
		}
		private async Task EndGameAndCleanup(string roomCode, IRoomService service)
		{
			var playerIds = service.GetPlayerIds(roomCode);
			var gameResult = await service.EndGame(roomCode);

			if (playerIds.HasValue)
			{
				userToConnection.TryRemove(playerIds.Value.player1Id, out _);
				if (!string.IsNullOrEmpty(playerIds.Value.player2Id))
					userToConnection.TryRemove(playerIds.Value.player2Id!, out _);
			}

			await hubContext.Clients.Group($"room_{roomCode}").SendAsync("GameEnded", gameResult);
		}
		#endregion
	}
}