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
		IRepository<UserSolution, int> solutionRepository,
		IOCRService ocrService,
		ICipherRecognizerService cipherRecognizerService,
		ILLMService llmService,
		IEnglishValidationService englishValidationService,
		UserManager<ApplicationUser> userRepository) : ICipherService
	{
		public async Task<bool> AnswerCipherAsync(string userId, string input, int cipherId)
		{
			Cipher cipher = cipherRepo.GetAllAttached()
				.Include(x => x.UsersSolved)
				.FirstOrDefault(x => x.Id == cipherId);

			if (cipher == null) 
				throw new InvalidOperationException("There is no cipher with the given Id");

			if (cipher.CreatedByUserId == userId)
				throw new InvalidOperationException("A cipher cannot be solved by it's user");

			if (cipher.ChallengeType == ChallengeType.Experimental)
				throw new InvalidOperationException("Experimental ciphers cannot be solved");

			if (cipher.UsersSolved.FirstOrDefault(x => x.UserId == userId) != null)
				throw new InvalidOperationException("Cannot solve the same cipher 2 times");

			string correctAnswer = string.Empty;
			if (cipher is TextCipher)
				correctAnswer = (cipher as TextCipher).DecryptedText;
			else if (cipher is ImageCipher)
				correctAnswer = (cipher as ImageCipher).DecryptedText;

			if (correctAnswer == input) //The answer is correct
			{
				ApplicationUser user = await userRepository.FindByIdAsync(userId);
				await solutionRepository.AddAsync(new UserSolution()
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
		public async Task<List<CipherOutputViewModel>> GetApprovedAsync(CipherFilter? filter)
		{
			List<Cipher> approved = (await cipherRepo.GetAllAsync()).Where(c => c.IsApproved).ToList();

			if (!string.IsNullOrEmpty(filter.SearchTerm))
				approved = approved.Where(c => c.Title.Contains(filter.SearchTerm)).ToList();

			if (filter.Tags != null)
				approved = approved.Where(c => c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type))).ToList();

			switch (filter.ChallengeType)
			{
				case ChallengeTypeDTO.None:
					approved = approved;
					break;
				case ChallengeTypeDTO.Standard:
					approved = approved.Where(x => x.ChallengeType == ChallengeType.Standard).ToList();
					break;
				case ChallengeTypeDTO.Experimental:
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
				throw new InvalidOperationException("There is no cipher with the given Id");

			return await ToOutputViewModel(cipher);
		}
		public Task<HintRequestResponse> RequestHintAsync(HintRequest request)
		{
			throw new NotImplementedException();
		}
		public async Task<Cipher> SubmitCipherAsync(SubmitCipherViewModel model, string userId)
		{
			if (await cipherRepo.GetAllAttached().AnyAsync(c => c.Title == model.Title))
				throw new InvalidOperationException("Cannot create two ciphers with the same name");

			Cipher? cipher = null;
			string encryptedTextForAnalysis = model.EncryptedText;

			var title = string.IsNullOrEmpty(model.Title) ? model.EncryptedText : model.Title;
			if (model.CipherDefinition == CipherDefinition.TextCipher)
			{
				cipher = new TextCipher()
				{
					Title = title,
					DecryptedText = model.DecryptedText,
					EncryptedText = model.EncryptedText,
					//TypeOfCipher = model.Type,
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

			//Assign the ML prediction to the property so later the admin can use it.
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
				cipher.IsPlaintextValid = await englishValidationService.IsLikelyEnglishAsync(model.DecryptedText);

			//TESTING:
			if (mlResult.TopPrediction.Type.ToLower() == "plaintext")
			{
				//You have to show this to the USER!!!
				throw new InvalidOperationException("PLAINTEXT ENCRYPTED TEXT IS NOT ALLOWED");
			}

			var mlConfidence = mlResult.TopPrediction.Confidence;
			var mlType = mlResult.TopPrediction.Type;
			var userProvidedType = model.Type != CipherTypeDTO.None;
			var userProvidedSolution = !string.IsNullOrWhiteSpace(model.DecryptedText);

			bool LLMRecommendation = false;

			if (userProvidedType && userProvidedSolution)
			{
				bool typesMatch = mlType.ToLower() == model.Type.ToString().ToLower();
				bool isProblematicType = ProblematicCipherTypes.Contains(mlType.ToLower());

				if (mlConfidence > 85 && typesMatch)
				{
					LLMRecommendation = false;

					if (cipher.IsPlaintextValid == false)
					{
						LLMRecommendation = true;
					}
				}
				else
				{
					LLMRecommendation = true;
				}
			}
			else if (userProvidedType && !userProvidedSolution)
			{
				LLMRecommendation = true;
			}
			else if (!userProvidedType && userProvidedSolution)
			{
				bool isProblematicType = ProblematicCipherTypes.Contains(mlType);

				if (mlConfidence > 85 && !isProblematicType)
				{
					LLMRecommendation = false;
					cipher.TypeOfCipher = (CipherType)Enum.Parse(typeof(CipherType), mlType);

					if (cipher.IsPlaintextValid == false)
					{
						LLMRecommendation = true;
					}
				}
				else
				{
					LLMRecommendation = true;
				}
			}
			else if (!userProvidedType && !userProvidedSolution)
			{
				throw new InvalidOperationException("Incomplete submission: Please provide cipher type and/or solution.");
			}

			cipher.IsLLMRecommended = LLMRecommendation;
			await cipherRepo.AddAsync(cipher);
			return cipher;
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
			if (cipher is ImageCipher)
			{
				ImageCipher cipherImage = cipher as ImageCipher;
				string imageFolderPath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), cipherImage.ImagePath);
				string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";

				return (new CipherOutputViewModel
				{
					Id = cipher.Id,
					Title = cipher.Title,
					IsApproved = cipher.IsApproved,
					Points = cipher.Points,
					CipherText = base64,
					AllowsAnswer = cipher.AllowSolution,
					AllowsHint = cipher.AllowHint,
					IsImage = true,
					ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				});
			}
			else
			{
				TextCipher cipherText = cipher as TextCipher;
				return (new CipherOutputViewModel
				{
					Id = cipher.Id,
					Title = cipher.Title,
					IsApproved = cipher.IsApproved,
					CipherText = cipherText.EncryptedText,
					Points = cipher.Points,
					AllowsAnswer = cipher.AllowSolution,
					AllowsHint = cipher.AllowHint,
					IsImage = false,
					ChallengeTypeDisplay = cipher.ChallengeType.ToString(),
				});
			}
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
		#endregion
		private static readonly HashSet<string> ProblematicCipherTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"columnar",
			"railFence",
			"vigenere",
			"trithemius",
			"route"
		};
	}
}
