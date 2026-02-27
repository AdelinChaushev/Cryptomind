using Cryptomind.Common.Constants;
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
		public async Task<HintResultDTO> RequestHintAsync(string userId, int cipherId, HintType hintType)
		{
			var cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.HintsRequested)
				.Include(x => x.UserSolutions)
				.Where(x => x.Status == ApprovalStatus.Approved)
				.FirstOrDefaultAsync(x => x.Id == cipherId);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);

			var userHints = cipher.HintsRequested
					.Where(x => x.UserId == userId)
					.OrderBy(x => x.HintType)
					.ToList();

			string hintContent = string.Empty;

			if (cipher.CreatedByUserId == userId)
				throw new ConflictException(HintErrorConstants.CannotRequestHintsForOwnCipher);

			if (cipher.UserSolutions.Any(x => x.UserId == userId && x.IsCorrect))
				throw new ConflictException(HintErrorConstants.CipherAlreadySolved);

			bool isAllowed = hintType switch
			{
				HintType.Type => cipher.AllowTypeHint,
				HintType.Hint => cipher.AllowHint,
				HintType.FullSolution => cipher.AllowSolution,
				_ => false
			};

			if (!isAllowed)
				throw new ConflictException(HintErrorConstants.ThisHintTypeIsNotAllowed);

			bool usedTypeHint = hintType == HintType.Type ? true : userHints.Any(h => h.HintType == HintType.Type);
			bool usedSolutionHint = hintType == HintType.Hint ? true : userHints.Any(h => h.HintType == HintType.Hint);
			bool usedFullSolution = hintType == HintType.FullSolution ? true : userHints.Any(h => h.HintType == HintType.FullSolution);

			var availablePoints = CalculatePointsHelper.CalculateAvailablePointsWithPenalty(
					cipher.Points,
					usedTypeHint,
					usedSolutionHint,
					usedFullSolution);

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
