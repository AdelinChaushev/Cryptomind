using Cryptomind.Common.DTOs;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cryptomind.Data.Enums;
using Cryptomind.Common.Enums;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;

namespace Cryptomind.Core.Services
{
	public class AdminCipherService (
		IRepository<Cipher, int> cipherRepo,
		IRepository<AnswerSuggestion, int> answerRepo,
		IRepository<UserSolution, int> solutionRepo,
		IRepository<Tag, int> tagRepo,
		UserManager<ApplicationUser> userManager,
		ILLMService llmService,
		INotificationService notificationService) : IAdminCipherService
	{
		private readonly Dictionary<CipherType, int> PointsForType = new Dictionary<CipherType, int>()
		{
			[CipherType.Base64] = 50,
			[CipherType.Hex] = 50,
			[CipherType.Binary] = 50,
			[CipherType.Morse] = 50,
			[CipherType.ROT13] = 75,
			[CipherType.Caesar] = 100,
			[CipherType.Atbash] = 125,
			[CipherType.SimpleSubstitution] = 250,
			[CipherType.RailFence] = 200,
			[CipherType.Trithemius] = 275,
			[CipherType.Vigenere] = 400,
			[CipherType.Columnar] = 350,
			[CipherType.Route] = 375,
			[CipherType.Autokey] = 500,
		};

		public async Task<List<CipherReviewOutputViewModel>> AllSubmittedCiphers()
		{
			var result = (await cipherRepo.GetAllAsync()).Where(c => c.Status == ApprovalStatus.Pending).OrderBy(x => x.CreatedAt).ToList();

			if (result == null) 
				throw new InvalidOperationException("Wasn't able to retrieve submitted ciphers");

			return await ToReviewOutputViewModelMany(result);
		}
		public async Task<List<CipherReviewOutputViewModel>> AllApprovedCiphers(CipherFilter filter)
		{
			var result = (await cipherRepo.GetAllAsync()).Where(c => c.Status == ApprovalStatus.Approved).ToList();

			if (result == null) 
				throw new InvalidOperationException("Wasn't able to retrieve approved ciphers");

			if (!string.IsNullOrEmpty(filter.SearchTerm))
				result = result.Where(c => c.Title.Contains(filter.SearchTerm)).ToList();

			if (filter.Tags != null)
				result = result.Where(c => c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type))).ToList();

			switch (filter.ChallengeType)
			{
				case ChallengeType.Standard:
					result = result.Where(x => x.ChallengeType == ChallengeType.Standard).ToList();
					break;
				case ChallengeType.Experimental:
					result = result.Where(x => x.ChallengeType == ChallengeType.Experimental).ToList();
					break;
			}

			switch (filter.OrderTerm)
			{
				case CipherOrderTerm.Newest:
					result = result.OrderByDescending(x => x.CreatedAt).ToList();
					break;
				case CipherOrderTerm.Oldest:
					result = result.OrderBy(x => x.CreatedAt).ToList();
					break;
				case CipherOrderTerm.MostPopular:
					result = result.OrderByDescending(x => x.UserSolutions).ToList();
					break;
			}

