using Cryptomind.Core.Rooms.Enums;

namespace Cryptomind.Core.Rooms
{
	public class RaceRoom
	{
		public RaceRoom()
		{
			Rounds = new List<Round>();
		}
		public string Code { get; set; }
		public CancellationTokenSource? RoundTimer { get; set; }
		public int CurrentRound { get; set; }
		public RoomStatus Status { get; set; }
		public List<Round> Rounds { get; set; }
		public string Player1Id { get; set; }
		public string? Player2Id { get; set; }
		public bool Player1Ready { get; set; }
		public bool Player2Ready { get; set; }
		public int Player1Score { get; set; }
		public int Player2Score { get; set; }
	}
}
