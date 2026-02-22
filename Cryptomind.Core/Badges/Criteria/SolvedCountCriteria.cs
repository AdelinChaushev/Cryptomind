using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Badges.Criteria
{
	public class SolvedCountCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService statsService;
		private readonly int requiredCount;
		public BadgeCategory Category => BadgeCategory.OnSolve;
		public SolvedCountCriteria(IBadgeStatisticsService statsService, int requiredCount)
		{
			this.statsService = statsService;
			this.requiredCount = requiredCount;
		}
		public async Task<bool> IsSatisfied(string userId)
		{
			var solvedCount = await statsService.GetSolvedCount(userId);
			return solvedCount >= requiredCount;
		}
	}
}
