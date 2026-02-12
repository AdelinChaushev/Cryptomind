using Cryptomind.Common.Enums;
using Cryptomind.Common.ViewModels.CipherViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services.OCR;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class CipherSubmissionService(
		IRepository<Cipher, int> cipherRepo,
		IOCRService ocrService,
		ICipherRecognizerService cipherRecognizerService,
		IEnglishValidationService englishValidationService) : ICipherSubmissionService
	{
		public async Task<Cipher> SubmitCipherAsync(SubmitCipherViewModel model, string userId)
		{
			if ((await cipherRepo.GetAllAsync()).FirstOrDefault(x => x.Title == model.Title) != null)
				throw new InvalidOperationException("There is already a cipher with this name");

			if ((await cipherRepo.GetAllAsync()).FirstOrDefault(x => x.EncryptedText == model.EncryptedText) != null)
				throw new InvalidOperationException("There is already a cipher like this");

			if (string.IsNullOrEmpty(model.DecryptedText) && model.CipherType == null)
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
					Status = ApprovalStatus.Pending,
					CreatedByUserId = userId,
					CipherTags = new List<CipherTag>(),
					HintsRequested = new List<HintRequest>(),
					CreatedAt = DateTime.UtcNow
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
						Status = ApprovalStatus.Pending,
						CreatedByUserId = userId,
						CipherTags = new List<CipherTag>(),
						HintsRequested = new List<HintRequest>(),
						EncryptedText = result.ExtractedText,
						OCRConfidence = result.Confidence,
						CreatedAt = DateTime.UtcNow
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
				throw new InvalidOperationException("Your text appears to already be in plaintext. Only encrypted text is allowed.");
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
				userProvidedType: model.CipherType != null,
				userProvidedSolution: !string.IsNullOrWhiteSpace(model.DecryptedText),
				isPlaintextValid: cipher.IsPlaintextValid,
				typesMatch: model.CipherType.ToString().ToLower() == mlResult.TopPrediction.Type.ToLower()
			);

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
