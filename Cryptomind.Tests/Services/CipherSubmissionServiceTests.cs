using Cryptomind.Common.Enums;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Common.ViewModels.CipherSubmissionViewModels;
using Cryptomind.Common.ViewModels.CipherViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Services
{
	public class CipherSubmissionServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> _cipherRepoMock = new();
		private readonly Mock<IOCRService> _ocrServiceMock = new();
		private readonly Mock<ICipherRecognizerService> _cipherRecognizerMock = new();
		private readonly Mock<IEnglishValidationService> _englishValidationMock = new();
		private readonly CipherSubmissionService _service;

		public CipherSubmissionServiceTests()
		{
			_service = new CipherSubmissionService(
				_cipherRepoMock.Object,
				_ocrServiceMock.Object,
				_cipherRecognizerMock.Object,
				_englishValidationMock.Object);

			_cipherRepoMock.Setup(r => r.AddAsync(It.IsAny<Cipher>()))
				.Returns(Task.CompletedTask);

			_cipherRecognizerMock.Setup(s => s.ClassifyCipher(It.IsAny<string>()))
				.ReturnsAsync(new CipherRecognitionResultViewModel
				{
					TopPrediction = new PredictionViewModel
					{
						Family = "Substitution",
						Type = "Caesar",
						Confidence = 0.95
					},
					AllPredictions = new List<PredictionViewModel>()
				});

			_englishValidationMock.Setup(s => s.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()))
				.ReturnsAsync(true);
		}

		private void SetupAttachedCiphers(params Cipher[] ciphers)
		{
			var mock = new List<Cipher>(ciphers).AsQueryable().BuildMock();
			_cipherRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private static SubmitCipherViewModel MakeTextCipherModel(
			string title = "Test Cipher",
			string encryptedText = "KHOOR",
			string? decryptedText = "HELLO",
			CipherType? cipherType = CipherType.Caesar) => new()
			{
				Title = title,
				EncryptedText = encryptedText,
				DecryptedText = decryptedText,
				CipherType = cipherType,
				CipherDefinition = CipherDefinition.TextCipher,
			};

		private static Mock<IFormFile> CreateMockImageFile(
			string fileName = "test.jpg",
			long length = 1024,
			string contentType = "image/jpeg")
		{
			var mock = new Mock<IFormFile>();
			mock.Setup(f => f.FileName).Returns(fileName);
			mock.Setup(f => f.Length).Returns(length);
			mock.Setup(f => f.ContentType).Returns(contentType);
			mock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("fake image data")));
			return mock;
		}

		#region SubmitCipherAsync - Guard Clauses

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenTitleIsEmpty()
		{
			var model = MakeTextCipherModel(title: "");

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenTitleAlreadyExists()
		{
			SetupAttachedCiphers(new TextCipher { Title = "Existing Title", CreatedByUserId = "u2" });
			var model = MakeTextCipherModel(title: "Existing Title");

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenTitleAlreadyExists_EvenFromSameUser()
		{
			SetupAttachedCiphers(new TextCipher { Title = "My Title", CreatedByUserId = "u1" });
			var model = MakeTextCipherModel(title: "My Title");

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenEncryptedTextAlreadyExists()
		{
			SetupAttachedCiphers(new TextCipher { EncryptedText = "KHOOR", CreatedByUserId = "u2" });
			var model = MakeTextCipherModel(encryptedText: "KHOOR");

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenEncryptedTextAlreadyExists_EvenFromSameUser()
		{
			SetupAttachedCiphers(new TextCipher
			{
				Title = "Old Title",
				EncryptedText = "KHOOR",
				CreatedByUserId = "u1"
			});

			var model = MakeTextCipherModel(
				title: "New Title",
				encryptedText: "KHOOR");

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenBothDecryptedTextAndTypeAreNull()
		{
			var model = MakeTextCipherModel(decryptedText: null, cipherType: null);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenMLClassifiesAsPlaintext()
		{
			_cipherRecognizerMock.Setup(s => s.ClassifyCipher(It.IsAny<string>()))
				.ReturnsAsync(new CipherRecognitionResultViewModel
				{
					TopPrediction = new PredictionViewModel { Type = "plaintext" },
					AllPredictions = new List<PredictionViewModel>()
				});
			var model = MakeTextCipherModel();
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		#endregion

		#region SubmitCipherAsync - Text Cipher Happy Path

		[Fact]
		public async Task SubmitCipherAsync_CreatesTextCipher_WithCorrectFields()
		{
			SetupAttachedCiphers();
			var model = MakeTextCipherModel(
				title: "My Cipher",
				encryptedText: "ABC",
				decryptedText: "XYZ");

			Cipher captured = null;
			_cipherRepoMock.Setup(r => r.AddAsync(It.IsAny<Cipher>()))
				.Callback<Cipher>(c => captured = c)
				.Returns(Task.CompletedTask);

			await _service.SubmitCipherAsync(model, "u1");

			Assert.NotNull(captured);
			Assert.IsType<TextCipher>(captured);
			Assert.Equal("My Cipher", captured.Title);
			Assert.Equal("ABC", captured.EncryptedText);
			Assert.Equal("XYZ", captured.DecryptedText);
			Assert.Equal("u1", captured.CreatedByUserId);
			Assert.Equal(ApprovalStatus.Pending, captured.Status);
		}

		[Fact]
		public async Task SubmitCipherAsync_CallsEnglishValidation_WhenDecryptedTextProvided()
		{
			SetupAttachedCiphers();
			var model = MakeTextCipherModel(decryptedText: "HELLO WORLD");

			await _service.SubmitCipherAsync(model, "u1");

			_englishValidationMock.Verify(s => s.IsLikelyEnglishAsync("HELLO WORLD", It.IsAny<double>()), Times.Once);
		}

		[Fact]
		public async Task SubmitCipherAsync_DoesNotCallEnglishValidation_WhenNoDecryptedText()
		{
			SetupAttachedCiphers();
			var model = MakeTextCipherModel(decryptedText: null, cipherType: CipherType.Caesar);

			await _service.SubmitCipherAsync(model, "u1");

			_englishValidationMock.Verify(s => s.IsLikelyEnglishAsync(It.IsAny<string>(), It.IsAny<double>()), Times.Never);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsIsLLMRecommendedFalse_WhenHighConfidenceAndTypesMatch()
		{
			SetupAttachedCiphers();
			_cipherRecognizerMock.Setup(s => s.ClassifyCipher(It.IsAny<string>()))
				.ReturnsAsync(new CipherRecognitionResultViewModel
				{
					TopPrediction = new PredictionViewModel
					{
						Family = "Substitution",
						Type = "Caesar",
						Confidence = 0.90
					},
					AllPredictions = new List<PredictionViewModel>()
				});

			var model = MakeTextCipherModel(cipherType: CipherType.Caesar);
			Cipher captured = null;
			_cipherRepoMock.Setup(r => r.AddAsync(It.IsAny<Cipher>()))
				.Callback<Cipher>(c => captured = c)
				.Returns(Task.CompletedTask);

			await _service.SubmitCipherAsync(model, "u1");

			Assert.False(captured.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsIsLLMRecommendedTrue_WhenTypesDoNotMatch()
		{
			SetupAttachedCiphers();
			_cipherRecognizerMock.Setup(s => s.ClassifyCipher(It.IsAny<string>()))
				.ReturnsAsync(new CipherRecognitionResultViewModel
				{
					TopPrediction = new PredictionViewModel
					{
						Type = "Caesar",
						Confidence = 0.90
					},
					AllPredictions = new List<PredictionViewModel>()
				});

			var model = MakeTextCipherModel(cipherType: CipherType.Vigenere);
			Cipher captured = null;
			_cipherRepoMock.Setup(r => r.AddAsync(It.IsAny<Cipher>()))
				.Callback<Cipher>(c => captured = c)
				.Returns(Task.CompletedTask);

			await _service.SubmitCipherAsync(model, "u1");

			Assert.True(captured.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsIsLLMRecommendedTrue_WhenConfidenceLow()
		{
			SetupAttachedCiphers();
			_cipherRecognizerMock.Setup(s => s.ClassifyCipher(It.IsAny<string>()))
				.ReturnsAsync(new CipherRecognitionResultViewModel
				{
					TopPrediction = new PredictionViewModel
					{
						Type = "Caesar",
						Confidence = 0.70
					},
					AllPredictions = new List<PredictionViewModel>()
				});

			var model = MakeTextCipherModel(cipherType: CipherType.Caesar);
			Cipher captured = null;
			_cipherRepoMock.Setup(r => r.AddAsync(It.IsAny<Cipher>())
)
				.Callback<Cipher>(c => captured = c)
				.Returns(Task.CompletedTask);

			await _service.SubmitCipherAsync(model, "u1");

			Assert.True(captured.IsLLMRecommended);
		}

		[Fact]
		public async Task SubmitCipherAsync_SetsIsLLMRecommendedTrue_WhenProblematicCipherType()
		{
			SetupAttachedCiphers();
			_cipherRecognizerMock.Setup(s => s.ClassifyCipher(It.IsAny<string>()))
				.ReturnsAsync(new CipherRecognitionResultViewModel
				{
					TopPrediction = new PredictionViewModel
					{
						Type = "Columnar",
						Confidence = 0.90
					},
					AllPredictions = new List<PredictionViewModel>()
				});

			var model = MakeTextCipherModel(cipherType: null, decryptedText: "HELLO");
			Cipher captured = null;
			_cipherRepoMock.Setup(r => r.AddAsync(It.IsAny<Cipher>()))
				.Callback<Cipher>(c => captured = c)
				.Returns(Task.CompletedTask);

			await _service.SubmitCipherAsync(model, "u1");

			Assert.True(captured.IsLLMRecommended);
		}

		#endregion

		#region SubmitCipherAsync - Image Cipher Validation

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenImageFileIsNull()
		{
			var model = new SubmitCipherViewModel
			{
				Title = "Test",
				CipherDefinition = CipherDefinition.ImageCipher,
				Image = null
			};

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenImageFileIsEmpty()
		{
			var imageFile = CreateMockImageFile(length: 0);
			var model = new SubmitCipherViewModel
			{
				Title = "Test",
				CipherDefinition = CipherDefinition.ImageCipher,
				Image = imageFile.Object
			};

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenImageFileTooLarge()
		{
			var imageFile = CreateMockImageFile(length: 6 * 1024 * 1024);
			var model = new SubmitCipherViewModel
			{
				Title = "Test",
				CipherDefinition = CipherDefinition.ImageCipher,
				Image = imageFile.Object
			};

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		[Fact]
		public async Task SubmitCipherAsync_Throws_WhenImageFileExtensionInvalid()
		{
			var imageFile = CreateMockImageFile(fileName: "test.bmp");
			var model = new SubmitCipherViewModel
			{
				Title = "Test",
				CipherDefinition = CipherDefinition.ImageCipher,
				Image = imageFile.Object
			};

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SubmitCipherAsync(model, "u1"));
		}

		#endregion

		#region SubmittedCiphers

		[Fact]
		public async Task SubmittedCiphers_ReturnsEmpty_WhenUserHasNoCiphers()
		{
			SetupAttachedCiphers();

			var result = await _service.SubmittedCiphers("u1");

			Assert.Empty(result);
		}

		[Fact]
		public async Task SubmittedCiphers_ReturnsOnlyCiphers_ForGivenUser()
		{
			SetupAttachedCiphers(
				new TextCipher { CreatedByUserId = "u1", Title = "User1 Cipher" },
				new TextCipher { CreatedByUserId = "u2", Title = "User2 Cipher" });

			var result = await _service.SubmittedCiphers("u1");

			Assert.Single(result);
			Assert.Equal("User1 Cipher", result[0].Title);
		}

		[Fact]
		public async Task SubmittedCiphers_PopulatesApprovedFields_WhenCipherIsApproved()
		{
			var approvedAt = DateTime.UtcNow.AddDays(-1);
			SetupAttachedCiphers(new TextCipher
			{
				CreatedByUserId = "u1",
				Title = "Test",
				Status = ApprovalStatus.Approved,
				ApprovedAt = approvedAt,
				ChallengeType = ChallengeType.Standard,
				UserSolutions = new List<UserSolution>
				{
					new() { IsCorrect = true },
					new() { IsCorrect = false },
				}
			});

			var result = await _service.SubmittedCiphers("u1");

			Assert.Equal("Approved", result[0].Status);
			Assert.Equal(approvedAt, result[0].ApprovedTime);
			Assert.Equal("Standard", result[0].ApprovedAs);
			Assert.Equal(1, result[0].SolvedByCount);
		}

		[Fact]
		public async Task SubmittedCiphers_PopulatesRejectionFields_WhenCipherIsRejected()
		{
			var rejectedAt = DateTime.UtcNow.AddDays(-1);
			SetupAttachedCiphers(new TextCipher
			{
				CreatedByUserId = "u1",
				Title = "Test",
				Status = ApprovalStatus.Rejected,
				RejectedAt = rejectedAt,
				RejectionReason = "Not appropriate"
			});

			var result = await _service.SubmittedCiphers("u1");

			Assert.Equal("Rejected", result[0].Status);
			Assert.Equal(rejectedAt, result[0].RejectionTime);
			Assert.Equal("Not appropriate", result[0].RejectionReason);
		}

		[Fact]
		public async Task SubmittedCiphers_SetsStatusToCipherDeleted_WhenCipherIsDeleted()
		{
			var deletedAt = DateTime.UtcNow.AddDays(-2);
			SetupAttachedCiphers(new TextCipher
			{
				CreatedByUserId = "u1",
				Title = "Test",
				IsDeleted = true,
				DeletedAt = deletedAt,
				Status = ApprovalStatus.Approved
			});

			var result = await _service.SubmittedCiphers("u1");

			Assert.Equal("CipherDeleted", result[0].Status);
			Assert.Equal(deletedAt, result[0].DeletedTime);
		}

		#endregion
	}
}