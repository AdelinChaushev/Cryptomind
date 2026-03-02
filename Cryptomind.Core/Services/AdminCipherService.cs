using Cryptomind.Common.Constants;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.Helpers;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;


namespace Cryptomind.Core.Services
{
	public class AdminCipherService(
		IRepository<Cipher, int> cipherRepo,
		IRepository<Tag, int> tagRepo,
		ILLMService llmService,
		INotificationService notificationService,
		UserManager<ApplicationUser> userManager) : IAdminCipherService
	{
		private const string DateFormat = "ddd, dd MMM yyyy HH:mm";

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
		public async Task<List<PendingCipherTitleViewModels>> GetRecentCipherSubmissionTitles()
		{
			return await cipherRepo.GetAllAttached()
				.Where(x => x.Status == ApprovalStatus.Pending)
				.OrderByDescending(x => x.CreatedAt)
				.Take(5)
				.Select(x => new PendingCipherTitleViewModels()
				{
					Id = x.Id,
					CreatedBy = x.CreatedByUser.UserName,
					Title = x.Title,
					SubmittedAt = x.CreatedAt.ToString(DateFormat),
				})
				.ToListAsync();
		}
		public async Task<int> GetPendingCiphersCount()
		{
			return (await cipherRepo.GetAllAsync())
				.Count(x => x.Status == ApprovalStatus.Pending && !x.IsDeleted);
		}
		public async Task<int> GetApprovedCiphersCount()
		{
			return (await cipherRepo.GetAllAsync())
				.Count(x => x.Status == ApprovalStatus.Approved && !x.IsDeleted);
		}
		public async Task<int> GetDeletedCiphersCount()
		{
			return (await cipherRepo.GetAllAsync())
				.Count(x => x.IsDeleted);
		}
		public async Task<List<CipherReviewOutputViewModel>> AllPendingCiphers(string? filter)
		{
			var result = await cipherRepo.GetAllAttached()
				.Include(c => c.CreatedByUser)
				.Where(c => c.Status == ApprovalStatus.Pending && !c.IsDeleted)
				.OrderBy(x => x.CreatedAt)
				.ToListAsync();

			if (filter != null)
				result = result.Where(x => x.Title.ToLower().Contains(filter.ToLower())).ToList();

			if (!result.Any())
				return new List<CipherReviewOutputViewModel>();

			return ToReviewOutputViewModelMany(result, true);
		}
		public async Task<List<CipherReviewOutputViewModel>> AllApprovedCiphers(CipherFilter filter)
		{
			var result = await cipherRepo.GetAllAttached()
				.Include(x => x.CipherTags)
				.ThenInclude(x => x.Tag)
				.Include(c => c.CreatedByUser)
				.Include(c => c.UserSolutions)
		.Where(c => c.Status == ApprovalStatus.Approved && !c.IsDeleted)
				.ToListAsync();

			if (!result.Any())
				return new List<CipherReviewOutputViewModel>();

			if (!string.IsNullOrEmpty(filter.SearchTerm))
				result = result.Where(c => c.Title.ToLower().Contains(filter.SearchTerm.ToLower())).ToList();

			if (filter.Tags != null && filter.Tags[0] != TagType.None)
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
					result = result.OrderByDescending(x => x.UserSolutions.Count).ToList();
					break;
				case CipherOrderTerm.LeastPopular:
					result = result.OrderBy(x => x.UserSolutions.Count).ToList();
					break;
			}

			return ToReviewOutputViewModelMany(result, false);
		}
		public async Task<List<CipherReviewOutputViewModel>> AllDeletedCiphers(CipherFilter filter)
		{
			var result = await cipherRepo.GetAllAttached()
		.Include(c => c.UserSolutions)
		.Include(c => c.CreatedByUser)
		.Include(c => c.CipherTags)
		.ThenInclude(x => x.Tag)
		.Where(c => c.IsDeleted)
		.ToListAsync();

			if (!result.Any())
				return new List<CipherReviewOutputViewModel>();

			if (!string.IsNullOrEmpty(filter.SearchTerm))
				result = result.Where(c => c.Title.ToLower().Contains(filter.SearchTerm.ToLower())).ToList();

			if (filter.Tags != null && !(filter.Tags.Count == 1 && filter.Tags[0] == TagType.None))
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
					result = result.OrderByDescending(x => x.UserSolutions.Count).ToList();
					break;
				case CipherOrderTerm.LeastPopular:
					result = result.OrderBy(x => x.UserSolutions.Count).ToList();
					break;
			}

