using Cryptomind.Common.Constants;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.AnswerSubmissionViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cryptomind.Core.Services
{
	public class AnswerSubmissionService(
		IRepository<Cipher, int> cipherRepo,
		IRepository<AnswerSuggestion, int> answerRepo) : IAnswerSubmissionService
	{
		private const string DateFormat = "ddd, dd MMM yyyy h:mm";

		public async Task SuggestAnswerAsync(SuggestAnswerDTO dto, string userId, int cipherId)
		{
			Cipher? cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.AnswerSuggestions)
				.Where(x => x.Status == ApprovalStatus.Approved)
				.FirstOrDefaultAsync(x => x.Id == cipherId);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);

			if (cipher.Status != ApprovalStatus.Approved)
				throw new ConflictException(CipherErrorConstants.ApprovedOnlySuggestion);

			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText))
				throw new ConflictException(CipherErrorConstants.AlreadyHasAnswer);

			if (cipher.ChallengeType == ChallengeType.Standard)
				throw new ConflictException(CipherErrorConstants.StandardCipherSuggestionConflict);

			if (string.IsNullOrWhiteSpace(dto.DecryptedText))
				throw new ConflictException(CipherErrorConstants.EmptyAnswerSuggestion);

			if (cipher.CreatedByUserId == userId)
				throw new ConflictException(CipherErrorConstants.OwnCipherSuggestionConflict);

			if (cipher.AnswerSuggestions.Any(x => x.UserId == userId &&
				x.DecryptedText.Trim().ToLower() == dto.DecryptedText.Trim().ToLower()))
				throw new ConflictException(CipherErrorConstants.DuplicateSuggestion);

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
					SubmittedAt = answer.UploadedTime.ToString(DateFormat),
				};

				if (answer.Status == ApprovalStatus.Approved)
				{
					model.PointsEarned = answer.PointsEarned;
					model.ApprovedDate = answer.ApprovalDate?.ToString(DateFormat);
					model.CipherId = answer.CipherId;
				}
				else if (answer.Status == ApprovalStatus.Rejected)
				{
					model.RejectionReason = answer.RejectionReason;
					model.RejectionDate = answer.RejectionDate?.ToString(DateFormat);
				}

				if (answer.Cipher.IsDeleted)
				{
					model.Status = "CipherDeleted";
					model.CipherDeletedAt = answer.Cipher.DeletedAt?.ToString(DateFormat);
				}

				models.Add(model);
			}

			return models;
		}
	}
}