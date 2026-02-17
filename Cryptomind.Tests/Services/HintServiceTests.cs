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

namespace Cryptomind.Tests.Services
{
	public class HintServiceTests
	{
		private readonly Mock<IRepository<Cipher, int>> _cipherRepoMock = new();
		private readonly Mock<IRepository<HintRequest, int>> _hintRequestRepoMock = new();
		private readonly Mock<ILLMService> _llmServiceMock = new();
		private readonly HintService _service;

		public HintServiceTests()
		{
			_service = new HintService(
				_cipherRepoMock.Object,
				_hintRequestRepoMock.Object,
				_llmServiceMock.Object);

			_hintRequestRepoMock.Setup(r => r.AddAsync(It.IsAny<HintRequest>()))
				.Returns(Task.CompletedTask);
			_cipherRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Cipher>()))
				.ReturnsAsync(true);
			_llmServiceMock.Setup(s => s.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()))
				.ReturnsAsync("Generated hint from LLM");
		}

		private static ConcreteCipher MakeCipher(int id,
			string createdByUserId = "owner",
			bool allowTypeHint = true,
			bool allowHint = true,
			bool allowSolution = true,
			List<HintRequest>? hintsRequested = null,
			List<UserSolution>? userSolutions = null) => new()
			{
				Id = id,
				Status = ApprovalStatus.Approved,
				CreatedByUserId = createdByUserId,
				AllowTypeHint = allowTypeHint,
				AllowHint = allowHint,
				AllowSolution = allowSolution,
				HintsRequested = hintsRequested ?? new List<HintRequest>(),
				UserSolutions = userSolutions ?? new List<UserSolution>(),
				LLMData = new CipherLLMData(),
			};

		private void SetupAttachedCiphers(params Cipher[] ciphers)
		{
			var mock = new List<Cipher>(ciphers).AsQueryable().BuildMock();
			_cipherRepoMock.Setup(r => r.GetAllAttached()).Returns(mock);
		}

		#region RequestHintAsync - Guard Clauses

		[Fact]
		public async Task RequestHintAsync_Throws_WhenCipherNotFound()
		{
			SetupAttachedCiphers();

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.RequestHintAsync("u1", 99, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenCipherIsNotApproved()
		{
			var cipher = MakeCipher(1);
			cipher.Status = ApprovalStatus.Pending;
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.RequestHintAsync("u1", 1, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenUserIsOwner()
		{
			SetupAttachedCiphers(MakeCipher(1, createdByUserId: "u1"));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.RequestHintAsync("u1", 1, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenUserAlreadySolvedCipher()
		{
			var cipher = MakeCipher(1, userSolutions: new List<UserSolution>
			{
				new() { UserId = "u1", IsCorrect = true }
			});
			SetupAttachedCiphers(cipher);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.RequestHintAsync("u1", 1, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_AllowsIncorrectSolutions()
		{
			var cipher = MakeCipher(1, userSolutions: new List<UserSolution>
			{
				new() { UserId = "u1", IsCorrect = false }
			});
			SetupAttachedCiphers(cipher);

			var result = await _service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.NotNull(result);
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenTypeHintNotAllowed()
		{
			SetupAttachedCiphers(MakeCipher(1, allowTypeHint: false));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.RequestHintAsync("u1", 1, HintType.Type));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenSolutionHintNotAllowed()
		{
			SetupAttachedCiphers(MakeCipher(1, allowHint: false));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.RequestHintAsync("u1", 1, HintType.Hint));
		}

		[Fact]
		public async Task RequestHintAsync_Throws_WhenFullSolutionNotAllowed()
		{
			SetupAttachedCiphers(MakeCipher(1, allowSolution: false));

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => _service.RequestHintAsync("u1", 1, HintType.FullSolution));
		}

		#endregion

		#region RequestHintAsync - Existing Hint

		[Fact]
		public async Task RequestHintAsync_ReturnsExistingHint_WhenUserAlreadyRequestedSameType()
		{
			var existingHint = new HintRequest
			{
				UserId = "u1",
				CipherId = 1,
				HintType = HintType.Type,
				HintContent = "Previously requested hint"
			};
			var cipher = MakeCipher(1, hintsRequested: new List<HintRequest> { existingHint });
			SetupAttachedCiphers(cipher);

			var result = await _service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal("Previously requested hint", result);
			_llmServiceMock.Verify(s => s.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()), Times.Never);
		}

		[Fact]
		public async Task RequestHintAsync_DoesNotReturnOtherUsersHint()
		{
			var otherUserHint = new HintRequest
			{
				UserId = "u2",
				CipherId = 1,
				HintType = HintType.Type,
				HintContent = "Other user's hint"
			};
			var cipher = MakeCipher(1, hintsRequested: new List<HintRequest> { otherUserHint });
			SetupAttachedCiphers(cipher);

			var result = await _service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.NotEqual("Other user's hint", result);
		}

		[Fact]
		public async Task RequestHintAsync_DoesNotReturnDifferentHintType()
		{
			var differentTypeHint = new HintRequest
			{
				UserId = "u1",
				CipherId = 1,
				HintType = HintType.Hint,
				HintContent = "Solution hint content"
			};
			var cipher = MakeCipher(1, hintsRequested: new List<HintRequest> { differentTypeHint });
			SetupAttachedCiphers(cipher);

			var result = await _service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.NotEqual("Solution hint content", result);
		}

		#endregion

		#region RequestHintAsync - Cached Hint

		[Fact]
		public async Task RequestHintAsync_ReturnsCachedTypeHint_WithoutCallingLLM()
		{
			var cipher = MakeCipher(1);
			cipher.LLMData.CachedTypeHint = "Cached type hint";
			SetupAttachedCiphers(cipher);

			var result = await _service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal("Cached type hint", result);
			_llmServiceMock.Verify(s => s.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()), Times.Never);
		}

		[Fact]
		public async Task RequestHintAsync_ReturnsCachedSolutionHint_WithoutCallingLLM()
		{
			var cipher = MakeCipher(1);
			cipher.LLMData.CachedHint = "Cached solution hint";
			SetupAttachedCiphers(cipher);

			var result = await _service.RequestHintAsync("u1", 1, HintType.Hint);

			Assert.Equal("Cached solution hint", result);
			_llmServiceMock.Verify(s => s.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()), Times.Never);
		}

		[Fact]
		public async Task RequestHintAsync_ReturnsCachedFullSolution_WithoutCallingLLM()
		{
			var cipher = MakeCipher(1);
			cipher.LLMData.CachedSolution = "Cached full solution";
			SetupAttachedCiphers(cipher);

			var result = await _service.RequestHintAsync("u1", 1, HintType.FullSolution);

			Assert.Equal("Cached full solution", result);
			_llmServiceMock.Verify(s => s.GetHint(It.IsAny<Cipher>(), It.IsAny<HintType>()), Times.Never);
		}

		#endregion

		#region RequestHintAsync - LLM Generation

		[Fact]
		public async Task RequestHintAsync_CallsLLM_WhenNoExistingOrCachedHint()
		{
			SetupAttachedCiphers(MakeCipher(1));
			_llmServiceMock.Setup(s => s.GetHint(It.IsAny<Cipher>(), HintType.Type))
				.ReturnsAsync("Fresh LLM hint");

			var result = await _service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal("Fresh LLM hint", result);
			_llmServiceMock.Verify(s => s.GetHint(It.IsAny<Cipher>(), HintType.Type), Times.Once);
		}

		[Fact]
		public async Task RequestHintAsync_SavesHintRequest_WhenGeneratingNewHint()
		{
			SetupAttachedCiphers(MakeCipher(1));
			HintRequest captured = null;
			_hintRequestRepoMock.Setup(r => r.AddAsync(It.IsAny<HintRequest>()))
				.Callback<HintRequest>(h => captured = h)
				.Returns(Task.CompletedTask);

			await _service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.NotNull(captured);
			Assert.Equal("u1", captured.UserId);
			Assert.Equal(1, captured.CipherId);
			Assert.Equal(HintType.Type, captured.HintType);
			Assert.NotNull(captured.HintContent);
		}

		[Fact]
		public async Task RequestHintAsync_CachesTypeHint_AfterLLMGeneration()
		{
			var cipher = MakeCipher(1);
			SetupAttachedCiphers(cipher);
			_llmServiceMock.Setup(s => s.GetHint(cipher, HintType.Type))
				.ReturnsAsync("New type hint");

			await _service.RequestHintAsync("u1", 1, HintType.Type);

			Assert.Equal("New type hint", cipher.LLMData.CachedTypeHint);
		}

		[Fact]
		public async Task RequestHintAsync_CachesSolutionHint_AfterLLMGeneration()
		{
			var cipher = MakeCipher(1);
			SetupAttachedCiphers(cipher);
			_llmServiceMock.Setup(s => s.GetHint(cipher, HintType.Hint))
				.ReturnsAsync("New solution hint");

			await _service.RequestHintAsync("u1", 1, HintType.Hint);

			Assert.Equal("New solution hint", cipher.LLMData.CachedHint);
		}

		[Fact]
		public async Task RequestHintAsync_CachesFullSolution_AfterLLMGeneration()
		{
			var cipher = MakeCipher(1);
			SetupAttachedCiphers(cipher);
			_llmServiceMock.Setup(s => s.GetHint(cipher, HintType.FullSolution))
				.ReturnsAsync("New full solution");

			await _service.RequestHintAsync("u1", 1, HintType.FullSolution);

			Assert.Equal("New full solution", cipher.LLMData.CachedSolution);
		}

		[Fact]
		public async Task RequestHintAsync_UpdatesCipher_AfterCachingHint()
		{
			var cipher = MakeCipher(1);
			SetupAttachedCiphers(cipher);

			await _service.RequestHintAsync("u1", 1, HintType.Type);

			_cipherRepoMock.Verify(r => r.UpdateAsync(cipher), Times.Once);
		}

		#endregion
	}
}