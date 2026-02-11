using Cryptomind.Common.DTOs;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Cryptomind.Common.ViewModels.AnswerSubmissionViewModels;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class AnswerSubmissionService (
		IRepository<Cipher, int> cipherRepo,
		IRepository<UserSolution, int> solutionRepo,
		IRepository<AnswerSuggestion, int> answerRepo) : IAnswerSubmissionService
	{
		public async Task SuggestAnswerAsync(SuggestAnswerDTO dto, string userId, int cipherId)
		{
			Cipher? cipher = cipherRepo.GetAllAttached()
				.Include(x => x.AnswerSuggestions)
				.FirstOrDefault(x => x.Id == cipherId);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText))
				throw new InvalidOperationException("Cipher already has an answer");

			if (cipher.ChallengeType == ChallengeType.Standard)
				throw new InvalidOperationException("Cannot suggest answer on standard cipher");

			if (cipher.CreatedByUserId == userId)
				throw new InvalidOperationException("You cannot suggest answers on ciphers created by you.");

			if (cipher.AnswerSuggestions.FirstOrDefault(x => x.UserId == userId) != null)
				throw new InvalidOperationException("You have already suggested an answer for this cipher");

			AnswerSuggestion answer = new AnswerSuggestion
			{
				UserId = userId,
				CipherId = cipherId,
				Description = dto.Description,
				DecryptedText = dto.DecryptedText,
				UplodaedTime = DateTime.UtcNow,
				Status = ApprovalStatus.Pending,
			};

			await answerRepo.AddAsync(answer);
		}
		public async Task<List<AnswerSubmissionViewModel>> GetSubmittedAnswers()
		{
			throw new NotImplementedException();
		}
	}
}
