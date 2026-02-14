using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cryptomind.Common.ViewModels.UserViewModels;

namespace Cryptomind.Core.Services
{
	public class UserService(
		IRepository<ApplicationUser, string> userRepo,
		UserManager<ApplicationUser> userManager) : IUserService
	{
		public async Task<IEnumerable<string>> GetRolesUsers(string id)
			=> await userManager.GetRolesAsync(await userManager.FindByIdAsync(id));
		public async Task RemoveUserFromRole(string userId, string role)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (!await userManager.IsInRoleAsync(user, role))
			{
				return;
			}

			await userManager.RemoveFromRoleAsync(user, role);
		} //Not used anywhere
		public async Task AddUserToRole(string userId, string role)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (await userManager.IsInRoleAsync(user, role))
			{
				return;
			}
			await userManager.AddToRoleAsync(user, role);
		} //Not used anywhere.
		public async Task<AccountViewModel?> GetUserAccountInfo(string id)
		{
			var user = userRepo.GetAllAttached()
				.Include(x => x.Badges)
				.ThenInclude(x => x.Badge)
				.FirstOrDefault(x => x.Id == id);

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
				Points = user.Score,
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
