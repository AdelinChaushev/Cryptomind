using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace Cryptomind.Core.Services
{
	public class AdminUserService (UserManager<ApplicationUser> userManager) : IAdminUserService
	{
		public async Task<List<UserViewModel>> GetAllUsers(UserFilter filter)
		{
			var userViewModels = new List<UserViewModel>();

			var users = await userManager
				.Users
				.Include(x => x.UploadedCiphers)
				.ToListAsync();

			if (!users.Any())
				return userViewModels;
			if(filter.Username != null)
			{
				users = users
				.Where(x => x.UserName.Contains(filter.Username))
                .ToList();
            }


            var adminIds = (await userManager.GetUsersInRoleAsync("Admin"))
				.Select(x => x.Id)
				.ToHashSet();

			switch (filter.IsBanned)
			{
				case true:
					users = users.Where(x => x.IsBanned).ToList();
					break;
				case false:
					users = users.Where(x => !x.IsBanned).ToList();
					break;
			}
			switch (filter.IsDeactivated)
			{
				case true:
					users = users.Where(x => x.IsDeactivated).ToList();
					break;
				case false:
					users = users.Where(x => !x.IsDeactivated).ToList();
					break;
			}

			foreach (var user in users)
			{
				var roles = await userManager.GetRolesAsync(user);

				userViewModels.Add(new UserViewModel
				{
					Id = user.Id,
					Username = user.UserName,
					Email = user.Email,
					IsAdmin = adminIds.Contains(user.Id),
					PendingCiphers = user.UploadedCiphers.Count(c => c.Status == ApprovalStatus.Pending),
					Role = roles.Contains("Admin") ? "Admin" : roles.FirstOrDefault() ?? "User",
				});
			}

			return userViewModels;
		}
		public async Task<UserDetailViewModel> GetUser(string userId)
		{
			ApplicationUser? user = await userManager.Users
					.Include(x => x.UploadedCiphers)
					.Include(x => x.CipherAnswers)
					.ThenInclude(x => x.Cipher)
					.Include(x => x.HintsRequested)
					.FirstOrDefaultAsync(x => x.Id == userId);

			if (user == null)
				throw new ArgumentException("User not found.");

			List<UserCipherViewModel> submittedCiphers = new List<UserCipherViewModel>();
			List<UserCipherViewModel> solvedCiphers = new List<UserCipherViewModel>();

			foreach (var cipher in user.UploadedCiphers)
			{
				var type = cipher.TypeOfCipher == null ? null : cipher.TypeOfCipher.ToString();

				var viewModel = new UserCipherViewModel
				{
					Id = cipher.Id,
					Title = cipher.Title,
					TypeOfCipher = type,
					Status = cipher.Status.ToString(),
					Points = cipher.Points,
					CreatedAt = cipher.CreatedAt,
					ChallengeType = cipher.ChallengeType,
				};

				submittedCiphers.Add(viewModel);
			}
			foreach (var userSolution in user.CipherAnswers)
			{
				var cipher = userSolution.Cipher;
				var type = cipher.TypeOfCipher == null ? null : cipher.TypeOfCipher.ToString();

				var viewModel = new UserCipherViewModel
				{
					Id = cipher.Id,
					Title = cipher.Title,
					TypeOfCipher = type,
					Status = cipher.Status.ToString(),
					Points = cipher.Points,
					CreatedAt = cipher.CreatedAt,
					ChallengeType = cipher.ChallengeType,
				};

				solvedCiphers.Add(viewModel);
			}

			bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
			return new UserDetailViewModel
			{
				Id = user.Id,
				Email = user.Email,
				IsAdmin = isAdmin,
				Username = user.UserName,
				IsEmailConfirmed = user.EmailConfirmed,
				IsBanned = user.IsBanned,
				BanReason = user.BanReason,
				BannedAt = user.BannedAt,
				RegisteredAt = user.RegisteredAt,
				TotalScore = user.Score,
				CiphersSubmitted = user.UploadedCiphers.Count,
				CiphersSolved = user.SolvedCount,
				HintsRequested = user.HintsRequested.Count(),
				SolveSuccessRate = user.SuccessRate,
				ApprovedCiphers = user.UploadedCiphers.Count(x => x.Status == ApprovalStatus.Approved),
				PendingCiphers = user.UploadedCiphers.Count(x => x.Status == ApprovalStatus.Pending),
				SubmittedCiphers = submittedCiphers,
				SolvedCiphers = solvedCiphers,
			};
		}
		public async Task MakeAdmin(string userId)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			if (!await userManager.IsInRoleAsync(user, "Admin"))
				await userManager.AddToRoleAsync(user, "Admin");
			else
				throw new ArgumentException("The user is already an admin");
		}
		public async Task BanUserAsync(string userId, string reason)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			if (user.IsBanned)
				throw new InvalidOperationException("This user is already banned");

			if (await userManager.IsInRoleAsync(user, "Admin"))
				throw new InvalidOperationException("Admins cannot be banned.");

			await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
			await userManager.SetLockoutEnabledAsync(user, true);

			user.IsBanned = true;
			user.BanReason = reason;
			user.BannedAt = DateTime.Now;
			await userManager.UpdateAsync(user);
		}
		public async Task UnbanUserAsync(string userId)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			if (!user.IsBanned)
				throw new InvalidOperationException("This is user is not banned");

			await userManager.SetLockoutEndDateAsync(user, DateTime.Now);

			user.IsBanned = false;
			user.BanReason = null;
			user.BannedAt = null;
			await userManager.UpdateAsync(user);
		}
	}
}
