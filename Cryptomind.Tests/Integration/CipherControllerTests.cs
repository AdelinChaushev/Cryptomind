using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class CipherControllerTests : IntegrationTestBase
	{
		private static readonly JsonSerializerOptions JsonOpts = new()
		{
			PropertyNameCaseInsensitive = true
		};

		public CipherControllerTests(CryptomindWebApplicationFactory factory) : base(factory)
		{
		}

		#region Seed Helpers

		private async Task<TextCipher> SeedApprovedStandardCipherAsync()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var cipher = new TextCipher
			{
				Title = $"Standard_{Guid.NewGuid():N}".Substring(0, 30),
				EncryptedText = $"Encrypted_{Guid.NewGuid():N}",
				DecryptedText = "This is the decrypted text",
				MLPrediction = @"{""Family"":""Substitution"",""Type"":""Caesar"",""Confidence"":0.9}",
				TypeOfCipher = CipherType.Caesar,
				ChallengeType = ChallengeType.Standard,
				Status = ApprovalStatus.Approved,
				CreatedAt = DateTime.UtcNow,
				ApprovedAt = DateTime.UtcNow,
				IsDeleted = false,
				IsPlaintextValid = true,
				IsLLMRecommended = false,
				AllowTypeHint = true,
				AllowHint = true,
				AllowSolution = true,
				Points = 10,
				CreatedByUserId = user.Id,
				LLMData = new CipherLLMData
				{
					CachedHint = "",
					CachedSolution = "",
					CachedTypeHint = ""
				}
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();
			return cipher;
		}

		private async Task<TextCipher> SeedApprovedExperimentalCipherAsync()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var cipher = new TextCipher
			{
				Title = $"Experimental_{Guid.NewGuid():N}".Substring(0, 30),
				EncryptedText = $"Encrypted_{Guid.NewGuid():N}",
				DecryptedText = null,
				MLPrediction = @"{""Family"":""Substitution"",""Type"":""Caesar"",""Confidence"":0.9}",
				TypeOfCipher = null,
				ChallengeType = ChallengeType.Experimental,
				Status = ApprovalStatus.Approved,
				CreatedAt = DateTime.UtcNow,
				ApprovedAt = DateTime.UtcNow,
				IsDeleted = false,
				IsPlaintextValid = false,
				IsLLMRecommended = false,
				AllowTypeHint = false,
				AllowHint = false,
				AllowSolution = false,
				Points = 50,
				CreatedByUserId = user.Id
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();
			return cipher;
		}

		#endregion

		#region Get All Ciphers

		[Fact]
		public async Task GetAllCiphers_Authenticated_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/ciphers/all");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAllCiphers_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/ciphers/all");
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetAllCiphers_WithFilters_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/ciphers/all?sortBy=date&order=desc");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAllCiphers_SeededCipher_AppearsInResults()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/ciphers/all");
			var body = await response.Content.ReadAsStringAsync();
			body.Should().Contain(cipher.Title);
		}

		#endregion

		#region Get Cipher By Id

		[Fact]
		public async Task GetCipherById_Authenticated_Returns200()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync($"/api/ciphers/cipher/{cipher.Id}");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetCipherById_Unauthenticated_Returns401()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var response = await Client.GetAsync($"/api/ciphers/cipher/{cipher.Id}");
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetCipherById_NonExistentId_Returns404()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync("/api/ciphers/cipher/99999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task GetCipherById_PendingCipher_Returns404OrConflict()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var cipher = new TextCipher
			{
				Title = $"Pending_{Guid.NewGuid():N}".Substring(0, 30),
				EncryptedText = $"Encrypted_{Guid.NewGuid():N}",
				MLPrediction = @"{""Family"":""Substitution"",""Type"":""Caesar"",""Confidence"":0.9}",
				ChallengeType = ChallengeType.Standard,
				Status = ApprovalStatus.Pending,
				CreatedAt = DateTime.UtcNow,
				IsDeleted = false,
				IsPlaintextValid = false,
				IsLLMRecommended = false,
				AllowTypeHint = true,
				AllowHint = true,
				AllowSolution = true,
				Points = 10,
				CreatedByUserId = user.Id
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();

			var userClient = await GetAuthenticatedUserClientAsync();
			var response = await userClient.GetAsync($"/api/ciphers/cipher/{cipher.Id}");
			response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Conflict);
		}

		#endregion

		#region Solve Cipher

		[Fact]
		public async Task SolveCipher_Unauthenticated_Returns401()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var dto = new { userSolution = "some answer" };
			var response = await Client.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/solve", dto);
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task SolveCipher_NonExistentId_Returns404()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var dto = new { userSolution = "some answer" };
			var response = await userClient.PostAsJsonAsync("/api/ciphers/cipher/99999/solve", dto);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task SolveCipher_CorrectAnswer_ReturnsTrue()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var userClient = await RegisterAndGetClientAsync();
			var dto = new { userSolution = cipher.DecryptedText };
			var response = await userClient.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/solve", dto);

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			var result = JsonSerializer.Deserialize<bool>(body, JsonOpts);
			result.Should().BeTrue();
		}

		[Fact]
		public async Task SolveCipher_WrongAnswer_ReturnsFalse()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var userClient = await RegisterAndGetClientAsync();
			var dto = new { userSolution = "completely wrong answer" };
			var response = await userClient.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/solve", dto);

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			var result = JsonSerializer.Deserialize<bool>(body, JsonOpts);
			result.Should().BeFalse();
		}

		[Fact]
		public async Task SolveCipher_ExperimentalCipher_ReturnsConflict()
		{
			var cipher = await SeedApprovedExperimentalCipherAsync();
			var userClient = await RegisterAndGetClientAsync();
			var dto = new { userSolution = "some answer" };
			var response = await userClient.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/solve", dto);
			response.StatusCode.Should().Be(HttpStatusCode.Conflict);
		}

		#endregion

		#region Submit Cipher

		[Fact]
		public async Task SubmitCipher_Unauthenticated_Returns401()
		{
			var content = new MultipartFormDataContent();
			content.Add(new StringContent("Test Cipher Title"), "title");
			content.Add(new StringContent("Some encrypted text that is long enough"), "encryptedText");

			var response = await Client.PostAsync("/api/ciphers/submit", content);
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task SubmitCipher_ValidTextCipher_Returns200()
		{
			var userClient = await RegisterAndGetClientAsync();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent($"Title_{Guid.NewGuid():N}".Substring(0, 20)), "title");
			content.Add(new StringContent(new string('A', 200)), "encryptedText");
			content.Add(new StringContent("Caesar"), "cipherType");
			content.Add(new StringContent("This is the decrypted text"), "decryptedText");

			var response = await userClient.PostAsync("/api/ciphers/submit", content);
			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
		}

		[Fact]
		public async Task SubmitCipher_MissingEncryptedText_ReturnsBadRequest()
		{
			var userClient = await RegisterAndGetClientAsync();

			var content = new MultipartFormDataContent();
			content.Add(new StringContent("Some Title"), "title");
			// No encryptedText

			var response = await userClient.PostAsync("/api/ciphers/submit", content);
			response.StatusCode.Should().Be(HttpStatusCode.Conflict);
		}

		#endregion

		#region Suggest Answer

		[Fact]
		public async Task SuggestAnswer_Unauthenticated_Returns401()
		{
			var cipher = await SeedApprovedExperimentalCipherAsync();
			var dto = new { decryptedText = "my answer", description = "my reasoning" };
			var response = await Client.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/suggest-answer", dto);
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task SuggestAnswer_NonExistentCipher_Returns404()
		{
			var userClient = await RegisterAndGetClientAsync();
			var dto = new { decryptedText = "my answer", description = "my reasoning" };
			var response = await userClient.PostAsJsonAsync("/api/ciphers/cipher/99999/suggest-answer", dto);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task SuggestAnswer_ExperimentalCipher_Returns200()
		{
			var cipher = await SeedApprovedExperimentalCipherAsync();
			var userClient = await RegisterAndGetClientAsync();
			var dto = new { decryptedText = "my answer", description = "my reasoning" };
			var response = await userClient.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/suggest-answer", dto);

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
		}

		[Fact]
		public async Task SuggestAnswer_StandardCipher_ReturnsConflict()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var userClient = await RegisterAndGetClientAsync();
			var dto = new { decryptedText = "my answer", description = "my reasoning" };
			var response = await userClient.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/suggest-answer", dto);
			response.StatusCode.Should().Be(HttpStatusCode.Conflict);
		}

		#endregion

		#region Request Hint

		[Fact]
		public async Task RequestHint_Unauthenticated_Returns401()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var dto = new { hintType = "Hint" };
			var response = await Client.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/hint", dto);
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task RequestHint_NonExistentCipher_Returns404()
		{
			var userClient = await RegisterAndGetClientAsync();
			var dto = new { hintType = "Hint" };
			var response = await userClient.PostAsJsonAsync("/api/ciphers/cipher/99999/hint", dto);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task RequestHint_ValidCipher_Returns200()
		{
			var cipher = await SeedApprovedStandardCipherAsync();
			var userClient = await RegisterAndGetClientAsync();
			var dto = new { hintType = "Hint" };
			var response = await userClient.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/hint", dto);

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);
		}

		[Fact]
		public async Task RequestHint_CipherWithHintDisabled_ReturnsConflict()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var cipher = new TextCipher
			{
				Title = $"NoHint_{Guid.NewGuid():N}".Substring(0, 30),
				EncryptedText = $"Encrypted_{Guid.NewGuid():N}",
				DecryptedText = "decrypted",
				MLPrediction = @"{""Family"":""Substitution"",""Type"":""Caesar"",""Confidence"":0.9}",
				TypeOfCipher = CipherType.Caesar,
				ChallengeType = ChallengeType.Standard,
				Status = ApprovalStatus.Approved,
				CreatedAt = DateTime.UtcNow,
				ApprovedAt = DateTime.UtcNow,
				IsDeleted = false,
				IsPlaintextValid = true,
				IsLLMRecommended = false,
				AllowTypeHint = false,
				AllowHint = false,
				AllowSolution = false,
				Points = 10,
				CreatedByUserId = user.Id
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();

			var userClient = await RegisterAndGetClientAsync();
			var dto = new { hintType = "Hint" };
			var response = await userClient.PostAsJsonAsync($"/api/ciphers/cipher/{cipher.Id}/hint", dto);
			response.StatusCode.Should().Be(HttpStatusCode.Conflict);
		}

		#endregion
	}
}