using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Badges.Criteria
{
	public class RevealSecretCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService statsService;
		public BadgeCategory Category => BadgeCategory.OnSecretRevealing;
		public RevealSecretCriteria(IBadgeStatisticsService statsService)
		{
			this.statsService = statsService;
		}

		public Task<bool> IsSatisfied(string userId)
			=> statsService.HasRevealedSecret(userId);
	}
}
