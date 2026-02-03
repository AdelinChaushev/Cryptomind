using Cryptomind.Common.CipherViewModels;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services.OCR;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cryptomind.Core.Services
{
	public class CipherService(
		IRepository<Cipher, int> cipherRepo,
		IRepository<UserSolution, int> solutionRepo,
		IRepository<AnswerSuggestion, int> answerRepo,
		IOCRService ocrService,
		ICipherRecognizerService cipherRecognizerService,
		ILLMService llmService,
		IEnglishValidationService englishValidationService,
		UserManager<ApplicationUser> userManager,
		UserManager<ApplicationUser> userRepository) : ICipherService
	{
		public async Task<List<CipherOutputViewModel>> GetApprovedAsync(CipherFilter? filter)
		{
			List<Cipher> approved = (await cipherRepo.GetAllAsync()).Where(c => c.IsApproved).ToList();

			if (!string.IsNullOrEmpty(filter.SearchTerm))
				approved = approved.Where(c => c.Title.Contains(filter.SearchTerm)).ToList();

			if (filter.Tags != null)
				approved = approved.Where(c => c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type))).ToList();

			switch (filter.ChallengeType)
			{
				case ChallengeType.Standard:
					approved = approved.Where(x => x.ChallengeType == ChallengeType.Standard).ToList();
					break;
				case ChallengeType.Experimental:
					approved = approved.Where(x => x.ChallengeType == ChallengeType.Experimental).ToList();
					break;
			}

			List<CipherOutputViewModel> result = new List<CipherOutputViewModel>();
			foreach (var cipher in approved)
			{
				result.Add(await ToOutputViewModel(cipher));
			}
			return result;
		}
		public async Task<CipherOutputViewModel?> GetCipherAsync(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			return await ToOutputViewModel(cipher);
		}
		public async Task<Cipher> SubmitCipherAsync(SubmitCipherViewModel model, string userId)
		{
			if ((await cipherRepo.GetAllAsync()).FirstOrDefault(x => x.Title == model.Title) != null)
				throw new InvalidOperationException("There is already a cipher with this name");

			if ((await cipherRepo.GetAllAsync()).FirstOrDefault(x => x.EncryptedText == model.EncryptedText) != null)
				throw new InvalidOperationException("There is already a cipher like this");

			if (string.IsNullOrEmpty(model.DecryptedText) && model.CipherType == CipherType.None)
				throw new InvalidOperationException("Cannot submit cipher with unknown decrypted text and cipher type");

			Cipher? cipher = null;
			string encryptedTextForAnalysis = model.EncryptedText;

			var title = string.IsNullOrEmpty(model.Title) ? "TESTING" : model.Title; //Decide what to do if there is no title provided
			if (model.CipherDefinition == CipherDefinition.TextCipher)
			{
				cipher = new TextCipher()
				{
					Title = title,
					DecryptedText = model.DecryptedText,
					EncryptedText = model.EncryptedText,
					TypeOfCipher = model.CipherType, //Can just send null values instead of having a None value, because what is that cipher type - "none"
					AllowHint = false,
					AllowSolution = false,
					IsApproved = false,
					CreatedByUserId = userId,
					CipherTags = new List<CipherTag>(),
					HintsRequested = new List<HintRequest>()
				};
			}
			else if (model.CipherDefinition == CipherDefinition.ImageCipher)
			{
				ValidateImageFile(model.Image);
				try
				{
					var result = await ocrService.ExtractTextFromImageAsync(model.Image);
					encryptedTextForAnalysis = result.ExtractedText; // Use OCR result for analysis

					string imageFolderPath = Path.GetFullPath(Path.Combine(
					AppContext.BaseDirectory, "..", "..", "..", "..", "Images"));
					Directory.CreateDirectory(imageFolderPath);
					string originalExtension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();
					string safeTitle = MakeSafeFilename(model.Title);
					string imageFilePath = Path.Combine(imageFolderPath, safeTitle + originalExtension);

					using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
					{
						using (var imageStream = model.Image.OpenReadStream())
						{
							await imageStream.CopyToAsync(fileStream);
						}
					}
					string relativePath = Path.Combine("Images", safeTitle + originalExtension);
					cipher = new ImageCipher()
					{
						Title = title,
						DecryptedText = model.DecryptedText,
						ImagePath = relativePath,
						AllowHint = false,
						AllowSolution = false,
						IsApproved = false,
						CreatedByUserId = userId,
						CipherTags = new List<CipherTag>(),
						HintsRequested = new List<HintRequest>(),
						EncryptedText = result.ExtractedText,
						OCRConfidence = result.Confidence,
					};
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException($"Failed to process image cipher: {ex.Message}", ex);
				}
			}

			var mlResult = await cipherRecognizerService.ClassifyCipher(encryptedTextForAnalysis);

			if (mlResult.TopPrediction.Type.ToLower() == "plaintext")
			{
				throw new InvalidOperationException(
					"Your text appears to already be in plaintext. Only encrypted text is allowed.");
			}

			cipher.MLPrediction = JsonSerializer.Serialize(new
			{
				family = mlResult.TopPrediction.Family,
				type = mlResult.TopPrediction.Type,
				confidence = mlResult.TopPrediction.Confidence,
				allPredictions = mlResult.AllPredictions.Select(p => new
				{
					family = p.Family,
					type = p.Type,
					confidence = p.Confidence
				}).ToList()
			});

			if (!string.IsNullOrWhiteSpace(model.DecryptedText))
			{
				cipher.IsPlaintextValid = await englishValidationService.IsLikelyEnglishAsync(model.DecryptedText);
			}

			cipher.IsLLMRecommended = DetermineLLMRecommendation(
				mlConfidence: mlResult.TopPrediction.Confidence * 100,
				mlType: mlResult.TopPrediction.Type,
				userProvidedType: model.CipherType != CipherType.None,
				userProvidedSolution: !string.IsNullOrWhiteSpace(model.DecryptedText),
				isPlaintextValid: cipher.IsPlaintextValid,
				typesMatch: model.CipherType.ToString().ToLower() == mlResult.TopPrediction.Type.ToLower()
			);

			await cipherRepo.AddAsync(cipher);
			return cipher;
		}
		public async Task SuggestAnswerAsync (SuggestAnswerDTO dto, string userId, int cipherId)
		{
			Cipher? cipher = cipherRepo.GetAllAttached()
				.Include(x => x.AnswerSuggestions)
				.FirstOrDefault(x => x.Id == cipherId);

			if (cipher == null)
				throw new InvalidOperationException("Cipher not found");

			if (!string.IsNullOrWhiteSpace(cipher.DecryptedText))
				throw new InvalidOperationException("Cipher already has an answer");

			if (cipher.ChallengeType == ChallengeType.Standard)
				throw new InvalidOperationException("Cannot suggest answer on standard cipher");

			if (cipher.CreatedByUserId == userId)
				throw new InvalidOperationException("You cannot suggest answers on ciphers created by you.");

			if (cipher.AnswerSuggestions.FirstOrDefault(x => x.UserId == userId) != null)
				throw new InvalidOperationException("You have already suggested an answer for this cipher");

			AnswerSuggestion answer = new AnswerSuggestion
			{
				UserId = userId,
				CipherId = cipherId,
				Description = dto.Description,
				DecryptedText = dto.DecryptedText,
				UplodaedTime = DateTime.UtcNow,
				IsApproved = false
			};

			await answerRepo.AddAsync(answer);
		}
		public async Task<bool> AnswerCipherAsync(string userId, string input, int cipherId)
		{
			Cipher? cipher = cipherRepo.GetAllAttached()
				.Include(x => x.UsersSolved)
				.FirstOrDefault(x => x.Id == cipherId);

			if (cipher == null) 
				throw new InvalidOperationException("Cipher not found");

			if (cipher.CreatedByUserId == userId)
				throw new InvalidOperationException("A cipher cannot be solved by it's user");

			if (cipher.ChallengeType == ChallengeType.Experimental)
				throw new InvalidOperationException("Experimental ciphers cannot be solved");

			if (cipher.UsersSolved.FirstOrDefault(x => x.UserId == userId) != null)
				throw new InvalidOperationException("Cannot solve the same cipher 2 times");

			string correctAnswer = cipher.DecryptedText;

			if (correctAnswer == input) //The answer is correct
			{
				ApplicationUser user = await userRepository.FindByIdAsync(userId);
				await solutionRepo.AddAsync(new UserSolution()
				{
					CipherId = cipherId,
					UserId = userId,
					TimeSolved = DateTime.Now,
				});
				user.Score += cipher.Points;
				user.SolvedCount += 1;
				return true;
			}

			return false;
		}
		public Task<HintRequestResponse> RequestHintAsync(HintRequest request)
		{
			throw new NotImplementedException();
		}

		#region Private methods
		private string MakeSafeFilename(string name)
		{
			foreach (char c in Path.GetInvalidFileNameChars())
			{
				name = name.Replace(c, '_');
			}
			return name;
		}
		private async Task<CipherOutputViewModel> ToOutputViewModel(Cipher cipher)
		{
			var model = new CipherOutputViewModel
			{
				Id = cipher.Id,
				Title = cipher.Title,
				CipherText = cipher.EncryptedText,
				Points = cipher.Points,
				AllowsAnswer = cipher.AllowSolution,
				AllowsHint = cipher.AllowHint,
				ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
			};
			if (cipher is ImageCipher)
			{
				ImageCipher cipherImage = cipher as ImageCipher;
				string imageFolderPath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), cipherImage.ImagePath);
				string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";
				//How are we going to pass the image?
				model.IsImage = true;
			}
			else
				model.IsImage = true;

			return model;
		}
		private void ValidateImageFile(IFormFile imageFile)
		{
			if (imageFile == null)
				throw new InvalidOperationException("Image file is required for image ciphers");

			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
			var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

			if (!allowedExtensions.Contains(extension))
				throw new InvalidOperationException($"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}");

			// File size validation (e.g., max 5MB)
			const int maxSizeInBytes = 5 * 1024 * 1024;
			if (imageFile.Length > maxSizeInBytes)
				throw new InvalidOperationException("File size cannot exceed 5MB");

			if (imageFile.Length == 0)
				throw new InvalidOperationException("File cannot be empty");
		}
		private bool DetermineLLMRecommendation(
			double mlConfidence,
			string mlType,
			bool userProvidedType,
			bool userProvidedSolution,
			bool isPlaintextValid,
			bool typesMatch)
		{
			if (!userProvidedType && !userProvidedSolution)
			{
				throw new InvalidOperationException(
					"Incomplete submission: Please provide cipher type and/or solution.");
			}

			bool isProblematic = ProblematicCipherTypes.Contains(mlType.ToLower());

			if (userProvidedType && userProvidedSolution)
			{
				if (mlConfidence > 85 && typesMatch && isPlaintextValid)
				{
					return false;
				}
				return true;
			}

			if (userProvidedType && !userProvidedSolution)
			{
				return true;
			}

			if (!userProvidedType && userProvidedSolution)
			{
				if (mlConfidence > 85 && !isProblematic && isPlaintextValid)
				{
					return false;
				}
				return true;
			}

			return true;
		}
		#endregion
		private static readonly HashSet<string> ProblematicCipherTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"columnar",
			"railfence",
			"vigenere",
			"trithemius",
			"route"
		};
	}
}
