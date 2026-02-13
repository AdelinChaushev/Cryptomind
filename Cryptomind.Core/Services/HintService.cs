using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class HintService(
		IRepository<Cipher, int> cipherRepo,
		IRepository<HintRequest, int> hintRequestRepo,
		ILLMService llmSerice
		) : IHintService
	{
		public async Task<string> RequestHintAsync(string userId, int cipherId, HintType hintType)
		{
			var cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.HintsRequested)
				.Include(x => x.UserSolutions)
				.FirstOrDefaultAsync(x => x.Id == cipherId);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			if (cipher.CreatedByUserId == userId)
				throw new InvalidOperationException("You cannot request hints for your own cipher");

			if (cipher.UserSolutions.Any(x => x.UserId == userId && x.IsCorrect))
				throw new InvalidOperationException("You have already solved this cipher");

			bool isAllowed = hintType switch
			{
				HintType.Type=> cipher.AllowTypeHint,
				HintType.Hint=> cipher.AllowHint,
				HintType.FullSolution => cipher.AllowSolution,
				_ => false
			};

			if (!isAllowed)
				throw new InvalidOperationException("This hint type is not available for this cipher");

			var existingHint = cipher.HintsRequested
					.FirstOrDefault(hr => hr.UserId == userId && hr.HintType == hintType);

			if (existingHint != null)
			{
				// User already requested this hint before - return the same content
				return existingHint.HintContent;
			}

			string hintContent = await llmSerice.GetHint(cipher, hintType);

			var hintRequest = new HintRequest
			{
				UserId = userId,
				CipherId = cipherId,
				HintType = hintType,
				RequestedAt = DateTime.UtcNow,
				HintContent = hintContent,
			};

			await hintRequestRepo.AddAsync(hintRequest);
			return hintContent;
		}
	}
}
