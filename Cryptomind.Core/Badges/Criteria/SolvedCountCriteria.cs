using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Badges.Criteria
{
	public class SolvedCountCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService _statsService;
		private readonly int _requiredCount;
		public BadgeCategory Category => BadgeCategory.OnSolve;
		public SolvedCountCriteria(IBadgeStatisticsService statsService, int requiredCount)
		{
			_statsService = statsService;
			_requiredCount = requiredCount;
		}

		public async Task<bool> IsSatisfied(string userId)
		{
			var solvedCount = await _statsService.GetSolvedCount(userId);
			return solvedCount >= _requiredCount;
		}
	}
}
