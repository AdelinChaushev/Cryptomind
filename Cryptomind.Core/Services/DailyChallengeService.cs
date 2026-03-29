using Cryptomind.Common.ViewModels.DailyChallengeViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class DailyChallengeService(
		IRepository<DailyChallengeEntry, int> entryRepo,
		IRepository<DailyChallengeParticipation, int> participationRepo,
		IRepository<ApplicationUser, string> userRepo) : IDailyChallengeService
	{
		public async Task<DailyChallengeViewModel> GetTodaysChallengeAsync(string userId)
		{
			var today = DateTime.UtcNow.Date;
			var entry = await GetOrAssignTodaysChallengeAsync(today);

			var participation = await participationRepo.FirstOrDefaultAsync(
				p => p.UserId == userId && p.ChallengeDate == today);

			var user = await userRepo.GetByIdAsync(userId);

			if (user != null && user.CurrentStreak > 0)
			{
				var yesterday = today.AddDays(-1);
				bool solvedYesterday = await participationRepo.GetAllAttached()
					.AnyAsync(p => p.UserId == userId && p.ChallengeDate == yesterday && p.IsCompleted);
				bool solvedToday = participation?.IsCompleted ?? false;

				if (!solvedYesterday && !solvedToday)
				{
					user.CurrentStreak = 0;
					await userRepo.UpdateAsync(user);
				}
			}

			return new DailyChallengeViewModel
			{
				EntryId = entry.Id,
				EncryptedText = entry.EncryptedText,
				ChallengeDate = today.ToString("yyyy-MM-dd"),
				AlreadySolvedToday = participation?.IsCompleted ?? false,
				AttemptCount = participation?.AttemptCount ?? 0,
				UserCurrentStreak = user?.CurrentStreak ?? 0,
			};
		}
		public async Task<DailyChallengeSubmitResultViewModel> SubmitAnswerAsync(string userId, string answer)
		{
			var today = DateTime.UtcNow.Date;
			var entry = await GetOrAssignTodaysChallengeAsync(today);

			var participation = await participationRepo.FirstOrDefaultAsync(
				p => p.UserId == userId && p.ChallengeDate == today);

			if (participation == null)
			{
				participation = new DailyChallengeParticipation
				{
					UserId = userId,
					DailyChallengeEntryId = entry.Id,
					ChallengeDate = today,
					AttemptCount = 0,
				};
				await participationRepo.AddAsync(participation);
			}

			if (participation.IsCompleted)
				return new DailyChallengeSubmitResultViewModel { IsCorrect = false };

			participation.AttemptCount++;

			bool isCorrect = Normalize(answer) == Normalize(entry.PlainText);

			if (isCorrect)
			{
				participation.IsCompleted = true;
				participation.SolvedAt = DateTime.UtcNow;

				var user = await userRepo.GetByIdAsync(userId);
				if (user != null)
				{
					var yesterday = today.AddDays(-1);
					bool solvedYesterday = await participationRepo.GetAllAttached()
						.AnyAsync(p => p.UserId == userId && p.ChallengeDate == yesterday && p.IsCompleted);

					user.CurrentStreak = solvedYesterday ? user.CurrentStreak + 1 : 1;

					if (user.CurrentStreak > user.LongestStreak)
						user.LongestStreak = user.CurrentStreak;

					await userRepo.UpdateAsync(user);

					await participationRepo.UpdateAsync(participation);

					return new DailyChallengeSubmitResultViewModel
					{
						IsCorrect = true,
						NewStreak = user.CurrentStreak,
						CorrectAnswer = entry.PlainText,
					};
				}
			}

			await participationRepo.UpdateAsync(participation);

			return new DailyChallengeSubmitResultViewModel { IsCorrect = false };
		}
		private async Task<DailyChallengeEntry> GetOrAssignTodaysChallengeAsync(DateTime today)
		{
			var entry = await entryRepo.FirstOrDefaultAsync(e => e.AssignedDate != null && e.AssignedDate.Value.Date == today);

			if (entry == null)
			{
				entry = await entryRepo.FirstOrDefaultAsync(e => !e.IsUsed);
				if (entry == null)
					throw new InvalidOperationException("Daily challenge pool is exhausted. Please reseed.");

				entry.IsUsed = true;
				entry.AssignedDate = today;
				await entryRepo.UpdateAsync(entry);
			}

			return entry;
		}
		#region Private-methods
		private static string Normalize(string text)
			=> new string(text.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
		#endregion
	}
}
