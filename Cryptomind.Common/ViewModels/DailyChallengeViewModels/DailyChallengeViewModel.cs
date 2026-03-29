namespace Cryptomind.Common.ViewModels.DailyChallengeViewModels
{
	public class DailyChallengeViewModel
	{
		public int EntryId { get; set; }
		public string EncryptedText { get; set; }
		public string ChallengeDate { get; set; }
		public bool AlreadySolvedToday { get; set; }
		public int AttemptCount { get; set; }
		public int UserCurrentStreak { get; set; }
	}
}
