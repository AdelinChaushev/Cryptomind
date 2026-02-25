using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.Helpers;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class HintService(
		IRepository<Cipher, int> cipherRepo,
		IRepository<HintRequest, int> hintRequestRepo,
		ILLMService llmService
		) : IHintService
	{
		private const double TypeHintPenalty = 0.20;
		private const double SolutionHintPenalty = 0.30;
		private const double FullSolutionHintPenalty = 0.40;
		public async Task<HintResultDTO> RequestHintAsync(string userId, int cipherId, HintType hintType)
		{
			var cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.HintsRequested)
				.Include(x => x.UserSolutions)
				.Where(x => x.Status == ApprovalStatus.Approved)
				.FirstOrDefaultAsync(x => x.Id == cipherId);

			if (cipher == null)
				throw new NotFoundException("Cipher not found");

			var userHints = cipher.HintsRequested
					.Where(x => x.UserId == userId)
					.OrderBy(x => x.HintType)
					.ToList();

			string hintContent = string.Empty;
			bool usedTypeHint = userHints.Any(h => h.HintType == HintType.Type);
			bool usedSolutionHint = userHints.Any(h => h.HintType == HintType.Hint);
			bool usedFullSolution = userHints.Any(h => h.HintType == HintType.FullSolution);

			var availablePoints = CalculatePointsHelper.CalculateAvailablePointsWithPenalty(
					cipher.Points,
					usedTypeHint,
					usedSolutionHint,
					usedFullSolution);


			if (cipher.CreatedByUserId == userId)
				throw new ConflictException("You cannot request hints for your own cipher");

			if (cipher.UserSolutions.Any(x => x.UserId == userId && x.IsCorrect))
				throw new ConflictException("You have already solved this cipher");

			bool isAllowed = hintType switch
			{
				HintType.Type => cipher.AllowTypeHint,
				HintType.Hint => cipher.AllowHint,
				HintType.FullSolution => cipher.AllowSolution,
				_ => false
			};

			if (!isAllowed)
				throw new ConflictException("This hint type is not available for this cipher");

			var existingHint = cipher.HintsRequested
					.FirstOrDefault(hr => hr.UserId == userId && hr.HintType == hintType);

			if (existingHint != null)
			{
				return new HintResultDTO
				{
					HintContent = existingHint.HintContent,
					AvailablePoints = availablePoints
				};
			}
			string cachedHint = GetCachedHint(cipher, hintType);

			if (!string.IsNullOrEmpty(cachedHint))
				hintContent = cachedHint;
			else
			{
				hintContent = await llmService.GetHint(cipher, hintType);
				SetCachedHint(cipher, hintType, hintContent);
			}

			var hintRequest = new HintRequest
			{
				UserId = userId,
				CipherId = cipherId,
				HintType = hintType,
				RequestedAt = DateTime.UtcNow.AddHours(2),
				HintContent = hintContent,
			};


			availablePoints = CalculateNewPointsWithPenalty(availablePoints, hintType);

			await hintRequestRepo.AddAsync(hintRequest);
			await cipherRepo.UpdateAsync(cipher);
			return new HintResultDTO
			{
				HintContent = hintContent,
				AvailablePoints = availablePoints
			};
		}

		#region Private methods
		private string GetCachedHint(Cipher cipher, HintType hintType) => hintType switch
		{
			HintType.Type => cipher.LLMData.CachedTypeHint,
			HintType.Hint => cipher.LLMData.CachedHint,
			HintType.FullSolution => cipher.LLMData.CachedSolution,
			_ => null
		};
		private int CalculateNewPointsWithPenalty(int basePoints, HintType hintType)
		{
			double penalty = hintType switch
			{
				HintType.Type => TypeHintPenalty,
				HintType.Hint => SolutionHintPenalty,
				HintType.FullSolution => FullSolutionHintPenalty,
				_ => 0
			};
			return (int)Math.Max(0, basePoints * (1.0 - penalty));
		}
		private void SetCachedHint(Cipher cipher, HintType hintType, string content)
		{
			switch (hintType)
			{
				case HintType.Type: cipher.LLMData.CachedTypeHint = content; break;
				case HintType.Hint: cipher.LLMData.CachedHint = content; break;
				case HintType.FullSolution: cipher.LLMData.CachedSolution = content; break;
			}
		}
		#endregion
	}
}
