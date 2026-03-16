namespace Cryptomind.Common.DTOs
{
	public class RoomSubmissionResultDTO
	{
		public string? WinnerUsername { get; set; }
		public bool DidBothSubmit { get; set; }
		public bool? WasLastRound { get; set; }
	}
}
