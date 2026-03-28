namespace Cryptomind.Common.DTOs
{
	public class WagerInfoDTO
	{
		public int WagerAmount { get; set; }
		public string CreatorUsername { get; set; } = string.Empty;
		public int JoinerBalance { get; set; }
	}
}