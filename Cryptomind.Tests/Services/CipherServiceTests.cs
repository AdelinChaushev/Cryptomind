using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
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

namespace Cryptomind.Tests.Services
{
	public class CipherServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> _cipherRepoMock = new();
		private readonly Mock<IRepository<UserSolution, int>> _solutionRepoMock = new();
		private readonly Mock<IRepository<AnswerSuggestion, int>> _answerRepoMock = new();
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly CipherService _service;

		public CipherServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			_service = new CipherService(
				_cipherRepoMock.Object,
				_solutionRepoMock.Object,
				_answerRepoMock.Object,
				_userManagerMock.Object);

			_solutionRepoMock.Setup(r => r.AddAsync(It.IsAny<UserSolution>()))
				.Returns(Task.CompletedTask);
		}

		private static ConcreteCipher MakeCipher(int id, ChallengeType challengeType,
			string? decryptedText = "HELLO", int points = 100,
			List<UserSolution>? userSolutions = null,
			List<HintRequest>? hintsRequested = null,
			string createdByUserId = "owner") => new()
			{
				Id = id,
				Status = ApprovalStatus.Approved,
				IsDeleted = false,
				ChallengeType = challengeType,
				DecryptedText = decryptedText,
				Points = points,
				Title = $"Cipher {id}",
				EncryptedText = "KHOOR",
				UserSolutions = userSolutions ?? new List<UserSolution>(),
				HintsRequested = hintsRequested ?? new List<HintRequest>(),
				CipherTags = new List<CipherTag>(),
				CreatedByUserId = createdByUserId,
				CreatedAt = DateTime.UtcNow,
			};

		private static ApplicationUser MakeUser(string id, int score = 0, int solvedCount = 0) => new()
		{
			Id = id,
			Score = score,
			SolvedCount = solvedCount,
		};

		private void SetupAttachedCiphers(params Cipher[] ciphers)
		{
			var mock = new List<Cipher>(ciphers).AsQueryable().BuildMock();
			_cipherRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region GetApprovedAsync

		[Fact]
		public async Task GetApprovedAsync_ReturnsOnlyApprovedNonDeleted()
		{
			var pending = MakeCipher(3, ChallengeType.Standard);
			pending.Status = ApprovalStatus.Pending;
			var deleted = MakeCipher(4, ChallengeType.Standard);
			deleted.IsDeleted = true;

			SetupAttachedCiphers(
				MakeCipher(1, ChallengeType.Standard),
				MakeCipher(2, ChallengeType.Standard),
				pending,
				deleted);

			var result = await _service.GetApprovedAsync(new CipherFilter(), "u1");

			Assert.Equal(2, result.Count);
		}

		[Fact]
		public async Task GetApprovedAsync_FiltersBySearchTerm()
		{
			var cipher1 = MakeCipher(1, ChallengeType.Standard);
			cipher1.Title = "Mystery Cipher";
			var cipher2 = MakeCipher(2, ChallengeType.Standard);
			cipher2.Title = "Easy Puzzle";

			SetupAttachedCiphers(cipher1, cipher2);

			var result = await _service.GetApprovedAsync(new CipherFilter { SearchTerm = "Mystery" }, "u1");

			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_FiltersByChallengeTypeStandard()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ChallengeType.Standard),
				MakeCipher(2, ChallengeType.Experimental));

			var result = await _service.GetApprovedAsync(new CipherFilter { ChallengeType = ChallengeType.Standard }, "u1");

			Assert.Single(result);
			Assert.Equal(1, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_FiltersByChallengeTypeExperimental()
		{
			SetupAttachedCiphers(
				MakeCipher(1, ChallengeType.Standard),
				MakeCipher(2, ChallengeType.Experimental));

			var result = await _service.GetApprovedAsync(new CipherFilter { ChallengeType = ChallengeType.Experimental }, "u1");

			Assert.Single(result);
			Assert.Equal(2, result[0].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_OrdersByNewest()
		{
			var old = MakeCipher(1, ChallengeType.Standard);
			old.CreatedAt = DateTime.UtcNow.AddDays(-2);
			var recent = MakeCipher(2, ChallengeType.Standard);
			recent.CreatedAt = DateTime.UtcNow.AddDays(-1);
			SetupAttachedCiphers(old, recent);

			var result = await _service.GetApprovedAsync(new CipherFilter { OrderTerm = CipherOrderTerm.Newest }, "u1");

			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_OrdersByOldest()
		{
			var old = MakeCipher(1, ChallengeType.Standard);
			old.CreatedAt = DateTime.UtcNow.AddDays(-2);
			var recent = MakeCipher(2, ChallengeType.Standard);
			recent.CreatedAt = DateTime.UtcNow.AddDays(-1);
			SetupAttachedCiphers(old, recent);

			var result = await _service.GetApprovedAsync(new CipherFilter { OrderTerm = CipherOrderTerm.Oldest }, "u1");

			Assert.Equal(1, result[0].Id);
			Assert.Equal(2, result[1].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_OrdersByMostPopular()
		{
			var lessPopular = MakeCipher(1, ChallengeType.Standard, userSolutions: new List<UserSolution> { new() });
			var morePopular = MakeCipher(2, ChallengeType.Standard, userSolutions: new List<UserSolution> { new(), new() });
			SetupAttachedCiphers(lessPopular, morePopular);

			var result = await _service.GetApprovedAsync(new CipherFilter { OrderTerm = CipherOrderTerm.MostPopular }, "u1");

			Assert.Equal(2, result[0].Id);
			Assert.Equal(1, result[1].Id);
		}

		[Fact]
		public async Task GetApprovedAsync_MarksAlreadySolved_WhenUserHasSolved()
		{
			var cipher = MakeCipher(1, ChallengeType.Standard, userSolutions: new List<UserSolution>
			{
				new() { UserId = "u1", IsCorrect = true }
			});
			SetupAttachedCiphers(cipher);

			var result = await _service.GetApprovedAsync(new CipherFilter(), "u1");

			Assert.True(result[0].AlreadySolved);
		}

		#endregion

		#region GetCipherAsync

		[Fact]
		public async Task GetCipherAsync_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetCipherAsync(99, "u1"));
		}

		[Fact]
		public async Task GetCipherAsync_Throws_WhenCipherIsNotApproved()
		{
			var cipher = MakeCipher(1, ChallengeType.Standard);
			cipher.Status = ApprovalStatus.Pending;
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetCipherAsync(1, "u1"));
		}

		[Fact]
		public async Task GetCipherAsync_Throws_WhenCipherIsDeleted()
		{
			var cipher = MakeCipher(1, ChallengeType.Standard);
			cipher.IsDeleted = true;
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetCipherAsync(1, "u1"));
		}

		#endregion

		#region SolveCipherAsync

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SolveCipherAsync("u1", "answer", 99));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenCipherIsNotApproved()
		{
			var cipher = MakeCipher(1, ChallengeType.Standard);
			cipher.Status = ApprovalStatus.Pending;
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SolveCipherAsync("u1", "answer", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenCipherIsDeleted()
		{
			var cipher = MakeCipher(1, ChallengeType.Standard);
			cipher.IsDeleted = true;
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SolveCipherAsync("u1", "answer", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenUserIsOwner()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, createdByUserId: "u1"));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SolveCipherAsync("u1", "answer", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenCipherIsExperimental()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Experimental));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SolveCipherAsync("u1", "answer", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenUserAlreadySolvedCipher()
		{
			var cipher = MakeCipher(1, ChallengeType.Standard, userSolutions: new List<UserSolution>
			{
				new() { UserId = "u1", IsCorrect = true }
			});
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SolveCipherAsync("u1", "HELLO", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_ReturnsFalse_WhenAnswerIsIncorrect()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO"));

			var result = await _service.SolveCipherAsync("u1", "WRONG", 1);

			Assert.False(result);
		}

		[Fact]
		public async Task SolveCipherAsync_SavesIncorrectSolution_WithZeroPoints()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO"));
			UserSolution captured = null;
			_solutionRepoMock.Setup(r => r.AddAsync(It.IsAny<UserSolution>()))
				.Callback<UserSolution>(s => captured = s)
				.Returns(Task.CompletedTask);

			await _service.SolveCipherAsync("u1", "WRONG", 1);

			Assert.NotNull(captured);
			Assert.False(captured.IsCorrect);
			Assert.Equal(0, captured.PointsEarned);
		}

		[Fact]
		public async Task SolveCipherAsync_ReturnsTrue_WhenAnswerIsCorrect()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO", points: 100));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(MakeUser("u1"));

			var result = await _service.SolveCipherAsync("u1", "HELLO", 1);

			Assert.True(result);
		}

		[Fact]
		public async Task SolveCipherAsync_IsCaseInsensitive()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO"));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(MakeUser("u1"));

			var result = await _service.SolveCipherAsync("u1", "hello", 1);

			Assert.True(result);
		}

		[Fact]
		public async Task SolveCipherAsync_TrimsWhitespace()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO"));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(MakeUser("u1"));

			var result = await _service.SolveCipherAsync("u1", "  HELLO  ", 1);

			Assert.True(result);
		}

		[Fact]
		public async Task SolveCipherAsync_AwardsFullPoints_WhenNoHintsUsed()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO", points: 100));
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.SolveCipherAsync("u1", "HELLO", 1);

			Assert.Equal(100, user.Score);
		}

		[Fact]
		public async Task SolveCipherAsync_Applies20PercentPenalty_WhenTypeHintUsed()
		{
			var hints = new List<HintRequest>
			{
				new() { UserId = "u1", HintType = HintType.Type }
			};
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO", points: 100, hintsRequested: hints));
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.SolveCipherAsync("u1", "HELLO", 1);

			Assert.Equal(80, user.Score);
		}

		[Fact]
		public async Task SolveCipherAsync_Applies30PercentPenalty_WhenSolutionHintUsed()
		{
			var hints = new List<HintRequest>
			{
				new() { UserId = "u1", HintType = HintType.Hint }
			};
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO", points: 100, hintsRequested: hints));
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.SolveCipherAsync("u1", "HELLO", 1);

			Assert.Equal(70, user.Score);
		}

		[Fact]
		public async Task SolveCipherAsync_Applies40PercentPenalty_WhenFullSolutionUsed()
		{
			var hints = new List<HintRequest>
			{
				new() { UserId = "u1", HintType = HintType.FullSolution }
			};
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO", points: 100, hintsRequested: hints));
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.SolveCipherAsync("u1", "HELLO", 1);

			Assert.Equal(60, user.Score);
		}

		[Fact]
		public async Task SolveCipherAsync_AppliesCumulativePenalties_WhenMultipleHintsUsed()
		{
			var hints = new List<HintRequest>
			{
				new() { UserId = "u1", HintType = HintType.Type },
				new() { UserId = "u1", HintType = HintType.Hint },
				new() { UserId = "u1", HintType = HintType.FullSolution }
			};
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO", points: 100, hintsRequested: hints));
			var user = MakeUser("u1");
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.SolveCipherAsync("u1", "HELLO", 1);

			Assert.Equal(9, user.Score);
		}

		[Fact]
		public async Task SolveCipherAsync_IncrementsSolvedCount_WhenCorrect()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO", points: 100));
			var user = MakeUser("u1", solvedCount: 5);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			await _service.SolveCipherAsync("u1", "HELLO", 1);

			Assert.Equal(6, user.SolvedCount);
		}

		[Fact]
		public async Task SolveCipherAsync_Throws_WhenUserNotFound()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO"));
			_userManagerMock.Setup(m => m.FindByIdAsync("ghost")).ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.SolveCipherAsync("ghost", "HELLO", 1));
		}

		[Fact]
		public async Task SolveCipherAsync_SavesCorrectSolution_WithHintFlags()
		{
			var hints = new List<HintRequest>
			{
				new() { UserId = "u1", HintType = HintType.Type },
				new() { UserId = "u1", HintType = HintType.Hint }
			};
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard, decryptedText: "HELLO", points: 100, hintsRequested: hints));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(MakeUser("u1"));

			UserSolution captured = null;
			_solutionRepoMock.Setup(r => r.AddAsync(It.IsAny<UserSolution>()))
				.Callback<UserSolution>(s => captured = s)
				.Returns(Task.CompletedTask);

			await _service.SolveCipherAsync("u1", "HELLO", 1);

			Assert.NotNull(captured);
			Assert.True(captured.IsCorrect);
			Assert.True(captured.UsedTypeHint);
			Assert.True(captured.UsedSolutionHint);
			Assert.False(captured.UsedFullSolution);
		}

		#endregion
	}
}