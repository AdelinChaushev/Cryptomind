using Cryptomind.Common.Constants;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.Helpers;
using Cryptomind.Common.ViewModels.CipherViewModels;
using Cryptomind.Common.ViewModels.UserViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class CipherService(
		IRepository<Cipher, int> cipherRepo,
		IRepository<UserSolution, int> solutionRepo,
		UserManager<ApplicationUser> userManager) : ICipherService
	{
		private const string DateFormat = "ddd, dd MMM yyyy h:mm";

		public async Task<List<CipherOutputViewModel>> GetApprovedAsync(CipherFilter filter, string userId)
		{
			IQueryable<Cipher> query = cipherRepo.GetAllAttached()
				.Include(x => x.UserSolutions)
				.Where(c => c.Status == ApprovalStatus.Approved && !c.IsDeleted);

			if (!string.IsNullOrEmpty(filter.SearchTerm))
				query = query.Where(c => c.Title.ToLower().Contains(filter.SearchTerm.ToLower()));

			if (filter.Tags != null && filter.Tags.Any())
				query = query.Where(c => c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type)));

			switch (filter.ChallengeType)
			{
				case ChallengeType.Standard:
					query = query.Where(x => x.ChallengeType == ChallengeType.Standard);
					break;
				case ChallengeType.Experimental:
					query = query.Where(x => x.ChallengeType == ChallengeType.Experimental);
					break;
			}

			switch (filter.CipherDefinition)
			{
				case CipherDefinition.ImageCipher:
					query = query.OfType<ImageCipher>();
					break;
				case CipherDefinition.TextCipher:
					query = query.OfType<TextCipher>();
					break;
			}

			switch (filter.OrderTerm)
			{
				case CipherOrderTerm.Newest:
					query = query.OrderByDescending(x => x.CreatedAt);
					break;
				case CipherOrderTerm.Oldest:
					query = query.OrderBy(x => x.CreatedAt);
					break;
				case CipherOrderTerm.MostPopular:
					query = query.OrderByDescending(x => x.UserSolutions.Count);
					break;
				case CipherOrderTerm.LeastPopular:
					query = query.OrderBy(x => x.UserSolutions.Count);
					break;
			}

			List<Cipher> approved = await query.ToListAsync();

			List<CipherOutputViewModel> result = new List<CipherOutputViewModel>();
			foreach (var cipher in approved)
			{
				result.Add(ToOutputViewModel(cipher, userId));
			}
			return result;
		}

		public async Task<CipherDetailedOutputViewModel?> GetCipherAsync(int id, string userId)
		{
			Cipher? cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.UserSolutions)
				.ThenInclude(x => x.User)
				.Include(x => x.HintsRequested)
				.Include(x => x.CipherTags)
					.ThenInclude(x => x.Tag)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (cipher == null || cipher.Status != ApprovalStatus.Approved)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);

			if (cipher.IsDeleted)
				throw new ConflictException(CipherErrorConstants.CipherDeletedConflict);

			return await ToDetailedOutputViewModel(cipher, userId);
		}

		public async Task<bool> SolveCipherAsync(string userId, string input, int cipherId)
		{
			Cipher? cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.UserSolutions)
				.Include(x => x.HintsRequested)
				.Where(x => x.Status == ApprovalStatus.Approved && !x.IsDeleted)
				.FirstOrDefaultAsync(x => x.Id == cipherId);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);

			if (cipher.CreatedByUserId == userId)
				throw new ConflictException(CipherErrorConstants.OwnCipherSolveConflict);

			if (cipher.ChallengeType == ChallengeType.Experimental)
				throw new ConflictException(CipherErrorConstants.ExperimentalSolveConflict);

			if (cipher.UserSolutions.FirstOrDefault(x => x.UserId == userId && x.IsCorrect) != null)
				throw new ConflictException(CipherErrorConstants.AlreadySolvedConflict);

			var userNow = await userManager.FindByIdAsync(userId);
			if (userNow == null)
				throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(userId, cipherId));

			if (!userNow.CipherAnswers.Any(x => x.CipherId == cipherId))
				userNow.AttemptedCiphers += 1;

			string correctAnswer = cipher.DecryptedText;

			static string Normalize(string s) =>
				s.Trim().TrimEnd('#').Replace(" ", "").ToUpper();

			bool isCorrect = Normalize(correctAnswer).Equals(Normalize(input));

			var userHints = cipher.HintsRequested
				.Where(hr => hr.UserId == userId)
				.ToList();

			bool usedTypeHint = userHints.Any(h => h.HintType == HintType.Type);
			bool usedSolutionHint = userHints.Any(h => h.HintType == HintType.Hint);
			bool usedFullSolution = userHints.Any(h => h.HintType == HintType.FullSolution);

			var userSolution = new UserSolution()
			{
				CipherId = cipherId,
				UserId = userId,
				TimeSolved = DateTime.UtcNow.AddHours(2),
				UsedTypeHint = usedTypeHint,
				UsedSolutionHint = usedSolutionHint,
				UsedFullSolution = usedFullSolution,
				PointsEarned = 0,
				IsCorrect = false,
			};

			if (isCorrect)
			{
				int pointsEarned = CalculatePointsHelper.CalculateAvailablePointsWithPenalty(
					cipher.Points,
					usedTypeHint,
					usedSolutionHint,
					usedFullSolution);

				userSolution.PointsEarned = pointsEarned;
				userSolution.IsCorrect = true;

				userNow.Score += pointsEarned;
			}
			await userManager.UpdateAsync(userNow);
			await solutionRepo.AddAsync(userSolution);
			return isCorrect;
		}

		#region Private methods	
		private CipherOutputViewModel ToOutputViewModel(Cipher cipher, string userId)
		{
			var model = new CipherOutputViewModel
			{
				Id = cipher.Id,
				Title = cipher.Title,
				AlreadySolved = cipher.UserSolutions.FirstOrDefault(x => x.UserId == userId && x.IsCorrect) != null,
				ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				IsImage = cipher is ImageCipher
			};

			return model;
		}

		private async Task<CipherDetailedOutputViewModel> ToDetailedOutputViewModel(Cipher cipher, string userId)
		{
			double successfullSolutionCount = cipher.UserSolutions.Count(x => x.IsCorrect);
			var allSolutions = cipher.UserSolutions.Count;

			double successRate = allSolutions == 0
				? 0
				: (successfullSolutionCount / allSolutions) * 100;

			List<CipherSolverViewModel> recentSolvers = new List<CipherSolverViewModel>();

			foreach (var userSolution in cipher.UserSolutions.Where(x => x.IsCorrect))
			{
				var userName = userSolution.User.IsDeactivated
					? CipherErrorConstants.AnonymousUser
					: userSolution.User.UserName;

				recentSolvers.Add(new CipherSolverViewModel
				{
					UserName = userName,
					SolvedSince = GetTimeSpan(userSolution.TimeSolved),
				});
			}

			var userHints = cipher.HintsRequested
					.Where(x => x.UserId == userId)
					.ToList();

			bool usedTypeHint = userHints.Any(h => h.HintType == HintType.Type);
			bool usedSolutionHint = userHints.Any(h => h.HintType == HintType.Hint);
			bool usedFullSolution = userHints.Any(h => h.HintType == HintType.FullSolution);

			var availablePoints = CalculatePointsHelper.CalculateAvailablePointsWithPenalty(
					cipher.Points,
					usedTypeHint,
					usedSolutionHint,
					usedFullSolution);

			var model = new CipherDetailedOutputViewModel
			{
				Id = cipher.Id,
				Title = cipher.Title,
				CipherText = cipher.EncryptedText,
				SolvedUsersCount = allSolutions,
				AlreadySolved = cipher.UserSolutions.FirstOrDefault(x => x.UserId == userId) != null,
				Points = availablePoints,
				IsImage = cipher is ImageCipher,
				SuccessRate = successRate,
				AllowsAnswer = cipher.AllowSolution,
				AllowsHint = cipher.AllowHint,
				ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				AllowsTypeHint = cipher.AllowTypeHint,
				AllowsSolutionHint = cipher.AllowHint,
				AllowsFullSolution = cipher.AllowSolution,
				DateSubmitted = cipher.CreatedAt.ToString(DateFormat),
				RecentSolvers = recentSolvers,
				TimesSolved = allSolutions,
				TypeHintUsed = usedTypeHint,
				SolutionHintUsed = usedSolutionHint,
				FullSolutionUsed = usedFullSolution,
				AllSubmissions = allSolutions,
				SuccessfulSubmissions = (int)successfullSolutionCount,
				Tags = cipher.CipherTags.Select(x => x.Tag.Type.ToString()).ToList(),
				PreviousHints = userHints.Select(x => new HintData
				{
					Type = x.HintType,
					Content = x.HintContent,
					RequestedAt = x.RequestedAt
				}).OrderBy(x => x.Type).ToList(),
			};

			if (cipher is ImageCipher imageCipher)
			{
				string imagePath = Path.Combine(PathHelper.GetImagesBasePath(), imageCipher.ImagePath);
				model.ImageBase64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imagePath))}";
			}
			return model;
		}

		private TimeSpan GetTimeSpan(DateTime solvedAt)
		{
			return DateTime.UtcNow.AddHours(2) - solvedAt;
		}
		#endregion
	}
}