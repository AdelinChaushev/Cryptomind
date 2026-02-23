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
using System.Threading.Tasks;
using Xunit;

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
		}

		private static ConcreteCipher MakeCipher(int id, ApprovalStatus status,
			bool isDeleted = false, string title = "Test Cipher",
			string? decryptedText = null, string createdByUserId = "u1",
			DateTime? createdAt = null, List<AnswerSuggestion>? answers = null,
			CipherType? typeOfCipher = null,
			ApplicationUser? createdByUser = null) => new()
			{
				Id = id,
				Status = status,
				IsDeleted = isDeleted,
				Title = title,
				DecryptedText = decryptedText,
				CreatedByUserId = createdByUserId,
				CreatedAt = createdAt ?? DateTime.UtcNow,
				EncryptedText = "encrypted text",
				AnswerSuggestions = answers ?? new List<AnswerSuggestion>(),
				CipherTags = new List<CipherTag>(),
				TypeOfCipher = typeOfCipher,
				CreatedByUser = createdByUser ?? new ApplicationUser { Id = createdByUserId, UserName = "testuser" },
				LLMData = new CipherLLMData(),
			};

		private static ApplicationUser User(string id, string userName = "testuser") => new()
		{
			Id = id,
			UserName = userName,
		};

		private void SetupAttachedCiphers(params Cipher[] ciphers)
		{
			var mock = new List<Cipher>(ciphers).AsQueryable().BuildMock();
			cipherRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region GetRecentCipherSubmissionTitles

		[Fact]
		public async Task GetRecentCipherSubmissionTitles_ReturnsTop5Pending_OrderedByNewest()
		{
			var ciphers = Enumerable.Range(1, 7)
				.Select(i => MakeCipher(i, ApprovalStatus.Pending, title: $"Cipher {i}",
					createdAt: DateTime.UtcNow.AddMinutes(i)))
				.ToArray();

			SetupAttachedCiphers(ciphers);

			var result = await service.GetRecentCipherSubmissionTitles();

			Assert.Equal(5, result.Count);
			Assert.Equal("Cipher 7", result[0].Title);
		}

		[Fact]
		public async Task GetRecentCipherSubmissionTitles_ExcludesNonPending()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, title: "Approved"),
				MakeCipher(2, ApprovalStatus.Pending, title: "Pending")
			);

			var result = await service.GetRecentCipherSubmissionTitles();

			Assert.Single(result);
			Assert.Equal("Pending", result[0].Title);
		}

		[Fact]
		public async Task GetRecentCipherSubmissionTitles_ReturnsEmpty_WhenNoPendingExist()
		{
			SetupAttachedCiphers();

			var result = await service.GetRecentCipherSubmissionTitles();

			Assert.Empty(result);
		}

		#endregion

		#region GetPendingCiphersCount

		[Fact]
		public async Task GetPendingCiphersCount_ReturnsOnlyPendingNonDeleted()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Pending),
				MakeCipher(2, ApprovalStatus.Pending),
				MakeCipher(3, ApprovalStatus.Pending, isDeleted: true),
				MakeCipher(4, ApprovalStatus.Approved),
			});

			var result = await service.GetPendingCiphersCount();
			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetPendingCiphersCount_ReturnsZero_WhenNoneExist()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());
			var result = await service.GetPendingCiphersCount();
			Assert.Equal(0, result);
		}

		#endregion

		#region GetApprovedCiphersCount

		[Fact]
		public async Task GetApprovedCiphersCount_ReturnsOnlyApprovedNonDeleted()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(3, ApprovalStatus.Pending),
			});

			var result = await service.GetApprovedCiphersCount();
			Assert.Equal(1, result);
		}

		[Fact]
		public async Task GetApprovedCiphersCount_ReturnsZero_WhenNoneExist()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());
			var result = await service.GetApprovedCiphersCount();
			Assert.Equal(0, result);
		}

		#endregion

		#region GetDeletedCiphersCount

		[Fact]
		public async Task GetDeletedCiphersCount_ReturnsAllDeleted_RegardlessOfStatus()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(2, ApprovalStatus.Pending, isDeleted: true),
				MakeCipher(3, ApprovalStatus.Approved, isDeleted: false),
			});

			var result = await service.GetDeletedCiphersCount();
			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetDeletedCiphersCount_ReturnsZero_WhenNoneDeleted()
		{
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved),
			});

			var result = await service.GetDeletedCiphersCount();
			Assert.Equal(0, result);
		}

		#endregion

		#region AllPendingCiphers

		[Fact]
		public async Task AllPendingCiphers_ReturnsPendingNonDeleted_OrderedByCreatedAt()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Pending, createdAt: DateTime.UtcNow.AddMinutes(2)),
				MakeCipher(2, ApprovalStatus.Pending, createdAt: DateTime.UtcNow.AddMinutes(1)),
				MakeCipher(3, ApprovalStatus.Approved),
				MakeCipher(4, ApprovalStatus.Pending, isDeleted: true)
			);

			var result = await service.AllPendingCiphers(null);

			Assert.Equal(2, result.Count);
			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		[Fact]
		public async Task AllPendingCiphers_ReturnsEmptyList_WhenNoPendingCiphers()
		{
			SetupAttachedCiphers();

			var result = await service.AllPendingCiphers(null);
			Assert.Empty(result);
		}

		#endregion

		#region AllApprovedCiphers

		[Fact]
		public async Task AllApprovedCiphers_ReturnsApprovedNonDeleted()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(3, ApprovalStatus.Pending)
			);

			var result = await service.AllApprovedCiphers(new CipherFilter());
			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task AllApprovedCiphers_ReturnsEmptyList_WhenNoApprovedCiphers()
		{
			SetupAttachedCiphers();

			var result = await service.AllApprovedCiphers(new CipherFilter());
			Assert.Empty(result);
		}

		[Fact]
		public async Task AllApprovedCiphers_FiltersBy_SearchTerm()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, title: "Mystery Cipher"),
				MakeCipher(2, ApprovalStatus.Approved, title: "Easy Puzzle")
			);

			var result = await service.AllApprovedCiphers(new CipherFilter { SearchTerm = "Mystery" });
			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task AllApprovedCiphers_FiltersBy_ChallengeTypeStandard()
		{
			var standard = MakeCipher(1, ApprovalStatus.Approved);
			standard.ChallengeType = ChallengeType.Standard;
			var experimental = MakeCipher(2, ApprovalStatus.Approved);
			experimental.ChallengeType = ChallengeType.Experimental;

			SetupAttachedCiphers(standard, experimental);

			var result = await service.AllApprovedCiphers(new CipherFilter { ChallengeType = ChallengeType.Standard });
			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task AllApprovedCiphers_FiltersBy_ChallengeTypeExperimental()
		{
			var standard = MakeCipher(1, ApprovalStatus.Approved);
			standard.ChallengeType = ChallengeType.Standard;
			var experimental = MakeCipher(2, ApprovalStatus.Approved);
			experimental.ChallengeType = ChallengeType.Experimental;

			SetupAttachedCiphers(standard, experimental);

			var result = await service.AllApprovedCiphers(new CipherFilter { ChallengeType = ChallengeType.Experimental });
			Assert.Single(result);
			Assert.Equal(2, result[0].Id);
		}

		[Fact]
		public async Task AllApprovedCiphers_OrdersBy_Newest()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, createdAt: DateTime.UtcNow.AddDays(-2)),
				MakeCipher(2, ApprovalStatus.Approved, createdAt: DateTime.UtcNow.AddDays(-1))
			);

			var result = await service.AllApprovedCiphers(new CipherFilter { OrderTerm = CipherOrderTerm.Newest });
			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		[Fact]
		public async Task AllApprovedCiphers_OrdersBy_Oldest()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, createdAt: DateTime.UtcNow.AddDays(-1)),
				MakeCipher(2, ApprovalStatus.Approved, createdAt: DateTime.UtcNow.AddDays(-2))
			);

			var result = await service.AllApprovedCiphers(new CipherFilter { OrderTerm = CipherOrderTerm.Oldest });
			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		#endregion

		#region AllDeletedCiphers

		[Fact]
		public async Task AllDeletedCiphers_ReturnsOnlyDeletedCiphers()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(2, ApprovalStatus.Pending, isDeleted: true),
				MakeCipher(3, ApprovalStatus.Approved, isDeleted: false)
			);

			var result = await service.AllDeletedCiphers(new CipherFilter());
			Assert.Equal(2, result.Count);
			Assert.DoesNotContain(result, r => r.Id == 3);
		}

		[Fact]
		public async Task AllDeletedCiphers_ReturnsEmptyList_WhenNoDeletedCiphers()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: false)
			);

			var result = await service.AllDeletedCiphers(new CipherFilter());
			Assert.Empty(result);
		}

		[Fact]
		public async Task AllDeletedCiphers_FiltersBy_SearchTerm()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, title: "Mystery Cipher"),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true, title: "Easy Puzzle")
			);

			var result = await service.AllDeletedCiphers(new CipherFilter { SearchTerm = "Mystery" });
			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task AllDeletedCiphers_FiltersBy_ChallengeTypeStandard()
		{
			var standard = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true);
			standard.ChallengeType = ChallengeType.Standard;
			var experimental = MakeCipher(2, ApprovalStatus.Approved, isDeleted: true);
			experimental.ChallengeType = ChallengeType.Experimental;

			SetupAttachedCiphers(standard, experimental);

			var result = await service.AllDeletedCiphers(new CipherFilter { ChallengeType = ChallengeType.Standard });
			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task AllDeletedCiphers_FiltersBy_ChallengeTypeExperimental()
		{
			var standard = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true);
			standard.ChallengeType = ChallengeType.Standard;
			var experimental = MakeCipher(2, ApprovalStatus.Approved, isDeleted: true);
			experimental.ChallengeType = ChallengeType.Experimental;

			SetupAttachedCiphers(standard, experimental);

			var result = await service.AllDeletedCiphers(new CipherFilter { ChallengeType = ChallengeType.Experimental });
			Assert.Single(result);
			Assert.Equal(2, result[0].Id);
		}

		[Fact]
		public async Task AllDeletedCiphers_OrdersBy_Newest()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, createdAt: DateTime.UtcNow.AddDays(-2)),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true, createdAt: DateTime.UtcNow.AddDays(-1))
			);

			var result = await service.AllDeletedCiphers(new CipherFilter { OrderTerm = CipherOrderTerm.Newest });
			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		[Fact]
		public async Task AllDeletedCiphers_OrdersBy_Oldest()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, createdAt: DateTime.UtcNow.AddDays(-1)),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true, createdAt: DateTime.UtcNow.AddDays(-2))
			);

			var result = await service.AllDeletedCiphers(new CipherFilter { OrderTerm = CipherOrderTerm.Oldest });
			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
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
		public async Task AnalyzeWithLLM_Throws_WhenCipherIsDeleted()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, isDeleted: true));

			await Assert.ThrowsAsync<ConflictException>(() => service.AnalyzeWithLLM(1));
		}

		[Fact]
		public async Task AnalyzeWithLLM_ReturnsCachedResult_WithoutCallingLLM_WhenReasoningAlreadyExists()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending);
			cipher.LLMData.Reasoning = "already analyzed";
			cipher.LLMData.PredictedType = "Caesar";
			cipher.LLMData.IsAppropriate = true;

			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);

			var result = await service.AnalyzeWithLLM(1);

			Assert.Equal("already analyzed", result.Reasoning);
			Assert.Equal("Caesar", result.PredictedType);

			llmServiceMock.Verify(l => l.ValidateCipherAsync(
				It.IsAny<string>(), It.IsAny<string?>(),
				It.IsAny<CipherRecognitionResultViewModel>(), It.IsAny<string>()), Times.Never);
		}

		#endregion

		#region ApproveCipherAsync - Guard Clauses

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cipher?)null);
			await Assert.ThrowsAsync<NotFoundException>(
				() => service.ApproveCipherAsync(99, new ApproveCipherViewModel { Title = "T", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenCipherIsDeleted()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, isDeleted: true));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "T", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenAlreadyApproved()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "T", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenUserNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, createdByUserId: "u1"));

			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "T", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTitleIsEmpty()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending));

			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTitleAlreadyExistsOnAnotherCipher()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending));

			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(2, ApprovalStatus.Approved, title: "Duplicate Title"),
			});

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Duplicate Title", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTypeOfCipherIsNull()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending));

			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Unique", TypeOfCipher = null }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenExperimentalCipherHasHintsEnabled()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, decryptedText: null));

			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveCipherAsync(1, new ApproveCipherViewModel
				{
					Title = "Unique",
					TypeOfCipher = CipherType.Caesar,
					AllowHint = true,
				}));
		}

		#endregion

		#region ApproveCipherAsync - Happy Path

		[Fact]
		public async Task ApproveCipherAsync_SetsChallengeTypeToStandard_WhenDecryptedTextExists()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: "hello world");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Caesar });

			Assert.Equal(ChallengeType.Standard, cipher.ChallengeType);
			Assert.Equal(ApprovalStatus.Approved, cipher.Status);
			Assert.NotNull(cipher.ApprovedAt);
		}

		[Fact]
		public async Task ApproveCipherAsync_SetsChallengeTypeToExperimental_WhenNoDecryptedText()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: null);
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Caesar });

			Assert.Equal(ChallengeType.Experimental, cipher.ChallengeType);
		}

		[Fact]
		public async Task ApproveCipherAsync_SetsCorrectPointsForType()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: "hello");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Autokey });

			Assert.Equal(500, cipher.Points);
		}

		[Fact]
		public async Task ApproveCipherAsync_SendsApprovalNotification_ToCreator()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: "hello", createdByUserId: "creator1");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("creator1")).ReturnsAsync(User("creator1"));
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Caesar });

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"creator1",
				NotificationType.CipherApproved,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task ApproveCipherAsync_ReturnsCreatorUserId()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: "hello", createdByUserId: "creator1");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("creator1")).ReturnsAsync(User("creator1"));
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Caesar });

			Assert.Equal("creator1", result);
		}

		#endregion

		#region RejectCipherAsync

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cipher?)null);
			await Assert.ThrowsAsync<NotFoundException>(() => service.RejectCipherAsync(99, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherIsDeleted()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, isDeleted: true));

			await Assert.ThrowsAsync<ConflictException>(() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherIsAlreadyApproved()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherIsAlreadyRejected()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Rejected));

			await Assert.ThrowsAsync<ConflictException>(() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenUserNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, createdByUserId: "u1"));

			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<NotFoundException>(() => service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_SetsStatusToRejected_WithReasonAndDate()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending);
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await service.RejectCipherAsync(1, "not appropriate");

			Assert.Equal(ApprovalStatus.Rejected, cipher.Status);
			Assert.Equal("not appropriate", cipher.RejectionReason);
			Assert.NotNull(cipher.RejectedAt);
		}

		[Fact]
		public async Task RejectCipherAsync_SendsRejectionNotification_ToCreator()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, createdByUserId: "creator1");
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("creator1")).ReturnsAsync(User("creator1"));

			await service.RejectCipherAsync(1, "not appropriate");

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"creator1",
				NotificationType.CipherRejected,
				"Your cipher Test Cipher was rejected. Reason: not appropriate",
				It.IsAny<string>()), Times.Once);
		}

		#endregion

		#region UpdateApprovedCipher

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherNotFound()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cipher?)null);
			await Assert.ThrowsAsync<NotFoundException>(
				() => service.UpdateApprovedCipher(99, new UpdateCipherViewModel { Title = "T" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherIsDeleted()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved, isDeleted: true));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.UpdateApprovedCipher(1, new UpdateCipherViewModel { Title = "T" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherIsNotApproved()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.UpdateApprovedCipher(1, new UpdateCipherViewModel { Title = "T" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenTitleIsEmpty()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<CustomValidationException>(
				() => service.UpdateApprovedCipher(1, new UpdateCipherViewModel { Title = "" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenTitleAlreadyExistsOnAnotherCipher()
		{
			cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved));

			cipherRepoMock.Setup(r => r.GetAll()).Returns(new List<Cipher>
			{
				MakeCipher(2, ApprovalStatus.Approved, title: "Taken Title"),
			}.AsQueryable());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.UpdateApprovedCipher(1, new UpdateCipherViewModel { Title = "Taken Title" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenExperimentalCipherHasHintsEnabled()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved);
			cipher.ChallengeType = ChallengeType.Experimental;

			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAll()).Returns(new List<Cipher>().AsQueryable());

			await Assert.ThrowsAsync<ConflictException>(
				() => service.UpdateApprovedCipher(1, new UpdateCipherViewModel
				{
					Title = "Unique Title",
					AllowHint = true,
				}));
		}

		[Fact]
		public async Task UpdateApprovedCipher_UpdatesTitleAndHintFlags()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved);
			cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			cipherRepoMock.Setup(r => r.GetAll()).Returns(new List<Cipher>().AsQueryable());

			await service.UpdateApprovedCipher(1, new UpdateCipherViewModel
			{
				Title = "New Title",
				AllowHint = true,
				AllowSolution = false,
			});

			Assert.Equal("New Title", cipher.Title);
			Assert.True(cipher.AllowHint);
			Assert.False(cipher.AllowSolution);
		}

		#endregion

		#region SoftDeleteCipher

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();
			await Assert.ThrowsAsync<NotFoundException>(() => service.SoftDeleteCipher(99));
		}

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherIsAlreadyDeleted()
		{
			SetupAttachedCiphers(MakeCipher(1, ApprovalStatus.Approved, isDeleted: true));
			await Assert.ThrowsAsync<ConflictException>(() => service.SoftDeleteCipher(1));
		}

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherIsRejected()
		{
			SetupAttachedCiphers(MakeCipher(1, ApprovalStatus.Rejected));
			await Assert.ThrowsAsync<ConflictException>(() => service.SoftDeleteCipher(1));
		}

		[Fact]
		public async Task SoftDeleteCipher_SetsIsDeletedAndDeletedAt()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved);
			SetupAttachedCiphers(cipher);

			await service.SoftDeleteCipher(1);

			Assert.True(cipher.IsDeleted);
			Assert.NotNull(cipher.DeletedAt);
		}

		[Fact]
		public async Task SoftDeleteCipher_SendsNotificationToCreator()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, createdByUserId: "creator1");
			SetupAttachedCiphers(cipher);

			await service.SoftDeleteCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"creator1",
				NotificationType.CipherDeleted,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task SoftDeleteCipher_SendsNotification_ToEachAnswerSubmitter()
		{
			var answers = new List<AnswerSuggestion>
			{
				new() { Id = 10, UserId = "user1", Status = ApprovalStatus.Pending },
				new() { Id = 11, UserId = "user2", Status = ApprovalStatus.Pending },
			};

			var cipher = MakeCipher(1, ApprovalStatus.Approved, answers: answers);
			SetupAttachedCiphers(cipher);

			await service.SoftDeleteCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"user1", NotificationType.AnswerCipherDeleted,
				It.IsAny<string>(), It.IsAny<string>()), Times.Once);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"user2", NotificationType.AnswerCipherDeleted,
				It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		#endregion

		#region RestoreCipher

		[Fact]
		public async Task RestoreCipher_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());
			await Assert.ThrowsAsync<NotFoundException>(() => service.RestoreCipher(99));
		}

		[Fact]
		public async Task RestoreCipher_Throws_WhenCipherIsNotDeleted()
		{
			SetupAttachedCiphers(MakeCipher(1, ApprovalStatus.Approved, isDeleted: false));
			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());
			await Assert.ThrowsAsync<ConflictException>(() => service.RestoreCipher(1));
		}

		[Fact]
		public async Task RestoreCipher_Throws_WhenTitleConflicts_AndNoNewTitleProvided()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, title: "Taken Title");
			SetupAttachedCiphers(cipher);

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(2, ApprovalStatus.Approved, title: "Taken Title"),
			});

			await Assert.ThrowsAsync<ConflictException>(() => service.RestoreCipher(1));
		}

		[Fact]
		public async Task RestoreCipher_Throws_WhenNewTitleAlsoConflicts()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, title: "Old Title");
			SetupAttachedCiphers(cipher);

			cipherRepoMock.SetupSequence(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher>()) // first call: no conflict on original title
				.ReturnsAsync(new List<Cipher>
				{
					MakeCipher(2, ApprovalStatus.Approved, title: "Conflicting New Title"),
				});

			await Assert.ThrowsAsync<ConflictException>(() => service.RestoreCipher(1, "Conflicting New Title"));
		}

		[Fact]
		public async Task RestoreCipher_Throws_WhenNewTitleIsEmptyString()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true);
			SetupAttachedCiphers(cipher);

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<CustomValidationException>(() => service.RestoreCipher(1, ""));
		}

		[Fact]
		public async Task RestoreCipher_SetsIsDeletedFalse_AndClearsDeletedAt()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true);
			cipher.DeletedAt = DateTime.UtcNow.AddDays(-1);
			SetupAttachedCiphers(cipher);

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.RestoreCipher(1);

			Assert.False(cipher.IsDeleted);
			Assert.Null(cipher.DeletedAt);
		}

		[Fact]
		public async Task RestoreCipher_UpdatesTitle_WhenNewTitleIsProvided()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, title: "Old Title");
			SetupAttachedCiphers(cipher);

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.RestoreCipher(1, "New Title");

			Assert.Equal("New Title", cipher.Title);
		}

		[Fact]
		public async Task RestoreCipher_SendsNotificationToCreator()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, createdByUserId: "creator1");
			SetupAttachedCiphers(cipher);

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.RestoreCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"creator1",
				NotificationType.CipherRestored,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task RestoreCipher_SendsNotification_ToEachAnswerSubmitter()
		{
			var answers = new List<AnswerSuggestion>
			{
				new() { Id = 10, UserId = "user1", Cipher = new ConcreteCipher { Title = "Test" } },
				new() { Id = 11, UserId = "user2", Cipher = new ConcreteCipher { Title = "Test" } },
			};

			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, answers: answers);
			SetupAttachedCiphers(cipher);

			cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await service.RestoreCipher(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"user1", NotificationType.AnswerCipherRestored,
				It.IsAny<string>(), It.IsAny<string>()), Times.Once);

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"user2", NotificationType.AnswerCipherRestored,
				It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		#endregion
	}
}