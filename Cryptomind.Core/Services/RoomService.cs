using Cryptomind.Common.Constants;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.Helpers;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Rooms;
using Cryptomind.Core.Rooms.Enums;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Cryptomind.Core.Services
{
	public class RoomService(IServiceScopeFactory scopeFactory) : IRoomService
	{
		private ConcurrentDictionary<string, RaceRoom> rooms = new ConcurrentDictionary<string, RaceRoom>();

		public async Task<string> CreateRoom(string userId)
		{
			using var scope = scopeFactory.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			if (await userManager.FindByIdAsync(userId) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			string code;
			do
			{
				code = GenerateRoomCodeHelper.CodeGenerator();
			} while (rooms.ContainsKey(code));

			var raceRoom = new RaceRoom
			{
				Code = code,
				CurrentRound = 0,
				Player1Id = userId,
				Player2Id = null,
				Player1Ready = false,
				Player2Ready = false,
				Player1Score = 0,
				Player2Score = 0,
				Status = RoomStatus.WaitingForPlayers
			};

			rooms.TryAdd(code, raceRoom);
			return code;
		}

		public void RemoveRoom(string roomCode)
		{
			rooms.TryRemove(roomCode, out _);
		}

		public async Task<bool> JoinRoom(string roomCode, string userId)
		{
			if (!rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = rooms[roomCode];

			if (room.Player2Id != null)
				throw new ConflictException(RoomConstants.RoomAlreadyFull);

			using var scope = scopeFactory.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			if (await userManager.FindByIdAsync(userId) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			room.Player2Id = userId;
			room.Status = RoomStatus.WaitingForReady;

			return true;
		}

		public async Task<bool> SetReady(string roomCode, string userId)
		{
			if (!rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			using var scope = scopeFactory.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			if (await userManager.FindByIdAsync(userId) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			var room = rooms[roomCode];
			if (room.Player1Id != userId && room.Player2Id != userId)
				throw new NotFoundException(RoomConstants.PlayerNotInRoom);

			lock (room)
			{
				if (userId == room.Player1Id)
					room.Player1Ready = true;
				else if (userId == room.Player2Id)
					room.Player2Ready = true;
				else
					throw new NotFoundException(RoomConstants.PlayerNotInRoom);

				return room.Player1Ready && room.Player2Ready;
			}
		}

		public string StartRoom(string roomCode)
		{
			if (!rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var (cipherType, encryptedText) = CipherGeneratorHelper.GenerateRandom();
			var room = rooms[roomCode];

			room.Status = RoomStatus.InProgress;
			room.CurrentRound = 1;
			room.Rounds.Add(new Round
			{
				EncryptedText = encryptedText,
				CorrectAnswer = Enum.Parse<CipherType>(cipherType),
				Submissions = new List<RoomSubmission>(),
				WinnerId = null
			});

			return encryptedText;
		}

		public async Task<RoomSubmissionResultDTO> SubmitAnwer(string roomCode, string userId, CipherType answer)
		{
			if (!rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			using var scope = scopeFactory.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			if (await userManager.FindByIdAsync(userId) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			var room = rooms[roomCode];
			var round = room.Rounds[room.CurrentRound - 1];

			string? winnerUsername = null;
			bool didBothSubmit = false;

			lock (room)
			{
				if (round.Submissions.Any(x => x.UserId == userId))
					throw new ConflictException(RoomConstants.AlreadySubmitted);

				if (room.Player1Id != userId && room.Player2Id != userId)
					throw new NotFoundException(RoomConstants.PlayerNotInRoom);

				round.Submissions.Add(new RoomSubmission
				{
					UserId = userId,
					Answer = answer,
					SubmissionTime = DateTime.Now,
					IsCorrect = round.CorrectAnswer == answer
				});

				didBothSubmit = round.Submissions.Count == 2;
			}

			bool? wasLastRound = null;

			if (didBothSubmit)
			{
				var result = await EndRound(roomCode, false);
				if (result == null) return new RoomSubmissionResultDTO
				{
					DidBothSubmit = false
				};

				winnerUsername = result.WinnerUsername;
				wasLastRound = result.WasLastRound;
			}

			return new RoomSubmissionResultDTO
			{
				DidBothSubmit = didBothSubmit,
				WinnerUsername = winnerUsername,
				WasLastRound = wasLastRound,
			};
		}

		public string NextRound(string roomCode)
		{
			if (!rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var (cipherType, encryptedText) = CipherGeneratorHelper.GenerateRandom();
			var room = rooms[roomCode];

			room.CurrentRound++;
			room.Rounds.Add(new Round
			{
				EncryptedText = encryptedText,
				CorrectAnswer = Enum.Parse<CipherType>(cipherType),
				Submissions = new List<RoomSubmission>(),
				WinnerId = null
			});

			return encryptedText;
		}

		public async Task<RoundResultDTO?> EndRound(string roomCode, bool didTimerRanOut)
		{
			if (!rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = rooms[roomCode];
			var round = room.Rounds[room.CurrentRound - 1];
			var submissions = round.Submissions
				.OrderBy(x => x.SubmissionTime)
				.ToList();

			string? winnerUserId = null;
			bool wasLastRound = room.CurrentRound == RoomConstants.RoundsPerRace;

			lock (room)
			{
				if (round.IsFinished)
					return null;

				round.IsFinished = true;

				if (didTimerRanOut || submissions.Count < 2)
					return new RoundResultDTO
					{
						RoundNumber = room.CurrentRound,
						WasLastRound = wasLastRound,
						WinnerUsername = null,
					};

				var firstSubmission = submissions[0];
				var secondSubmission = submissions[1];

				if (!firstSubmission.IsCorrect && !secondSubmission.IsCorrect)
					return new RoundResultDTO
					{
						RoundNumber = room.CurrentRound,
						WasLastRound = wasLastRound,
						WinnerUsername = null,
					};

				if (firstSubmission.IsCorrect && !secondSubmission.IsCorrect || (firstSubmission.IsCorrect && secondSubmission.IsCorrect))
					winnerUserId = firstSubmission.UserId;
				else
					winnerUserId = secondSubmission.UserId;
			}

			using var scope = scopeFactory.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			var user = await userManager.FindByIdAsync(winnerUserId);
			if (user == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			if (winnerUserId == room.Player1Id)
				room.Player1Score++;
			else
				room.Player2Score++;

			return new RoundResultDTO
			{
				RoundNumber = room.CurrentRound,
				WasLastRound = wasLastRound,
				WinnerUsername = user.UserName
			};
		}

		public void SetRoundTimer(string roomCode, CancellationTokenSource cts)
		{
			var room = rooms[roomCode];
			room.RoundTimer = cts;
		}

		public void CancelRoundTimer(string roomCode)
		{
			var room = rooms[roomCode];
			room.RoundTimer?.Cancel();
			room.RoundTimer = null;
		}

		public async Task<GameResultDTO> EndGame(string roomCode)
		{
			if (!rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = rooms[roomCode];
			room.Status = RoomStatus.Finished;

			using var scope = scopeFactory.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			var firstPlayer = await userManager.FindByIdAsync(room.Player1Id);
			var secondPlayer = await userManager.FindByIdAsync(room.Player2Id);

			if (firstPlayer == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);
			if (secondPlayer == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			string? winnerUsername = null;

			if (room.Player1Score > room.Player2Score)
				winnerUsername = firstPlayer.UserName;
			else if (room.Player2Score > room.Player1Score)
				winnerUsername = secondPlayer.UserName;

			rooms.TryRemove(roomCode, out _);

			return new GameResultDTO
			{
				WinnerUsername = winnerUsername,
				Player1Score = room.Player1Score,
				Player2Score = room.Player2Score,
				Player1Username = firstPlayer.UserName,
				Player2Username = secondPlayer.UserName,
			};
		}
	}
}