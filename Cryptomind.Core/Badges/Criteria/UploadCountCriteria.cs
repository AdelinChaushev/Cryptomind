using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Badges.Criteria
{
	public class UploadCountCriteria : IBadgeCriteria
	{
		private readonly IBadgeStatisticsService _statsService;
		private readonly int _requiredCount;
		public BadgeCategory Category => BadgeCategory.OnUpload;
		public UploadCountCriteria(IBadgeStatisticsService statsService, int requiredCount)
		{
			_statsService = statsService;
			_requiredCount = requiredCount;
		}
		public async Task<bool> IsSatisfied(string userId)
		{
			var uploadedCount = await _statsService.GetApprovedCount(userId);
			return uploadedCount >= _requiredCount;
		}
	}
}
