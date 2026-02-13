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
		public async Task<List<CipherOutputViewModel>> GetApprovedAsync(CipherFilter? filter, string userId)
		{
			List<Cipher> approved = cipherRepo.GetAllAttached()
				.Include(x => x.UserSolutions)
				.Where(c => c.Status == ApprovalStatus.Approved)
				.ToList();

			if (!string.IsNullOrEmpty(filter.SearchTerm))
				approved = approved.Where(c => c.Title.Contains(filter.SearchTerm)).ToList();

			if (filter.Tags != null)
				approved = approved.Where(c => c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type))).ToList();

			//Standard, experimental
			switch (filter.ChallengeType)
			{
				case ChallengeType.Standard:
					approved = approved.Where(x => x.ChallengeType == ChallengeType.Standard).ToList();
					break;
				case ChallengeType.Experimental:
					approved = approved.Where(x => x.ChallengeType == ChallengeType.Experimental).ToList();
					break;
			}

			//Text or image cipher
			switch (filter.CipherDefinition)
			{
				case CipherDefinition.ImageCipher:
					approved = approved.Where(x => x is ImageCipher).ToList();
					break;

				case CipherDefinition.TextCipher:
					approved = approved.Where(x => x is TextCipher).ToList();
					break;
			}

			//Sorting
			switch (filter.OrderTerm)
			{
				case CipherOrderTerm.Newest:
					approved = approved.OrderByDescending(x => x.CreatedAt).ToList();
					break;
				case CipherOrderTerm.Oldest:
					approved = approved.OrderBy(x => x.CreatedAt).ToList();
					break;
				case CipherOrderTerm.MostPopular:
					approved = approved.OrderByDescending(x => x.UserSolutions).ToList();
					break;
			}

			List<CipherOutputViewModel> result = new List<CipherOutputViewModel>();
			foreach (var cipher in approved)
			{
				result.Add(await ToOutputViewModel(cipher, userId));
			}
			return result;
		}
		public async Task<CipherDetailedOutputViewModel?> GetCipherAsync(int id, string userId)
		{
			Cipher? cipher = cipherRepo.GetAllAttached()
				.Include(x => x.UserSolutions)
				.ThenInclude(x => x.User)
				.FirstOrDefault(x => x.Id == id);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			return await ToDetailedOutputViewModel(cipher, userId);
		}
		public async Task<bool> SolveCipherAsync(string userId, string input, int cipherId)
		{
			Cipher? cipher = cipherRepo.GetAllAttached()
				.Include(x => x.UserSolutions)
				.Include(x => x.HintsRequested)
				.FirstOrDefault(x => x.Id == cipherId);

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
			}

			await solutionRepo.AddAsync(userSolution);
			return isCorrect;
		}

		#region Private methods	
		private async Task<CipherOutputViewModel> ToOutputViewModel(Cipher cipher, string userId)
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

			var successfullSolutionCount = cipher.UserSolutions.Count(x => x.IsCorrect);
			var allSolutions = cipher.UserSolutions.Count;

			var successRate = allSolutions == 0 
				? 0
				: ((double)successfullSolutionCount / allSolutions) * 100;

			List<CipherSolverViewModel> recentSolvers = new List<CipherSolverViewModel>();

			foreach (var userSolution in cipher.UserSolutions)
			{
				var solvedAt = userSolution.TimeSolved;
				var userName = userSolution.User.UserName;

				var cipherSolver = new CipherSolverViewModel
				{
					UserName = userName,
					SolvedSince = GetTimeSpan(solvedAt)
				};
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
				multiplier -= 0.20; //-50%

			if (usedSolutionHint)
				multiplier -= 0.30; //-50%

			if (usedFullSolution)
				multiplier -= 0.40; //-90%

			return (int)(basePoints * multiplier);
		}
		private TimeSpan GetTimeSpan (DateTime solvedAt)
		{
			return DateTime.UtcNow - solvedAt;
		}
		#endregion
	}
}