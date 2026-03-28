namespace Cryptomind.Common.DTOs
{
	public class GameStateDTO
	{
		public string RoomCode { get; set; } = string.Empty;
		public string EncryptedText { get; set; } = string.Empty;
		public string? NextEncryptedText { get; set; }
		public int CurrentRound { get; set; }
		public int SecondsElapsed { get; set; }
		public int TransitionMsRemaining { get; set; }
		public bool HasSubmitted { get; set; }
		public bool IsRoundEnd { get; set; }
	}
}