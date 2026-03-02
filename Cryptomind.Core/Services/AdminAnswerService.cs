using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.Helpers;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Cryptomind.Core.Services
{

	public class AdminAnswerService(
		IRepository<Cipher, int> cipherRepo,
		IRepository<AnswerSuggestion, int> answerRepo,
		IRepository<UserSolution, int> solutionRepo,
		INotificationService notificationService,
		UserManager<ApplicationUser> userManager) : IAdminAnswerService
	{

		public async Task<int> GetPendingAnswersCount()
		{
			return (await answerRepo.GetAllAsync())
				.Count(x => x.Status == ApprovalStatus.Pending);
		}
		public async Task<int> GetApprovedAnswersCount()
		{
			return (await answerRepo.GetAllAsync())
				.Count(x => x.Status == ApprovalStatus.Approved);
		}
		public async Task<List<AnswerSuggestionViewModel>> AllSubmittedAnswersAsync(string? cipherName, string? username)
		{
			var answerSuggestions = await answerRepo.GetAllAttached()
				.Include(x => x.ApplicationUser)
				.Include(x => x.Cipher)
				.Where(x => x.Status == ApprovalStatus.Pending)
				.OrderBy(x => x.UploadedTime)
				.ToListAsync();

			if (cipherName != null)
			{
				answerSuggestions = answerSuggestions.Where(x => x.Cipher.Title.ToLower().Contains(cipherName.ToLower())).ToList();

			}
			if (username != null)
			{
				answerSuggestions = answerSuggestions.Where(x => x.ApplicationUser.UserName.ToLower().Contains(username.ToLower())).ToList();
			}
			if (answerSuggestions.Count == 0)
			{
				return new List<AnswerSuggestionViewModel>();
			}

			var userIds = answerSuggestions.Select(x => x.UserId).Distinct().ToList();
			var users = (await userManager.Users
				.Where(x => userIds.Contains(x.Id))
				.ToListAsync())
				.ToDictionary(x => x.Id);

			var models = new List<AnswerSuggestionViewModel>();

			foreach (var answer in answerSuggestions)
			{
				if (!users.TryGetValue(answer.UserId, out var user))
					throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(answer.UserId, answer.Id));

				models.Add(new AnswerSuggestionViewModel
				{
					Id = answer.Id,
					Description = answer.Description,
					CipherId = answer.CipherId,
					CipherName = answer.Cipher.Title,
					Username = user.UserName,
					SubmittedAt = answer.UploadedTime.ToString("ddd, dd MMM yyyy HH:mm")
				});
			}

			return models;
		}
		public async Task<AnswerSuggestionReviewViewModel> GetAnswerById(int id)
		{
			var answer = await answerRepo.GetAllAttached()
				.Include(x => x.Cipher)
				.FirstOrDefaultAsync(x => x.Id == id);
			if (answer == null)
				throw new NotFoundException(AnswerErrorConstants.AnswerNotFound);

			if (answer.Status != ApprovalStatus.Pending)
				throw new ConflictException(AnswerErrorConstants.AnswerResolved);

			var user = await userManager.FindByIdAsync(answer.UserId);

			if (user == null)
				throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(answer.UserId, answer.Id));

			var userName = user.UserName;

			var model = new AnswerSuggestionReviewViewModel
			{
				CipherId = answer.CipherId,
				Description = answer.Description,
				DecryptedText = answer.DecryptedText,
				Username = userName,
				CipherEncryptedText = answer.Cipher.EncryptedText,
				CipherName = answer.Cipher.Title,

			};
			if (answer.Cipher.TypeOfCipher is { } typeOfCipher)
			{
				model.Type = CipherTypeMapperHelper.ToDisplayName(typeOfCipher);
			}
			return model;
		}
		public async Task<List<string>> ApproveAnswerAsync(int id)
		{
			var selectedAnswer = await answerRepo.FirstOrDefaultAsync(x => x.Id == id);

			if (selectedAnswer == null)
				throw new NotFoundException(AnswerErrorConstants.AnswerNotFound);

			if (selectedAnswer.Status != ApprovalStatus.Pending)
				throw new ConflictException(AnswerErrorConstants.AnswerResolved);

			var firstCorrectAnswerSuggestion = (await answerRepo.GetAllAsync())
				.Where(x => x.CipherId == selectedAnswer.CipherId)
				.Where(x => x.DecryptedText.Trim().ToLower() == selectedAnswer.DecryptedText.Trim().ToLower())
				.OrderBy(x => x.UploadedTime)
				.First();

			var cipher = await cipherRepo.FirstOrDefaultAsync(x => x.Id == firstCorrectAnswerSuggestion.CipherId);

			if (cipher == null)
				throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(selectedAnswer.CipherId, selectedAnswer.Id));

			if (cipher.ChallengeType == ChallengeType.Standard)
				throw new ConflictException(CipherErrorConstants.ExperimentalSolveConflict);

			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText))
				throw new ConflictException(CipherErrorConstants.AlreadyHasAnswer);

			var user = await userManager.FindByIdAsync(firstCorrectAnswerSuggestion.UserId);

			if (user == null)
				throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(cipher.CreatedByUserId, selectedAnswer.Id));

			var otherCorrectAnswerSuggestions = (await answerRepo.GetAllAsync())
				.Where(x => x.CipherId == selectedAnswer.CipherId)
				.Where(x => x.DecryptedText.Trim().ToLower() == selectedAnswer.DecryptedText.Trim().ToLower() && x.Id != firstCorrectAnswerSuggestion.Id)
				.Where(x => x.Status == ApprovalStatus.Pending);

			var wrongAnswerSuggestions = (await answerRepo.GetAllAsync())
				.Where(x => x.CipherId == selectedAnswer.CipherId)
				.Where(x => x.DecryptedText.Trim().ToLower() != selectedAnswer.DecryptedText.Trim().ToLower())
				.Where(x => x.Status == ApprovalStatus.Pending);


			int pointsGranted = cipher.Points * 2;

			var userSolution = new UserSolution
			{
				CipherId = firstCorrectAnswerSuggestion.CipherId,
				UserId = firstCorrectAnswerSuggestion.UserId,
				PointsEarned = pointsGranted,
				TimeSolved = firstCorrectAnswerSuggestion.UploadedTime,
				IsCorrect = true,
				Solution = firstCorrectAnswerSuggestion.DecryptedText,
				IsRareSolved = true
			};

			cipher.DecryptedText = firstCorrectAnswerSuggestion.DecryptedText;
			cipher.ChallengeType = ChallengeType.Standard;

			firstCorrectAnswerSuggestion.Status = ApprovalStatus.Approved;
			firstCorrectAnswerSuggestion.ApprovalDate = DateTime.UtcNow.AddHours(2);
			firstCorrectAnswerSuggestion.PointsEarned = pointsGranted;

			user.Score += pointsGranted;

			var userIds = new List<string>();
			userIds.Add(user.Id);

			foreach (var correctAnswer in otherCorrectAnswerSuggestions)
			{
				var currentUser = await userManager.FindByIdAsync(correctAnswer.UserId);

				if (currentUser == null)
					throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(cipher.CreatedByUserId, selectedAnswer.Id));

				int pointsGrantedForOtherCorrectSolutions = (int)Math.Round(pointsGranted * 0.75, MidpointRounding.AwayFromZero);

				var otherUserSolution = new UserSolution
				{
					CipherId = correctAnswer.CipherId,
					UserId = correctAnswer.UserId,
					PointsEarned = pointsGrantedForOtherCorrectSolutions,
					TimeSolved = correctAnswer.UploadedTime,
					IsCorrect = true,
					Solution = correctAnswer.DecryptedText,
					IsRareSolved = true
				};
				currentUser.Score += pointsGrantedForOtherCorrectSolutions;
				userIds.Add(currentUser.Id);

				correctAnswer.Status = ApprovalStatus.Approved;
				correctAnswer.ApprovalDate = DateTime.UtcNow.AddHours(2);
				correctAnswer.PointsEarned = pointsGrantedForOtherCorrectSolutions;

				await answerRepo.UpdateAsync(correctAnswer);
				await solutionRepo.AddAsync(otherUserSolution);
				await userManager.UpdateAsync(currentUser);

				await notificationService.CreateAndSendNotification(
					currentUser.Id,
					NotificationType.AnswerApproved,
					$"Предложението ви за отговор беше одобрено +{pointsGrantedForOtherCorrectSolutions} точки",
					$"cipher/{cipher.Id}");
			}

			foreach (var wrongAnswer in wrongAnswerSuggestions)
			{
				bool userAlsoHadCorrectAnswer =
					firstCorrectAnswerSuggestion.UserId == wrongAnswer.UserId ||
					otherCorrectAnswerSuggestions.Any(x => x.UserId == wrongAnswer.UserId);

				if (userAlsoHadCorrectAnswer)
					await RejectAnswer("Другият ви отговор беше одобрен", wrongAnswer);
				else
					await RejectAnswer("За този шифър беше одобрен друг отговор.", wrongAnswer);
			}

			await solutionRepo.AddAsync(userSolution);
			await cipherRepo.UpdateAsync(cipher);
			await answerRepo.UpdateAsync(firstCorrectAnswerSuggestion);
			await userManager.UpdateAsync(user);

			await notificationService.CreateAndSendNotification(
				user.Id,
				NotificationType.AnswerApproved,
				$"Вашият отговор беше одобрен +{pointsGranted} точки",
				$"cipher/{cipher.Id}");

			return userIds;
		}
		public async Task RejectAnswerAsync(int id, string reason)
		{
			AnswerSuggestion? answer = await answerRepo.GetByIdAsync(id);

			await RejectAnswer(reason, answer);
		}

		#region Private methods
		private async Task RejectAnswer(string reason, AnswerSuggestion? answer)
		{
			if (answer == null)
				throw new NotFoundException(AnswerErrorConstants.AnswerNotFound);

			else if (answer.Status != ApprovalStatus.Pending) throw new ConflictException(AnswerErrorConstants.AnswerResolved);

			answer.Status = ApprovalStatus.Rejected;
			answer.RejectionDate = DateTime.UtcNow.AddHours(2);
			answer.RejectionReason = reason;

			if (await userManager.FindByIdAsync(answer.UserId) == null)
				throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(answer.UserId, answer.Id));

			var userSolution = new UserSolution
			{
				CipherId = answer.CipherId,
				UserId = answer.UserId,
				PointsEarned = 0,
				TimeSolved = DateTime.UtcNow.AddHours(2),
				IsCorrect = false,
				Solution = answer.DecryptedText
			};

			await solutionRepo.AddAsync(userSolution);
			await answerRepo.UpdateAsync(answer);
			await notificationService.CreateAndSendNotification(
				answer.UserId,
				NotificationType.AnswerRejected,
				$"Вашият отговор беше отхвърлен: {reason}",
				"my_submissions");
		}

		#endregion
	}
}
