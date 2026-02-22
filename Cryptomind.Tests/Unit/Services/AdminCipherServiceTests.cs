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

namespace Cryptomind.Tests.Unit.Services
{
	public class AdminCipherServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> _cipherRepoMock = new();
		private readonly Mock<IRepository<UserSolution, int>> _solutionRepoMock = new();
		private readonly Mock<IRepository<Tag, int>> _tagRepoMock = new();
		private readonly Mock<ILLMService> _llmServiceMock = new();
		private readonly Mock<INotificationService> _notificationMock = new();
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly AdminCipherService _service;

		public AdminCipherServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			_service = new AdminCipherService(
				_cipherRepoMock.Object,
				_solutionRepoMock.Object,
				_tagRepoMock.Object,
				_llmServiceMock.Object,
				_notificationMock.Object,
				_userManagerMock.Object);

			_cipherRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cipher>())).ReturnsAsync(true);
			_notificationMock.Setup(n => n.CreateAndSendNotification(
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
				CreatedByUser = createdByUser,
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
			_cipherRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		// -----------------------------------------------------------------------
		// GetRecentCipherSubmissionTitles
		// -----------------------------------------------------------------------

		[Fact]
		public async Task GetRecentCipherSubmissionTitles_ReturnsTop5Pending_OrderedByNewest()
		{
			var ciphers = Enumerable.Range(1, 7)
				.Select(i => MakeCipher(i, ApprovalStatus.Pending, title: $"Cipher {i}",
					createdAt: DateTime.UtcNow.AddMinutes(i)))
				.ToList<Cipher>();

			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(ciphers);

			var result = await _service.GetRecentCipherSubmissionTitles();

			Assert.Equal(5, result.Count);
			Assert.Equal("Cipher 7", result[0]);
		}

		[Fact]
		public async Task GetRecentCipherSubmissionTitles_ExcludesNonPending()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, title: "Approved"),
				MakeCipher(2, ApprovalStatus.Pending, title: "Pending"),
			});

			var result = await _service.GetRecentCipherSubmissionTitles();

			Assert.Single(result);
			Assert.Equal("Pending", result[0]);
		}

		[Fact]
		public async Task GetRecentCipherSubmissionTitles_ReturnsEmpty_WhenNoPendingExist()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await _service.GetRecentCipherSubmissionTitles();

			Assert.Empty(result);
		}

		// -----------------------------------------------------------------------
		// GetPendingCiphersCount
		// -----------------------------------------------------------------------

		[Fact]
		public async Task GetPendingCiphersCount_ReturnsOnlyPendingNonDeleted()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Pending),
				MakeCipher(2, ApprovalStatus.Pending),
				MakeCipher(3, ApprovalStatus.Pending, isDeleted: true),
				MakeCipher(4, ApprovalStatus.Approved),
			});

			var result = await _service.GetPendingCiphersCount();

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetPendingCiphersCount_ReturnsZero_WhenNoneExist()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await _service.GetPendingCiphersCount();

			Assert.Equal(0, result);
		}

		// -----------------------------------------------------------------------
		// GetApprovedCiphersCount
		// -----------------------------------------------------------------------

		[Fact]
		public async Task GetApprovedCiphersCount_ReturnsOnlyApprovedNonDeleted()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(3, ApprovalStatus.Pending),
			});

			var result = await _service.GetApprovedCiphersCount();

			Assert.Equal(1, result);
		}

		[Fact]
		public async Task GetApprovedCiphersCount_ReturnsZero_WhenNoneExist()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await _service.GetApprovedCiphersCount();

			Assert.Equal(0, result);
		}

		// -----------------------------------------------------------------------
		// GetDeletedCiphersCount
		// -----------------------------------------------------------------------

		[Fact]
		public async Task GetDeletedCiphersCount_ReturnsAllDeleted_RegardlessOfStatus()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(2, ApprovalStatus.Pending, isDeleted: true),
				MakeCipher(3, ApprovalStatus.Approved, isDeleted: false),
			});

			var result = await _service.GetDeletedCiphersCount();

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetDeletedCiphersCount_ReturnsZero_WhenNoneDeleted()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved),
			});

			var result = await _service.GetDeletedCiphersCount();

			Assert.Equal(0, result);
		}

		// -----------------------------------------------------------------------
		// AllPendingCiphers
		// -----------------------------------------------------------------------

		[Fact]
		public async Task AllPendingCiphers_ReturnsPendingNonDeleted_OrderedByCreatedAt()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Pending, createdAt: DateTime.UtcNow.AddMinutes(2)),
				MakeCipher(2, ApprovalStatus.Pending, createdAt: DateTime.UtcNow.AddMinutes(1)),
				MakeCipher(3, ApprovalStatus.Approved),
				MakeCipher(4, ApprovalStatus.Pending, isDeleted: true),
			});

			var result = await _service.AllPendingCiphers();

			Assert.Equal(2, result.Count);
			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		[Fact]
		public async Task AllPendingCiphers_ReturnsEmptyList_WhenNoPendingCiphers()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await _service.AllPendingCiphers();

			Assert.Empty(result);
		}

		// -----------------------------------------------------------------------
		// AllApprovedCiphers
		// -----------------------------------------------------------------------

		[Fact]
		public async Task AllApprovedCiphers_ReturnsApprovedNonDeleted()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(3, ApprovalStatus.Pending),
			});

			var result = await _service.AllApprovedCiphers(new CipherFilter());

			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task AllApprovedCiphers_ReturnsEmptyList_WhenNoApprovedCiphers()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await _service.AllApprovedCiphers(new CipherFilter());

			Assert.Empty(result);
		}

		[Fact]
		public async Task AllApprovedCiphers_FiltersBy_SearchTerm()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, title: "Mystery Cipher"),
				MakeCipher(2, ApprovalStatus.Approved, title: "Easy Puzzle"),
			});

			var result = await _service.AllApprovedCiphers(new CipherFilter { SearchTerm = "Mystery" });

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

			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher> { standard, experimental });

			var result = await _service.AllApprovedCiphers(new CipherFilter { ChallengeType = ChallengeType.Standard });

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

			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher> { standard, experimental });

			var result = await _service.AllApprovedCiphers(new CipherFilter { ChallengeType = ChallengeType.Experimental });

			Assert.Single(result);
			Assert.Equal(2, result[0].Id);
		}

		[Fact]
		public async Task AllApprovedCiphers_OrdersBy_Newest()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, createdAt: DateTime.UtcNow.AddDays(-2)),
				MakeCipher(2, ApprovalStatus.Approved, createdAt: DateTime.UtcNow.AddDays(-1)),
			});

			var result = await _service.AllApprovedCiphers(new CipherFilter { OrderTerm = CipherOrderTerm.Newest });

			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		[Fact]
		public async Task AllApprovedCiphers_OrdersBy_Oldest()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, createdAt: DateTime.UtcNow.AddDays(-1)),
				MakeCipher(2, ApprovalStatus.Approved, createdAt: DateTime.UtcNow.AddDays(-2)),
			});

			var result = await _service.AllApprovedCiphers(new CipherFilter { OrderTerm = CipherOrderTerm.Oldest });

			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		// -----------------------------------------------------------------------
		// AllDeletedCiphers
		// -----------------------------------------------------------------------

		[Fact]
		public async Task AllDeletedCiphers_ReturnsOnlyDeletedCiphers()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true),
				MakeCipher(2, ApprovalStatus.Pending, isDeleted: true),
				MakeCipher(3, ApprovalStatus.Approved, isDeleted: false),
			});

			var result = await _service.AllDeletedCiphers(new CipherFilter());

			Assert.Equal(2, result.Count);
			Assert.DoesNotContain(result, r => r.Id == 3);
		}

		[Fact]
		public async Task AllDeletedCiphers_ReturnsEmptyList_WhenNoDeletedCiphers()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved),
			});

			var result = await _service.AllDeletedCiphers(new CipherFilter());

			Assert.Empty(result);
		}

		[Fact]
		public async Task AllDeletedCiphers_FiltersBy_SearchTerm()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, title: "Mystery Cipher"),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true, title: "Easy Puzzle"),
			});

			var result = await _service.AllDeletedCiphers(new CipherFilter { SearchTerm = "Mystery" });

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

			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher> { standard, experimental });

			var result = await _service.AllDeletedCiphers(new CipherFilter { ChallengeType = ChallengeType.Standard });

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

			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher> { standard, experimental });

			var result = await _service.AllDeletedCiphers(new CipherFilter { ChallengeType = ChallengeType.Experimental });

			Assert.Single(result);
			Assert.Equal(2, result[0].Id);
		}

		[Fact]
		public async Task AllDeletedCiphers_OrdersBy_Newest()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, createdAt: DateTime.UtcNow.AddDays(-2)),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true, createdAt: DateTime.UtcNow.AddDays(-1)),
			});

			var result = await _service.AllDeletedCiphers(new CipherFilter { OrderTerm = CipherOrderTerm.Newest });

			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		[Fact]
		public async Task AllDeletedCiphers_OrdersBy_Oldest()
		{
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, createdAt: DateTime.UtcNow.AddDays(-1)),
				MakeCipher(2, ApprovalStatus.Approved, isDeleted: true, createdAt: DateTime.UtcNow.AddDays(-2)),
			});

			var result = await _service.AllDeletedCiphers(new CipherFilter { OrderTerm = CipherOrderTerm.Oldest });

			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		// -----------------------------------------------------------------------
		// AnalyzeWithLLM
		// -----------------------------------------------------------------------

		[Fact]
		public async Task AnalyzeWithLLM_Throws_WhenCipherNotFound()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AnalyzeWithLLM(99));
		}

		[Fact]
		public async Task AnalyzeWithLLM_Throws_WhenCipherIsDeleted()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, isDeleted: true));

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AnalyzeWithLLM(1));
		}

		[Fact]
		public async Task AnalyzeWithLLM_ReturnsCachedResult_WithoutCallingLLM_WhenReasoningAlreadyExists()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending);
			cipher.LLMData.Reasoning = "already analyzed";
			cipher.LLMData.PredictedType = "Caesar";
			cipher.LLMData.IsAppropriate = true;

			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);

			var result = await _service.AnalyzeWithLLM(1);

			Assert.Equal("already analyzed", result.Reasoning);
			Assert.Equal("Caesar", result.PredictedType);
			_llmServiceMock.Verify(l => l.ValidateCipherAsync(
				It.IsAny<string>(), It.IsAny<string?>(),
				It.IsAny<CipherRecognitionResultViewModel>(), It.IsAny<string>()), Times.Never);
		}

		// -----------------------------------------------------------------------
		// ApproveCipherAsync — guard clauses
		// -----------------------------------------------------------------------

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenCipherNotFound()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ApproveCipherAsync(99, new ApproveCipherViewModel { Title = "T", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenCipherIsDeleted()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, isDeleted: true));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "T", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenAlreadyApproved()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "T", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenUserNotFound()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, createdByUserId: "u1"));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "T", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTitleIsEmpty()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTitleAlreadyExistsOnAnotherCipher()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(2, ApprovalStatus.Approved, title: "Duplicate Title"),
			});

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Duplicate Title", TypeOfCipher = CipherType.Caesar }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenTypeOfCipherIsNull()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Unique", TypeOfCipher = null }));
		}

		[Fact]
		public async Task ApproveCipherAsync_Throws_WhenExperimentalCipherHasHintsEnabled()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, decryptedText: null));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.ApproveCipherAsync(1, new ApproveCipherViewModel
				{
					Title = "Unique",
					TypeOfCipher = CipherType.Caesar,
					AllowHint = true,
				}));
		}

		// -----------------------------------------------------------------------
		// ApproveCipherAsync — happy path
		// -----------------------------------------------------------------------

		[Fact]
		public async Task ApproveCipherAsync_SetsChallengeTypeToStandard_WhenDecryptedTextExists()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: "hello world");
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Caesar });

			Assert.Equal(ChallengeType.Standard, cipher.ChallengeType);
			Assert.Equal(ApprovalStatus.Approved, cipher.Status);
			Assert.NotNull(cipher.ApprovedAt);
		}

		[Fact]
		public async Task ApproveCipherAsync_SetsChallengeTypeToExperimental_WhenNoDecryptedText()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: null);
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Caesar });

			Assert.Equal(ChallengeType.Experimental, cipher.ChallengeType);
		}

		[Fact]
		public async Task ApproveCipherAsync_SetsCorrectPointsForType()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: "hello");
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Autokey });

			Assert.Equal(500, cipher.Points);
		}

		[Fact]
		public async Task ApproveCipherAsync_SendsApprovalNotification_ToCreator()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: "hello", createdByUserId: "creator1");
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("creator1")).ReturnsAsync(User("creator1"));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Caesar });

			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"creator1",
				NotificationType.CipherApproved,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task ApproveCipherAsync_ReturnsCreatorUserId()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, decryptedText: "hello", createdByUserId: "creator1");
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("creator1")).ReturnsAsync(User("creator1"));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			var result = await _service.ApproveCipherAsync(1, new ApproveCipherViewModel { Title = "Title", TypeOfCipher = CipherType.Caesar });

			Assert.Equal("creator1", result);
		}

		// -----------------------------------------------------------------------
		// RejectCipherAsync
		// -----------------------------------------------------------------------

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherNotFound()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RejectCipherAsync(99, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherIsDeleted()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, isDeleted: true));

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenCipherIsAlreadyApproved()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_Throws_WhenUserNotFound()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending, createdByUserId: "u1"));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RejectCipherAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectCipherAsync_SetsStatusToRejected_WithReasonAndDate()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending);
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await _service.RejectCipherAsync(1, "not appropriate");

			Assert.Equal(ApprovalStatus.Rejected, cipher.Status);
			Assert.Equal("not appropriate", cipher.RejectionReason);
			Assert.NotNull(cipher.RejectedAt);
		}

		[Fact]
		public async Task RejectCipherAsync_SendsRejectionNotification_ToCreator()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Pending, createdByUserId: "creator1");
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("creator1")).ReturnsAsync(User("creator1"));

			await _service.RejectCipherAsync(1, "not appropriate");

			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"creator1",
				NotificationType.CipherRejected,
				"not appropriate",
				It.IsAny<string>()), Times.Once);
		}

		// -----------------------------------------------------------------------
		// UpdateApprovedCipher
		// -----------------------------------------------------------------------

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherNotFound()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.UpdateApprovedCipher(99, new UpdateCipherViewModel { Title = "T" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherIsDeleted()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved, isDeleted: true));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.UpdateApprovedCipher(1, new UpdateCipherViewModel { Title = "T" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenCipherIsNotApproved()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Pending));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.UpdateApprovedCipher(1, new UpdateCipherViewModel { Title = "T" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenTitleIsEmpty()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.UpdateApprovedCipher(1, new UpdateCipherViewModel { Title = "" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_Throws_WhenTitleAlreadyExistsOnAnotherCipher()
		{
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(MakeCipher(1, ApprovalStatus.Approved));
			_cipherRepoMock.Setup(r => r.GetAll()).Returns(new List<Cipher>
			{
				MakeCipher(2, ApprovalStatus.Approved, title: "Taken Title"),
			}.AsQueryable());

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.UpdateApprovedCipher(1, new UpdateCipherViewModel { Title = "Taken Title" }));
		}

		[Fact]
		public async Task UpdateApprovedCipher_UpdatesTitleAndHintFlags()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved);
			_cipherRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cipher);
			_cipherRepoMock.Setup(r => r.GetAll()).Returns(new List<Cipher>().AsQueryable());

			await _service.UpdateApprovedCipher(1, new UpdateCipherViewModel
			{
				Title = "New Title",
				AllowHint = true,
				AllowSolution = false,
			});

			Assert.Equal("New Title", cipher.Title);
			Assert.True(cipher.AllowHint);
			Assert.False(cipher.AllowSolution);
		}

		// -----------------------------------------------------------------------
		// SoftDeleteCipher
		// -----------------------------------------------------------------------

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SoftDeleteCipher(99));
		}

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherIsAlreadyDeleted()
		{
			SetupAttachedCiphers(MakeCipher(1, ApprovalStatus.Approved, isDeleted: true));

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SoftDeleteCipher(1));
		}

		[Fact]
		public async Task SoftDeleteCipher_Throws_WhenCipherIsRejected()
		{
			SetupAttachedCiphers(MakeCipher(1, ApprovalStatus.Rejected));

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SoftDeleteCipher(1));
		}

		[Fact]
		public async Task SoftDeleteCipher_SetsIsDeletedAndDeletedAt()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved);
			SetupAttachedCiphers(cipher);

			await _service.SoftDeleteCipher(1);

			Assert.True(cipher.IsDeleted);
			Assert.NotNull(cipher.DeletedAt);
		}

		[Fact]
		public async Task SoftDeleteCipher_SendsNotificationToCreator()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, createdByUserId: "creator1");
			SetupAttachedCiphers(cipher);

			await _service.SoftDeleteCipher(1);

			_notificationMock.Verify(n => n.CreateAndSendNotification(
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
				new() { Id = 10, UserId = "user1" },
				new() { Id = 11, UserId = "user2" },
			};
			var cipher = MakeCipher(1, ApprovalStatus.Approved, answers: answers);
			SetupAttachedCiphers(cipher);

			await _service.SoftDeleteCipher(1);

			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"user1", NotificationType.AnswerCipherDeleted,
				It.IsAny<string>(), It.IsAny<string>()), Times.Once);
			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"user2", NotificationType.AnswerCipherDeleted,
				It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}

		// -----------------------------------------------------------------------
		// Restore
		// -----------------------------------------------------------------------

		[Fact]
		public async Task Restore_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Restore(99));
		}

		[Fact]
		public async Task Restore_Throws_WhenCipherIsNotDeleted()
		{
			SetupAttachedCiphers(MakeCipher(1, ApprovalStatus.Approved, isDeleted: false));
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Restore(1));
		}

		[Fact]
		public async Task Restore_ThrowsTitleConflictException_WhenTitleConflicts_AndNoNewTitleProvided()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, title: "Taken Title");
			SetupAttachedCiphers(cipher);
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>
			{
				MakeCipher(2, ApprovalStatus.Approved, title: "Taken Title"),
			});

			await Assert.ThrowsAsync<TitleConflictException>(() => _service.Restore(1));
		}

		[Fact]
		public async Task Restore_ThrowsTitleConflictException_WhenNewTitleAlsoConflicts()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, title: "Old Title");
			SetupAttachedCiphers(cipher);
			_cipherRepoMock.SetupSequence(r => r.GetAllAsync())
				.ReturnsAsync(new List<Cipher>())
				.ReturnsAsync(new List<Cipher>
				{
					MakeCipher(2, ApprovalStatus.Approved, title: "Conflicting New Title"),
				});

			await Assert.ThrowsAsync<TitleConflictException>(() => _service.Restore(1, "Conflicting New Title"));
		}

		[Fact]
		public async Task Restore_Throws_WhenNewTitleIsEmptyString()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true);
			SetupAttachedCiphers(cipher);
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Restore(1, ""));
		}

		[Fact]
		public async Task Restore_SetsIsDeletedFalse_AndClearsDeletedAt()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true);
			cipher.DeletedAt = DateTime.UtcNow.AddDays(-1);
			SetupAttachedCiphers(cipher);
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await _service.Restore(1);

			Assert.False(cipher.IsDeleted);
			Assert.Null(cipher.DeletedAt);
		}

		[Fact]
		public async Task Restore_UpdatesTitle_WhenNewTitleIsProvided()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, title: "Old Title");
			SetupAttachedCiphers(cipher);
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await _service.Restore(1, "New Title");

			Assert.Equal("New Title", cipher.Title);
		}

		[Fact]
		public async Task Restore_SendsNotificationToCreator()
		{
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, createdByUserId: "creator1");
			SetupAttachedCiphers(cipher);
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await _service.Restore(1);

			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"creator1",
				NotificationType.CipherRestored,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task Restore_SendsNotification_ToEachAnswerSubmitter()
		{
			var answers = new List<AnswerSuggestion>
			{
				new() { Id = 10, UserId = "user1", Cipher = new ConcreteCipher { Title = "Test" } },
				new() { Id = 11, UserId = "user2", Cipher = new ConcreteCipher { Title = "Test" } },
			};
			var cipher = MakeCipher(1, ApprovalStatus.Approved, isDeleted: true, answers: answers);
			SetupAttachedCiphers(cipher);
			_cipherRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Cipher>());

			await _service.Restore(1);

			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"user1", NotificationType.AnswerCipherRestored,
				It.IsAny<string>(), It.IsAny<string>()), Times.Once);
			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"user2", NotificationType.AnswerCipherRestored,
				It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}
	}
}