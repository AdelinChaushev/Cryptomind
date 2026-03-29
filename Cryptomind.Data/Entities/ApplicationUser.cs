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
			SuggestedAnswers = new List<AnswerSuggestion>();
			DailyChallengeParticipations = new List<DailyChallengeParticipation>();
		}

		public int Score { get; set; }
		public int CurrentStreak { get; set; }
		public int LongestStreak { get; set; }
		public bool IsBanned { get; set; }
		public bool IsDeactivated { get; set; }
		public string? BanReason { get; set; }
		public int LeaderBoardPlace { get; set; }
		public int RoomsWon { get; set; }
		public DateTime RegisteredAt { get; set; }
		public DateTime? BannedAt { get; set; }
		public DateTime? DeactivatedAt { get; set; }
		public int SolvedCount => CipherAnswers.Count(x => x.IsCorrect);
		public int AttemptedCiphersCount => CipherAnswers.Select(x => x.CipherId).Distinct().Count();
		public double SuccessRate => AttemptedCiphersCount == 0 ? 0 : ((double)SolvedCount / AttemptedCiphersCount) * 100;
		public ICollection<Cipher> UploadedCiphers { get; set; }
		public ICollection<UserSolution> CipherAnswers { get; set; }
		public ICollection<HintRequest> HintsRequested { get; set; }
		public ICollection<AnswerSuggestion> SuggestedAnswers { get; set; }
		public ICollection<UserBadge> Badges { get; set; }
		public ICollection<Notification> Notifications { get; set; }
		public ICollection<DailyChallengeParticipation> DailyChallengeParticipations { get; set; }
	}
}