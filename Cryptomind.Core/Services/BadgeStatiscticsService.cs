using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class BadgeStatiscticsService(
		IRepository<Cipher, int> cipherRepo, 
		IRepository<ApplicationUser, string> userRepo,
		IRepository<UserSolution, int> solutionRepo) : IBadgeStatisticsService
	{
		public async Task<int> GetApprovedCount(string userId)
		{
			return userRepo.GetAllAttached()
				.Include(x => x.UploadedCiphers)
				.FirstOrDefault(x => x.Id == userId).UploadedCiphers.Count;
		}

		public async Task<int> GetDestinctCipherTypesSolved(string userId)
		{
			return solutionRepo.GetAllAttached()
				.Include(x => x.Cipher)
				.Where(x => x.UserId == userId)
				.Select(x => x.Cipher.TypeOfCipher)
				.Distinct()
				.Count();
		}

		public async Task<int> GetSolvedCount(string userId)
			=> (await userRepo.GetByIdAsync(userId)).SolvedCount;

		public async Task<ApplicationUser> GetUser(string userId)
			=> await userRepo.GetByIdAsync(userId);
	}
}
