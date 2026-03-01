using Cryptomind.Common.Constants;
using Cryptomind.Common.Enums;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.Helpers;
using Cryptomind.Common.ViewModels.CipherSubmissionViewModels;
using Cryptomind.Common.ViewModels.CipherViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using CustomValidationException = Cryptomind.Common.Exceptions.CustomValidationException;

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
			if (string.IsNullOrEmpty(model.Title))
				throw new CustomValidationException(CipherErrorConstants.TitleRequiredMessage);

			if (await cipherRepo.GetAllAttached().AnyAsync(x => x.Title == model.Title && !x.IsDeleted))
				throw new ConflictException(CipherErrorConstants.DuplicateTitleMessage);

			if (await cipherRepo.GetAllAttached().AnyAsync(x => x.EncryptedText == model.EncryptedText))
				throw new ConflictException(CipherErrorConstants.DuplicateCipherContent);

			if (string.IsNullOrWhiteSpace(model.DecryptedText) && model.CipherType == null)
				throw new ConflictException(CipherErrorConstants.UnknownCipherSolutionConflict);

			if (model.EncryptedText != null && model.EncryptedText.Length >= 450)
				throw new CustomValidationException(CipherErrorConstants.MaxLengthExceeded);

			Cipher? cipher = null;
			string? encryptedTextForAnalysis = model.EncryptedText;

			if (model.CipherDefinition == CipherDefinition.TextCipher)
			{
				cipher = new TextCipher()
				{
					Title = model.Title,
					DecryptedText = model.DecryptedText,
					EncryptedText = model.EncryptedText,
					TypeOfCipher = model.CipherType,
					AllowHint = false,
					AllowSolution = false,
					Status = ApprovalStatus.Pending,
					CreatedByUserId = userId,
					CipherTags = new List<CipherTag>(),
					HintsRequested = new List<HintRequest>(),
					CreatedAt = DateTime.UtcNow.AddHours(2),
				};
			}
			else if (model.CipherDefinition == CipherDefinition.ImageCipher)
			{
				ValidateImageFile(model.Image);

				string finalText;
				double? ocrConfidence = null;

				if (!string.IsNullOrWhiteSpace(model.ReviewedText))
				{
					finalText = model.ReviewedText;
				}
				else
				{
					var result = await ocrService.ExtractTextFromImageAsync(model.Image);
					if (string.IsNullOrWhiteSpace(result.ExtractedText))
						throw new CustomValidationException(CipherErrorConstants.OCRFailedMessage);
					finalText = result.ExtractedText;
					ocrConfidence = result.Confidence;
				}

				encryptedTextForAnalysis = finalText;

				string imageFolderPath = Path.Combine(PathHelper.GetImagesBasePath(), "Ciphers");
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

				string relativePath = Path.Combine("Ciphers", safeTitle + originalExtension);

				if ((await cipherRepo.GetAllAsync()).FirstOrDefault(x => x.EncryptedText == finalText) != null)
					throw new ConflictException(CipherErrorConstants.DuplicateCipherContent);

				cipher = new ImageCipher()
				{
					Title = model.Title,
					DecryptedText = model.DecryptedText,
					TypeOfCipher = model.CipherType,
					ImagePath = relativePath,
					AllowHint = false,
					AllowSolution = false,
					Status = ApprovalStatus.Pending,
					CreatedByUserId = userId,
					CipherTags = new List<CipherTag>(),
					HintsRequested = new List<HintRequest>(),
					EncryptedText = finalText,
					OCRConfidence = ocrConfidence ?? 0,
					CreatedAt = DateTime.UtcNow.AddHours(2)
				};

			}

			var mlResult = await cipherRecognizerService.ClassifyCipher(encryptedTextForAnalysis);

			if (mlResult.TopPrediction.Type.ToLower() == "plaintext")
			{
				throw new CustomValidationException(CipherErrorConstants.PlaintextNotAllowed);
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
				typesMatch: model.CipherType?.ToString().ToLower() == mlResult.TopPrediction.Type.ToLower()
			);

			await cipherRepo.AddAsync(cipher);
			return cipher;
		}
		public async Task<List<CipherSubmissionViewModel>> SubmittedCiphers(string userId)
		{
			var ciphers = await cipherRepo.GetAllAttached()
				.Include(x => x.UserSolutions)
				.Include(x => x.CipherTags)
				.Where(x => x.CreatedByUserId == userId)
				.ToListAsync();

			if (ciphers.Count == 0)
				return new List<CipherSubmissionViewModel>();

			var models = new List<CipherSubmissionViewModel>();
			foreach (var cipher in ciphers)
			{
				var model = new CipherSubmissionViewModel()
				{
					Id = cipher.Id,
					Title = cipher.Title,
					CipherText = cipher.EncryptedText,
					SubmittedTime = cipher.CreatedAt.ToString("ddd, dd MMM yyyy h:mm"),
					Status = cipher.Status.ToString(),
				};

				if (cipher.IsDeleted)
				{
					model.Status = "CipherDeleted";
					model.DeletedTime = cipher.DeletedAt?.ToString("ddd, dd MMM yyyy h:mm");
				}
				else if (cipher.Status == ApprovalStatus.Approved)
				{
					model.ApprovedTime = cipher.ApprovedAt?.ToString("ddd, dd MMM yyyy h:mm");
					model.ApprovedAs = cipher.ChallengeType.ToString();
					model.AssignedTags = cipher.CipherTags.Select(x => x.Tag).ToList();
					model.SolvedByCount = cipher.UserSolutions.Count(x => x.IsCorrect);

				}
				else if (cipher.Status == ApprovalStatus.Rejected)
				{
					model.RejectionTime = cipher.RejectedAt?.ToString("ddd, dd MMM yyyy h:mm");
					model.RejectionReason = cipher.RejectionReason;
				}

				models.Add(model);
			}

			return models;
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
				throw new CustomValidationException(CipherErrorConstants.ImageRequired);

			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
			var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

			if (!allowedExtensions.Contains(extension))
				throw new CustomValidationException(CipherErrorConstants.InvalidFileType);

			const int maxSizeInBytes = 5 * 1024 * 1024;
			if (imageFile.Length == 0)
				throw new CustomValidationException(CipherErrorConstants.EmptyFileError);

			if (imageFile.Length > maxSizeInBytes)
				throw new CustomValidationException(CipherErrorConstants.FileTooLarge);

		}
		private bool DetermineLLMRecommendation(
			double mlConfidence,
			string mlType,
			bool userProvidedType,
			bool userProvidedSolution,
			bool isPlaintextValid,
			bool typesMatch)
		{
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
