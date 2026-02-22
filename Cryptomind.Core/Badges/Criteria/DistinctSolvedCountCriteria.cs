using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
namespace Cryptomind.Core.Badges.Criteria
{
	public class DistinctSolvedCountCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService statsService;
		private readonly int requiredCount;
		public BadgeCategory Category => BadgeCategory.OnSolve;
		public DistinctSolvedCountCriteria(IBadgeStatisticsService statsService, int requiredCount)
		{
			this.statsService = statsService;
			this.requiredCount = requiredCount;
		}

		public async Task<bool> IsSatisfied(string userId)
		{
			var distinctSolvedCount = await statsService.GetDistinctCipherTypesSolved(userId);
			return distinctSolvedCount >= requiredCount;
		}
	}
}
