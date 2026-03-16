namespace Cryptomind.Common.DTOs
{
	public class RoundResultDTO
	{
		public string? WinnerUsername { get; set; }
		public int RoundNumber { get; set; }
		public bool WasLastRound { get; set; }
	}
}
