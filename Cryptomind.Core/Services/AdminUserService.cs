using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class AdminUserService (UserManager<ApplicationUser> userManager) : IAdminUserService
	{
		public async Task<List<UserViewModel>> GetAllUsers()
		{
			var userViewModels = new List<UserViewModel>();

			foreach (var user in userManager.Users.ToList())
			{
				userViewModels.Add(new UserViewModel
				{
					Id = user.Id,
					Username = user.UserName,
					Email = user.Email,
					IsAdmin = await userManager.IsInRoleAsync(user, "Admin"),
					PendingCiphers = user.UploadedCiphers.Count(c => c.Status == ApprovalStatus.Pending)
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
				throw new ArgumentException("There is no user with this ID");

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
				IsBanned = user.isBanned,
				BanReason = user.BanReason,
				BannedAt = user.BannedAt,
				RegisteredAt = user.RegisteredAt,
				TotalScore = user.Score,
				CiphersSubmitted = user.UploadedCiphers.Count(),
				CiphersSolved = user.SolvedCount,
				HintsRequested = user.HintsRequested.Count(),
				SolveSuccessRate = user.SuccessRate,
				ApprovedCiphers = user.UploadedCiphers.Where(x => x.Status == ApprovalStatus.Approved).Count(),
				PendingCiphers = user.UploadedCiphers.Where(x => x.Status == ApprovalStatus.Pending).Count(),
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
			{
				await userManager.AddToRoleAsync(user, "Admin");
			}
			else
			{
				throw new ArgumentException("The user is already an admin");
			}
		}
		public async Task BanUserAsync(string userId, string reason)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			if (user.isBanned)
				throw new InvalidOperationException("This user is already banned");

			await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
			await userManager.SetLockoutEnabledAsync(user, true);

			user.isBanned = true;
			user.BanReason = reason;
			user.BannedAt = DateTime.Now;
			await userManager.UpdateAsync(user);
		}
		public async Task UnbanUserAsync(string userId)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			if (!user.isBanned)
				throw new InvalidOperationException("This is user is not banned");

			await userManager.SetLockoutEndDateAsync(user, DateTime.Now);

			user.isBanned = false;
			user.BanReason = null;
			user.BannedAt = null;
			await userManager.UpdateAsync(user);
		}
	}
}
