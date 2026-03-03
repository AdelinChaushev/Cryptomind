using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
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
			{ 3, new SolvedCountCriteria(statsService, 50) },
			{ 4, new SolvedCountCriteria(statsService, 100) },

			{ 5, new DistinctSolvedCountCriteria(statsService, 5) },
			{ 6, new DistinctSolvedCountCriteria(statsService, 10) },

			{ 7, new ApprovedCiphersCountCriteria(statsService, 1)},
			{ 8, new ApprovedCiphersCountCriteria(statsService, 5) },
			{ 9, new ApprovedCiphersCountCriteria(statsService, 15) },

			{ 10, new ApprovedAnswerSuggestionCountCriteria(statsService, 1)},
			{ 11, new ApprovedAnswerSuggestionCountCriteria(statsService, 10)},

			{ 12, new NoHintsSolvedCriteria(statsService, 10)},

			{ 13, new PerfectSolveCountCriteria(statsService, 10) },
			{ 14, new HintUsageCountCriteria(statsService, 25)},
			{ 15, new RareSolveCriteria(statsService, 25)},
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

				if (userBadgeIds.Contains(badgeId)) //First time checking
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
				throw new Exception(string.Format(BadgeErrorConstants.UserNotFoundForBadge, badgeId));

			if (user.Badges.Any(x => x.BadgeId == badgeId)) //Second time checking
				throw new ConflictException(BadgeErrorConstants.AlreadyHaveThisBadge);

			var badge = await badgeRepo.GetAllAttached()
				.Include(x => x.UserBadges)
				.FirstOrDefaultAsync(x => x.Id == badgeId);

			if (badge == null)
				throw new NotFoundException(BadgeErrorConstants.BadgeNotFound);

			var userBadge = new UserBadge
			{
				UserId = userId,
				BadgeId = badgeId,
				EarnedAt = DateTime.UtcNow.AddHours(2)
			};
			try
			{
				await userBadgeRepo.AddAsync(userBadge);

				badge.EarnedBy++;

				await badgeRepo.UpdateAsync(badge);
				await notificationService.CreateAndSendNotification(
					userId,
					NotificationType.BadgeEarned,
					$"Спечелихте значка {badge.Title}!",
					"account-info"); //There might be a different page with badges.
			}
			catch (DbUpdateException) //Third time checking
			{
				// Badge already awarded by concurrent request
				return;
			}
		}
	}
}