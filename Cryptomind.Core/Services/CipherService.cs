using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
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
		IRepository<AnswerSuggestion, int> answerRepo,
		UserManager<ApplicationUser> userManager) : ICipherService
	{
		private const double TypeHintPenalty = 0.20;
		private const double SolutionHintPenalty = 0.30;
		private const double FullSolutionHintPenalty = 0.40;

		public async Task<List<CipherOutputViewModel>> GetApprovedAsync(CipherFilter filter, string userId)
		{
			IQueryable<Cipher> query = cipherRepo.GetAllAttached()
				.Include(x => x.UserSolutions)
				.Where(c => c.Status == ApprovalStatus.Approved && !c.IsDeleted);

			if (!string.IsNullOrEmpty(filter.SearchTerm))
				query = query.Where(c => c.Title.Contains(filter.SearchTerm));

			if (filter.Tags != null && filter.Tags.Any())
				query = query.Where(c => c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type)));

			// Apply challenge type filter
			switch (filter.ChallengeType)
			{
				case ChallengeType.Standard:
					query = query.Where(x => x.ChallengeType == ChallengeType.Standard);
					break;
				case ChallengeType.Experimental:
					query = query.Where(x => x.ChallengeType == ChallengeType.Experimental);
					break;
			}

			// Apply cipher definition filter
			switch (filter.CipherDefinition)
			{
				case CipherDefinition.ImageCipher:
					query = query.OfType<ImageCipher>();
					break;
				case CipherDefinition.TextCipher:
					query = query.OfType<TextCipher>();
					break;
			}

			// Apply ordering
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

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			if (cipher.IsDeleted)
				throw new InvalidOperationException("This cipher has been removed");

			if (cipher.Status != ApprovalStatus.Approved)
				throw new InvalidOperationException("Cipher not found");

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
				throw new InvalidOperationException("Cipher not found");

			if (cipher.CreatedByUserId == userId)
				throw new InvalidOperationException("A cipher cannot be solved by it's user");

			if (cipher.ChallengeType == ChallengeType.Experimental)
				throw new InvalidOperationException("Experimental ciphers cannot be solved");

			if (cipher.UserSolutions.FirstOrDefault(x => x.UserId == userId && x.IsCorrect) != null)
				throw new InvalidOperationException("Cannot solve the same cipher 2 times");

			string correctAnswer = cipher.DecryptedText;

			bool isCorrect = correctAnswer.Trim().Equals(input.Trim(), StringComparison.OrdinalIgnoreCase);

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
				TimeSolved = DateTime.UtcNow,
				UsedTypeHint = usedTypeHint,
				UsedSolutionHint = usedSolutionHint,
				UsedFullSolution = usedFullSolution,
				PointsEarned = 0,
				IsCorrect = false,
			};

			if (isCorrect) // The answer is correct
			{
				int pointsEarned = CalculatePointsWithPenalty(
					cipher.Points,
					usedTypeHint,
					usedSolutionHint,
					usedFullSolution);

				ApplicationUser user = await userManager.FindByIdAsync(userId);

				if (user == null)
					throw new InvalidOperationException("User not found");

				userSolution.PointsEarned = pointsEarned;
				userSolution.IsCorrect = true;

				user.Score += pointsEarned;
				user.SolvedCount += 1;

				await userManager.UpdateAsync(user);
			}

			await solutionRepo.AddAsync(userSolution);
			return isCorrect;
		}

		#region Private methods	
		private CipherOutputViewModel ToOutputViewModel(Cipher cipher, string userId)
		{
			bool isSolved = cipher.UserSolutions.Any(x => x.UserId == userId);

			var userHints = cipher.HintsRequested
					.Where(x => x.UserId == userId)
					.OrderBy(x => x.HintType)
					.ToList();

			var model = new CipherOutputViewModel
			{
				Id = cipher.Id,
				Title = cipher.Title,
				AlreadySolved = cipher.UserSolutions.FirstOrDefault(x => x.UserId == userId) != null,
				ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				IsImage = cipher is ImageCipher
			};

			return model;
		}
		private async Task<CipherDetailedOutputViewModel> ToDetailedOutputViewModel(Cipher cipher, string userId)
		{
			bool isSolved = cipher.UserSolutions.Any(x => x.UserId == userId);

			var userHints = cipher.HintsRequested
					.Where(x => x.UserId == userId)
					.OrderBy(x => x.HintType)
					.ToList();

			double successfullSolutionCount = cipher.UserSolutions.Count(x => x.IsCorrect);
			double unsuccessfullSolutionCount = cipher.UserSolutions.Count(x => !x.IsCorrect);
			double successRate = 0;
			successRate = successfullSolutionCount / (unsuccessfullSolutionCount + successfullSolutionCount) * 100;

			var allSolutions = cipher.UserSolutions.Count;

			successRate = allSolutions == 0
			? 0
			: ((double)successfullSolutionCount / allSolutions) * 100;

			List<CipherSolverViewModel> recentSolvers = new List<CipherSolverViewModel>();

			foreach (var userSolution in cipher.UserSolutions.Where(x => x.IsCorrect))
			{
				var solvedAt = userSolution.TimeSolved;
				var userName = userSolution.User.IsDeactivated ? "Anonymous" : userSolution.User.UserName;

				var cipherSolver = new CipherSolverViewModel
				{
					UserName = userName,
					SolvedSince = GetTimeSpan(solvedAt),
				};
				recentSolvers.Add(cipherSolver);
			}

			var model = new CipherDetailedOutputViewModel
			{
				Id = cipher.Id,
				Title = cipher.Title,
				CipherText = cipher.EncryptedText,
				SolvedUsersCount = cipher.UserSolutions.Count,
				AlreadySolved = cipher.UserSolutions.FirstOrDefault(x => x.UserId == userId) != null,
				Points = cipher.Points,
				IsImage = cipher is ImageCipher,
				SuccessRate = successRate,
				AllowsAnswer = cipher.AllowSolution,
				AllowsHint = cipher.AllowHint,
				ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				AllowsTypeHint = cipher.AllowTypeHint,
				AllowsSolutionHint = cipher.AllowHint,
				AllowsFullSolution = cipher.AllowSolution,
				DateSubmitted = cipher.CreatedAt,
				RecentSolvers = recentSolvers,
				TimesSolved = cipher.UserSolutions.Count,
				TypeHintUsed = userHints.Any(x => x.HintType == HintType.Type),
				SolutionHintUsed = userHints.Any(x => x.HintType == HintType.Hint),
				FullSolutionUsed = userHints.Any(x => x.HintType == HintType.FullSolution),
				AllSubmissions = cipher.UserSolutions.Count(),
				SuccessfulSubmissions = (int)successfullSolutionCount,
				Tags = cipher.CipherTags.Select(x => x.Tag).ToList(),
				PreviousHints = userHints.Select(x => new HintData
				{
					Type = x.HintType,
					Content = x.HintContent,
					RequestedAt = x.RequestedAt
				}).OrderBy(x => x.Type).ToList(),
				
			};
			if (cipher is ImageCipher)
			{
				ImageCipher cipherImage = cipher as ImageCipher;
				string imageFolderPath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), cipherImage.ImagePath);
				string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";
				model.ImageBase64 = base64;
			}
			return model;
		}
		private int CalculatePointsWithPenalty(
			int basePoints,
			bool usedTypeHint,
			bool usedSolutionHint,
			bool usedFullSolution)
		{
			double multiplier = 1.0;

			if (usedTypeHint)
				multiplier -= TypeHintPenalty; //-20%

			if (usedSolutionHint)
				multiplier -= SolutionHintPenalty; //-30%

			if (usedFullSolution)
				multiplier -= FullSolutionHintPenalty; //-40%

			return (int)Math.Max(0, basePoints * multiplier);
		}
		private TimeSpan GetTimeSpan(DateTime solvedAt)
		{
			return DateTime.UtcNow - solvedAt;
		}
		#endregion
	}
}