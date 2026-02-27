using Cryptomind.Common.Constants;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Cryptomind.Core.Services
{
	public class AdminUserService(UserManager<ApplicationUser> userManager) : IAdminUserService
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

			if (filter.Username != null)
			{
				users = users
				.Where(x => x.UserName.ToLower().Contains(filter.Username.ToLower()))
		.ToList();
			}


			var adminIds = (await userManager.GetUsersInRoleAsync(UserConstants.AdminRole))
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
					Role = roles.Contains(UserConstants.AdminRole) ? UserConstants.AdminRole : roles.FirstOrDefault() ?? UserConstants.UserRole,
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
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			List<UserCipherViewModel> submittedCiphers = new List<UserCipherViewModel>();
			List<UserCipherViewModel> solvedCiphers = new List<UserCipherViewModel>();

			foreach (var cipher in user.UploadedCiphers)
			{
				var type = cipher.TypeOfCipher?.ToString();

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
				var type = cipher.TypeOfCipher?.ToString();

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

			bool isAdmin = await userManager.IsInRoleAsync(user, UserConstants.AdminRole);
			var roles = await userManager.GetRolesAsync(user);

			return new UserDetailViewModel
			{
				Id = user.Id,
				Email = user.Email,
				IsAdmin = isAdmin,
				Username = user.UserName,
				Role = roles.Contains(UserConstants.AdminRole) ? UserConstants.AdminRole : roles.FirstOrDefault() ?? UserConstants.UserRole,
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
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			if (!await userManager.IsInRoleAsync(user, UserConstants.AdminRole))
				await userManager.AddToRoleAsync(user, UserConstants.AdminRole);
			else
				throw new ConflictException(UserConstants.UserAlreadyAdmin);
		}
		public async Task BanUserAsync(string userId, string reason)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			if (user.IsDeactivated)
				throw new ConflictException(UserConstants.UserAlreadyDeactivated);

			if (user.IsBanned)
				throw new ConflictException(UserConstants.UserAlreadyBanned);

			if (await userManager.IsInRoleAsync(user, UserConstants.AdminRole))
				throw new ConflictException(UserConstants.AdminCannotBeBanned);

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
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			if (!user.IsBanned)
				throw new ConflictException(UserConstants.UserNotBanned);

			await userManager.SetLockoutEndDateAsync(user, DateTime.Now);

			user.IsBanned = false;
			user.BanReason = null;
			user.BannedAt = null;
			await userManager.UpdateAsync(user);
		}
	}
}