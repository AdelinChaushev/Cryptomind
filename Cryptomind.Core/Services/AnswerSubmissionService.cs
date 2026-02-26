using Cryptomind.Common.DTOs;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Cryptomind.Common.ViewModels.AnswerSubmissionViewModels;
using Microsoft.EntityFrameworkCore;
using Cryptomind.Common.Exceptions;

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
				.Where(x => x.Status == ApprovalStatus.Approved)
				.FirstOrDefaultAsync(x => x.Id == cipherId);

			if (cipher == null)
				throw new NotFoundException("Cipher not found");

			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText))
				throw new ConflictException("Cipher already has an answer");

			if (cipher.ChallengeType == ChallengeType.Standard)
				throw new ConflictException("Cannot suggest answer on standard cipher");

			if (string.IsNullOrWhiteSpace(dto.DecryptedText))
				throw new ConflictException("You cannot suggest empty answer");

			if (cipher.CreatedByUserId == userId)
				throw new ConflictException("You cannot suggest answers on ciphers created by you.");

			if (cipher.AnswerSuggestions.Any(x => x.UserId == userId && 
				x.DecryptedText.Trim().ToLower() == dto.DecryptedText.Trim().ToLower()))
				throw new ConflictException("You cannot suggest the same answer twice.");

			AnswerSuggestion answer = new AnswerSuggestion
			{
				UserId = userId,
				CipherId = cipherId,
				Description = dto.Description,
				DecryptedText = dto.DecryptedText,
				UploadedTime = DateTime.UtcNow.AddHours(2),
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
					CipherId = answer.CipherId,
					CipherTitle = answer.Cipher.Title,
					SuggestedAnswer = answer.DecryptedText,
					Status = answer.Status.ToString(),
					SubmittedAt = answer.UploadedTime.ToString("ddd, dd MMM yyyy h:mm"),
				};

				if (answer.Status == ApprovalStatus.Approved)
				{
					model.PointsEarned = answer.PointsEarned;
					model.ApprovedDate = answer.ApprovalDate?.ToString("ddd, dd MMM yyyy h:mm");
					model.CipherId = answer.CipherId;
				}
				else if (answer.Status == ApprovalStatus.Rejected)
				{
					model.RejectionReason = answer.RejectionReason;
					model.RejectionDate = answer.RejectionDate?.ToString("ddd, dd MMM yyyy h:mm");
				}
				if (answer.Cipher.IsDeleted)
				{
					model.Status = "CipherDeleted";
					model.CipherDeletedAt = answer.Cipher.DeletedAt?.ToString("ddd, dd MMM yyyy h:mm");
				}

				models.Add(model);
			}

			return models;
		}
	}
}
