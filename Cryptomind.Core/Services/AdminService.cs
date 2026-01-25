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
		IRepository<UserSolution, int> solutionRepo,
		UserManager<ApplicationUser> userManager,
		ILLMService llmService) : IAdminService
	{
		#region Cipher admin methods
		public async Task<List<CipherReviewOutputViewModel>> AllSubmittedCiphers()
		{
			var result = (await cipherRepo.GetAllAsync()).Where(c => c.IsApproved == false).ToList();
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
				result = result.Where(c => c.Title.Contains(filter.SearchTerm)).Where(x => x.ChallengeType == filter.challengeType).ToList();

			if (filter.Tags != null)
				result = result.Where(c => c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type))).ToList();

			return await ToReviewOutputViewModelMany(result);
        }
		public async Task<CipherReviewOutputViewModel> GetCipherById(int id) 
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			var viewModel = new CipherReviewOutputViewModel();
			if (cipher == null)
				throw new InvalidOperationException("There is no cipher with the given Id");

			//string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";
			if (cipher is ImageCipher)
			{
				viewModel.Id = cipher.Id;
				viewModel.Title = cipher.Title;
				viewModel.DecryptedText = cipher.DecryptedText;
				viewModel.Points = cipher.Points;
				//viewModel.CipherText = base64;
				viewModel.AllowsAnswer = cipher.AllowSolution;
				viewModel.AllowsHint = cipher.AllowHint;
				viewModel.IsApproved = cipher.IsApproved;
				viewModel.IsImage = true;
				viewModel.ChallengeTypeDisplay = cipher.ChallengeType.ToString();
			}
			else
			{
				TextCipher textCipher = cipher as TextCipher;
				viewModel.Id = cipher.Id;
				viewModel.Title = cipher.Title;
				viewModel.DecryptedText = cipher.DecryptedText;
				viewModel.Points = cipher.Points;
				viewModel.CipherText = textCipher.EncryptedText;
				viewModel.AllowsAnswer = cipher.AllowSolution;
				viewModel.AllowsHint = cipher.AllowHint;
				viewModel.IsApproved = cipher.IsApproved;
				viewModel.IsImage = true;
				viewModel.ChallengeTypeDisplay = cipher.ChallengeType.ToString();
			}

			return viewModel;
		}
		public async Task<string> ApproveCipherAsync(int id, ApproveUpdateCipherViewModel model) 
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) 
				throw new InvalidOperationException("There is no cipher with the given Id");
			else if (cipher.IsApproved) 
				throw new InvalidOperationException("The cipher with the given Id is already approved");


			foreach (var cipherIter in cipherRepo.GetAll())
			{
				if (cipherIter.Title == model.Title && cipherIter.Id != id)
					throw new InvalidOperationException("There is already a cipher with this title");
			}

			//Check this when you see it for SAMUIL
			//if (cipherRepo.FirstOrDefaultAsync(x => x.Title == model.Title && x.Id != id) != null)
			//	throw new InvalidOperationException("There is already a cipher with this title");


			//Applying tags to the actual DB entity
			if (model.TagIds != null && model.TagIds.Count > 0)
                await DefineTagsAsync(cipher, model.TagIds.ToList());
			
			if (cipher is ImageCipher) //We give permission for the admin to change the encrypted text extracted from OCR
				(cipher as ImageCipher).EncryptedText = model.EncryptedText;

			if (string.IsNullOrEmpty(cipher.DecryptedText) && model.isStandard)  //When text is not given we cannot approve it as standard
			{
				model.isStandard = false;
				throw new InvalidOperationException("Cipher with unknown answe shouldn't be aproved as standard");
			}

			cipher.Title = model.Title;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;
			cipher.IsApproved = true;
			cipher.ChallengeType = model.isStandard ? ChallengeType.Standard : ChallengeType.Experimental;

			await cipherRepo.UpdateAsync(cipher);

			return cipher.CreatedByUserId;
		}
		public async Task<string> UnapproveCipherAsync(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new InvalidOperationException("There is no cipher with the given Id");
			else if (!cipher.IsApproved)
				throw new InvalidOperationException("The cipher with the given Id is not approved");

			cipher.IsApproved = false;
			await cipherRepo.UpdateAsync(cipher);

			return cipher.CreatedByUserId;
		}
		public async Task RejectCipherAsync(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);
			if (cipher == null) 
				throw new InvalidOperationException("There is no cipher with the given Id");
			else if (cipher.IsApproved) throw new InvalidOperationException("The cipher with the given Id is already approved");

			bool isDeleted = await cipherRepo.DeleteAsync(cipher);
			if (!isDeleted) throw new InvalidOperationException("Wasn't able to reject the cipher");
		}
		public async Task DeleteApprovedCipher(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) 
				throw new InvalidOperationException("There is no cipher with the given Id");
			else if (!cipher.IsApproved) 
				throw new InvalidOperationException("The cipher with the given Id is not approved");

			var solutions = solutionRepo.GetAllAttached().Where(x => x.CipherId == id);

			foreach (var solution in solutions)
			{
				await solutionRepo.DeleteAsync(solution);
			}

			bool result = await cipherRepo.DeleteAsync(cipher);
			if (!result) throw new InvalidOperationException("Wasn't able to delete the cipher");
		}
		public async Task UpdateApprovedCipher(int id, ApproveUpdateCipherViewModel model)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) 
				throw new InvalidOperationException("There is no cipher with the given Id");
			else if (!cipher.IsApproved) 
				throw new InvalidOperationException("The cipher with the given Id is not approved");

			foreach (var cipherIter in cipherRepo.GetAll())
			{
				if (cipherIter.Title == model.Title && cipher.Id != id)
					throw new InvalidOperationException("There is already a cipher with this title");
			}

			//Check this when you see it for SAMUIL
			//if (cipherRepo.FirstOrDefaultAsync(x => x.Title == model.Title && x.Id != id) != null)
			//	throw new InvalidOperationException("There is already a cipher with this title");

			Console.WriteLine(model.Title);
			cipher.Title = model.Title;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;
			if (model.TagIds != null && model.TagIds.Count > 0)
			{
				try
				{
					await DefineTagsAsync(cipher, model.TagIds.ToList());
				}
				catch (InvalidOperationException ex)
				{
					throw new InvalidOperationException(ex.Message, ex);
				}
			}

			await cipherRepo.UpdateAsync(cipher);
		}
		public async Task<string> SolveCipherWithLLM(int cipherId)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(cipherId);

			if (cipher == null)
				throw new InvalidOperationException("There is no cipher with the given Id");

			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText) &&
				!string.IsNullOrWhiteSpace(cipher.LLMAnalysis))
			{
				return cipher.LLMAnalysis;
			}

			if (string.IsNullOrWhiteSpace(cipher.MLPrediction))
			{
				throw new InvalidOperationException(
					"Cannot solve cipher: ML prediction not available. Cipher may not have been properly submitted.");
			}

			var mlPrediction = JsonSerializer.Deserialize<MlPredictionData>(cipher.MLPrediction);

			if (mlPrediction == null)
			{
				throw new InvalidOperationException("Failed to parse ML prediction data");
			}

			var mlResult = new CipherRecognitionResultViewModel
			{
				TopPrediction = new PredictionViewModel
				{
					Family = mlPrediction.Family,
					Type = mlPrediction.Type,
					Confidence = mlPrediction.Confidence
				},
				AllPredictions = mlPrediction.AllPredictions?.Select(p => new PredictionViewModel
				{
					Family = p.Family,
					Type = p.Type,
					Confidence = p.Confidence
				}).ToList() ?? new List<PredictionViewModel>()
			};

			string encryptedText;
			if (cipher is ImageCipher imageCipher)
			{
				encryptedText = imageCipher.EncryptedText;
			}
			else if (cipher is TextCipher textCipher)
			{
				encryptedText = textCipher.EncryptedText;
			}
			else
			{
				throw new InvalidOperationException("Unknown cipher type");
			}

			var solution = await llmService.SolveCipherAsync(
				encryptedText,
				cipher.DecryptedText,
				mlResult);

			//Cache the solution, optional
			cipher.LLMAnalysis = solution;
			await cipherRepo.UpdateAsync(cipher);

			return solution;
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
						ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
					});
                }
            }
            return output;
        }
		#endregion
	}
}
