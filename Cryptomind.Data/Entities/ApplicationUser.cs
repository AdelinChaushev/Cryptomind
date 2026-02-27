using Microsoft.AspNetCore.Identity;

namespace Cryptomind.Data.Entities
{
	public class ApplicationUser : IdentityUser
	{
		public ApplicationUser()
		{
			UploadedCiphers = new List<Cipher>();
			CipherAnswers = new List<UserSolution>();
			HintsRequested = new List<HintRequest>();
			Notifications = new List<Notification>();
			Badges = new List<UserBadge>();
		}
		public int Score { get; set; }
		public int SolvedCount => CipherAnswers.Count(x => x.UserId == Id && x.IsCorrect);
		public int AttemptedCiphers { get; set; }
		public bool IsBanned { get; set; }
		public bool IsDeactivated { get; set; }
		public string? BanReason { get; set; }
		public int LeaderBoardPlace { get; set; }
		public DateTime RegisteredAt { get; set; }
		public DateTime? BannedAt { get; set; }
		public DateTime? DeactivatedAt { get; set; }
		public double SuccessRate => CalculateSuccessRate();
		public ICollection<Cipher> UploadedCiphers { get; set; }
		public ICollection<UserSolution> CipherAnswers { get; set; }
		public ICollection<HintRequest> HintsRequested { get; set; }
		public ICollection<AnswerSuggestion> SuggestedAnswers { get; set; }
		public ICollection<UserBadge> Badges { get; set; }
		public ICollection<Notification> Notifications { get; set; }
		private double CalculateSuccessRate()
		{
			if (AttemptedCiphers == 0) return 0;

			return ((double)SolvedCount / AttemptedCiphers) * 100;
		}
	}
}
