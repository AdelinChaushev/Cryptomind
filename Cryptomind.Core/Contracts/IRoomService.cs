using Cryptomind.Common.DTOs;
using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Contracts
{
	public interface IRoomService
	{
		Task<string> CreateRoom(string creatorId);
		void RemoveRoom(string roomCode);
		Task<(string player1Username, string player2Username)> GetPlayerUsernames(string roomCode);
		Task<bool> JoinRoom(string roomCode, string userId);
		Task<bool> SetReady(string roomCode, string userId);
		bool IsPlayerInRoom(string roomCode, string userId);
		string StartRoom(string roomCode);
		Task<RoomSubmissionResultDTO> SubmitAnwer(string roomCode, string userId, CipherType answer);
		string NextRound(string roomCode);
		Task<RoundResultDTO?> EndRound(string roomCode, bool didTimerRanOut);
		void SetRoundTimer(string roomCode, CancellationTokenSource cts);
		void CancelRoundTimer(string roomCode);
		Task<GameResultDTO> EndGame(string roomCode);
	}
}
