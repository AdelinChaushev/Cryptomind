using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class AdminAnswerService(
		IRepository<Cipher, int> cipherRepo,
		IRepository<AnswerSuggestion, int> answerRepo,
		IRepository<UserSolution, int> solutionRepo,
		INotificationService notificationService,
		UserManager<ApplicationUser> userManager) : IAdminAnswerService
	{
		public async Task<List<AnswerSuggestionViewModel>> AllSubmittedAnswersAsync()
		{
			var answerSuggestions = (await answerRepo.GetAllAsync())
				.Where(x => x.Status == ApprovalStatus.Pending)
				.OrderBy(x => x.UplodaedTime);

			if (answerSuggestions == null)
				throw new InvalidOperationException("There are no suggested answers that aren't reviewed");

			var models = new List<AnswerSuggestionViewModel>();

			foreach (var answer in answerSuggestions)
			{
				var userName = (await userManager.FindByIdAsync(answer.UserId)).UserName;

				if (userName == null)
					throw new InvalidOperationException("User not found");
				var model = new AnswerSuggestionViewModel
				{
					Id = answer.Id,
					Description = answer.Description,
					CipherId = answer.CipherId,
					Username = userName,
				};

				models.Add(model);
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
		public async Task<string> ApproveAnswerAsync(int id, int points)
		{
			var answer = await answerRepo.FirstOrDefaultAsync(x => x.Id == id);

			if (answer == null)
				throw new InvalidOperationException("Answer not found");

			var cipher = await cipherRepo.FirstOrDefaultAsync(x => x.Id == answer.CipherId);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			if (cipher.ChallengeType == ChallengeType.Standard)
				throw new InvalidOperationException("Answer suggestions can only be upplied to standard ciphers");

			//The above statement already checks it
			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText)) 
				throw new InvalidOperationException("Cipher already has an approved answer");

			var user = await userManager.FindByIdAsync(answer.UserId);

			if (user == null)
				throw new InvalidOperationException("User not found");

			var userSolution = new UserSolution
			{
				CipherId = answer.CipherId,
				UserId = answer.UserId,
				PointsEarned = points,
				TimeSolved = DateTime.UtcNow,
				IsCorrect = true,
			};

			cipher.DecryptedText = answer.DecryptedText;
			cipher.ChallengeType = ChallengeType.Standard;

			answer.Status = ApprovalStatus.Approved;
			answer.ApprovalDate = DateTime.UtcNow;
			answer.PointsEarned = points;

			user.Score += points;

			solutionRepo.Add(userSolution);
			cipherRepo.Update(cipher);
			answerRepo.Update(answer);
			await userManager.UpdateAsync(user);

			await notificationService.CreateAndSendNotification(user.Id, NotificationType.AnswerApproved, 
				$"Your answer suggestion was approved +{points} points", 
				cipher.Id, string.Empty);
			return answer.UserId;
		}
		public async Task RejectAnswerAsync(int id, string reason)
		{
			AnswerSuggestion? answer = await answerRepo.GetByIdAsync(id);
			if (answer == null)
				throw new InvalidOperationException("Answer not found");

			else if (answer.Status == ApprovalStatus.Approved) throw new InvalidOperationException("Answer already approved");

			answer.Status = ApprovalStatus.Rejected;
			answer.RejectionDate = DateTime.UtcNow;
			answer.RejectionReason = reason;

			await answerRepo.UpdateAsync(answer);
			await notificationService.CreateAndSendNotification(answer.UserId, NotificationType.AnswerRejected, reason, null, string.Empty);
		}
	}
}
