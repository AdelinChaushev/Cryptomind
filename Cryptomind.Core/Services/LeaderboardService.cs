using Cryptomind.Common.ViewModels.LeaderboardPlaceViewModel;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class LeaderboardService(
		IRepository<ApplicationUser, string> userRepo) : ILeaderboardService
	{
		public async Task<List<LeaderboardPlaceViewModel>> GetLeaderboard()
		{
			var users = (await userRepo.GetAllAsync())
				.OrderByDescending(x => x.Score)
				.ToList();

			var models = new List<LeaderboardPlaceViewModel>();

			for (int i = 0; i < users.Count; i++)
			{
				var model = new LeaderboardPlaceViewModel
				{
					Username = users[i].UserName,
					Points = users[i].Score,
					Place = i + 1,
				};

				models.Add(model);
			}

			return models;
		}
	}
}
