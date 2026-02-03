using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Badges.Criteria
{
	public class SuggestedAnswerCountCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService _statsService;
		private readonly int _requiredCount;
		public BadgeCategory Category => BadgeCategory.OnSuggesting;
		public SuggestedAnswerCountCriteria(IBadgeStatisticsService statsService, int requiredCount)
		{
			_statsService = statsService;
			_requiredCount = requiredCount;
		}

		public async Task<bool> IsSatisfied(string userId)
		{
			var approvedAnswersCount = await _statsService.GetApprovedAnswersCount(userId);
			return _requiredCount >= approvedAnswersCount;
		}
	}
}
