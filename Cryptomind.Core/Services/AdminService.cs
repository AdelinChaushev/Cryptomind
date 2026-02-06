using Cryptomind.Common.AdminViewModels;
using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Common.CipherRecognitionViewModels;
using Cryptomind.Common.CipherViewModels;
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

namespace Cryptomind.Core.Services
{
	public class AdminService (
		IRepository<Cipher, int> cipherRepo,
		IRepository<Tag, int> tagRepo,
		IRepository<AnswerSuggestion, int> answerRepo,
		IRepository<UserSolution, int> solutionRepo,
		UserManager<ApplicationUser> userManager,
		ILLMService llmService,
		INotificationService notificationService) : IAdminService
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

		#region Cipher admin methods
		public async Task<List<CipherReviewOutputViewModel>> AllSubmittedCiphers()
		{
			var result = (await cipherRepo.GetAllAsync()).Where(c => c.IsApproved == false).OrderBy(x => x.CreatedAt).ToList();

			if (result == null) 
				throw new InvalidOperationException("Wasn't able to retrieve submitted ciphers");

			return await ToReviewOutputViewModelMany(result);
		}
		public async Task<List<CipherReviewOutputViewModel>> AllApprovedCiphers(CipherFilter filter)
		{
			var result = (await cipherRepo.GetAllAsync()).Where(c => c.IsApproved == true).ToList();

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

			return await ToReviewOutputViewModelMany(result);
        }
		public async Task<List<AnswerSuggestionViewModel>> AllSubmittedAnswersAsync()
		{
			var answerSuggestions = (await answerRepo.GetAllAsync())
				.Where(x => !x.IsApproved)
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
		public async Task<CipherReviewOutputViewModel> GetCipherById(int id) 
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			var model = new CipherReviewOutputViewModel();
			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			model.Id = cipher.Id;
			model.Title = cipher.Title;
			model.DecryptedText = cipher.DecryptedText;
			model.Points = cipher.Points;
			model.CipherText = cipher.EncryptedText;
			model.AllowsAnswer = cipher.AllowSolution;
			model.AllowsHint = cipher.AllowHint;
			model.IsApproved = cipher.IsApproved;
			model.IsLLMRecommended = cipher.IsLLMRecommended;
			model.ChallengeTypeDisplay = cipher.ChallengeType.ToString();

			//What is that?
			//string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";

			if (cipher is ImageCipher)
				model.IsImage = true;
			else
				model.IsImage = false;	

			return model;
		}
		public async Task<AnswerSuggestionReviewViewModel> GetAnswerById(int id)
		{
			var answer = await answerRepo.GetByIdAsync(id);
			if (answer == null)
				throw new InvalidOperationException("Answer not found");

			var userName = (await userManager.FindByIdAsync(answer.UserId)).UserName;

			if (userName == null)
				throw new InvalidOperationException("User not found");

			var model = new AnswerSuggestionReviewViewModel
			{
				CipherId = answer.CipherId,
				Description = answer.Description,
				DecryptedText = answer.DecryptedText,
				Username = userName,
			};

			return model;
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

			var type = cipher.TypeOfCipher != CipherType.None ? cipher.TypeOfCipher.ToString() : null;

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
			else if (cipher.IsApproved) 
				throw new InvalidOperationException("Cipher is already approved");

			if (cipherRepo.GetAll().Where(x => x.IsApproved).FirstOrDefault(x => x.Title == model.Title) != null)
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
			cipher.TypeOfCipher = model.CipherType;
			cipher.IsApproved = true;
			cipher.ChallengeType = model.ChallengeType; //Give permission to the admin for him to decide which is experimental or not
			cipher.Points = PointsForType[cipher.TypeOfCipher];

			if (model.TagIds != null && model.TagIds.Count > 0)
                await DefineTagsAsync(cipher, model.TagIds.ToList());

			await cipherRepo.UpdateAsync(cipher);
			await notificationService.CreateAndSendNotification(userId, NotificationType.CipherApproved, "Your cipher was successfully approved", cipher.Id, string.Empty);
			return cipher.CreatedByUserId;
		}
		public async Task<string> ApproveAnswerAsync(int id, int points)
		{
			var answer = await answerRepo.FirstOrDefaultAsync(x => x.Id == id);

			if (answer == null)
				throw new InvalidOperationException("Answer not found");

			var cipher = await cipherRepo.FirstOrDefaultAsync(x => x.Id == answer.CipherId);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

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
			};

