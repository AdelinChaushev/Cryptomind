using Cryptomind.Common.Exceptions;
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
		public async Task<string> RequestHintAsync(string userId, int cipherId, HintType hintType)
		{
			var cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.HintsRequested)
				.Include(x => x.UserSolutions)
				.Where(x => x.Status == ApprovalStatus.Approved)
				.FirstOrDefaultAsync(x => x.Id == cipherId);

			if (cipher == null)
				throw new NotFoundException("Cipher not found");

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

			if (existingHint != null) // User already requested this hint before - return the same content
				return existingHint.HintContent;

			string cachedHint = GetCachedHint(cipher, hintType);

			if (!string.IsNullOrEmpty(cachedHint))
				return cachedHint;

			string hintContent = await llmService.GetHint(cipher, hintType);

			var hintRequest = new HintRequest
			{
				UserId = userId,
				CipherId = cipherId,
				HintType = hintType,
				RequestedAt = DateTime.UtcNow.AddHours(2),
				HintContent = hintContent,
			};

			SetCachedHint(cipher, hintType, hintContent);

			await hintRequestRepo.AddAsync(hintRequest);
			await cipherRepo.UpdateAsync(cipher);
			return hintContent;
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
