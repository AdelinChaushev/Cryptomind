using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Badges.Criteria
{
	public class ApprovedCiphersCountCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService statsService;
		private readonly int requiredCount;
		public BadgeCategory Category => BadgeCategory.OnUpload;
		public ApprovedCiphersCountCriteria(IBadgeStatisticsService statsService, int requiredCount)
		{
			this.statsService = statsService;
			this.requiredCount = requiredCount;
		}
		public async Task<bool> IsSatisfied(string userId)
		{
			var uploadedCount = await statsService.GetApprovedCount(userId);
			return uploadedCount >= requiredCount;
		}
	}
}
