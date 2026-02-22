using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Badges.Criteria
{
	public class RareSolveCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService statsService;
		private readonly int requiredCount;
		public BadgeCategory Category => BadgeCategory.OnSolve;
		public RareSolveCriteria(IBadgeStatisticsService statsService, int requiredCount)
		{
			this.statsService = statsService;
			this.requiredCount = requiredCount;
		}

		public async Task<bool> IsSatisfied(string userId)
		{
			var solvedCount = await statsService.GetRareSolves(userId);
			return solvedCount >= requiredCount;
		}
	}
}
