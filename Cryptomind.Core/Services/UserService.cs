using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cryptomind.Common.ViewModels.UserViewModels;
using Cryptomind.Common.Exceptions;

namespace Cryptomind.Core.Services
{
	public class UserService(
		IRepository<ApplicationUser, string> userRepo,
		UserManager<ApplicationUser> userManager) : IUserService
	{
		public async Task<IEnumerable<string>> GetRolesUsers(string id)
		{
			var user = await userManager.FindByIdAsync(id);

			if (user == null)
				throw new NotFoundException("User not found");

			return await userManager.GetRolesAsync(user);

		}
		public async Task<AccountViewModel?> GetUserAccountInfo(string id)
		{
			var user = await userRepo.GetAllAttached()
				.Include(x => x.Badges)
				.ThenInclude(x => x.Badge)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (user == null)
				throw new NotFoundException("User not found");

			ICollection<BadgeViewModel> badges = user.Badges.Select(x => new BadgeViewModel
			{
				Title = x.Badge.Title,
				Description = x.Badge.Description,
				EarnedBy = x.Badge.EarnedBy,
			}).ToList();

			AccountViewModel result = new AccountViewModel()
			{
				Username = user.UserName,
				Email = user.Email,
				Roles = (await userManager.GetRolesAsync(user)).ToArray(),
				RegisteredAt = user.RegisteredAt,
				SolvedCount = user.SolvedCount,
				Score = user.Score,
				AttemptedCiphers = user.AttemptedCiphers,
				LeaderBoardPlace = user.LeaderBoardPlace,
				SuccessRate = user.SuccessRate,
				Badges = badges,
			};
			return result;
		}
	}
}
