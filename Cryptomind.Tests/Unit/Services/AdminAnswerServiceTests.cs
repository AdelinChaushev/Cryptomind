using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.AdminViewModels;
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
	public class ConcreteCipher : Cipher { }
	public class AdminAnswerServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> _cipherRepoMock = new();
		private readonly Mock<IRepository<AnswerSuggestion, int>> _answerRepoMock = new();
		private readonly Mock<IRepository<UserSolution, int>> _solutionRepoMock = new();
		private readonly Mock<INotificationService> _notificationMock = new();
		private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
		private readonly AdminAnswerService _service;

		public AdminAnswerServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			_userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			_service = new AdminAnswerService(
				_cipherRepoMock.Object,
				_answerRepoMock.Object,
				_solutionRepoMock.Object,
				_notificationMock.Object,
				_userManagerMock.Object);

			_answerRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AnswerSuggestion>())).ReturnsAsync(true);
			_cipherRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cipher>())).ReturnsAsync(true);
			_solutionRepoMock.Setup(r => r.AddAsync(It.IsAny<UserSolution>())).Returns(Task.CompletedTask);
			_userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
			_notificationMock.Setup(n => n.CreateAndSendNotification(
				It.IsAny<string>(), It.IsAny<NotificationType>(),
				It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Task.CompletedTask);
		}

		private static AnswerSuggestion Answer(int id, string userId, int cipherId,
			ApprovalStatus status, string text = "hello world",
			DateTime? uploadedAt = null) => new()
			{
				Id = id,
				UserId = userId,
				CipherId = cipherId,
				Status = status,
				DecryptedText = text,
				UplodaedTime = uploadedAt ?? DateTime.UtcNow,
				Cipher = new ConcreteCipher { Id = cipherId, Title = "Test Cipher" },
				ApplicationUser = new ApplicationUser { Id = userId }
			};

		private static ConcreteCipher Cipher(int id, ChallengeType type,
			string? decryptedText = null, int points = 100) => new()
			{
				Id = id,
				ChallengeType = type,
				DecryptedText = decryptedText,
				Points = points,
			};

		private static ApplicationUser User(string id, string userName = "testuser") => new()
		{
			Id = id,
			UserName = userName,
			Score = 0,
		};

		private void SetupUsers(params ApplicationUser[] users)
		{
			var mock = users.AsQueryable().BuildMock();
			_userManagerMock.Setup(m => m.Users).Returns(mock);
		}

		// -------------------------------------------------------------------------
		// GetPendingAnswersCount
		// -------------------------------------------------------------------------

		[Fact]
		public async Task GetPendingAnswersCount_ReturnsOnlyPendingOnes()
		{
			_answerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AnswerSuggestion>
			{
				Answer(1, "u1", 1, ApprovalStatus.Pending),
				Answer(2, "u2", 1, ApprovalStatus.Pending),
				Answer(3, "u3", 1, ApprovalStatus.Approved),
				Answer(4, "u4", 1, ApprovalStatus.Rejected),
			});

			var result = await _service.GetPendingAnswersCount();

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetPendingAnswersCount_ReturnsZero_WhenNoAnswersExist()
		{
			_answerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AnswerSuggestion>());

			var result = await _service.GetPendingAnswersCount();

			Assert.Equal(0, result);
		}

		// -------------------------------------------------------------------------
		// GetApprovedAnswersCount
		// -------------------------------------------------------------------------

		[Fact]
		public async Task GetApprovedAnswersCount_ReturnsOnlyApprovedOnes()
		{
			_answerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AnswerSuggestion>
			{
				Answer(1, "u1", 1, ApprovalStatus.Approved),
				Answer(2, "u2", 1, ApprovalStatus.Pending),
				Answer(3, "u3", 1, ApprovalStatus.Rejected),
			});

			var result = await _service.GetApprovedAnswersCount();

			Assert.Equal(1, result);
		}

		// -------------------------------------------------------------------------
		// AllSubmittedAnswersAsync
		// -------------------------------------------------------------------------

		[Fact]
		public async Task AllSubmittedAnswersAsync_ReturnsPendingAnswersOnly_WithCorrectUsername()
		{
			var answers = new List<AnswerSuggestion>
			{
				Answer(1, "u1", 1, ApprovalStatus.Pending),
				Answer(2, "u2", 1, ApprovalStatus.Approved), // should be excluded
			};

			_answerRepoMock.Setup(r => r.GetAllAttached()).Returns(answers.AsQueryable().BuildMock());
			SetupUsers(User("u1", "alice"));

			var result = await _service.AllSubmittedAnswersAsync(null, null);

			Assert.Single(result);
			Assert.Equal("alice", result[0].Username);
		}

		[Fact]
		public async Task AllSubmittedAnswersAsync_Throws_WhenUserDoesNotExist()
		{
			// The service uses GetAllAttached(), not GetAllAsync(), so we mock that.
			var answers = new List<AnswerSuggestion>
			{
				Answer(1, "ghost", 1, ApprovalStatus.Pending),
			};

			_answerRepoMock.Setup(r => r.GetAllAttached()).Returns(answers.AsQueryable().BuildMock());
			SetupUsers(); // no users in the system

			await Assert.ThrowsAsync<Exception>(
				() => _service.AllSubmittedAnswersAsync(null, null));
		}

		// -------------------------------------------------------------------------
		// GetAnswerById
		// -------------------------------------------------------------------------

		[Fact]
		public async Task GetAnswerById_ReturnsViewModel_WhenAnswerIsPending()
		{
			_answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 5, ApprovalStatus.Pending, "decrypted text"));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(User("u1", "bob"));

			var result = await _service.GetAnswerById(1);

			Assert.Equal("bob", result.Username);
			Assert.Equal("decrypted text", result.DecryptedText);
			Assert.Equal(5, result.CipherId);
		}

		[Fact]
		public async Task GetAnswerById_Throws_WhenAnswerNotFound()
		{
			_answerRepoMock.Setup(r => r.GetByIdAsync(99))
				.ReturnsAsync((AnswerSuggestion?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => _service.GetAnswerById(99));
		}

		[Fact]
		public async Task GetAnswerById_Throws_WhenAnswerIsAlreadyApproved()
		{
			_answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => _service.GetAnswerById(1));
		}

		[Fact]
		public async Task GetAnswerById_Throws_WhenAnswerIsAlreadyRejected()
		{
			_answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Rejected));

			await Assert.ThrowsAsync<ConflictException>(
				() => _service.GetAnswerById(1));
		}

		[Fact]
		public async Task GetAnswerById_Throws_WhenUserNotFound()
		{
			_answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Pending));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<Exception>(
				() => _service.GetAnswerById(1));
		}

		// -------------------------------------------------------------------------
		// ApproveAnswerAsync
		// -------------------------------------------------------------------------

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenAnswerNotFound()
		{
			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync((AnswerSuggestion?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => _service.ApproveAnswerAsync(1, 50));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenAnswerIsAlreadyApproved()
		{
			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => _service.ApproveAnswerAsync(1, 50));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenPointsAreZeroOrNegative()
		{
			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Pending));

			await Assert.ThrowsAsync<ValidationException>(
				() => _service.ApproveAnswerAsync(1, 0));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenCipherNotFound()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<Exception>(
				() => _service.ApproveAnswerAsync(1, 50));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenCipherIsStandard()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(Cipher(1, ChallengeType.Standard));

			await Assert.ThrowsAsync<ConflictException>(
				() => _service.ApproveAnswerAsync(1, 50));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenCipherAlreadyHasDecryptedText()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(Cipher(1, ChallengeType.Experimental, "already solved"));

			await Assert.ThrowsAsync<ConflictException>(
				() => _service.ApproveAnswerAsync(1, 50));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenFirstUserNotFound()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(Cipher(1, ChallengeType.Experimental));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<Exception>(
				() => _service.ApproveAnswerAsync(1, 50));
		}

		[Fact]
		public async Task ApproveAnswerAsync_GrantsFullPoints_ToFirstCorrectSubmitter()
		{
			// bonus (50) + cipher.Points (100) = 150
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");
			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);
			var user = User("u1");

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			var returnedIds = await _service.ApproveAnswerAsync(1, 50);

			Assert.Equal(150, user.Score);
			Assert.Contains("u1", returnedIds);
		}

		[Fact]
		public async Task ApproveAnswerAsync_UpdatesCipher_ToStandardAndSetsDecryptedText()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "the answer");
			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await _service.ApproveAnswerAsync(1, 50);

			Assert.Equal("the answer", cipher.DecryptedText);
			Assert.Equal(ChallengeType.Standard, cipher.ChallengeType);
		}

		[Fact]
		public async Task ApproveAnswerAsync_GivesReducedPoints_ToOtherCorrectSubmitters()
		{
			// u1 submitted first (older timestamp), u2 submitted the same correct answer later
			var first = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello",
				DateTime.UtcNow.AddMinutes(-10));
			var second = Answer(2, "u2", 1, ApprovalStatus.Pending, "hello",
				DateTime.UtcNow);

			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);
			var user1 = User("u1");
			var user2 = User("u2");

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(first);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { first, second });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user1);
			_userManagerMock.Setup(m => m.FindByIdAsync("u2")).ReturnsAsync(user2);

			await _service.ApproveAnswerAsync(1, 50);

			// first submitter: 50 + 100 = 150; second submitter: cipher.Points only = 100
			Assert.Equal(150, user1.Score);
			Assert.Equal(100, user2.Score);
		}

		[Fact]
		public async Task ApproveAnswerAsync_RejectsAllWrongAnswers()
		{
			var correct = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");
			var wrong = Answer(2, "u2", 1, ApprovalStatus.Pending, "wrong answer");

			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(correct);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { correct, wrong });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await _service.ApproveAnswerAsync(1, 50);

			Assert.Equal(ApprovalStatus.Rejected, wrong.Status);
			Assert.NotNull(wrong.RejectionDate);
		}

		[Fact]
		public async Task ApproveAnswerAsync_SendsApprovalNotification_ToFirstSubmitter()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");
			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await _service.ApproveAnswerAsync(1, 50);

			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.AnswerApproved,
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task ApproveAnswerAsync_ReturnsAllCorrectUserIds()
		{
			var first = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello", DateTime.UtcNow.AddMinutes(-5));
			var second = Answer(2, "u2", 1, ApprovalStatus.Pending, "hello", DateTime.UtcNow);

			_answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(first);
			_answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { first, second });
			_cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(Cipher(1, ChallengeType.Experimental, null, 100));
			_userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			_userManagerMock.Setup(m => m.FindByIdAsync("u2")).ReturnsAsync(User("u2"));

			var result = await _service.ApproveAnswerAsync(1, 50);

			Assert.Contains("u1", result);
			Assert.Contains("u2", result);
		}

		// -------------------------------------------------------------------------
		// RejectAnswerAsync
		// -------------------------------------------------------------------------

		[Fact]
		public async Task RejectAnswerAsync_Throws_WhenAnswerNotFound()
		{
			_answerRepoMock.Setup(r => r.GetByIdAsync(99))
				.ReturnsAsync((AnswerSuggestion?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => _service.RejectAnswerAsync(99, "bad answer"));
		}

		[Fact]
		public async Task RejectAnswerAsync_Throws_WhenAnswerIsAlreadyApproved()
		{
			_answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => _service.RejectAnswerAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectAnswerAsync_SetsStatusToRejected_WithCorrectReason()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending);
			_answerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(answer);

			await _service.RejectAnswerAsync(1, "not good enough");

			Assert.Equal(ApprovalStatus.Rejected, answer.Status);
			Assert.Equal("not good enough", answer.RejectionReason);
			Assert.NotNull(answer.RejectionDate);
		}

		[Fact]
		public async Task RejectAnswerAsync_SendsRejectionNotification_ToSubmitter()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending);
			_answerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(answer);

			await _service.RejectAnswerAsync(1, "not good enough");

			_notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.AnswerRejected,
				"not good enough",
				It.IsAny<string>()), Times.Once);
		}
	}
}