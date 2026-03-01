using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.UserViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
				throw new NotFoundException(UserConstants.UserNotFound);

			return await userManager.GetRolesAsync(user);

		}
		public async Task<AccountViewModel?> GetUserAccountInfo(string id)
		{
			var user = await userRepo.GetAllAttached()
				.Include(x => x.Badges)
				.ThenInclude(x => x.Badge)
				.Include(x => x.CipherAnswers)
				.FirstOrDefaultAsync(x => x.Id == id);

			ICollection<BadgeViewModel> badges = new List<BadgeViewModel>();

			int rank = await userRepo.GetAllAttached()
				.CountAsync(u => u.Score > user.Score && !u.IsDeactivated && !u.IsBanned) + 1;

			if (user == null)
				throw new NotFoundException(UserConstants.UserNotFound);

			foreach (var badge in user.Badges)
			{
				string folderPath;
				if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
				{
					folderPath = Path.Combine(AppContext.BaseDirectory, "Images", "Badges", $"Badge_{badge.BadgeId}.png");
				}
				else
				{
					folderPath = Path.GetFullPath(Path.Combine(
						AppContext.BaseDirectory, "..", "..", "..", "..", "Images/Badges", $"Badge_{badge.BadgeId}.png"));
				}

				string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(folderPath))}";
				var badgeViewModel = new BadgeViewModel()
				{
					BadgeImage = base64,
					Title = badge.Badge.Title,
					Description = badge.Badge.Description,
					EarnedBy = badge.Badge.EarnedBy,
				};
				badges.Add(badgeViewModel);
			}
			;

			AccountViewModel result = new AccountViewModel()
			{
				Username = user.UserName,
				Email = user.Email,
				Roles = (await userManager.GetRolesAsync(user)).ToArray(),
				RegisteredAt = user.RegisteredAt,
				SolvedCount = user.SolvedCount,
				Score = user.Score,
				AttemptedCiphers = user.AttemptedCiphersCount,
				LeaderBoardPlace = rank,
				SuccessRate = user.SuccessRate,
				Badges = badges,
			};
			return result;
		}
	}
}
