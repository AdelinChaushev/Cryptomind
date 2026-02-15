using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Cryptomind.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class BadgeStatisticsService(
		IRepository<Cipher, int> cipherRepo, 
		IRepository<ApplicationUser, string> userRepo,
		IRepository<UserSolution, int> solutionRepo) : IBadgeStatisticsService
	{
		public async Task<int> GetApprovedCount(string userId)
		{
			return await userRepo.GetAllAttached()
				.Where(u => u.Id == userId)
				.SelectMany(u => u.UploadedCiphers)
				.CountAsync(c => c.Status == ApprovalStatus.Approved);
		}
		public async Task<int> GetDistinctCipherTypesSolved(string userId)
		{
			return await solutionRepo.GetAllAttached()
				.Where(s => s.UserId == userId)
				.Select(s => s.Cipher.TypeOfCipher)
				.Distinct()
				.CountAsync();
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
			var user = await userRepo.GetByIdAsync(userId);
			if (user == null)
				throw new InvalidOperationException("User not found");
			return user.SolvedCount;
		}
	}
}
