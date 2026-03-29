namespace Cryptomind.Common.ViewModels.DailyChallengeViewModels
{
	public class DailyChallengeSubmitResultViewModel
	{
		public bool IsCorrect { get; set; }
		public int NewStreak { get; set; }
		public string? CorrectAnswer { get; set; }
	}
}
