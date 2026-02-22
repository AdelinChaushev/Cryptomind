namespace Cryptomind.Common.ViewModels.UserViewModels
{
	public class AccountViewModel
	{
		public string Username { get; set; }
		public string Email { get; set; }
		public string[] Roles { get; set; }
		public DateTime RegisteredAt { get; set; }
		public int SolvedCount { get; set; }
		public int Score { get; set; }
		public int AttemptedCiphers { get; set; }
		public int LeaderBoardPlace { get; set; }
		public double SuccessRate {get; set;}
		public ICollection<BadgeViewModel> Badges { get; set; }
	}
}
