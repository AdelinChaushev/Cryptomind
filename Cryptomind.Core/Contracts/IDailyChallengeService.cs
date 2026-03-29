using Cryptomind.Common.ViewModels.DailyChallengeViewModels;

namespace Cryptomind.Core.Contracts
{
	public interface IDailyChallengeService
	{
		Task<DailyChallengeViewModel> GetTodaysChallengeAsync(string userId);
		Task<DailyChallengeSubmitResultViewModel> SubmitAnswerAsync(string userId, string answer);
	}
}
