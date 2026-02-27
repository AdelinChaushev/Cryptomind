using Cryptomind.Common.Exceptions;
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
	public class HintServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> cipherRepoMock = new();
		private readonly Mock<IRepository<HintRequest, int>> hintRequestRepoMock = new();
		private readonly Mock<ILLMService> llmServiceMock = new();
		private readonly HintService service;

		public HintServiceTests()
		{
			service = new HintService(
				cipherRepoMock.Object,
				hintRequestRepoMock.Object,
				llmServiceMock.Object);

			hintRequestRepoMock.Setup(r => r.AddAsync(It.IsAny<HintRequest>()))
				.Returns(Task.CompletedTask);
			cipherRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cipher>()))
				.ReturnsAsync(true);
			llmServiceMock.Setup(l => l.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()))
				.ReturnsAsync("hint content from LLM");
		}

		private static TextCipher MakeCipher(
			int id = 1,
			string ownerId = "owner",
			int points = 100,
			bool allowTypeHint = true,
			bool allowHint = true,
			bool allowSolution = true,
			List<HintRequest>? hints = null,
			List<UserSolution>? solutions = null,
			CipherLLMData? llmData = null) => new()
			{
				Id = id,
				CreatedByUserId = ownerId,
				Status = ApprovalStatus.Approved,
				Points = points,
				AllowTypeHint = allowTypeHint,
				AllowHint = allowHint,
				AllowSolution = allowSolution,
				HintsRequested = hints ?? new List<HintRequest>(),
				UserSolutions = solutions ?? new List<UserSolution>(),
				EncryptedText = "encrypted",
				DecryptedText = "decrypted",
				MLPrediction = "",
				CipherTags = new List<CipherTag>(),
				AnswerSuggestions = new List<AnswerSuggestion>(),
				LLMData = llmData ?? new CipherLLMData(),
			};

		private static HintRequest MakeHintRequest(
			string userId, int cipherId, HintType hintType,
			string content = "cached hint") => new()
			{
				UserId = userId,
				CipherId = cipherId,
				HintType = hintType,
				HintContent = content,
				RequestedAt = DateTime.UtcNow,
			};

		private void SetupAttachedCiphers(params Cipher[] ciphers)
		{
			cipherRepoMock.Setup(r => r.GetAllAttached())
				.Returns(ciphers.AsQueryable().BuildMock());
		}

		#region RequestHintAsync - Guards

		[Fact]
		public async Task RequestHintAsync_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<NotFoundException>(
				() => service.RequestHintAsync("u1", 99, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenUserIsOwner()
		{
			SetupAttachedCiphers(MakeCipher(1, ownerId: "u1"));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RequestHintAsync("u1", 1, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenUserAlreadySolvedCipher()
		{
			var cipher = MakeCipher(1, solutions: new List<UserSolution>
			{
				new UserSolution { UserId = "u1", IsCorrect = true }
			});
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RequestHintAsync("u1", 1, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenTypeHintNotAllowed()
		{
			SetupAttachedCiphers(MakeCipher(1, allowTypeHint: false));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RequestHintAsync("u1", 1, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenSolutionHintNotAllowed()
		{
			SetupAttachedCiphers(MakeCipher(1, allowHint: false));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RequestHintAsync("u1", 1, HintType.Hint));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenFullSolutionNotAllowed()
		{
			SetupAttachedCiphers(MakeCipher(1, allowSolution: false));

			await Assert.ThrowsAsync<ConflictException>(
				() => service.RequestHintAsync("u1", 1, HintType.FullSolution));
		}

		#endregion

		#region RequestHintAsync - Returning cached existing hint

		[Fact]
		public async Task RequestHintAsync_ReturnsCachedHint_WhenUserAlreadyRequestedSameHint()
		{
			var existing = MakeHintRequest("u1", 1, HintType.Type, "already got this");
			var cipher = MakeCipher(1, hints: new List<HintRequest> { existing });
			SetupAttachedCiphers(cipher);

			var result = await service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal("already got this", result.HintContent);
			llmServiceMock.Verify(l => l.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()), Times.Never);
			hintRequestRepoMock.Verify(r => r.AddAsync(It.IsAny<HintRequest>()), Times.Never);
		}

		[Fact]
		public async Task RequestHintAsync_DoesNotSaveNewHintRequest_WhenReturningCachedHint()
		{
			var existing = MakeHintRequest("u1", 1, HintType.Hint, "cached solution hint");
			var cipher = MakeCipher(1, hints: new List<HintRequest> { existing });
			SetupAttachedCiphers(cipher);

			await service.RequestHintAsync("u1", 1, HintType.Hint);

			hintRequestRepoMock.Verify(r => r.AddAsync(It.IsAny<HintRequest>()), Times.Never);
		}

		#endregion

		#region RequestHintAsync - LLM hint generation

		[Fact]
		public async Task RequestHintAsync_CallsLLM_WhenNoCachedHintExists()
		{
			SetupAttachedCiphers(MakeCipher(1));

			await service.RequestHintAsync("u1", 1, HintType.Type);

			llmServiceMock.Verify(l => l.GetHint(It.IsAny<Cipher>(), HintType.Type), Times.Once);
		}

		[Fact]
		public async Task RequestHintAsync_UsesCachedLLMData_WhenCachedTypeHintExists()
		{
			var llmData = new CipherLLMData { CachedTypeHint = "cached type hint" };
			SetupAttachedCiphers(MakeCipher(1, llmData: llmData));

			var result = await service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal("cached type hint", result.HintContent);
			llmServiceMock.Verify(l => l.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()), Times.Never);
		}

		[Fact]
		public async Task RequestHintAsync_UsesCachedLLMData_WhenCachedSolutionHintExists()
		{
			var llmData = new CipherLLMData { CachedHint = "cached solution hint" };
			SetupAttachedCiphers(MakeCipher(1, llmData: llmData));

			var result = await service.RequestHintAsync("u1", 1, HintType.Hint);

			Assert.Equal("cached solution hint", result.HintContent);
			llmServiceMock.Verify(l => l.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()), Times.Never);
		}

		[Fact]
		public async Task RequestHintAsync_UsesCachedLLMData_WhenCachedFullSolutionExists()
		{
			var llmData = new CipherLLMData { CachedSolution = "cached full solution" };
			SetupAttachedCiphers(MakeCipher(1, llmData: llmData));

			var result = await service.RequestHintAsync("u1", 1, HintType.FullSolution);

			Assert.Equal("cached full solution", result.HintContent);
			llmServiceMock.Verify(l => l.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()), Times.Never);
		}

		[Fact]
		public async Task RequestHintAsync_SavesHintRequest_AfterLLMCall()
		{
			SetupAttachedCiphers(MakeCipher(1));

			await service.RequestHintAsync("u1", 1, HintType.Type);

			hintRequestRepoMock.Verify(r => r.AddAsync(It.Is<HintRequest>(h =>
				h.UserId == "u1" &&
				h.CipherId == 1 &&
				h.HintType == HintType.Type &&
				h.HintContent == "hint content from LLM"
			)), Times.Once);
		}

		[Fact]
		public async Task RequestHintAsync_UpdatesCipherRepo_AfterSavingHint()
		{
			var cipher = MakeCipher(1);
			SetupAttachedCiphers(cipher);

			await service.RequestHintAsync("u1", 1, HintType.Type);

			cipherRepoMock.Verify(r => r.UpdateAsync(cipher), Times.Once);
		}

		[Fact]
		public async Task RequestHintAsync_StoresLLMResponseInCachedData_ForTypeHint()
		{
			var cipher = MakeCipher(1);
			SetupAttachedCiphers(cipher);
			llmServiceMock.Setup(l => l.GetHint(cipher, HintType.Type))
				.ReturnsAsync("type hint from LLM");

			await service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal("type hint from LLM", cipher.LLMData.CachedTypeHint);
		}

		[Fact]
		public async Task RequestHintAsync_StoresLLMResponseInCachedData_ForSolutionHint()
		{
			var cipher = MakeCipher(1);
			SetupAttachedCiphers(cipher);
			llmServiceMock.Setup(l => l.GetHint(cipher, HintType.Hint))
				.ReturnsAsync("solution hint from LLM");

			await service.RequestHintAsync("u1", 1, HintType.Hint);

			Assert.Equal("solution hint from LLM", cipher.LLMData.CachedHint);
		}

		[Fact]
		public async Task RequestHintAsync_StoresLLMResponseInCachedData_ForFullSolution()
		{
			var cipher = MakeCipher(1);
			SetupAttachedCiphers(cipher);
			llmServiceMock.Setup(l => l.GetHint(cipher, HintType.FullSolution))
				.ReturnsAsync("full solution from LLM");

			await service.RequestHintAsync("u1", 1, HintType.FullSolution);

			Assert.Equal("full solution from LLM", cipher.LLMData.CachedSolution);
		}

		#endregion

		#region RequestHintAsync - Points penalty

		[Fact]
		public async Task RequestHintAsync_AppliesTypeHintPenalty_OnAvailablePoints()
		{
			SetupAttachedCiphers(MakeCipher(1, points: 100));

			var result = await service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal(70, result.AvailablePoints);
		}

		[Fact]
		public async Task RequestHintAsync_AppliesSolutionHintPenalty_OnAvailablePoints()
		{
			SetupAttachedCiphers(MakeCipher(1, points: 100));

			var result = await service.RequestHintAsync("u1", 1, HintType.Hint);

			Assert.Equal(50, result.AvailablePoints);
		}

		[Fact]
		public async Task RequestHintAsync_AppliesFullSolutionPenalty_OnAvailablePoints()
		{
			SetupAttachedCiphers(MakeCipher(1, points: 100));

			var result = await service.RequestHintAsync("u1", 1, HintType.FullSolution);

			Assert.Equal(5, result.AvailablePoints);
		}

		[Fact]
		public async Task RequestHintAsync_StacksPenalties_WhenUserAlreadyUsedTypeHint()
		{
			var existing = MakeHintRequest("u1", 1, HintType.Type);
			var cipher = MakeCipher(1, points: 100, hints: new List<HintRequest> { existing });
			SetupAttachedCiphers(cipher);

			var result = await service.RequestHintAsync("u1", 1, HintType.Hint);

			Assert.Equal(19, result.AvailablePoints);
		}

		[Fact]
		public async Task RequestHintAsync_ReturnsPointsWithoutNewPenalty_WhenReturningCachedHint()
		{
			var existing = MakeHintRequest("u1", 1, HintType.Type);
			var cipher = MakeCipher(1, points: 100, hints: new List<HintRequest> { existing });
			SetupAttachedCiphers(cipher);

			var result = await service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal(70, result.AvailablePoints);
		}

		#endregion
	}
}