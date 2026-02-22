namespace Cryptomind.Core.Contracts
{
	public interface IBadgeStatisticsService
	{
		Task<int> GetSolvedCount(string userId);
		Task<int> GetApprovedCount(string userId);
		Task<int> GetDistinctCipherTypesSolved(string userId);
		Task<int> GetApprovedAnswersCount(string userId);
		Task<int> GetSolvedWithoutHintCount(string userId);
		Task<int> GetSolvedOnFirstAttemptCount(string userId);
		Task<int> GetUsedHints(string userId);
		Task<int> GetRareSolves(string userId);
	}
}
