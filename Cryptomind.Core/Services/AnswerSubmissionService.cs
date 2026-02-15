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
		IRepository<AnswerSuggestion, int> answerRepo) : IAnswerSubmissionService
	{
		public async Task SuggestAnswerAsync(SuggestAnswerDTO dto, string userId, int cipherId)
		{
			Cipher? cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.AnswerSuggestions)
				.FirstOrDefaultAsync(x => x.Id == cipherId);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			if (cipher.Status != ApprovalStatus.Approved)
				throw new InvalidOperationException("Can suggest answers only on approved ciphers");

			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText))
				throw new InvalidOperationException("Cipher already has an answer");

			if (cipher.ChallengeType == ChallengeType.Standard)
				throw new InvalidOperationException("Cannot suggest answer on standard cipher");

			if (string.IsNullOrWhiteSpace(dto.DecryptedText))
				throw new InvalidOperationException("You cannot suggest empty answer");

			if (cipher.CreatedByUserId == userId)
				throw new InvalidOperationException("You cannot suggest answers on ciphers created by you.");

			if (cipher.AnswerSuggestions.Any(x => x.UserId == userId && 
				x.DecryptedText.Trim().ToLower() == dto.DecryptedText.Trim().ToLower()))
				throw new InvalidOperationException("You cannot suggest the same answer twice.");

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
		public async Task<List<AnswerSubmissionViewModel>> SubmittedAnswers(string userId)
		{
			var answers = await answerRepo.GetAllAttached()
				.Include(x => x.Cipher)
				.Where(x => x.UserId == userId)
				.ToListAsync();

			var models = new List<AnswerSubmissionViewModel>();
			foreach (var answer in answers)
			{
				var model = new AnswerSubmissionViewModel()
				{
					CipherTitle = answer.Cipher.Title,
					SuggestedAnswer = answer.DecryptedText,
					Status = answer.Status.ToString(),
					SubmittedAt = answer.UplodaedTime,
				};

				if (answer.Status == ApprovalStatus.Approved)
				{
					model.PointsEarned = answer.PointsEarned;
					model.ApprovedDate = answer.ApprovalDate;
				}
				else if (answer.Status == ApprovalStatus.Rejected)
				{
					model.RejectionReason = answer.RejectionReason;
					model.RejectionDate = answer.RejectionDate;
				}

				models.Add(model);
			}

			return models;
		}
	}
}