			return ToReviewOutputViewModelMany(result, false);
		}
		public async Task<CipherDetailedReviewOutputViewModel> GetCipherById(int id)
		{
			Cipher? cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.CreatedByUser)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);

			if (cipher.CreatedByUser == null)
				throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(cipher.CreatedByUserId, cipher.Id));

			return await ToDetailedReviewOutputViewModel(cipher);
		}
		public async Task<CipherValidationResultDTO> AnalyzeWithLLM(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);

			if (cipher.Status != ApprovalStatus.Pending)
				throw new ConflictException(CipherErrorConstants.CannotAnalyzeResolvedCiphers);

			if (cipher.IsDeleted)
				throw new ConflictException(CipherErrorConstants.CipherDeletedConflict);

			if (cipher.LLMData.Reasoning != null)
				return new CipherValidationResultDTO
				{
					PredictedType = cipher.LLMData.PredictedType,
					Reasoning = cipher.LLMData.Reasoning,
					Confidence = cipher.LLMData.Confidence,
					Issues = cipher.LLMData.Issues,
					SolutionCorrect = cipher.LLMData.SolutionCorrect,
					IsAppropriate = cipher.LLMData.IsAppropriate ?? true,
					IsSolvable = cipher.LLMData.IsSolvable,
					Recommendation = cipher.LLMData.Recommendation
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

			string? type = cipher.TypeOfCipher?.ToString();

			var validation = await llmService.ValidateCipherAsync(cipher.EncryptedText, cipher.DecryptedText, mlResult, type);
			cipher.LLMData.Reasoning = validation.Reasoning;
			cipher.LLMData.Confidence = validation.Confidence;
			cipher.LLMData.Issues = validation.Issues;
			cipher.LLMData.PredictedType = validation.PredictedType;
			cipher.LLMData.SolutionCorrect = validation.SolutionCorrect;
			cipher.LLMData.IsAppropriate = validation.IsAppropriate;
			cipher.LLMData.IsSolvable = validation.IsSolvable;
			cipher.LLMData.Recommendation = validation.Recommendation;

			await cipherRepo.UpdateAsync(cipher);
			return validation;
		}
		public async Task<string> ApproveCipherAsync(int id, ApproveCipherViewModel model)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);
			else if (cipher.IsDeleted)
				throw new ConflictException(CipherErrorConstants.CipherDeletedConflict);
			else if (cipher.Status != ApprovalStatus.Pending)
				throw new ConflictException(CipherErrorConstants.CanApproveOnlyPendingCiphers);

			string userId = cipher.CreatedByUserId;

			if ((await userManager.FindByIdAsync(userId)) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			if (string.IsNullOrEmpty(model.Title))
				throw new CustomValidationException(CipherErrorConstants.TitleRequiredMessage);

			if ((await cipherRepo.GetAllAsync()).FirstOrDefault(x => x.Title == model.Title && x.Id != id && !x.IsDeleted) != null)
				throw new ConflictException(CipherErrorConstants.DuplicateTitleMessage);

			//When type is not given we cannot approve it
			if (model.TypeOfCipher == null)
				throw new ConflictException(CipherErrorConstants.CanApproveOnlyCiphersWithType);

			cipher.ChallengeType = string.IsNullOrWhiteSpace(cipher.DecryptedText)
				? ChallengeType.Experimental
				: ChallengeType.Standard;

			if (cipher.ChallengeType == ChallengeType.Experimental && (model.AllowHint || model.AllowSolution || model.AllowTypeHint))
				throw new ConflictException(CipherErrorConstants.HintExperimentalConflict);

			cipher.Title = model.Title;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;
			cipher.AllowTypeHint = model.AllowTypeHint;
			cipher.Status = ApprovalStatus.Approved;
			cipher.ApprovedAt = DateTime.UtcNow.AddHours(2);
			cipher.TypeOfCipher = model.TypeOfCipher;

			if (!PointsForType.ContainsKey(model.TypeOfCipher.Value))
				throw new ConflictException(CipherErrorConstants.InvalidCipherType);

			cipher.Points = cipher.TypeOfCipher.HasValue
				? PointsForType[cipher.TypeOfCipher.Value]
				: 0;

			if (model.TagIds != null && model.TagIds.Count > 0)
				await DefineTagsAsync(cipher, model.TagIds.ToList());

			await cipherRepo.UpdateAsync(cipher);
			await notificationService.CreateAndSendNotification(
				userId,
				NotificationType.CipherApproved,
				$"Вашият шифър беше успешно одобрен {cipher.Title}",
				$"cipher/{cipher.Id}");
			return cipher.CreatedByUserId;
		}
		public async Task RejectCipherAsync(int id, string reason)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);
			else if (cipher.IsDeleted)
				throw new ConflictException(CipherErrorConstants.CipherDeletedConflict);
			else if (cipher.Status == ApprovalStatus.Approved)
				throw new ConflictException(CipherErrorConstants.CipherIsAlreadyApproved);
			else if (cipher.Status == ApprovalStatus.Rejected)
				throw new ConflictException(CipherErrorConstants.CipherIsAlreadyRejected);

			string userId = cipher.CreatedByUserId;

			if ((await userManager.FindByIdAsync(userId)) == null)
				throw new NotFoundException(CipherErrorConstants.UserNotFoundMessage);

			cipher.Status = ApprovalStatus.Rejected;
			cipher.RejectedAt = DateTime.UtcNow.AddHours(2);
			cipher.RejectionReason = reason;

			await cipherRepo.UpdateAsync(cipher);
			await notificationService.CreateAndSendNotification(
				userId,
				NotificationType.CipherRejected,
				$"Вашият шифър {cipher.Title} беше отхвърлен. Причина:" + reason,
		CipherErrorConstants.MySubmissionsPath + "?tab=ciphers");
		}
		public async Task UpdateApprovedCipher(int id, UpdateCipherViewModel model)
		{
			Cipher? cipher = await cipherRepo.GetAllAttached()
				.Include(c => c.CipherTags)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);
			else if (cipher.IsDeleted)
				throw new ConflictException(CipherErrorConstants.CipherDeletedConflict);
			else if (cipher.Status != ApprovalStatus.Approved)
				throw new ConflictException(CipherErrorConstants.CipherIsNotApproved);

			if (string.IsNullOrEmpty(model.Title))
				throw new CustomValidationException(CipherErrorConstants.TitleRequiredMessage);
			if (cipherRepo.GetAll().FirstOrDefault(x => x.Title == model.Title && x.Id != id && !x.IsDeleted) != null)
				throw new ConflictException(CipherErrorConstants.DuplicateTitleMessage);

			if (cipher.ChallengeType == ChallengeType.Experimental && (model.AllowHint || model.AllowSolution || model.AllowTypeHint))
				throw new ConflictException(CipherErrorConstants.HintExperimentalConflict);

			cipher.Title = model.Title;
			cipher.AllowTypeHint = model.AllowTypeHint;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;

			if (model.TagIds != null && model.TagIds.Count > 0)
				await DefineTagsAsync(cipher, model.TagIds.ToList());
			else
			{
				cipher.CipherTags = new HashSet<CipherTag>();

			}


			await cipherRepo.UpdateAsync(cipher);

			await notificationService.CreateAndSendNotification(
				cipher.CreatedByUserId,
				NotificationType.CipherUpdated,
				$"Шифърът ви беше актуализиран {cipher.Title}",
				$"cipher/{cipher.Id}");
		}
		public async Task SoftDeleteCipher(int id)
		{
			Cipher? cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.AnswerSuggestions)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);
			else if (cipher.IsDeleted)
				throw new ConflictException(CipherErrorConstants.CipherIsAlreadyDeleted);
			else if (cipher.Status == ApprovalStatus.Rejected)
				throw new ConflictException(CipherErrorConstants.CipherIsAlreadyRejected);

			if (cipher.AnswerSuggestions.Any(x => x.Status == ApprovalStatus.Pending))
			{
				foreach (var answer in cipher.AnswerSuggestions)
				{
					await notificationService.CreateAndSendNotification(
						answer.UserId,
						NotificationType.AnswerCipherDeleted,
						string.Format(CipherNotificationsText.AnswerDeletedNotification, cipher.Title),
			CipherErrorConstants.MySubmissionsPath + "tab=answers");
				}
			}

			cipher.IsDeleted = true;
			cipher.DeletedAt = DateTime.UtcNow.AddHours(2);

			await notificationService.CreateAndSendNotification(
				cipher.CreatedByUserId,
				NotificationType.CipherDeleted,
				string.Format(CipherNotificationsText.CipherDeletedNotification, cipher.Title),
		CipherErrorConstants.MySubmissionsPath+ "?tab=ciphers");
			await cipherRepo.UpdateAsync(cipher);
		}
		public async Task RestoreCipher(int id, string? newTitle = null)
		{
			Cipher? cipher = await cipherRepo.GetAllAttached()
				.Include(x => x.AnswerSuggestions)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (cipher == null)
				throw new NotFoundException(CipherErrorConstants.CipherNotFoundMessage);
			else if (!cipher.IsDeleted)
				throw new ConflictException("Шифърът не е изтрит");

			bool titleConflict = (await cipherRepo.GetAllAsync())
				.Any(x => x.Title == cipher.Title && x.Id != cipher.Id && !x.IsDeleted);

			if (titleConflict && newTitle == null)
				throw new ConflictException(CipherErrorConstants.DuplicateTitleMessage);

			if (newTitle != null)
			{
				if (string.IsNullOrEmpty(newTitle))
					throw new CustomValidationException(CipherErrorConstants.TitleRequiredMessage);

				titleConflict = (await cipherRepo.GetAllAsync())
					.Any(x => x.Title == newTitle && x.Id != cipher.Id && !x.IsDeleted);

				if (titleConflict)
					throw new ConflictException(CipherErrorConstants.DuplicateTitleMessage);

				cipher.Title = newTitle;
			}

			if (cipher.AnswerSuggestions.Any(x => x.Status == ApprovalStatus.Pending))
			{
				foreach (var answer in cipher.AnswerSuggestions)
				{
					await notificationService.CreateAndSendNotification(
						answer.UserId,
						NotificationType.AnswerCipherRestored,
						string.Format(CipherNotificationsText.AnswerRestoredNotification, answer.Cipher.Title),
						CipherErrorConstants.MySubmissionsPath + "?tab=answers"
                        );
				}
			}

			cipher.IsDeleted = false;
			cipher.DeletedAt = null;
			await notificationService.CreateAndSendNotification(
				cipher.CreatedByUserId,
				NotificationType.CipherRestored,
				string.Format(CipherNotificationsText.CipherRestoredNotification, cipher.Title),
				$"cipher/{cipher.Id}");
			await cipherRepo.UpdateAsync(cipher);
		}

		#region Common methods
		private async Task DefineTagsAsync(Cipher cipher, List<int> tagIds)
		{
			List<Tag> assignedExistingTags = (await tagRepo.GetAllAsync())
				.Where(x => tagIds.Contains(x.Id))
				.ToList();

			//ADD THIS IN PRODUCTION!!! - Check it first.
			//if (assignedExistingTags.Count != tagIds.Count)
			//	throw new ConflictException("One or more tag IDs is not valid");

			//This is the creation of the cipher
			if (cipher.CipherTags.Count > 0)
				cipher.CipherTags?.Clear();

			foreach (var tag in assignedExistingTags)
			{
				cipher.CipherTags.Add(new CipherTag
				{
					TagId = tag.Id,
					CipherId = cipher.Id,
				});
			}
		}
		private List<CipherReviewOutputViewModel> ToReviewOutputViewModelMany(List<Cipher> result, bool useMlPrediction = false)
		{
			List<CipherReviewOutputViewModel> output = new List<CipherReviewOutputViewModel>();
			foreach (var cipher in result)
			{
				MlPredictionType mlData = Deserialize(cipher.MLPrediction);
				string challengeType = !cipher.IsDeleted ? cipher.ChallengeType.ToString() : "CipherDeleted";

				if (cipher.CreatedByUser == null)
					throw new Exception(GeneralErrorConstants.MatchDataIntegrityError(cipher.CreatedByUserId, cipher.Id));

				CipherType? cipherType = useMlPrediction
					? Enum.Parse<CipherType>(mlData.Type)
					: cipher.TypeOfCipher;

				output.Add(new CipherReviewOutputViewModel
				{
					Id = cipher.Id,
					Title = cipher.Title,
					IsImage = cipher is ImageCipher,
					IsTypeHintAllowed = cipher.AllowTypeHint,
					IsHintAllowed = cipher.AllowHint,
					IsSolutionAllowed = cipher.AllowSolution,
					Tags = cipher.CipherTags.Select(x => x.Tag.Type.ToString()).ToList(),
					SubmittedBy = cipher.CreatedByUser.UserName,
					ChallengeType = (int)cipher.ChallengeType == 0 ? true : false,
					SubmittedAt = (int)cipher.Status == 1 ? (cipher.ApprovedAt?.ToString(DateFormat)) : (int)cipher.Status == 0 ? cipher.CreatedAt.ToString(DateFormat) : cipher.DeletedAt?.ToString(DateFormat),
					CipherType = cipherType.HasValue ? CipherTypeMapperHelper.ToDisplayName(cipherType.Value) : null,
					PercentageOfConfidence = (int)Math.Floor(mlData.Confidence * 100),
					IsLLMRecommended = cipher.IsLLMRecommended,
				});
			}
			return output;
		}
		private async Task<CipherDetailedReviewOutputViewModel> ToDetailedReviewOutputViewModel(Cipher cipher)
		{
			MlPredictionType mlData = Deserialize(cipher.MLPrediction);

			int cipherType = cipher.TypeOfCipher.HasValue ? (int)cipher.TypeOfCipher.Value : -1;
			var model = new CipherDetailedReviewOutputViewModel()
			{
				Id = cipher.Id,
				Title = cipher.Title,
				DecryptedText = cipher.DecryptedText,
				Points = cipher.Points,
				CipherText = cipher.EncryptedText,
				SetCipherType = cipher.TypeOfCipher.HasValue ? (int)cipher.TypeOfCipher.Value : -1,
				CreatorUserName = cipher.CreatedByUser.UserName,
				AllowType = cipher.AllowTypeHint,
				AllowHint = cipher.AllowHint,
				AllowFullSolution = cipher.AllowSolution,
				Status = cipher.Status.ToString(),
				IsLLMRecommended = cipher.IsLLMRecommended,
				ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				IsImage = cipher is ImageCipher,
				SubmittedBy = cipher.CreatedByUser.UserName,
				SubmittedAt = (int)cipher.Status == 1 ? (cipher.ApprovedAt?.ToString(DateFormat)) : (int)cipher.Status == 0 ? cipher.CreatedAt.ToString(DateFormat) : cipher.DeletedAt?.ToString(DateFormat),
				CipherType = CipherTypeMapperHelper.ToDisplayName(Enum.Parse<CipherType>(mlData.Type)),
				PercentageOfConfidence = (int)Math.Floor(mlData.Confidence * 100)
			};

			if (cipher is ImageCipher)
			{
				ImageCipher cipherImage = cipher as ImageCipher;
				string imagePath = Path.Combine(PathHelper.GetImagesBasePath(), cipherImage.ImagePath);
				string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imagePath))}";
				model.ImageBase64 = base64;
			}

			return model;
		}
		#endregion
		private MlPredictionType Deserialize(string mlPrediction)
		{
			MlPredictionType mlData = new MlPredictionType();
			if (!mlPrediction.IsNullOrEmpty())
			{
				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				};
				mlData = JsonSerializer.Deserialize<MlPredictionType>(mlPrediction, options);
			}
			return mlData;
		}

	}
}