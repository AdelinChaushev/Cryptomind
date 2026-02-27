using Cryptomind.Common.ViewModels.LeaderboardPlaceViewModel;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class LeaderboardService(
		IRepository<ApplicationUser, string> userRepo,
		UserManager<ApplicationUser> userManager) : ILeaderboardService
	{
		public async Task<List<LeaderboardPlaceViewModel>> GetLeaderboard()
		{
			var allUsers = await userRepo.GetAllAttached()
				.Where(x => !x.IsBanned && !x.IsDeactivated)
				.OrderByDescending(x => x.Score)
				.ToListAsync();

			var filteredUsers = new List<ApplicationUser>();
			foreach (var user in allUsers)
			{
				var roles = await userManager.GetRolesAsync(user);
				if (!roles.Contains("Admin"))
					filteredUsers.Add(user);
			}

			return filteredUsers
				.Take(20)
				.Select((user, index) => new LeaderboardPlaceViewModel
				{
					Username = user.UserName,
					Points = user.Score,
					Place = index + 1,
				}).ToList();
		}
	}
}
