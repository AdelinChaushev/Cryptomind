using Cryptomind.Core.Badges;
using Cryptomind.Core.Badges.Criteria;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class BadgeService : IBadgeService
	{
		private readonly Dictionary<int, IBadgeCriteria> badgeCriteria;
		private readonly IRepository<UserBadge, int> userBadgeRepo;
		private readonly IRepository<ApplicationUser, string> userRepo;
		private readonly IRepository<Badge, int> badgeRepo;

		private readonly IBadgeStatisticsService statsService;
		private readonly INotificationService notificationService;
		public BadgeService(
			IRepository<Badge, int> badgeRepo,
			IRepository<UserBadge, int> userBadgeRepo,
			IRepository<ApplicationUser, string> userRepo,
			IBadgeStatisticsService statsService,
			INotificationService notificationService)
		{
			this.statsService = statsService;
			this.userBadgeRepo = userBadgeRepo;
			this.userRepo = userRepo;
			this.badgeRepo = badgeRepo;
			this.notificationService = notificationService;

			// All criteria use the same statsService
			badgeCriteria = new Dictionary<int, IBadgeCriteria>
			{
				{ 1, new SolvedCountCriteria(this.statsService, 1) },
				{ 2, new SolvedCountCriteria(this.statsService, 25) },
				//new ScoreCriteria(statsService, 500)
				//new ScoreCriteria(statsService, 2000)
				{ 3, new UploadCountCriteria(this.statsService, 1) },
				{ 4, new UploadCountCriteria(this.statsService, 5) },
				//NoHintsCriteria(statsService, 10)
				{ 5, new DistinctSolvedCountCriteria(this.statsService, 5) },
				{ 6, new SuggestedAnswerCountCriteria(this.statsService, 1) },
			};
		}

		public async Task CheckBadgesByCategory(string userId, BadgeCategory category)
		{
			var userBadgeIds = userBadgeRepo.GetAllAttached()
				.Where(x => x.UserId == userId)
				.Select(x => x.BadgeId)
				.ToList();

			foreach (var kvp in badgeCriteria.Where(b => b.Value.Category == category))
			{
				int badgeId = kvp.Key;
				IBadgeCriteria criteria = kvp.Value;

				if (userBadgeIds.Contains(badgeId))
					continue;

				if (await criteria.IsSatisfied(userId))
				{
					await AwardBadge(userId, badgeId);
				}
			}
		}
		private async Task AwardBadge(string userId, int badgeId)
		{
			var userBadge = new UserBadge
			{
				UserId = userId,
				BadgeId = badgeId,
				EarnedAt = DateTime.UtcNow
			};

			var user = userRepo.GetAllAttached().Include(x => x.Badges).FirstOrDefault(x => x.Id == userId);
			if (user.Badges.FirstOrDefault(x => x.BadgeId == userBadge.BadgeId) != null)
				throw new InvalidOperationException("User already has this badge"); // second time checking it

			user.Badges.Add(userBadge);

			var badge = badgeRepo.GetAllAttached().Include(x => x.UserBadges).FirstOrDefault(x => x.Id == badgeId);
			if (badge.UserBadges.FirstOrDefault(x => x.UserId == userId) != null)
				throw new InvalidOperationException("This badge is already assigned to this user"); // third time checking it

			badge.UserBadges.Add(userBadge);
			badge.EarnedBy++;

			await userBadgeRepo.AddAsync(userBadge);
			await notificationService.CreateAndSendNotification(userId, NotificationType.BadgeEarned, $"You earned {badge.Title} badge!", badgeId, string.Empty);
		}
	}
}