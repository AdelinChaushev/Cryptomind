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
using System.Net.WebSockets;

namespace Cryptomind.Core.Services
{
	public class RoomService(UserManager<ApplicationUser> userManager, RoomStore store) : IRoomService
	{
		public async Task<string> CreateRoom(string userId, int wagerAmount)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			if (GetRoomCodeForPlayer(userId) != null)
				throw new ConflictException(RoomConstants.AlreadyInRoom);

			if (wagerAmount < 0)
				throw new ConflictException(RoomConstants.WagerCannotBeNegative);

			if (wagerAmount > 0 && user.Score < wagerAmount)
				throw new ConflictException(RoomConstants.NotEnoughPointsForWager);

			string code;
			do
			{
				code = GenerateRoomCodeHelper.CodeGenerator();
			} while (store.Rooms.ContainsKey(code));

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
				WagerAmount = wagerAmount,
				WagersLocked = false,
				Status = RoomStatus.WaitingForPlayers
			};

			store.Rooms.TryAdd(code, raceRoom);
			return code;
		}
		public void RemoveRoom(string roomCode)
		{
			store.Rooms.TryRemove(roomCode, out _);
		}
		public bool RoomExists(string roomCode)
		{
			return store.Rooms.ContainsKey(roomCode);
		}
		public string? GetRoomCodeForPlayer(string userId)
		{
			return store.Rooms.Values
				.FirstOrDefault(r => r.Player1Id == userId || r.Player2Id == userId)
				?.Code;
		}
		public bool IsRoomInProgress(string roomCode)
		{
			return store.Rooms.TryGetValue(roomCode, out var room) &&
				room.Status == RoomStatus.InProgress;
		}
		public GameStateDTO? GetPlayerGameState(string userId)
		{
			var room = store.Rooms.Values
				.FirstOrDefault(r => r.Player1Id == userId || r.Player2Id == userId);

			if (room == null || room.Status != RoomStatus.InProgress) return null;

			var currentRound = room.Rounds[room.CurrentRound - 1];

			bool inTransition = room.RoundStartedAt.HasValue &&
				room.RoundStartedAt.Value > DateTime.Now;

			if (inTransition && room.CurrentRound > 1)
			{
				var transitionMs = (int)(room.RoundStartedAt!.Value - DateTime.Now).TotalMilliseconds;
				return new GameStateDTO
				{
					RoomCode = room.Code,
					CurrentRound = room.CurrentRound,
					IsRoundEnd = true,
					NextEncryptedText = currentRound.EncryptedText,
					TransitionMsRemaining = Math.Max(0, transitionMs),
					WagerAmount = room.WagerAmount
				};
			}

			var secondsElapsed = room.RoundStartedAt.HasValue
				? Math.Max(0, (int)(DateTime.Now - room.RoundStartedAt.Value).TotalSeconds)
				: 0;

			int preRoundSecondsRemaining = secondsElapsed < RoomConstants.PreRoundSeconds
				? RoomConstants.PreRoundSeconds - secondsElapsed
				: 0;

			return new GameStateDTO
			{
				RoomCode = room.Code,
				EncryptedText = currentRound.EncryptedText,
				CurrentRound = room.CurrentRound,
				SecondsElapsed = Math.Min(secondsElapsed, RoomConstants.RoundDurationSeconds),
				HasSubmitted = currentRound.Submissions.Any(s => s.UserId == userId),
				HasOpponentSubmitted = currentRound.Submissions.Any(s => s.UserId != userId),
				IsRoundEnd = false,
				WagerAmount = room.WagerAmount,
				PreRoundSecondsRemaining = preRoundSecondsRemaining
			};
		}
		public (string player1Id, string? player2Id)? GetPlayerIds(string roomCode)
		{
			if (!store.Rooms.TryGetValue(roomCode, out var room)) return null;
			return (room.Player1Id, room.Player2Id);
		}
		public async Task<List<PastRoundDTO>> GetCompletedRounds(string roomCode)
		{
			if (!store.Rooms.TryGetValue(roomCode, out var room)) return new();

			var result = new List<PastRoundDTO>();
			for (int i = 0; i < room.Rounds.Count; i++)
			{
				var round = room.Rounds[i];
				if (!round.IsFinished) continue;

				string? winnerUsername = null;
				if (round.WinnerId != null)
				{
					var winner = await userManager.FindByIdAsync(round.WinnerId);
					winnerUsername = winner?.UserName;
				}
				result.Add(new PastRoundDTO { Round = i + 1, WinnerUsername = winnerUsername });
			}
			return result;
		}
		public async Task<(string player1Username, string player2Username)> GetPlayerUsernames(string roomCode)
		{
			if (!store.Rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = store.Rooms[roomCode];

			var player1 = await userManager.FindByIdAsync(room.Player1Id);
			var player2 = await userManager.FindByIdAsync(room.Player2Id!);

			if (player1 == null || player2 == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			return (player1.UserName!, player2.UserName!);
		}
		public async Task<WagerInfoDTO> GetWagerInfo(string roomCode, string joinerId)
		{
			if (!store.Rooms.TryGetValue(roomCode, out var room))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			if (room.Player2Id != null)
				throw new ConflictException(RoomConstants.RoomAlreadyFull);

			if (room.Player1Id == joinerId)
				throw new ConflictException(RoomConstants.PlayerAlreadyInRoom);

			var creator = await userManager.FindByIdAsync(room.Player1Id);
			if (creator == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			var joiner = await userManager.FindByIdAsync(joinerId);
			if (joiner == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			return new WagerInfoDTO
			{
				WagerAmount = room.WagerAmount,
				CreatorUsername = creator.UserName!,
				JoinerBalance = joiner.Score
			};
		}
		public async Task<bool> JoinRoom(string roomCode, string userId)
		{
			if (!store.Rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			if (GetRoomCodeForPlayer(userId) != null)
				throw new ConflictException(RoomConstants.AlreadyInRoom);

			var room = store.Rooms[roomCode];

			if (room.Player2Id != null)
				throw new ConflictException(RoomConstants.RoomAlreadyFull);
			if (await userManager.FindByIdAsync(userId) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);
			if (room.Player1Id == userId)
				throw new ConflictException(RoomConstants.PlayerAlreadyInRoom);

			room.Player2Id = userId;
			room.Status = RoomStatus.WaitingForReady;

			return true;
		}
		public async Task<bool> SetReady(string roomCode, string userId)
		{	
			if (!store.Rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = store.Rooms[roomCode];
			if (room.Status == RoomStatus.InProgress || room.Status == RoomStatus.Finished)
				return false;

			if (await userManager.FindByIdAsync(userId) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);
			else if (room.Player1Id != userId && room.Player2Id != userId)
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
		public async Task LockWagers(string roomCode)
		{
			if (!store.Rooms.TryGetValue(roomCode, out var room)) return;
			if (room.WagerAmount == 0 || room.WagersLocked) return;

			var player1 = await userManager.FindByIdAsync(room.Player1Id);
			var player2 = await userManager.FindByIdAsync(room.Player2Id!);

			if (player1 == null || player2 == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			if (player1.Score < room.WagerAmount)
				throw new ConflictException(string.Format(RoomConstants.UserDoesNotHaveEnoughPoints, player1.UserName));
			if (player2.Score < room.WagerAmount)
				throw new ConflictException(string.Format(RoomConstants.UserDoesNotHaveEnoughPoints, player2.UserName));

			player1.Score -= room.WagerAmount;
			player2.Score -= room.WagerAmount;

			await userManager.UpdateAsync(player1);
			await userManager.UpdateAsync(player2);

			room.WagersLocked = true;
		}
		public async Task RefundWagers(string roomCode)
		{
			if (!store.Rooms.TryGetValue(roomCode, out var room)) return;
			if (!room.WagersLocked || room.WagerAmount == 0 || room.Status == RoomStatus.Finished) return;

			var player1 = await userManager.FindByIdAsync(room.Player1Id);
			if (player1 != null)
			{
				player1.Score += room.WagerAmount;
				await userManager.UpdateAsync(player1);
			}

			if (!string.IsNullOrEmpty(room.Player2Id))
			{
				var player2 = await userManager.FindByIdAsync(room.Player2Id);
				if (player2 != null)
				{
					player2.Score += room.WagerAmount;
					await userManager.UpdateAsync(player2);
				}
			}

			room.WagersLocked = false;
		}
		public bool IsPlayerInRoom(string roomCode, string userId)
		{
			if (!store.Rooms.ContainsKey(roomCode)) return false;
			var room = store.Rooms[roomCode];
			return room.Player1Id == userId || room.Player2Id == userId;
		}
		public string StartRoom(string roomCode)
		{
			if (!store.Rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = store.Rooms[roomCode];
			var (cipherType, encryptedText, plaintext) =
				CipherGeneratorHelper.GenerateRandom(room.UsedCipherTypes, room.UsedSentences);

			room.UsedCipherTypes.Add(cipherType);
			room.UsedSentences.Add(plaintext);
			room.Status = RoomStatus.InProgress;
			room.CurrentRound = 1;
			room.RoundStartedAt = DateTime.Now.AddSeconds(RoomConstants.PreRoundSeconds);
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
			if (!store.Rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			if (await userManager.FindByIdAsync(userId) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			var room = store.Rooms[roomCode];

			if (room.RoundStartedAt.HasValue && room.RoundStartedAt.Value > DateTime.Now)
				throw new ConflictException(RoomConstants.RoundNotStarted);

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
				if (result == null) return new RoomSubmissionResultDTO { DidBothSubmit = false };

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
			if (!store.Rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = store.Rooms[roomCode];
			var (cipherType, encryptedText, plaintext) =
				CipherGeneratorHelper.GenerateRandom(room.UsedCipherTypes, room.UsedSentences);

			room.UsedCipherTypes.Add(cipherType);
			room.UsedSentences.Add(plaintext);
			room.RoundStartedAt = DateTime.Now.AddSeconds(RoomConstants.PreRoundSeconds * 2);
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
			if (!store.Rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = store.Rooms[roomCode];
			var round = room.Rounds[room.CurrentRound - 1];
			var submissions = round.Submissions
				.OrderBy(x => x.SubmissionTime)
				.ToList();

			string? winnerUserId = null;
			bool wasLastRound = room.CurrentRound == RoomConstants.RoundsPerRace;
			int majority = (RoomConstants.RoundsPerRace / 2) + 1;

			lock (room)
			{
				if (round.IsFinished) return null;

				if (didTimerRanOut || submissions.Count < 2)
				{
					round.IsFinished = true;
					bool alreadyDecided = room.Player1Score >= majority || room.Player2Score >= majority;
					return new RoundResultDTO
					{
						RoundNumber = room.CurrentRound,
						WasLastRound = wasLastRound || alreadyDecided,
						WinnerUsername = null,
					};
				}

				var firstSubmission = submissions[0];
				var secondSubmission = submissions[1];

				if (!firstSubmission.IsCorrect && !secondSubmission.IsCorrect)
				{
					round.IsFinished = true;
					bool alreadyDecided = room.Player1Score >= majority || room.Player2Score >= majority;
					return new RoundResultDTO
					{
						RoundNumber = room.CurrentRound,
						WasLastRound = wasLastRound || alreadyDecided,
						WinnerUsername = null,
					};
				}

				if (firstSubmission.IsCorrect && !secondSubmission.IsCorrect ||
					(firstSubmission.IsCorrect && secondSubmission.IsCorrect))
					winnerUserId = firstSubmission.UserId;
				else
					winnerUserId = secondSubmission.UserId;

				round.WinnerId = winnerUserId;
				round.IsFinished = true;
			}

			var user = await userManager.FindByIdAsync(winnerUserId);
			if (user == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			if (winnerUserId == room.Player1Id)
				room.Player1Score++;
			else
				room.Player2Score++;

			wasLastRound = room.CurrentRound == RoomConstants.RoundsPerRace ||
						   room.Player1Score >= majority ||
						   room.Player2Score >= majority;

			return new RoundResultDTO
			{
				RoundNumber = room.CurrentRound,
				WasLastRound = wasLastRound,
				WinnerUsername = user.UserName
			};
		}
		public void SetRoundTimer(string roomCode, CancellationTokenSource cts)
		{
			if (!store.Rooms.TryGetValue(roomCode, out var room)) return;
			room.RoundTimer = cts;
		}
		public void CancelRoundTimer(string roomCode)
		{
			if (!store.Rooms.TryGetValue(roomCode, out var room)) return;
			room.RoundTimer?.Cancel();
			room.RoundTimer = null;
		}
		public async Task<GameResultDTO> EndGame(string roomCode)
		{
			if (!store.Rooms.ContainsKey(roomCode))
				throw new NotFoundException(RoomConstants.RoomNotFound);

			var room = store.Rooms[roomCode];
			room.Status = RoomStatus.Finished;

			var firstPlayer = await userManager.FindByIdAsync(room.Player1Id);
			var secondPlayer = await userManager.FindByIdAsync(room.Player2Id!);

			if (firstPlayer == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);
			if (secondPlayer == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			string? winnerUsername = null;

			if (room.Player1Score > room.Player2Score)
				winnerUsername = firstPlayer.UserName;
			else if (room.Player2Score > room.Player1Score)
				winnerUsername = secondPlayer.UserName;

			if (winnerUsername != null)
			{
				var winner = room.Player1Score > room.Player2Score ? firstPlayer : secondPlayer;
				winner.RoomsWon++;

				if (room.WagersLocked && room.WagerAmount > 0)
					winner.Score += room.WagerAmount * 2;

				await userManager.UpdateAsync(winner);
			}
			else if (room.WagersLocked && room.WagerAmount > 0)
			{
				firstPlayer.Score += room.WagerAmount;
				secondPlayer.Score += room.WagerAmount;
				await userManager.UpdateAsync(firstPlayer);
				await userManager.UpdateAsync(secondPlayer);
			}

			store.Rooms.TryRemove(roomCode, out _);

			return new GameResultDTO
			{
				WinnerUsername = winnerUsername,
				Player1Score = room.Player1Score,
				Player2Score = room.Player2Score,
				Player1Username = firstPlayer.UserName!,
				Player2Username = secondPlayer.UserName!,
				WagerAmount = room.WagerAmount
			};
		}
	}
}