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
		private readonly Mock<IRepository<Cipher, int>> cipherRepoMock = new();
		private readonly Mock<IRepository<AnswerSuggestion, int>> answerRepoMock = new();
		private readonly Mock<IRepository<UserSolution, int>> solutionRepoMock = new();
		private readonly Mock<INotificationService> notificationMock = new();
		private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
		private readonly AdminAnswerService service;

		public AdminAnswerServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();
			userManagerMock = new Mock<UserManager<ApplicationUser>>(
				store.Object, null, null, null, null, null, null, null, null);

			service = new AdminAnswerService(
				cipherRepoMock.Object,
				answerRepoMock.Object,
				solutionRepoMock.Object,
				notificationMock.Object,
				userManagerMock.Object);

			answerRepoMock.Setup(r => r.UpdateAsync(It.IsAny<AnswerSuggestion>())).ReturnsAsync(true);
			cipherRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cipher>())).ReturnsAsync(true);
			solutionRepoMock.Setup(r => r.AddAsync(It.IsAny<UserSolution>())).Returns(Task.CompletedTask);
			userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
			userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
				.ReturnsAsync((string id) => User(id));
			notificationMock.Setup(n => n.CreateAndSendNotification(
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
			userManagerMock.Setup(m => m.Users).Returns(mock);
		}

		#region GetPendingAnswersCount

		[Fact]
		public async Task GetPendingAnswersCount_ReturnsOnlyPendingOnes()
		{
			answerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AnswerSuggestion>
			{
				Answer(1, "u1", 1, ApprovalStatus.Pending),
				Answer(2, "u2", 1, ApprovalStatus.Pending),
				Answer(3, "u3", 1, ApprovalStatus.Approved),
				Answer(4, "u4", 1, ApprovalStatus.Rejected),
			});

			var result = await service.GetPendingAnswersCount();

			Assert.Equal(2, result);
		}

		[Fact]
		public async Task GetPendingAnswersCount_ReturnsZero_WhenNoAnswersExist()
		{
			answerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AnswerSuggestion>());

			var result = await service.GetPendingAnswersCount();

			Assert.Equal(0, result);
		}

		#endregion

		#region GetApprovedAnswersCount

		[Fact]
		public async Task GetApprovedAnswersCount_ReturnsOnlyApprovedOnes()
		{
			answerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AnswerSuggestion>
			{
				Answer(1, "u1", 1, ApprovalStatus.Approved),
				Answer(2, "u2", 1, ApprovalStatus.Pending),
				Answer(3, "u3", 1, ApprovalStatus.Rejected),
			});

			var result = await service.GetApprovedAnswersCount();

			Assert.Equal(1, result);
		}

		#endregion

		#region AllSubmittedAnswersAsync

		[Fact]
		public async Task AllSubmittedAnswersAsync_ReturnsPendingAnswersOnly_WithCorrectUsername()
		{
			var answers = new List<AnswerSuggestion>
			{
				Answer(1, "u1", 1, ApprovalStatus.Pending),
				Answer(2, "u2", 1, ApprovalStatus.Approved),
			};

			answerRepoMock.Setup(r => r.GetAllAttached()).Returns(answers.AsQueryable().BuildMock());
			SetupUsers(User("u1", "alice"));

			var result = await service.AllSubmittedAnswersAsync(null, null);

			Assert.Single(result);
			Assert.Equal("alice", result[0].Username);
		}

		[Fact]
		public async Task AllSubmittedAnswersAsync_Throws_WhenUserDoesNotExist()
		{
			var answers = new List<AnswerSuggestion>
			{
				Answer(1, "ghost", 1, ApprovalStatus.Pending),
			};

			answerRepoMock.Setup(r => r.GetAllAttached()).Returns(answers.AsQueryable().BuildMock());
			SetupUsers();

			await Assert.ThrowsAsync<Exception>(
				() => service.AllSubmittedAnswersAsync(null, null));
		}

		#endregion

		#region GetAnswerById

		[Fact]
		public async Task GetAnswerById_ReturnsViewModel_WhenAnswerIsPending()
		{
			answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 5, ApprovalStatus.Pending, "decrypted text"));
			userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync(User("u1", "bob"));

			var result = await service.GetAnswerById(1);

			Assert.Equal("bob", result.Username);
			Assert.Equal("decrypted text", result.DecryptedText);
			Assert.Equal(5, result.CipherId);
		}

		[Fact]
		public async Task GetAnswerById_Throws_WhenAnswerNotFound()
		{
			answerRepoMock.Setup(r => r.GetByIdAsync(99))
				.ReturnsAsync((AnswerSuggestion?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.GetAnswerById(99));
		}

		[Fact]
		public async Task GetAnswerById_Throws_WhenAnswerIsAlreadyApproved()
		{
			answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.GetAnswerById(1));
		}

		[Fact]
		public async Task GetAnswerById_Throws_WhenAnswerIsAlreadyRejected()
		{
			answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Rejected));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.GetAnswerById(1));
		}

		[Fact]
		public async Task GetAnswerById_Throws_WhenUserNotFound()
		{
			answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Pending));
			userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<Exception>(
				() => service.GetAnswerById(1));
		}

		#endregion

		#region ApproveAnswerAsync

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenAnswerNotFound()
		{
			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync((AnswerSuggestion?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.ApproveAnswerAsync(1));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenAnswerIsAlreadyApproved()
		{
			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveAnswerAsync(1));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenCipherNotFound()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync((Cipher?)null);

			await Assert.ThrowsAsync<Exception>(
				() => service.ApproveAnswerAsync(1));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenCipherIsStandard()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(Cipher(1, ChallengeType.Standard));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveAnswerAsync(1));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenCipherAlreadyHasDecryptedText()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(Cipher(1, ChallengeType.Experimental, "already solved"));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.ApproveAnswerAsync(1));
		}

		[Fact]
		public async Task ApproveAnswerAsync_Throws_WhenFirstUserNotFound()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(Cipher(1, ChallengeType.Experimental));
			userManagerMock.Setup(m => m.FindByIdAsync("u1"))
				.ReturnsAsync((ApplicationUser?)null);

			await Assert.ThrowsAsync<Exception>(
				() => service.ApproveAnswerAsync(1));
		}

		[Fact]
		public async Task ApproveAnswerAsync_GrantsFullPoints_ToFirstCorrectSubmitter()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");
			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);
			var user = User("u1");

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user);

			var returnedIds = await service.ApproveAnswerAsync(1);

			Assert.Equal(200, user.Score);
			Assert.Contains("u1", returnedIds);
		}

		[Fact]
		public async Task ApproveAnswerAsync_UpdatesCipher_ToStandardAndSetsDecryptedText()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "the answer");
			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await service.ApproveAnswerAsync(1);

			Assert.Equal("the answer", cipher.DecryptedText);
			Assert.Equal(ChallengeType.Standard, cipher.ChallengeType);
		}

		[Fact]
		public async Task ApproveAnswerAsync_GivesReducedPoints_ToOtherCorrectSubmitters()
		{
			var first = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello",
				DateTime.UtcNow.AddMinutes(-10));
			var second = Answer(2, "u2", 1, ApprovalStatus.Pending, "hello",
				DateTime.UtcNow);

			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);
			var user1 = User("u1");
			var user2 = User("u2");

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(first);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { first, second });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(user1);
			userManagerMock.Setup(m => m.FindByIdAsync("u2")).ReturnsAsync(user2);

			await service.ApproveAnswerAsync(1);

			Assert.Equal(200, user1.Score);
			Assert.Equal(150, user2.Score);
		}

		[Fact]
		public async Task ApproveAnswerAsync_RejectsAllWrongAnswers()
		{
			var correct = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");
			var wrong = Answer(2, "u2", 1, ApprovalStatus.Pending, "wrong answer");
			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(correct);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { correct, wrong });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await service.ApproveAnswerAsync(1);

			Assert.Equal(ApprovalStatus.Rejected, wrong.Status);
			Assert.NotNull(wrong.RejectionDate);
		}

		[Fact]
		public async Task ApproveAnswerAsync_SendsApprovalNotification_ToFirstSubmitter()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending, "hello");
			var cipher = Cipher(1, ChallengeType.Experimental, null, 100);

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(answer);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { answer });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(cipher);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await service.ApproveAnswerAsync(1);

			notificationMock.Verify(n => n.CreateAndSendNotification(
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

			answerRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AnswerSuggestion, bool>>>()))
				.ReturnsAsync(first);
			answerRepoMock.Setup(r => r.GetAllAsync())
				.ReturnsAsync(new List<AnswerSuggestion> { first, second });
			cipherRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Cipher, bool>>>()))
				.ReturnsAsync(Cipher(1, ChallengeType.Experimental, null, 100));
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));
			userManagerMock.Setup(m => m.FindByIdAsync("u2")).ReturnsAsync(User("u2"));

			var result = await service.ApproveAnswerAsync(1);

			Assert.Contains("u1", result);
			Assert.Contains("u2", result);
		}

		#endregion

		#region RejectAnswerAsync

		[Fact]
		public async Task RejectAnswerAsync_Throws_WhenAnswerNotFound()
		{
			answerRepoMock.Setup(r => r.GetByIdAsync(99))
				.ReturnsAsync((AnswerSuggestion?)null);

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.RejectAnswerAsync(99, "bad answer"));
		}

		[Fact]
		public async Task RejectAnswerAsync_Throws_WhenAnswerIsAlreadyApproved()
		{
			answerRepoMock.Setup(r => r.GetByIdAsync(1))
				.ReturnsAsync(Answer(1, "u1", 1, ApprovalStatus.Approved));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RejectAnswerAsync(1, "reason"));
		}

		[Fact]
		public async Task RejectAnswerAsync_SetsStatusToRejected_WithCorrectReason()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending);
			answerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(answer);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await service.RejectAnswerAsync(1, "not good enough");

			Assert.Equal(ApprovalStatus.Rejected, answer.Status);
			Assert.Equal("not good enough", answer.RejectionReason);
			Assert.NotNull(answer.RejectionDate);
		}

		[Fact]
		public async Task RejectAnswerAsync_SendsRejectionNotification_ToSubmitter()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending);
			answerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(answer);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await service.RejectAnswerAsync(1, "not good enough");

			notificationMock.Verify(n => n.CreateAndSendNotification(
				"u1",
				NotificationType.AnswerRejected,
				"not good enough",
				It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task RejectAnswerAsync_Throws_WhenAnswerIsAlreadyRejected()
		{
			var answer = Answer(1, "u1", 1, ApprovalStatus.Pending);
			answerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(answer);
			userManagerMock.Setup(m => m.FindByIdAsync("u1")).ReturnsAsync(User("u1"));

			await service.RejectAnswerAsync(1, "not good enough");

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RejectAnswerAsync(1, "not good enough"));
		}

		#endregion
	}
}