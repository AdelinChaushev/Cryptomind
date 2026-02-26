using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using static Cryptomind.Core.Services.LLMService;

namespace Cryptomind.Tests.Unit.Services
{
	public class AdminCipherServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> cipherRepoMock = new();
		private readonly Mock<IRepository<UserSolution, int>> solutionRepoMock = new();
		private readonly Mock<IRepository<Tag, int>> tagRepoMock = new();
		private readonly Mock<ILLMService> llmServiceMock = new();
		private readonly Mock<INotificationService> notificationMock = new();
		private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
		private readonly AdminCipherService service;

		public AdminCipherServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			service = new AdminCipherService(
				cipherRepoMock.Object,
				solutionRepoMock.Object,
				tagRepoMock.Object,
				llmServiceMock.Object,
				notificationMock.Object,
				userManagerMock.Object);

			cipherRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cipher>())).ReturnsAsync(true);
			notificationMock.Setup(n => n.CreateAndSendNotification(
				It.IsAny<string>(), It.IsAny<NotificationType>(),
				It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Task.CompletedTask);
			userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((string id) => new ApplicationUser { Id = id, UserName = "testuser" });
		}

		// Valid MLPrediction JSON satisfies both MlPredictionType and MlPredictionData
		private const string ValidMlPrediction =
				"""{"family":"Substitution","type":"Caesar","confidence":0.9913200787778678,"allPredictions":[{"family":"Substitution","type":"Caesar","confidence":0.9913200787778678},{"family":"Polyalphabetic","type":"Vigenere","confidence":5.099530124846473E-07}]}""";

		private static TextCipher MakeCipher(
			int id = 1,
			string title = "Test Cipher",
			string userId = "u1",
			ApprovalStatus status = ApprovalStatus.Pending,
			bool isDeleted = false,
			string? decryptedText = "hello",
			CipherType? type = CipherType.Caesar,
			ChallengeType challengeType = ChallengeType.Standard,
			List<AnswerSuggestion>? answerSuggestions = null) => new()
			{
				Id = id,
				Title = title,
				CreatedByUserId = userId,
				Status = status,
				IsDeleted = isDeleted,
				DecryptedText = decryptedText,
				TypeOfCipher = type,
				ChallengeType = challengeType,
				EncryptedText = "encrypted",
				MLPrediction = ValidMlPrediction,
				CipherTags = new List<CipherTag>(),
				AnswerSuggestions = answerSuggestions ?? new List<AnswerSuggestion>(),
				CreatedAt = DateTime.UtcNow,
				CreatedByUser = new ApplicationUser { Id = userId, UserName = "testuser" },
			};

		private static ApproveCipherViewModel ApproveModel(
			string title = "My Cipher",
			CipherType? type = CipherType.Caesar,
			bool allowHint = false,
			bool allowSolution = false,
			bool allowTypeHint = false,
			List<int>? tagIds = null) => new()
			{
				Title = title,
				TypeOfCipher = type,
				AllowHint = allowHint,
				AllowSolution = allowSolution,
				AllowTypeHint = allowTypeHint,
				TagIds = tagIds ?? new List<int>()
			};

		private static UpdateCipherViewModel UpdateModel(
			string title = "Updated Title",
			bool allowHint = false,
			bool allowSolution = false,
			bool allowTypeHint = false,
			List<int>? tagIds = null) => new()
			{
				Title = title,
				AllowHint = allowHint,
				AllowSolution = allowSolution,
				AllowTypeHint = allowTypeHint,
				TagIds = tagIds ?? new List<int>()
			};

		#region GetPendingCiphersCount

		[Fact]
		public async Task GetPendingCiphersCount_ReturnsOnlyPendingAndNotDeleted()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, status: ApprovalStatus.Pending, isDeleted: false),
				MakeCipher(2, status: ApprovalStatus.Pending, isDeleted: false),
				MakeCipher(3, status: ApprovalStatus.Pending, isDeleted: true),
				MakeCipher(4, status: ApprovalStatus.Approved, isDeleted: false),
			});

			var result = await service.GetPendingCiphersCount();

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetPendingCiphersCount_ReturnsZero_WhenNoneMatch()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await service.GetPendingCiphersCount();

			Assert.Equal(0, result);
		}

		#endregion

		#region GetApprovedCiphersCount

		[Fact]
		public async Task GetApprovedCiphersCount_ReturnsOnlyApprovedAndNotDeleted()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, status: ApprovalStatus.Approved, isDeleted: false),
				MakeCipher(2, status: ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(3, status: ApprovalStatus.Pending, isDeleted: false),
			});

			var result = await service.GetApprovedCiphersCount();

			Assert.Equal(1, result);
		}

		#endregion

		#region GetDeletedCiphersCount

		[Fact]
		public async Task GetDeletedCiphersCount_ReturnsAllDeleted_RegardlessOfStatus()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, isDeleted: true, status: ApprovalStatus.Approved),
				MakeCipher(2, isDeleted: true, status: ApprovalStatus.Pending),
				MakeCipher(3, isDeleted: false, status: ApprovalStatus.Approved),
			});

			var result = await service.GetDeletedCiphersCount();

			Assert.Equal(2, result);
		}

		#endregion

		#region AllPendingCiphers

		[Fact]
		public async Task AllPendingCiphers_ReturnsEmptyList_WhenNoPendingCiphers()
		{
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher>().AsQueryable().BuildMock());

			var result = await service.AllPendingCiphers(null);

			Assert.Empty(result);
		}

		[Fact]
		public async Task AllPendingCiphers_FiltersCorrectly_ByTitle()
		{
			var ciphers = new List<Cipher>
			{
				MakeCipher(1, title: "Caesar Challenge"),
				MakeCipher(2, title: "Vigenere Puzzle"),
			};
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(ciphers.AsQueryable().BuildMock());

			var result = await service.AllPendingCiphers("caesar");

			Assert.Single(result);
			Assert.Equal("Caesar Challenge", result[0].Title);
		}

		[Fact]
		public async Task AllPendingCiphers_Throws_WhenCreatedByUserIsNull()
		{
			var cipher = MakeCipher(1);
			cipher.CreatedByUser = null;

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await Assert.ThrowsAsync<Exception>(() => service.AllPendingCiphers(null));
		}

		#endregion

		#region GetCipherById

		[Fact]
		public async Task GetCipherById_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher>().AsQueryable().BuildMock());

			await Assert.ThrowsAsync<NotFoundException>(() => service.GetCipherById(99));
		}

		[Fact]
		public async Task GetCipherById_Throws_WhenCreatedByUserIsNull()
		{
			var cipher = MakeCipher(1);
			cipher.CreatedByUser = null;

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await Assert.ThrowsAsync<Exception>(() => service.GetCipherById(1));
		}

		[Fact]
		public async Task GetCipherById_ReturnsCorrectModel_WhenCipherExists()
		{
			var cipher = MakeCipher(1, title: "My Cipher");
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			var result = await service.GetCipherById(1);

			Assert.Equal("My Cipher", result.Title);
			Assert.Equal(1, result.Id);
		}

		#endregion

		#region AnalyzeWithLLM

		[Fact]
		public async Task AnalyzeWithLLM_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => service.AnalyzeWithLLM(99));
		}

		[Fact]
		public async Task AnalyzeWithLLM_Throws_WhenCipherIsNotPending()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, status: ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(() => service.AnalyzeWithLLM(1));
		}

		[Fact]
		public async Task AnalyzeWithLLM_Throws_WhenCipherIsDeleted()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, isDeleted: true));

			await Assert.ThrowsAsync<ConflictException>(() => service.AnalyzeWithLLM(1));
		}

		[Fact]
		public async Task AnalyzeWithLLM_ReturnsCachedResult_WhenLLMDataReasoningExists()
		{
			var cipher = MakeCipher(1);
			cipher.LLMData = new CipherLLMData
			{
				Reasoning = "cached reasoning",
				Confidence = "0.9",
				PredictedType = "Caesar",
				SolutionCorrect = true,
				IsAppropriate = true,
				IsSolvable = true,
				Issues = new List<string> { "none" }
			};

			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);

			var result = await service.AnalyzeWithLLM(1);

			Assert.Equal("cached reasoning", result.Reasoning);
			llmServiceMock.Verify(l => l.ValidateCipherAsync(
				It.IsAny<string>(), It.IsAny<string>(),
				It.IsAny<CipherRecognitionResultViewModel>(), It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public async Task AnalyzeWithLLM_CallsLLMAndUpdates_WhenNoCacheExists()
		{
			var cipher = MakeCipher(1);
			cipher.LLMData = null;

			var llmResult = new CipherValidationResult
			{
				Reasoning = "fresh reasoning",
				Confidence = "0.8",
				PredictedType = "Caesar",
				SolutionCorrect = true,
				IsAppropriate = true,
				IsSolvable = true,
				Issues = new List<string> { "none" }
			};

			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			llmServiceMock.Setup(l => l.ValidateCipherAsync(
				It.IsAny<string>(), It.IsAny<string>(),
				It.IsAny<CipherRecognitionResultViewModel>(), It.IsAny<string>()))
				.ReturnsAsync(llmResult);

			var result = await service.AnalyzeWithLLM(1);

			Assert.Equal("fresh reasoning", result.Reasoning);
			cipherRepoMock.Verify(r => r.UpdateAsync(cipher), Times.Once);
		}

		#endregion

		#region ApproveCipherAsync

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.ApproveCipherAsync(1, ApproveModel()));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenCipherIsDeleted()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, isDeleted: true));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, ApproveModel()));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenCipherIsNotPending()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, status: ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, ApproveModel()));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenUserNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCipher(1));
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.ApproveCipherAsync(1, ApproveModel()));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTitleIsEmpty()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCipher(1));

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.ApproveCipherAsync(1, ApproveModel(title: "")));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTitleAlreadyExists()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCipher(1));
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher> { MakeCipher(2, title: "Taken Title") });

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, ApproveModel(title: "Taken Title")));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTypeIsNull()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCipher(1));
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, ApproveModel(type: null)));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTypeIsMinusOne()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCipher(1));
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, ApproveModel(type: (CipherType)(-1))));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenExperimentalCipherHasHintsEnabled()
		{
			// Cipher with no decrypted text → Experimental
			var cipher = MakeCipher(1, decryptedText: null);
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, ApproveModel(allowHint: true)));
		}

		[Fact]
		public async Task ApproveCipherAsync_SetsChallengeTypeToStandard_WhenDecryptedTextExists()
		{
			var cipher = MakeCipher(1, decryptedText: "solution");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, ApproveModel());

			Assert.Equal(ChallengeType.Standard, cipher.ChallengeType);
		}

		[Fact]
		public async Task ApproveCipherAsync_SetsChallengeTypeToExperimental_WhenDecryptedTextIsEmpty()
		{
			var cipher = MakeCipher(1, decryptedText: null);
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, ApproveModel(allowHint: false, allowSolution: false, allowTypeHint: false));

			Assert.Equal(ChallengeType.Experimental, cipher.ChallengeType);
		}

		[Fact]
		public async Task ApproveCipherAsync_SetsCorrectPoints_ForCipherType()
		{
			var cipher = MakeCipher(1);
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, ApproveModel(type: CipherType.Vigenere));

			Assert.Equal(400, cipher.Points);
		}

		[Fact]
		public async Task ApproveCipherAsync_SetsStatusToApproved_AndSetsApprovedAt()
		{
			var cipher = MakeCipher(1);
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, ApproveModel());

			Assert.Equal(ApprovalStatus.Approved, cipher.Status);
			Assert.NotNull(cipher.ApprovedAt);
		}

		[Fact]
		public async Task ApproveCipherAsync_SendsApprovalNotification()
		{
			var cipher = MakeCipher(1, userId: "u1");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, ApproveModel());

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.CipherApproved,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task ApproveCipherAsync_ReturnsCreatedByUserId()
		{
			var cipher = MakeCipher(1, userId: "u1");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await service.ApproveCipherAsync(1, ApproveModel());

			Assert.Equal("u1", result);
		}

		#endregion

		#region RejectCipherAsync

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherIsDeleted()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, isDeleted: true));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherIsAlreadyApproved()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, status: ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherIsAlreadyRejected()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, status: ApprovalStatus.Rejected));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenUserNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MakeCipher(1));
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_SetsStatusRejectedAndReason()
		{
			var cipher = MakeCipher(1);
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);

			await service.RejectCipherAsync(1, "bad cipher");

			Assert.Equal(ApprovalStatus.Rejected, cipher.Status);
			Assert.Equal("bad cipher", cipher.RejectionReason);
			Assert.NotNull(cipher.RejectedAt);
		}

		[Fact]
		public async Task RejectCipherAsync_SendsRejectionNotification()
		{
			var cipher = MakeCipher(1, userId: "u1");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);

			await service.RejectCipherAsync(1, "bad cipher");

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.CipherRejected,
				It.Is<string>(s => s.Contains("bad cipher")),
				It.IsAny<string>()), Times.Once);
		}

		#endregion

		#region UpdateApprovedCipher

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher>().AsQueryable().BuildMock());

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.UpdateApprovedCipher(1, UpdateModel()));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherIsDeleted()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved, isDeleted: true);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.UpdateApprovedCipher(1, UpdateModel()));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherIsNotApproved()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Pending);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.UpdateApprovedCipher(1, UpdateModel()));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenTitleIsEmpty()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.UpdateApprovedCipher(1, UpdateModel(title: "")));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenTitleAlreadyExists()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAll())
				.Returns(new List<Cipher> { MakeCipher(2, title: "Taken Title") }.AsQueryable());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.UpdateApprovedCipher(1, UpdateModel(title: "Taken Title")));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenExperimentalCipherHasHintsEnabled()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved,
				challengeType: ChallengeType.Experimental);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAll()).Returns(new List<Cipher>().AsQueryable());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.UpdateApprovedCipher(1, UpdateModel(allowHint: true)));
		}

		[Fact]
		public async Task UpdateApprovedCipher_UpdatesTitleAndSendsNotification()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved, userId: "u1");
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAll()).Returns(new List<Cipher>().AsQueryable());

			await service.UpdateApprovedCipher(1, UpdateModel(title: "New Title"));

			Assert.Equal("New Title", cipher.Title);
			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.CipherUpdated,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		#endregion

		#region SoftDeleteCipher

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher>().AsQueryable().BuildMock());

			await Assert.ThrowsAsync<NotFoundException>(() => service.SoftDeleteCipher(1));
		}

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherIsAlreadyDeleted()
		{
			var cipher = MakeCipher(1, isDeleted: true);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await Assert.ThrowsAsync<ConflictException>(() => service.SoftDeleteCipher(1));
		}

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherIsRejected()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Rejected);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await Assert.ThrowsAsync<ConflictException>(() => service.SoftDeleteCipher(1));
		}

		[Fact]
		public async Task SoftDeleteCipher_SetsIsDeletedAndDeletedAt()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await service.SoftDeleteCipher(1);

			Assert.True(cipher.IsDeleted);
			Assert.NotNull(cipher.DeletedAt);
		}

		[Fact]
		public async Task SoftDeleteCipher_SendsDeleteNotification_ToOwner()
		{
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved, userId: "u1");
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await service.SoftDeleteCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.CipherDeleted,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task SoftDeleteCipher_SendsNotification_ToAnswerSubmitters_WhenPendingAnswersExist()
		{
			var pendingAnswer = new AnswerSuggestion
			{
				Id = 1,
				UserId = "u2",
				Status = ApprovalStatus.Pending
			};
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved,
				answerSuggestions: new List<AnswerSuggestion> { pendingAnswer });

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await service.SoftDeleteCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u2",
				NotificationType.AnswerCipherDeleted,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task SoftDeleteCipher_DoesNotNotifyAnswerSubmitters_WhenNoPendingAnswersExist()
		{
			var approvedAnswer = new AnswerSuggestion
			{
				Id = 1,
				UserId = "u2",
				Status = ApprovalStatus.Approved
			};
			var cipher = MakeCipher(1, status: ApprovalStatus.Approved,
				answerSuggestions: new List<AnswerSuggestion> { approvedAnswer });

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await service.SoftDeleteCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u2",
				NotificationType.AnswerCipherDeleted,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Never);
		}

		#endregion

		#region RestoreCipher

		[Fact]
		public async Task RestoreCipher_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher>().AsQueryable().BuildMock());

			await Assert.ThrowsAsync<NotFoundException>(() => service.RestoreCipher(1));
		}

		[Fact]
		public async Task RestoreCipher_Throws_WhenCipherIsNotDeleted()
		{
			var cipher = MakeCipher(1, isDeleted: false);
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());

			await Assert.ThrowsAsync<ConflictException>(() => service.RestoreCipher(1));
		}

		[Fact]
		public async Task RestoreCipher_Throws_WhenTitleConflictsAndNoNewTitleProvided()
		{
			var cipher = MakeCipher(1, title: "Same Title", isDeleted: true);
			var conflicting = MakeCipher(2, title: "Same Title");

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher> { cipher, conflicting });

			await Assert.ThrowsAsync<ConflictException>(() => service.RestoreCipher(1));
		}

		[Fact]
		public async Task RestoreCipher_Throws_WhenNewTitleIsEmptyString()
		{
			var cipher = MakeCipher(1, isDeleted: true);

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher> { cipher });

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.RestoreCipher(1, ""));
		}

		[Fact]
		public async Task RestoreCipher_Throws_WhenNewTitleAlsoConflicts()
		{
			var cipher = MakeCipher(1, title: "Old Title", isDeleted: true);
			var conflicting = MakeCipher(2, title: "New Title");

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher> { cipher, conflicting });

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RestoreCipher(1, "New Title"));
		}

		[Fact]
		public async Task RestoreCipher_RestoresSuccessfully_WhenNoTitleConflict()
		{
			var cipher = MakeCipher(1, isDeleted: true);
			cipher.DeletedAt = DateTime.UtcNow;

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher> { cipher });

			await service.RestoreCipher(1);

			Assert.False(cipher.IsDeleted);
			Assert.Null(cipher.DeletedAt);
		}

		[Fact]
		public async Task RestoreCipher_UpdatesTitle_WhenNewTitleProvided()
		{
			var cipher = MakeCipher(1, title: "Old Title", isDeleted: true);

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher> { cipher });

			await service.RestoreCipher(1, "Unique New Title");

			Assert.Equal("Unique New Title", cipher.Title);
		}

		[Fact]
		public async Task RestoreCipher_SendsRestoredNotification_ToOwner()
		{
			var cipher = MakeCipher(1, userId: "u1", isDeleted: true);

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher> { cipher });

			await service.RestoreCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.CipherRestored,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task RestoreCipher_NotifiesAnswerSubmitters_WhenPendingAnswersExist()
		{
			var pendingAnswer = new AnswerSuggestion
			{
				Id = 1,
				UserId = "u2",
				Status = ApprovalStatus.Pending,
				Cipher = new TextCipher { Title = "Test Cipher" }
			};
			var cipher = MakeCipher(1, isDeleted: true,
				answerSuggestions: new List<AnswerSuggestion> { pendingAnswer });

			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(new List<Cipher> { cipher }.AsQueryable().BuildMock());
			cipherRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher> { cipher });

			await service.RestoreCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u2",
				NotificationType.AnswerCipherRestored,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		#endregion
	}
}