			return await ToReviewOutputViewModelMany(result);
        }
		public async Task<CipherDetailedReviewOutputViewModel> GetCipherById(int id) 
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			return await ToDetailedReviewOutputViewModel(cipher);
		}
		public async Task <CipherValidationResult> AnalyzeWithLLM (int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new InvalidOperationException("Ciper not found");

			if (cipher.LLMData.Analysis != null)
				return new CipherValidationResult
				{
					Recommendation = cipher.LLMData.Analysis,
					Reasoning = cipher.LLMData.Reasoning,
					Confidence = cipher.LLMData.Confidence,
					Issues = cipher.LLMData.Issues,
				};

			var mlPredictionJson = cipher.MLPrediction;
			var mlPredictionData = JsonSerializer.Deserialize<MlPredictionData>(mlPredictionJson);

			var mlResult = new CipherRecognitionResultViewModel
			{
				TopPrediction = new PredictionViewModel
				{
					Family = mlPredictionData.Family,
					Type = mlPredictionData.Type,
					Confidence = mlPredictionData.Confidence,
				},
				AllPredictions = mlPredictionData.AllPredictions.Select(p => new PredictionViewModel
				{
					Family = p.Family,
					Type = p.Type,
					Confidence = p.Confidence
				}).ToList()
			};

			string? type = cipher.TypeOfCipher == null ? null : cipher.TypeOfCipher.ToString();

			var validation = await llmService.ValidateCipherAsync(cipher.EncryptedText, cipher.DecryptedText, mlResult, type);
			cipher.LLMData.Analysis = validation.Recommendation.ToString();
			cipher.LLMData.Confidence = validation.Reasoning;
			cipher.LLMData.Confidence = validation.Confidence;
			cipher.LLMData.Issues = validation.Issues;
			await cipherRepo.UpdateAsync(cipher);
			return validation;
		}
		public async Task<string> ApproveCipherAsync(int id, ApproveCipherViewModel model) 
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);
			string userId = cipher.CreatedByUserId;
			if (cipher == null) 
				throw new InvalidOperationException("Cipher not found");
			else if (cipher.Status == ApprovalStatus.Approved) 
				throw new InvalidOperationException("Cipher is already approved");

			if (cipherRepo.GetAll().Where(x => x.Status == ApprovalStatus.Approved).FirstOrDefault(x => x.Title == model.Title) != null)
					throw new InvalidOperationException("There is already a cipher with this title");

			//When text is not given we cannot approve it as standard
			if (string.IsNullOrWhiteSpace(cipher.DecryptedText) && model.ChallengeType == ChallengeType.Standard)
				throw new InvalidOperationException("Cipher with unknown answer shouldn't be aproved as standard");

			if (model.ChallengeType == ChallengeType.Experimental && string.IsNullOrWhiteSpace(cipher.DecryptedText) && (model.AllowHint || model.AllowSolution || model.AllowTypeHint))
				throw new InvalidOperationException("Hints cannot be used for Experimental Ciphers");

			var title = string.IsNullOrEmpty(model.Title) ? cipher.EncryptedText : model.Title;
			cipher.Title = title;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;
			cipher.AllowTypeHint = model.AllowTypeHint;
			cipher.Status = ApprovalStatus.Approved;
			cipher.ApprovedAt = DateTime.UtcNow;
			cipher.ChallengeType = model.ChallengeType; //Give permission to the admin for him to decide which is experimental or not

			//if (cipher.TypeOfCipher == null) If the admin decides to approve it, maybe we should assing the ML predicted type
			//{
			//	string jsonString = cipher.MLPrediction;

			//	var response = JsonSerializer.Deserialize<MlPredictionData>(jsonString);

			//	var topPrediction = response.AllPredictions
			//				.MaxBy(p => p.Confidence);
			//}

			cipher.Points = cipher.TypeOfCipher.HasValue
				? PointsForType[cipher.TypeOfCipher.Value]
				: 0;

			if (model.TagIds != null && model.TagIds.Count > 0)
                await DefineTagsAsync(cipher, model.TagIds.ToList());

			await cipherRepo.UpdateAsync(cipher);
			await notificationService.CreateAndSendNotification(userId, NotificationType.CipherApproved, "Your cipher was successfully approved", cipher.Id, string.Empty);
			return cipher.CreatedByUserId;
		}
		public async Task RejectCipherAsync(int id, string reason)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);
			string userId = cipher.CreatedByUserId;
			if (cipher == null) 
				throw new InvalidOperationException("Cipher not found");

			else if (cipher.Status == ApprovalStatus.Approved) throw new InvalidOperationException("Cipher already approved");

			cipher.Status = ApprovalStatus.Rejected;
			if (!(cipher.Status == ApprovalStatus.Rejected)) throw new InvalidOperationException("Wasn't able to reject the cipher");
			cipher.RejectedAt = DateTime.UtcNow;
			cipher.RejectionReason = reason;

			await cipherRepo.UpdateAsync(cipher);
			await notificationService.CreateAndSendNotification(userId, NotificationType.CipherRejected, reason, null, string.Empty);
		}
		public async Task UnapproveCipherAsync(int id) //Still not sure if needed - REVIEW THIS
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");
			else if (cipher.Status != ApprovalStatus.Approved)
				throw new InvalidOperationException("Cipher is not approved");

			cipher.Status = ApprovalStatus.Rejected;

			await cipherRepo.UpdateAsync(cipher);
		}
		public async Task UpdateApprovedCipher(int id, UpdateCipherViewModel model)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) 
				throw new InvalidOperationException("Cipher not found");

			else if (cipher.Status != ApprovalStatus.Approved) 
				throw new InvalidOperationException("Cipher is not approved");

			if (cipherRepo.GetAll().Where(x => x.Status == ApprovalStatus.Approved).FirstOrDefault(x => x.Title == model.Title) != null)
				throw new InvalidOperationException("There is already a cipher with this title");

			cipher.Title = model.Title;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;

			if (model.TagIds != null && model.TagIds.Count > 0)
				await DefineTagsAsync(cipher, model.TagIds.ToList());

			await cipherRepo.UpdateAsync(cipher);
		}
		public async Task DeleteApprovedCipher(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) 
				throw new InvalidOperationException("Cipher not found");
			else if (cipher.Status != ApprovalStatus.Approved) 
				throw new InvalidOperationException("Cipher is not approved.");

			var solutions = (await solutionRepo.GetAllAsync()).Where(x => x.CipherId == id);

			foreach (var solution in solutions)
			{
				await solutionRepo.DeleteAsync(solution);
			}

			bool result = await cipherRepo.DeleteAsync(cipher);
			if (!result) throw new InvalidOperationException("Wasn't able to delete the cipher");
		}

		#region Common methods
		private async Task DefineTagsAsync (Cipher? cipher, List<int> tagIds)
		{
			List<Tag> existingAssignedTags = (await tagRepo.GetAllAsync()).Where(x => tagIds.Contains(x.Id)).ToList();

			//ADD THIS IN PRODUCTION!!!
			//if (existingAssignedTags.Count != tagIds.Count)
			//	throw new InvalidOperationException("One or more tag IDs don't exist");
			
			if (cipher.CipherTags != null)
				cipher.CipherTags?.Clear();
			else
				cipher.CipherTags = new List<CipherTag>();

			foreach (var tag in existingAssignedTags)
			{
				cipher.CipherTags.Add(new CipherTag
				{
					TagId = tag.Id,
					CipherId = cipher.Id,
				});
			}
		}
		private async Task<List<CipherReviewOutputViewModel>> ToReviewOutputViewModelMany(List<Cipher> result)
		{
			List<CipherReviewOutputViewModel> output = new List<CipherReviewOutputViewModel>();
			foreach (var cipher in result)
			{
				output.Add(new CipherReviewOutputViewModel
				{
					Id = cipher.Id,
					Title = cipher.Title,
					DecryptedText = cipher.DecryptedText,
					Status = cipher.Status.ToString(),
					IsImage = cipher is ImageCipher,
					IsLLMRecommended = cipher.IsLLMRecommended,
					ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				});
			}
			return output;
		}
		private async Task<CipherDetailedReviewOutputViewModel> ToDetailedReviewOutputViewModel(Cipher cipher)
		{
			var model = new CipherDetailedReviewOutputViewModel()
			{
				Id = cipher.Id,
				Title = cipher.Title,
				DecryptedText = cipher.DecryptedText,
				Points = cipher.Points,
				CipherText = cipher.EncryptedText,
				AllowType = cipher.AllowTypeHint,
				AllowHint = cipher.AllowHint,
				AllowFullSolution = cipher.AllowSolution,
				Status = cipher.Status.ToString(),
				IsLLMRecommended = cipher.IsLLMRecommended,
				ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				IsImage = cipher is ImageCipher,
			};

			if (cipher is ImageCipher)
			{
				ImageCipher cipherImage = cipher as ImageCipher;
				string imageFolderPath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), cipherImage.ImagePath);
				string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";
				model.ImageBase64 = base64;
			}

			return model;
		}
		#endregion
	}
}