			cipher.DecryptedText = answer.DecryptedText;
			cipher.ChallengeType = ChallengeType.Standard;
			answer.IsApproved = true;
			user.Score += points;

			solutionRepo.Add(userSolution);
			cipherRepo.Update(cipher);
			answerRepo.Update(answer);
			await userManager.UpdateAsync(user);
			await notificationService.CreateAndSendNotification(user.Id, NotificationType.AnswerApproved, $"Your answer suggestion was approved +{points} points", cipher.Id, string.Empty);

			return answer.UserId;
		}
		public async Task UnapproveCipherAsync(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");
			else if (!cipher.IsApproved)
				throw new InvalidOperationException("Cipher is not approved");

			cipher.IsApproved = false;

			await cipherRepo.UpdateAsync(cipher);
		}
		public async Task RejectCipherAsync(int id, string reason)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);
			string userId = cipher.CreatedByUserId;
			if (cipher == null) 
				throw new InvalidOperationException("Cipher not found");

			else if (cipher.IsApproved) throw new InvalidOperationException("Cipher already approved");

			bool isDeleted = await cipherRepo.DeleteAsync(cipher);

			//WE HAVE TO SEND THE REJECT NOTIFICATION WITH IT'S REASONING TO THE USER
			if (!isDeleted) throw new InvalidOperationException("Wasn't able to reject the cipher");

			await notificationService.CreateAndSendNotification(userId, NotificationType.CipherRejected, reason, null, string.Empty);
		}
		public async Task RejectAnswerAsync(int id, string reason)
		{
			AnswerSuggestion? answer = await answerRepo.GetByIdAsync(id);
			if (answer == null)
				throw new InvalidOperationException("Answer not found");

			else if (answer.IsApproved) throw new InvalidOperationException("Answer already approved");

			bool isDeleted = await answerRepo.DeleteAsync(answer);

			//WE HAVE TO SEND THE REJECT NOTIFICATION WITH IT'S REASONING TO THE USER
			if (!isDeleted) throw new InvalidOperationException("Wasn't able to reject the answer");

			await notificationService.CreateAndSendNotification(answer.UserId, NotificationType.AnswerRejected, reason, null, string.Empty);
		}
		public async Task DeleteApprovedCipher(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) 
				throw new InvalidOperationException("Cipher not found");
			else if (!cipher.IsApproved) 
				throw new InvalidOperationException("Cipher is not approved, instead you can reject it");

			var solutions = (await solutionRepo.GetAllAsync()).Where(x => x.CipherId == id);

			foreach (var solution in solutions)
			{
				await solutionRepo.DeleteAsync(solution);
			}

			bool result = await cipherRepo.DeleteAsync(cipher);
			if (!result) throw new InvalidOperationException("Wasn't able to delete the cipher");
		}
		public async Task UpdateApprovedCipher(int id, UpdateCipherViewModel model)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) 
				throw new InvalidOperationException("Cipher not found");

			else if (!cipher.IsApproved) 
				throw new InvalidOperationException("Cipher is not approved");

			if (cipherRepo.GetAll().Where(x => x.IsApproved).FirstOrDefault(x => x.Title == model.Title) != null)
				throw new InvalidOperationException("There is already a cipher with this title");

			cipher.Title = model.Title;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;

			if (model.TagIds != null && model.TagIds.Count > 0)
				await DefineTagsAsync(cipher, model.TagIds.ToList());

			await cipherRepo.UpdateAsync(cipher);
		}
		#endregion

		#region User-modifying methods
		public async Task<List<UserViewModel>> GetAllUsers()
		{
			var userViewModels = new List<UserViewModel>();

			foreach (var user in userManager.Users.ToList())
			{
				userViewModels.Add(new UserViewModel
				{
					Id = user.Id,
					Username = user.UserName,
					Email = user.Email,
					IsAdmin = await userManager.IsInRoleAsync(user, "Admin"),
					PendingCiphers = user.UploadedCiphers.Count(c => !c.IsApproved)
				});
			}

			return userViewModels;
		}
		public async Task<UserDetailViewModel> GetUser (string userId)
		{
			ApplicationUser? user = await userManager.Users
					.Include(x => x.UploadedCiphers)
					.Include(x => x.SolvedCiphers)
						.ThenInclude(x => x.Cipher)
					.Include(x => x.HintsRequested)
					.FirstOrDefaultAsync(x => x.Id == userId);

			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			List<UserCipherViewModel> submittedCiphers = new List<UserCipherViewModel>();
			List<UserCipherViewModel> solvedCiphers = new List<UserCipherViewModel>();

			foreach (var cipher in user.UploadedCiphers)
			{
				var viewModel = new UserCipherViewModel
				{
					Id = cipher.Id,
					Title = cipher.Title,
					TypeOfCipher = cipher.TypeOfCipher,
					IsApproved = cipher.IsApproved,
					Points = cipher.Points,
					CreatedAt = cipher.CreatedAt,
					ChallengeType = cipher.ChallengeType,
				};

				submittedCiphers.Add(viewModel);
			}
			foreach (var userSolution in user.SolvedCiphers)
			{
				var cipher = userSolution.Cipher;

				var viewModel = new UserCipherViewModel
				{
					Id = cipher.Id,
					Title = cipher.Title,
					TypeOfCipher = cipher.TypeOfCipher,
					IsApproved = cipher.IsApproved,
					Points = cipher.Points,
					CreatedAt = cipher.CreatedAt,
					ChallengeType = cipher.ChallengeType,
				};

				solvedCiphers.Add(viewModel);
			}

			bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
			return new UserDetailViewModel
			{
				Id = user.Id,
				Email = user.Email,
				IsAdmin = isAdmin,
				Username = user.UserName,
				IsEmailConfirmed = user.EmailConfirmed,
				IsBanned = user.isBanned,
				BanReason = user.BanReason,
				BannedAt = user.BannedAt,
				RegisteredAt = user.RegisteredAt,
				TotalScore = user.Score,
				CiphersSubmitted = user.UploadedCiphers.Count(),
				CiphersSolved = user.SolvedCount,
				HintsRequested = user.HintsRequested.Count(),
				SolveSuccessRate = user.SuccessRate,
				ApprovedCiphers = user.UploadedCiphers.Where(x => x.IsApproved).Count(),
				PendingCiphers = user.UploadedCiphers.Where(x => !x.IsApproved).Count(),
				SubmittedCiphers = submittedCiphers,
				SolvedCiphers = solvedCiphers,
			};
		}
		public async Task MakeAdmin (string userId)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			if (!await userManager.IsInRoleAsync(user, "Admin"))
			{
				await userManager.AddToRoleAsync(user, "Admin");
			}
			else
			{
				throw new ArgumentException("The user is already an admin");
			}
		}
		public async Task BanUserAsync(string userId, string reason)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			if (user.isBanned)
				throw new InvalidOperationException("This user is already banned");

			await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
			await userManager.SetLockoutEnabledAsync(user, true);

			user.isBanned = true;
			user.BanReason = reason;
			user.BannedAt = DateTime.Now;
			await userManager.UpdateAsync(user);
		}
		public async Task UnbanUserAsync(string userId)
		{
			ApplicationUser? user = await userManager.FindByIdAsync(userId);
			if (user == null)
				throw new ArgumentException("There is no user with this ID");

			if (!user.isBanned)
				throw new InvalidOperationException("This is user is not banned");

			await userManager.SetLockoutEndDateAsync(user, DateTime.Now);

			user.isBanned = false;
			user.BanReason = null;
			user.BannedAt = null;
			await userManager.UpdateAsync(user);
		}
		#endregion

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

                if (cipher is ImageCipher)
                {
                    ImageCipher cipherImage = cipher as ImageCipher;
					string cipherText = (cipher as ImageCipher).EncryptedText;
                    string imageFolderPath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), cipherImage.ImagePath);
                    string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";

                    output.Add(new CipherReviewOutputViewModel
                    {
                        Id = cipher.Id,
                        Title = cipher.Title,
                        DecryptedText = cipher.DecryptedText,
                        Points = cipher.Points,
                        //Image = base64,
						CipherText = cipherText,
                        AllowsAnswer = cipher.AllowSolution,
                        AllowsHint = cipher.AllowHint,
                        IsApproved = cipher.IsApproved,
                        IsImage = true,
						IsLLMRecommended = cipher.IsLLMRecommended,
						ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
					});
                }
                else
                {
                    TextCipher cipherText = cipher as TextCipher;
                    output.Add(new CipherReviewOutputViewModel
                    {
                        Id = cipher.Id,
                        Title = cipher.Title,
                        DecryptedText = cipher.DecryptedText,
                        CipherText = cipherText.EncryptedText,
                        Points = cipher.Points,
                        AllowsAnswer = cipher.AllowSolution,
                        AllowsHint = cipher.AllowHint,
                        IsApproved = cipher.IsApproved,
                        IsImage = false,
						IsLLMRecommended = cipher.IsLLMRecommended,
						ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
					});
                }
            }
            return output;
        }
		#endregion
	}
}
