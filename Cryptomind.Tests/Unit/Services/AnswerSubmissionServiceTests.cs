using Cryptomind.Common.DTOs;
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
	public class AnswerSubmissionServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> cipherRepoMock = new();
		private readonly Mock<IRepository<AnswerSuggestion, int>> answerRepoMock = new();
		private readonly AnswerSubmissionService service;

		private const string DateFormat = "ddd, dd MMM yyyy HH:mm";

		public AnswerSubmissionServiceTests()
		{
			var store = new Mock<IUserStore<ApplicationUser>>();

			service = new AnswerSubmissionService(
				cipherRepoMock.Object,
				answerRepoMock.Object);

			answerRepoMock.Setup(r => r.AddAsync(It.IsAny<AnswerSuggestion>()))
				.Returns(Task.CompletedTask);
		}

		private static ConcreteCipher MakeCipher(int id, ChallengeType challengeType,
			string? decryptedText = null, string createdByUserId = "owner",
			List<AnswerSuggestion>? answerSuggestions = null) => new()
			{
				Id = id,
				Status = ApprovalStatus.Approved,
				ChallengeType = challengeType,
				DecryptedText = decryptedText,
				CreatedByUserId = createdByUserId,
				Title = $"Cipher {id}",
				AnswerSuggestions = answerSuggestions ?? new List<AnswerSuggestion>(),
			};

		private static AnswerSuggestion MakeAnswer(int id, string userId, int cipherId,
			string decryptedText, ApprovalStatus status = ApprovalStatus.Pending,
			Cipher? cipher = null) => new()
			{
				Id = id,
				UserId = userId,
				CipherId = cipherId,
				DecryptedText = decryptedText,
				Status = status,
				UploadedTime = DateTime.UtcNow.AddHours(2),
				Cipher = cipher,
			};

		private void SetupAttachedCiphers(params Cipher[] ciphers)
		{
			var mock = new List<Cipher>(ciphers).AsQueryable().BuildMock();
			cipherRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		private void SetupAttachedAnswers(params AnswerSuggestion[] answers)
		{
			var mock = new List<AnswerSuggestion>(answers).AsQueryable().BuildMock();
			answerRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region SuggestAnswerAsync

		[Fact]
		public async Task SuggestAnswerAsync_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.SuggestAnswerAsync(new SuggestAnswerDTO { DecryptedText = "answer" }, "u1", 99));
		}

		[Fact]
		public async Task SuggestAnswerAsync_Throws_WhenCipherAlreadyHasDecryptedText()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Experimental, decryptedText: "already solved"));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SuggestAnswerAsync(new SuggestAnswerDTO { DecryptedText = "answer" }, "u1", 1));
		}

		[Fact]
		public async Task SuggestAnswerAsync_Throws_WhenCipherIsStandard()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Standard));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SuggestAnswerAsync(new SuggestAnswerDTO { DecryptedText = "answer" }, "u1", 1));
		}

		[Fact]
		public async Task SuggestAnswerAsync_Throws_WhenDecryptedTextIsEmpty()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Experimental));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SuggestAnswerAsync(new SuggestAnswerDTO { DecryptedText = "   " }, "u1", 1));
		}

		[Fact]
		public async Task SuggestAnswerAsync_Throws_WhenUserIsTheCipherOwner()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Experimental, createdByUserId: "u1"));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SuggestAnswerAsync(new SuggestAnswerDTO { DecryptedText = "answer" }, "u1", 1));
		}

		[Fact]
		public async Task SuggestAnswerAsync_Throws_WhenUserAlreadySuggestedSameAnswer()
		{
			var existingAnswer = MakeAnswer(1, "u1", 1, "hello world");
			var cipher = MakeCipher(1, ChallengeType.Experimental, answerSuggestions: new List<AnswerSuggestion> { existingAnswer });
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SuggestAnswerAsync(new SuggestAnswerDTO { DecryptedText = "Hello World" }, "u1", 1));
		}

		[Fact]
		public async Task SuggestAnswerAsync_Throws_WhenUserSuggestsSameAnswer_CaseInsensitive()
		{
			var existingAnswer = MakeAnswer(1, "u1", 1, "HELLO WORLD");
			var cipher = MakeCipher(1, ChallengeType.Experimental, answerSuggestions: new List<AnswerSuggestion> { existingAnswer });
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<ConflictException>(
				() => service.SuggestAnswerAsync(new SuggestAnswerDTO { DecryptedText = "hello world" }, "u1", 1));
		}

		[Fact]
		public async Task SuggestAnswerAsync_AllowsDifferentUserToSuggestSameAnswer()
		{
			var existingAnswer = MakeAnswer(1, "u1", 1, "hello world");
			var cipher = MakeCipher(1, ChallengeType.Experimental, answerSuggestions: new List<AnswerSuggestion> { existingAnswer });
			SetupAttachedCiphers(cipher);

			await service.SuggestAnswerAsync(new SuggestAnswerDTO { DecryptedText = "hello world" }, "u2", 1);

			answerRepoMock.Verify(r => r.AddAsync(It.IsAny<AnswerSuggestion>()), Times.Once);
		}

		[Fact]
		public async Task SuggestAnswerAsync_AddsAnswer_WithCorrectFields()
		{
			SetupAttachedCiphers(MakeCipher(1, ChallengeType.Experimental));
			AnswerSuggestion? captured = null;
			answerRepoMock.Setup(r => r.AddAsync(It.IsAny<AnswerSuggestion>()))
				.Callback<AnswerSuggestion>(a => captured = a)
				.Returns(Task.CompletedTask);

			await service.SuggestAnswerAsync(new SuggestAnswerDTO
			{
				DecryptedText = "my answer",
				Description = "my reasoning",
			}, "u1", 1);

			Assert.NotNull(captured);
			Assert.Equal("u1", captured.UserId);
			Assert.Equal(1, captured.CipherId);
			Assert.Equal("my answer", captured.DecryptedText);
			Assert.Equal("my reasoning", captured.Description);
			Assert.Equal(ApprovalStatus.Pending, captured.Status);
		}

		#endregion

		#region SubmittedAnswers

		[Fact]
		public async Task SubmittedAnswers_ReturnsEmptyList_WhenNoAnswersExist()
		{
			SetupAttachedAnswers();

			var result = await service.SubmittedAnswers("u1");

			Assert.Empty(result);
		}

		[Fact]
		public async Task SubmittedAnswers_ReturnsOnlyAnswers_ForGivenUser()
		{
			var cipher = MakeCipher(1, ChallengeType.Experimental);
			SetupAttachedAnswers(
				MakeAnswer(1, "u1", 1, "answer one", cipher: cipher),
				MakeAnswer(2, "u2", 1, "answer two", cipher: cipher));

			var result = await service.SubmittedAnswers("u1");

			Assert.Single(result);
			Assert.Equal("answer one", result[0].SuggestedAnswer);
		}

		[Fact]
		public async Task SubmittedAnswers_PopulatesPointsAndApprovedDate_WhenAnswerIsApproved()
		{
			var approvedDate = DateTime.UtcNow.AddHours(2).AddDays(-1);
			var cipher = MakeCipher(1, ChallengeType.Experimental);
			var answer = MakeAnswer(1, "u1", 1, "answer", ApprovalStatus.Approved, cipher: cipher);
			answer.PointsEarned = 150;
			answer.ApprovalDate = approvedDate;
			SetupAttachedAnswers(answer);

			var result = await service.SubmittedAnswers("u1");

			Assert.Equal(150, result[0].PointsEarned);
			Assert.Equal(approvedDate.ToString(DateFormat), result[0].ApprovedDate);
		}

		[Fact]
		public async Task SubmittedAnswers_PopulatesRejectionFields_WhenAnswerIsRejected()
		{
			var rejectionDate = DateTime.UtcNow.AddHours(2).AddDays(-1);
			var cipher = MakeCipher(1, ChallengeType.Experimental);
			var answer = MakeAnswer(1, "u1", 1, "wrong answer", ApprovalStatus.Rejected, cipher: cipher);
			answer.RejectionReason = "incorrect";
			answer.RejectionDate = rejectionDate;
			SetupAttachedAnswers(answer);

			var result = await service.SubmittedAnswers("u1");

			Assert.Equal("incorrect", result[0].RejectionReason);
			Assert.Equal(rejectionDate.ToString(DateFormat), result[0].RejectionDate);
		}

		[Fact]
		public async Task SubmittedAnswers_SetsStatusToCipherDeleted_WhenCipherIsDeleted()
		{
			var deletedAt = DateTime.UtcNow.AddHours(2).AddDays(-2);
			var cipher = MakeCipher(1, ChallengeType.Experimental);
			cipher.IsDeleted = true;
			cipher.DeletedAt = deletedAt;
			var answer = MakeAnswer(1, "u1", 1, "answer", cipher: cipher);
			SetupAttachedAnswers(answer);

			var result = await service.SubmittedAnswers("u1");

			Assert.Equal("CipherDeleted", result[0].Status);
			Assert.Equal(deletedAt.ToString(DateFormat), result[0].CipherDeletedAt);
		}

		[Fact]
		public async Task SubmittedAnswers_KeepsOriginalStatus_WhenCipherIsNotDeleted()
		{
			var cipher = MakeCipher(1, ChallengeType.Experimental);
			var answer = MakeAnswer(1, "u1", 1, "answer", ApprovalStatus.Pending, cipher: cipher);
			SetupAttachedAnswers(answer);

			var result = await service.SubmittedAnswers("u1");

			Assert.Equal("Pending", result[0].Status);
		}

		#endregion
	}
}