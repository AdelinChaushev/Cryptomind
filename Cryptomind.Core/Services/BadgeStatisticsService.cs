using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class BadgeStatisticsService(
		IRepository<ApplicationUser, string> userRepo,
		IRepository<UserSolution, int> solutionRepo,
		IRepository<HintRequest, int> hintRepo) : IBadgeStatisticsService
	{
		public async Task<int> GetApprovedCount(string userId)
		{
			return await userRepo.GetAllAttached()
				.Where(u => u.Id == userId)
				.SelectMany(u => u.UploadedCiphers)
				.CountAsync(c => c.Status == ApprovalStatus.Approved);
		}
		public async Task<int> GetApprovedAnswersCount(string userId)
		{
			return await userRepo.GetAllAttached()
				.Where(u => u.Id == userId)
				.SelectMany(u => u.SuggestedAnswers)
				.CountAsync(a => a.Status == ApprovalStatus.Approved);
		}
		public async Task<int> GetSolvedCount(string userId)
		{
			return await solutionRepo.GetAllAttached()
				.CountAsync(s => s.UserId == userId && s.IsCorrect);
		}
		public async Task<int> GetDistinctCipherTypesSolved(string userId)
		{
			return await solutionRepo.GetAllAttached()
				.Where(s => s.UserId == userId && s.IsCorrect && s.Cipher.Status == ApprovalStatus.Approved)
				.Select(s => s.Cipher.TypeOfCipher)
				.Distinct()
				.CountAsync();
		}
		public async Task<int> GetSolvedWithoutHintCount(string userId)
		{
			return await solutionRepo.GetAllAttached()
				.CountAsync(s => s.UserId == userId && s.IsCorrect
					&& !s.UsedFullSolution && !s.UsedTypeHint && !s.UsedSolutionHint);
		}
		public async Task<int> GetSolvedOnFirstAttemptCount(string userId)
		{
			var groups = await solutionRepo.GetAllAttached()
				.Where(s => s.UserId == userId)
				.GroupBy(s => s.CipherId)
				.Select(g => new
				{
					Count = g.Count(),
					HasCorrect = g.Any(s => s.IsCorrect)
				})
				.ToListAsync();

			return groups.Count(g => g.Count == 1 && g.HasCorrect);
		}
		public async Task<int> GetUsedHints(string userId)
		{
			return await hintRepo.GetAllAttached()
				.Where(h => h.UserId == userId)
				.Select(h => h.CipherId)
				.Distinct()
				.CountAsync();
		}
		public async Task<int> GetRareSolves(string userId)
		{
			return await solutionRepo.GetAllAttached()
				.CountAsync(s => s.UserId == userId && s.IsCorrect && s.IsRareSolved);
		}
	}
}
