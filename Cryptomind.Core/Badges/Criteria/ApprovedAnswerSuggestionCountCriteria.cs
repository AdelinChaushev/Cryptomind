using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Badges.Criteria
{
	public class ApprovedAnswerSuggestionCountCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService statsService;
		private readonly int requiredCount;
		public BadgeCategory Category => BadgeCategory.OnSuggesting;
		public ApprovedAnswerSuggestionCountCriteria(IBadgeStatisticsService statsService, int requiredCount)
		{
			this.statsService = statsService;
			this.requiredCount = requiredCount;
		}
		public async Task<bool> IsSatisfied(string userId)
		{
			var approvedAnswersCount = await statsService.GetApprovedAnswersCount(userId);
			return requiredCount >= approvedAnswersCount;
		}
	}
}
