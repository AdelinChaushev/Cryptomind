using Cryptomind.Common.ViewModels.LeaderboardPlaceViewModel;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class LeaderboardService(
		IRepository<ApplicationUser, string> userRepo) : ILeaderboardService
	{
		public async Task<List<LeaderboardPlaceViewModel>> GetLeaderboard()
		{
			var users = await userRepo.GetAllAttached()
				.Where(x => !x.IsBanned && !x.IsDeactivated)
				.OrderByDescending(x => x.Score)
				.ToListAsync();

			var models = new List<LeaderboardPlaceViewModel>();

			return users.Select((user, index) => new LeaderboardPlaceViewModel
			{
				Username = user.UserName,
				Points = user.Score,
				Place = index + 1,
			}).ToList();
		}
	}
}
