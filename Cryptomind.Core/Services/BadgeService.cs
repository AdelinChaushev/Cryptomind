using Cryptomind.Core.Badges;
using Cryptomind.Core.Badges.Criteria;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class BadgeService(
		IRepository<UserBadge, int> userBadgeRepo,
		IRepository<ApplicationUser, string> userRepo,
		IRepository<Badge, int> badgeRepo,
		IBadgeStatisticsService statsService,
		INotificationService notificationService) : IBadgeService
	{
		//The key should be matching with the badge ID!!!
		private readonly Dictionary<int, IBadgeCriteria> badgeCriteria = new Dictionary<int, IBadgeCriteria>
		{
			{ 1, new SolvedCountCriteria(statsService, 1) },
			{ 2, new SolvedCountCriteria(statsService, 25) },
			//new ScoreCriteria(statsService, 500)
			//new ScoreCriteria(statsService, 2000)
			{ 3, new UploadCountCriteria(statsService, 1) },
			{ 4, new UploadCountCriteria(statsService, 5) },
			//NoHintsCriteria(statsService, 10)
			{ 5, new DistinctSolvedCountCriteria(statsService, 5) },
			{ 6, new SuggestedAnswerCountCriteria(statsService, 1) },
		};

		public async Task CheckBadgesByCategory(string userId, BadgeCategory category)
		{
			var userBadgeIds = await userBadgeRepo.GetAllAttached()
				.Where(x => x.UserId == userId)
				.Select(x => x.BadgeId)
				.ToListAsync();

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
			var user = await userRepo.GetAllAttached()
				.Include(x => x.Badges)
				.FirstOrDefaultAsync(x => x.Id == userId);

			if (user == null)
				throw new InvalidOperationException("User not found");

			var badge = await badgeRepo.GetAllAttached()
				.Include(x => x.UserBadges)
				.FirstOrDefaultAsync(x => x.Id == badgeId);

			if (badge == null)
				throw new InvalidOperationException("Badge not found");

			var userBadge = new UserBadge
			{
				UserId = userId,
				BadgeId = badgeId,
				EarnedAt = DateTime.UtcNow
			};
			try
			{
				await userBadgeRepo.AddAsync(userBadge);

				badge.EarnedBy++;

				await badgeRepo.UpdateAsync(badge);
				await notificationService.CreateAndSendNotification(
					userId,
					NotificationType.BadgeEarned,
					$"You earned {badge.Title} badge!",
					"api/user/get-account-info"); //There might be a different page with badges.
			}
			catch (DbUpdateException)
			{
				// Badge already awarded by concurrent request
				return;
			}
		}
	}
}