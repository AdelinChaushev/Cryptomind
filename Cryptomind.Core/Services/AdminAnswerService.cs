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
		public async Task<List<AnswerSuggestionViewModel>> AllSubmittedAnswersAsync()
		{
			var answerSuggestions = (await answerRepo.GetAllAsync())
				.Where(x => x.Status == ApprovalStatus.Pending)
				.OrderBy(x => x.UplodaedTime)
				.ToList();

			var userIds = answerSuggestions.Select(x => x.UserId).Distinct().ToList();
			var users = (await userManager.Users
				.Where(x => userIds.Contains(x.Id))
				.ToListAsync())
				.ToDictionary(x => x.Id);

			var models = new List<AnswerSuggestionViewModel>();

			foreach (var answer in answerSuggestions)
			{
				if (!users.TryGetValue(answer.UserId, out var user))
					throw new InvalidOperationException("User not found");

				models.Add(new AnswerSuggestionViewModel
				{
					Id = answer.Id,
					Description = answer.Description,
					CipherId = answer.CipherId,
					Username = user.UserName,
				});
			}

			return models;
		}
		public async Task<AnswerSuggestionReviewViewModel> GetAnswerById(int id)
		{
			var answer = await answerRepo.GetByIdAsync(id);
			if (answer == null)
				throw new InvalidOperationException("Answer not found");

			var user = await userManager.FindByIdAsync(answer.UserId);

			if (user == null)
				throw new InvalidOperationException("User not found");
			
			var userName = user.UserName;

			var model = new AnswerSuggestionReviewViewModel
			{
				CipherId = answer.CipherId,
				Description = answer.Description,
				DecryptedText = answer.DecryptedText,
				Username = userName,
			};

			return model;
		}
		public async Task<List<string>> ApproveAnswerAsync(int id, int points)
		{
			var selectedAnswer = await answerRepo.FirstOrDefaultAsync(x => x.Id == id);

			if (selectedAnswer == null)
				throw new InvalidOperationException("Answer not found");

			var firstCorrectAnswerSuggestion = (await answerRepo.GetAllAsync())
				.Where(x => x.CipherId == selectedAnswer.CipherId)
				.Where(x => x.DecryptedText.Trim().ToLower() == selectedAnswer.DecryptedText.Trim().ToLower())
				.OrderBy(x => x.UplodaedTime)
				.First();

			var cipher = await cipherRepo.FirstOrDefaultAsync(x => x.Id == firstCorrectAnswerSuggestion.CipherId);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			if (cipher.ChallengeType == ChallengeType.Standard)
				throw new InvalidOperationException("Answer suggestions can only be applied to experimental ciphers");

			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText)) 
				throw new InvalidOperationException("Cipher already has an approved answer");

			var user = await userManager.FindByIdAsync(firstCorrectAnswerSuggestion.UserId);

			if (user == null)
				throw new InvalidOperationException("User not found");

			var otherCorrectAnswerSuggestions = (await answerRepo.GetAllAsync())
				.Where(x => x.CipherId == selectedAnswer.CipherId)
				.Where(x => x.DecryptedText.Trim().ToLower() == selectedAnswer.DecryptedText.Trim().ToLower() && x.Id != firstCorrectAnswerSuggestion.Id)
				.Where(x => x.Status == ApprovalStatus.Pending);

			var wrongAnswerSuggestions = (await answerRepo.GetAllAsync())
				.Where(x => x.CipherId == selectedAnswer.CipherId)
				.Where(x => x.DecryptedText.Trim().ToLower() != selectedAnswer.DecryptedText.Trim().ToLower())
				.Where(x => x.Status == ApprovalStatus.Pending);


			int pointsGranted = points + cipher.Points;

			var userSolution = new UserSolution
			{
				CipherId = firstCorrectAnswerSuggestion.CipherId,
				UserId = firstCorrectAnswerSuggestion.UserId,
				PointsEarned = pointsGranted,
				TimeSolved = DateTime.UtcNow,
				IsCorrect = true,
			};

			cipher.DecryptedText = firstCorrectAnswerSuggestion.DecryptedText;
			cipher.ChallengeType = ChallengeType.Standard;

			firstCorrectAnswerSuggestion.Status = ApprovalStatus.Approved;
			firstCorrectAnswerSuggestion.ApprovalDate = DateTime.UtcNow;
			firstCorrectAnswerSuggestion.PointsEarned = pointsGranted;

			user.Score += pointsGranted;

			var userIds = new List<string>();
			userIds.Add(user.Id);

			foreach (var correctAnswer in otherCorrectAnswerSuggestions)
			{
				var currentUser = await userManager.FindByIdAsync(correctAnswer.UserId);

				if (currentUser == null)
					throw new InvalidOperationException("User not found.");

				var otherUserSolution = new UserSolution
				{
					CipherId = correctAnswer.CipherId,
					UserId = correctAnswer.UserId,
					PointsEarned = cipher.Points,
					TimeSolved = DateTime.UtcNow,
					IsCorrect = true,
				};
				currentUser.Score += cipher.Points;
				userIds.Add(currentUser.Id);

				correctAnswer.Status = ApprovalStatus.Approved;
				correctAnswer.ApprovalDate = DateTime.UtcNow;
				correctAnswer.PointsEarned = cipher.Points;

				await answerRepo.UpdateAsync(correctAnswer);
				await solutionRepo.AddAsync(otherUserSolution);
				await userManager.UpdateAsync(currentUser);

				await notificationService.CreateAndSendNotification(currentUser.Id, NotificationType.AnswerApproved,
					$"Your answer suggestion was approved +{cipher.Points} points",
					cipher.Id, string.Empty);
			}

			foreach(var wrongAnswer in wrongAnswerSuggestions)
			{
				await RejectAnswer("Another answer was approved for this cipher", wrongAnswer);
			}

			await solutionRepo.AddAsync(userSolution);
			await cipherRepo.UpdateAsync(cipher);
			await answerRepo.UpdateAsync(firstCorrectAnswerSuggestion);
			await userManager.UpdateAsync(user);

			await notificationService.CreateAndSendNotification(user.Id, NotificationType.AnswerApproved, 
				$"Your answer was approved +{pointsGranted} points", 
				cipher.Id, string.Empty);

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
				throw new InvalidOperationException("Answer not found");

			else if (answer.Status == ApprovalStatus.Approved) throw new InvalidOperationException("Answer already approved");

			answer.Status = ApprovalStatus.Rejected;
			answer.RejectionDate = DateTime.UtcNow;
			answer.RejectionReason = reason;

			if (userManager.FindByIdAsync(answer.UserId) == null)
				throw new InvalidOperationException("User not found");

			await answerRepo.UpdateAsync(answer);
			await notificationService.CreateAndSendNotification(answer.UserId, NotificationType.AnswerRejected, reason, answer.Id, string.Empty);
		}
		#endregion
	}
}
