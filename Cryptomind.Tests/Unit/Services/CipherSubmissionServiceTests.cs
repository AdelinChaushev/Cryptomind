using Cryptomind.Common.Enums;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Common.ViewModels.CipherViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Unit.Services
{
	public class CipherSubmissionServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> cipherRepoMock = new();
		private readonly Mock<IOCRService> ocrServiceMock = new();
		private readonly Mock<ICipherRecognizerService> cipherRecognizerMock = new();
		private readonly Mock<IEnglishValidationService> englishValidationMock = new();
		private readonly CipherSubmissionService service;

		public CipherSubmissionServiceTests()
		{
			service = new CipherSubmissionService(
				cipherRepoMock.Object,
				ocrServiceMock.Object,
				cipherRecognizerMock.Object,
				englishValidationMock.Object);

			cipherRepoMock.Setup(r => r.AddAsync(It.IsAny<Cipher>()))
				.Returns(Task.CompletedTask);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher>().AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher>());

			englishValidationMock.Setup(e => e.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()))
				.ReturnsAsync(true);

			SetupMlResult("Caesar", "Substitution", 0.95);
		}

		private void SetupMlResult(string type, string family, double confidence)
		{
			cipherRecognizerMock.Setup(m => m.ClassifyCipher(It.IsAny<string>()))
				.ReturnsAsync(new CipherRecognitionResultViewModel
				{
					TopPrediction = new PredictionViewModel
					{
						Type = type,
						Family = family,
						Confidence = confidence,
					},
					AllPredictions = new List<PredictionViewModel>
					{
						new PredictionViewModel { Type = type, Family = family, Confidence = confidence }
					}
				});
		}

		private static SubmitCipherViewModel TextModel(
			string title = "My Cipher",
			string encryptedText = "KHOOR ZRUOG",
			string? decryptedText = "hello world",
			CipherType? cipherType = CipherType.Caesar) => new()
			{
				Title = title,
				EncryptedText = encryptedText,
				DecryptedText = decryptedText,
				CipherType = cipherType,
				CipherDefinition = CipherDefinition.TextCipher,
			};

		private void SetupAttachedCiphers(params Cipher[] ciphers)
		{
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(ciphers.AsQueryable().BuildMock());
		}

		private static TextCipher MakeCipher(
			int id = 1,
			string title = "Test",
			string userId = "u1",
			ApprovalStatus status = ApprovalStatus.Pending,
			bool isDeleted = false,
			ChallengeType challengeType = ChallengeType.Standard,
			string encryptedText = "encrypted") => new()
			{
				Id = id,
				Title = title,
				CreatedByUserId = userId,
				Status = status,
				IsDeleted = isDeleted,
				ChallengeType = challengeType,
				EncryptedText = encryptedText,
				MLPrediction = "",
				CipherTags = new List<CipherTag>(),
				UserSolutions = new List<UserSolution>(),
				AnswerSuggestions = new List<AnswerSuggestion>(),
				HintsRequested = new List<HintRequest>(),
				CreatedAt = DateTime.UtcNow,
			};

		#region SubmitCipherAsync - Validation

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenTitleIsEmpty()
		{
			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.SubmitCipherAsync(TextModel(title: ""), "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenTitleAlreadyExists()
		{
			SetupAttachedCiphers(MakeCipher(1, title: "My Cipher"));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SubmitCipherAsync(TextModel(title: "My Cipher"), "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenEncryptedTextAlreadyExists()
		{
			SetupAttachedCiphers(MakeCipher(1, encryptedText: "KHOOR ZRUOG"));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SubmitCipherAsync(TextModel(encryptedText: "KHOOR ZRUOG"), "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenBothDecryptedTextAndTypeAreNull()
		{
			await Assert.ThrowsAsync<ConflictException>(
				() => service.SubmitCipherAsync(
					TextModel(decryptedText: null, cipherType: null), "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenEncryptedTextExceeds450Characters()
		{
			var longText = new string('A', 451);

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.SubmitCipherAsync(TextModel(encryptedText: longText), "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenMLReturnsPlaintext()
		{
			SetupMlResult("plaintext", "Plaintext", 0.99);

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.SubmitCipherAsync(TextModel(), "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenMLReturnsPlaintext_CaseInsensitive()
		{
			SetupMlResult("Plaintext", "Plaintext", 0.99);

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.SubmitCipherAsync(TextModel(), "u1"));
		}

		#endregion

		#region SubmitCipherAsync - TextCipher creation

		[Fact]
		public async Task SubmitCipherAsync_ReturnsCipher_WhenTextCipherIsValid()
		{
			var result = await service.SubmitCipherAsync(TextModel(), "u1");

			Assert.NotNull(result);
			Assert.IsType<TextCipher>(result);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsCorrectFields_OnTextCipher()
		{
			var result = await service.SubmitCipherAsync(TextModel(
				title: "My Cipher",
				encryptedText: "KHOOR ZRUOG",
				decryptedText: "hello world",
				cipherType: CipherType.Caesar), "u1");

			Assert.Equal("My Cipher", result.Title);
			Assert.Equal("KHOOR ZRUOG", result.EncryptedText);
			Assert.Equal("hello world", result.DecryptedText);
			Assert.Equal(ApprovalStatus.Pending, result.Status);
			Assert.Equal("u1", result.CreatedByUserId);
			Assert.Equal(CipherType.Caesar, result.TypeOfCipher);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsPendingStatus_OnNewCipher()
		{
			var result = await service.SubmitCipherAsync(TextModel(), "u1");

			Assert.Equal(ApprovalStatus.Pending, result.Status);
		}

		[Fact]
		public async Task SubmitCipherAsync_SerializesMLPrediction_OnCipher()
		{
			var result = await service.SubmitCipherAsync(TextModel(), "u1");

			Assert.False(string.IsNullOrEmpty(result.MLPrediction));
			Assert.Contains("Caesar", result.MLPrediction);
		}

		[Fact]
		public async Task SubmitCipherAsync_CallsAddAsync_OnCipherRepo()
		{
			await service.SubmitCipherAsync(TextModel(), "u1");

			cipherRepoMock.Verify(r => r.AddAsync(It.IsAny<Cipher>()), Times.Once);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsIsPlaintextValid_WhenDecryptedTextProvided()
		{
			englishValidationMock.Setup(e => e.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()))
				.ReturnsAsync(true);

			var result = await service.SubmitCipherAsync(TextModel(decryptedText: "hello world"), "u1");

			Assert.True(result.IsPlaintextValid);
		}

		[Fact]
		public async Task SubmitCipherAsync_DoesNotCallEnglishValidation_WhenNoDecryptedText()
		{
			await service.SubmitCipherAsync(TextModel(decryptedText: null, cipherType: CipherType.Caesar), "u1");

			englishValidationMock.Verify(e => e.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()), Times.Never);
		}

		#endregion

		#region SubmitCipherAsync - LLM recommendation logic

		[Fact]
		public async Task SubmitCipherAsync_SetsLLMRecommendedFalse_WhenHighConfidenceTypesMatchAndPlaintextValid()
		{
			// userProvidedType=true, userProvidedSolution=true, mlConfidence>85, typesMatch=true, isPlaintextValid=true
			SetupMlResult("Caesar", "Substitution", 0.95);
			englishValidationMock.Setup(e => e.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()))
				.ReturnsAsync(true);
			var result = await service.SubmitCipherAsync(
				TextModel(
					decryptedText: "hello world",
					cipherType: CipherType.Caesar,
					encryptedText: "encryptedText: \"WKLV LV D ORQJ FDHVDU FLSKHU WHAW XVHG IRU WHVWLQJ FODVVLILFDWLRQ DQG WUDLQLQJ PDFKLQH OHDUQLQJ PRGHOV LQ D FLSKHU VROYHU DSSOLFDWLRQ WKH JRDO LV WR HQVXUH WKH WHAW OHQJWK LV VXIILFLHQW IRU VWDEOH SUHGLFWLRQV\""
				), "u1");
			Assert.False(result.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsLLMRecommendedTrue_WhenTypesDoNotMatch()
		{
			// userProvidedType=true, userProvidedSolution=true, but typesMatch=false
			SetupMlResult("Vigenere", "Polyalphabetic", 0.95);

			var result = await service.SubmitCipherAsync(
				TextModel(decryptedText: "hello world", cipherType: CipherType.Caesar), "u1");

			Assert.True(result.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsLLMRecommendedTrue_WhenTypeProvidedButNoSolution()
		{
			// userProvidedType=true, userProvidedSolution=false → always true
			var result = await service.SubmitCipherAsync(
				TextModel(decryptedText: null, cipherType: CipherType.Caesar), "u1");

			Assert.True(result.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsLLMRecommendedTrue_WhenNoTypeAndNoSolution()
		{
			// Both null → ConflictException, but this case is caught by earlier validation
			// Instead test: no type provided, solution provided, low confidence
			SetupMlResult("Caesar", "Substitution", 0.70);

			var result = await service.SubmitCipherAsync(
				TextModel(decryptedText: "hello world", cipherType: null), "u1");

			Assert.True(result.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsLLMRecommendedFalse_WhenNoTypeHighConfidenceNotProblematicAndPlaintextValid()
		{
			// userProvidedType=false, userProvidedSolution=true, mlConfidence>85, not problematic type, isPlaintextValid=true
			SetupMlResult("Caesar", "Substitution", 0.95);
			englishValidationMock.Setup(e => e.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()))
				.ReturnsAsync(true);

			var result = await service.SubmitCipherAsync(
				TextModel(
					decryptedText: "hello world",
					cipherType: null,
					encryptedText: "encryptedText: \"WKLV LV D ORQJ FDHVDU FLSKHU WHAW XVHG IRU WHVWLQJ FODVVLILFDWLRQ DQG WUDLQLQJ PDFKLQH OHDUQLQJ PRGHOV LQ D FLSKHU VROYHU DSSOLFDWLRQ WKH JRDO LV WR HQVXUH WKH WHAW OHQJWK LV VXIILFLHQW IRU VWDEOH SUHGLFWLRQV\""
				), "u1");

			Assert.False(result.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsLLMRecommendedTrue_WhenTypeIsProblematic()
		{
			// Vigenere is in ProblematicCipherTypes
			SetupMlResult("vigenere", "Polyalphabetic", 0.95);
			englishValidationMock.Setup(e => e.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()))
					.ReturnsAsync(true);

			var result = await service.SubmitCipherAsync(
				TextModel(decryptedText: "hello world", cipherType: null), "u1");

			Assert.True(result.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsLLMRecommendedTrue_WhenPlaintextIsInvalid()
		{
			SetupMlResult("Caesar", "Substitution", 0.95);
			englishValidationMock.Setup(e => e.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()))
				.ReturnsAsync(false);

			var result = await service.SubmitCipherAsync(
				TextModel(decryptedText: "hello world", cipherType: null), "u1");

			Assert.True(result.IsLLMRecommended);
		}

		#endregion

		#region SubmittedCiphers

		[Fact]
		public async Task SubmittedCiphers_ReturnsEmptyList_WhenUserHasNoCiphers()
		{
			SetupAttachedCiphers();

			var result = await service.SubmittedCiphers("u1");

			Assert.Empty(result);
		}

		[Fact]
		public async Task SubmittedCiphers_ReturnsOnlyCiphers_ForGivenUser()
		{
			SetupAttachedCiphers(
				MakeCipher(1, userId: "u1"),
				MakeCipher(2, userId: "u2"));

			var result = await service.SubmittedCiphers("u1");

			Assert.Single(result);
		}

		[Fact]
		public async Task SubmittedCiphers_SetsStatusToCipherDeleted_WhenIsDeleted()
		{
			var cipher = MakeCipher(1, userId: "u1", isDeleted: true);
			cipher.DeletedAt = DateTime.UtcNow;
			SetupAttachedCiphers(cipher);

			var result = await service.SubmittedCiphers("u1");

			Assert.Equal("CipherDeleted", result[0].Status);
			Assert.NotNull(result[0].DeletedTime);
		}

		[Fact]
		public async Task SubmittedCiphers_SetsApprovedFields_WhenStatusIsApproved()
		{
			var cipher = MakeCipher(1, userId: "u1", status: ApprovalStatus.Approved,
				challengeType: ChallengeType.Standard);
			cipher.ApprovedAt = DateTime.UtcNow;
			cipher.UserSolutions = new List<UserSolution>
			{
				new UserSolution { IsCorrect = true },
				new UserSolution { IsCorrect = false },
			};
			SetupAttachedCiphers(cipher);

			var result = await service.SubmittedCiphers("u1");

			Assert.Equal("Approved", result[0].Status);
			Assert.NotNull(result[0].ApprovedTime);
			Assert.Equal("Standard", result[0].ApprovedAs);
			Assert.Equal(1, result[0].SolvedByCount);
		}

		[Fact]
		public async Task SubmittedCiphers_SetsRejectionFields_WhenStatusIsRejected()
		{
			var cipher = MakeCipher(1, userId: "u1", status: ApprovalStatus.Rejected);
			cipher.RejectedAt = DateTime.UtcNow;
			cipher.RejectionReason = "low quality";
			SetupAttachedCiphers(cipher);

			var result = await service.SubmittedCiphers("u1");

			Assert.Equal("Rejected", result[0].Status);
			Assert.NotNull(result[0].RejectionTime);
			Assert.Equal("low quality", result[0].RejectionReason);
		}

		[Fact]
		public async Task SubmittedCiphers_SetsDeletedStatus_EvenWhenApproved()
		{
			// IsDeleted check takes priority over status in the service
			var cipher = MakeCipher(1, userId: "u1",
				status: ApprovalStatus.Approved, isDeleted: true);
			cipher.DeletedAt = DateTime.UtcNow;
			SetupAttachedCiphers(cipher);

			var result = await service.SubmittedCiphers("u1");

			Assert.Equal("CipherDeleted", result[0].Status);
		}

		[Fact]
		public async Task SubmittedCiphers_ReturnsPendingStatus_ForPendingCipher()
		{
			SetupAttachedCiphers(MakeCipher(1, userId: "u1", status: ApprovalStatus.Pending));

			var result = await service.SubmittedCiphers("u1");

			Assert.Equal("Pending", result[0].Status);
		}

		#endregion
	}
}