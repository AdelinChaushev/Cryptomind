using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Common.Exceptions;
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
	public class CipherServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> cipherRepoMock = new();
		private readonly Mock<IRepository<UserSolution, int>> solutionRepoMock = new();
		private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
		private readonly CipherService service;

		public CipherServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			service = new CipherService(
				cipherRepoMock.Object,
				solutionRepoMock.Object,
				userManagerMock.Object);

			solutionRepoMock.Setup(r => r.AddAsync(It.IsAny<UserSolution>()))
				.Returns(Task.CompletedTask);
			userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
				.ReturnsAsync(IdentityResult.Success);
		}

		private static TextCipher MakeCipher(
			int id = 1,
			string title = "Test Cipher",
			string userId = "owner",
			ApprovalStatus status = ApprovalStatus.Approved,
			bool isDeleted = false,
			ChallengeType challengeType = ChallengeType.Standard,
			string decryptedText = "hello world",
			int points = 100,
			List<UserSolution>? solutions = null,
			List<HintRequest>? hints = null,
			List<CipherTag>? tags = null) => new()
			{
				Id = id,
				Title = title,
				CreatedByUserId = userId,
				Status = status,
				IsDeleted = isDeleted,
				ChallengeType = challengeType,
				DecryptedText = decryptedText,
				EncryptedText = "encrypted",
				Points = points,
				CreatedAt = DateTime.UtcNow,
				MLPrediction = "",
				UserSolutions = solutions ?? new List<UserSolution>(),
				HintsRequested = hints ?? new List<HintRequest>(),
				CipherTags = tags ?? new List<CipherTag>(),
				AnswerSuggestions = new List<AnswerSuggestion>(),
			};

		private static ApplicationUser MakeUser(
			string id = "u1",
			int score = 0,
			List<UserSolution>? cipherAnswers = null) => new()
			{
				Id = id,
				UserName = "testuser",
				Score = score,
				CipherAnswers = cipherAnswers ?? new List<UserSolution>(),
				AttemptedCiphers = 0,
			};

		private static CipherFilter NoFilter() => new CipherFilter
		{
			SearchTerm = null,
			Tags = null,
			OrderTerm = CipherOrderTerm.Newest,
		};

		private void SetupAttachedCiphers(params Cipher[] ciphers)
		{
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(ciphers.AsQueryable().BuildMock());
		}

		#region GetApprovedAsync

		[Fact]
		public async Task GetApprovedAsync_ReturnsEmptyList_WhenNoCiphersExist()
		{
			SetupAttachedCiphers();

			var result = await service.GetApprovedAsync(NoFilter(), "u1");

			Assert.Empty(result);
		}

		[Fact]
		public async Task GetApprovedAsync_ReturnsOnlyApprovedAndNotDeleted()
		{
			SetupAttachedCiphers(
				MakeCipher(1, status: ApprovalStatus.Approved, isDeleted: false),
				MakeCipher(2, status: ApprovalStatus.Pending, isDeleted: false),
				MakeCipher(3, status: ApprovalStatus.Approved, isDeleted: true));

			var result = await service.GetApprovedAsync(NoFilter(), "u1");

			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_FiltersCorrectly_BySearchTerm()
		{
			SetupAttachedCiphers(
				MakeCipher(1, title: "Caesar Challenge"),
				MakeCipher(2, title: "Vigenere Puzzle"));

			var result = await service.GetApprovedAsync(
				new CipherFilter { SearchTerm = "caesar" }, "u1");

			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_FiltersCorrectly_ByStandardChallengeType()
		{
			SetupAttachedCiphers(
				MakeCipher(1, challengeType: ChallengeType.Standard),
				MakeCipher(2, challengeType: ChallengeType.Experimental));

			var result = await service.GetApprovedAsync(
				new CipherFilter { ChallengeType = ChallengeType.Standard }, "u1");

			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_FiltersCorrectly_ByExperimentalChallengeType()
		{
			SetupAttachedCiphers(
				MakeCipher(1, challengeType: ChallengeType.Standard),
				MakeCipher(2, challengeType: ChallengeType.Experimental));

			var result = await service.GetApprovedAsync(
				new CipherFilter { ChallengeType = ChallengeType.Experimental }, "u1");

			Assert.Single(result);
			Assert.Equal(2, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_OrdersCorrectly_ByNewest()
		{
			var older = MakeCipher(1);
			older.CreatedAt = DateTime.UtcNow.AddDays(-2);
			var newer = MakeCipher(2);
			newer.CreatedAt = DateTime.UtcNow;

			SetupAttachedCiphers(older, newer);

			var result = await service.GetApprovedAsync(
				new CipherFilter { OrderTerm = CipherOrderTerm.Newest }, "u1");

			Assert.Equal(2, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_OrdersCorrectly_ByOldest()
		{
			var older = MakeCipher(1);
			older.CreatedAt = DateTime.UtcNow.AddDays(-2);
			var newer = MakeCipher(2);
			newer.CreatedAt = DateTime.UtcNow;

			SetupAttachedCiphers(older, newer);

			var result = await service.GetApprovedAsync(
				new CipherFilter { OrderTerm = CipherOrderTerm.Oldest }, "u1");

			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_OrdersCorrectly_ByMostPopular()
		{
			var unpopular = MakeCipher(1, solutions: new List<UserSolution>());
			var popular = MakeCipher(2, solutions: new List<UserSolution>
			{
				new UserSolution { UserId = "u2", IsCorrect = true },
				new UserSolution { UserId = "u3", IsCorrect = true },
			});

			SetupAttachedCiphers(unpopular, popular);

			var result = await service.GetApprovedAsync(
				new CipherFilter { OrderTerm = CipherOrderTerm.MostPopular }, "u1");

			Assert.Equal(2, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_SetsAlreadySolved_WhenUserHasCorrectSolution()
		{
			var cipher = MakeCipher(1, solutions: new List<UserSolution>
			{
				new UserSolution { UserId = "u1", IsCorrect = true }
			});
			SetupAttachedCiphers(cipher);

			var result = await service.GetApprovedAsync(NoFilter(), "u1");

			Assert.True(result[0].AlreadySolved);
		}

		[Fact]
		public async Task GetApprovedAsync_DoesNotSetAlreadySolved_WhenUserHasNoCorrectSolution()
		{
			var cipher = MakeCipher(1, solutions: new List<UserSolution>
			{
				new UserSolution { UserId = "u1", IsCorrect = false }
			});
			SetupAttachedCiphers(cipher);

			var result = await service.GetApprovedAsync(NoFilter(), "u1");

			Assert.False(result[0].AlreadySolved);
		}

		#endregion

		#region GetCipherAsync

		[Fact]
		public async Task GetCipherAsync_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.GetCipherAsync(99, "u1"));
		}

		[Fact]
		public async Task GetCipherAsync_Throws_WhenCipherIsNotApproved()
		{
			SetupAttachedCiphers(MakeCipher(1, status: ApprovalStatus.Pending));

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.GetCipherAsync(1, "u1"));
		}

		[Fact]
		public async Task GetCipherAsync_Throws_WhenCipherIsDeleted()
		{
			SetupAttachedCiphers(MakeCipher(1, isDeleted: true));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.GetCipherAsync(1, "u1"));
		}

		#endregion

		#region SolveCipherAsync

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.SolveCipherAsync("u1", "hello world", 99));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenUserIsOwner()
		{
			SetupAttachedCiphers(MakeCipher(1, userId: "u1"));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SolveCipherAsync("u1", "hello world", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenCipherIsExperimental()
		{
			SetupAttachedCiphers(MakeCipher(1, challengeType: ChallengeType.Experimental));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SolveCipherAsync("u1", "hello world", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenUserAlreadySolvedCorrectly()
		{
			var cipher = MakeCipher(1, solutions: new List<UserSolution>
			{
				new UserSolution { UserId = "u1", IsCorrect = true }
			});
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SolveCipherAsync("u1", "hello world", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenUserNotFound()
		{
			SetupAttachedCiphers(MakeCipher(1));
			userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<Exception>(
				() => service.SolveCipherAsync("u1", "hello world", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_ReturnsTrue_WhenAnswerIsCorrect()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "hello world"));
			userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(MakeUser("u1"));

			var result = await service.SolveCipherAsync("u1", "hello world", 1);

			Assert.True(result);
		}

		[Fact]
		public async Task SolveCipherAsync_ReturnsFalse_WhenAnswerIsWrong()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "hello world"));
			userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(MakeUser("u1"));

			var result = await service.SolveCipherAsync("u1", "wrong answer", 1);

			Assert.False(result);
		}

		[Fact]
		public async Task SolveCipherAsync_IsCaseInsensitive_WhenComparingAnswers()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "Hello World"));
			userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(MakeUser("u1"));

			var result = await service.SolveCipherAsync("u1", "HELLO WORLD", 1);

			Assert.True(result);
		}

		[Fact]
		public async Task SolveCipherAsync_IgnoresWhitespace_WhenComparingAnswers()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "  hello world  "));
			userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(MakeUser("u1"));

			var result = await service.SolveCipherAsync("u1", "hello world", 1);

			Assert.True(result);
		}

		[Fact]
		public async Task SolveCipherAsync_AddsPoints_WhenAnswerIsCorrect()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "hello world", points: 100));
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.SolveCipherAsync("u1", "hello world", 1);

			Assert.True(user.Score > 0);
		}

		[Fact]
		public async Task SolveCipherAsync_DoesNotAddPoints_WhenAnswerIsWrong()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "hello world", points: 100));
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.SolveCipherAsync("u1", "wrong answer", 1);

			Assert.Equal(0, user.Score);
		}

		[Fact]
		public async Task SolveCipherAsync_IncrementsAttemptedCiphers_WhenFirstAttempt()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "hello world"));
			var user = MakeUser("u1", cipherAnswers: new List<UserSolution>());
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.SolveCipherAsync("u1", "wrong answer", 1);

			Assert.Equal(1, user.AttemptedCiphers);
		}

		[Fact]
		public async Task SolveCipherAsync_DoesNotIncrementAttemptedCiphers_WhenAlreadyAttempted()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "hello world"));
			var user = MakeUser("u1", cipherAnswers: new List<UserSolution>
			{
				new UserSolution { CipherId = 1, IsCorrect = false }
			});
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.SolveCipherAsync("u1", "wrong answer", 1);

			Assert.Equal(0, user.AttemptedCiphers);
		}

		[Fact]
		public async Task SolveCipherAsync_TracksHintUsage_WhenTypeHintWasUsed()
		{
			var cipher = MakeCipher(1, decryptedText: "hello world", hints: new List<HintRequest>
			{
				new HintRequest { UserId = "u1", HintType = HintType.Type }
			});
			SetupAttachedCiphers(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(MakeUser("u1"));
			UserSolution? captured = null;
			solutionRepoMock.Setup(r => r.AddAsync(It.IsAny<UserSolution>()))
				.Callback<UserSolution>(s => captured = s)
				.Returns(Task.CompletedTask);

			await service.SolveCipherAsync("u1", "hello world", 1);

			Assert.NotNull(captured);
			Assert.True(captured.UsedTypeHint);
			Assert.False(captured.UsedSolutionHint);
			Assert.False(captured.UsedFullSolution);
		}

		[Fact]
		public async Task SolveCipherAsync_SavesSolution_WithCorrectFields()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "hello world"));
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(MakeUser("u1"));
			UserSolution? captured = null;
			solutionRepoMock.Setup(r => r.AddAsync(It.IsAny<UserSolution>()))
				.Callback<UserSolution>(s => captured = s)
				.Returns(Task.CompletedTask);

			await service.SolveCipherAsync("u1", "hello world", 1);

			Assert.NotNull(captured);
			Assert.Equal("u1", captured.UserId);
			Assert.Equal(1, captured.CipherId);
			Assert.True(captured.IsCorrect);
		}

		[Fact]
		public async Task SolveCipherAsync_CallsUpdateAsync_OnUser()
		{
			SetupAttachedCiphers(MakeCipher(1, decryptedText: "hello world"));
			var user = MakeUser("u1");
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await service.SolveCipherAsync("u1", "hello world", 1);

			userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
		}

		#endregion
	}
}