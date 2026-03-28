namespace Cryptomind.Common.DTOs
{
	public class GameResultDTO
	{
		public string? WinnerUsername { get; set; }
		public string Player1Username { get; set; } = string.Empty;
		public string Player2Username { get; set; } = string.Empty;
		public int Player1Score { get; set; }
		public int Player2Score { get; set; }
		public int WagerAmount { get; set; }
	}
}