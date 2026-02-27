using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class SubmissionControllerTests : IntegrationTestBase
	{
		private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

		public SubmissionControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		// -------------------------------------------------------------------------
		// Seed helpers
		// -------------------------------------------------------------------------

		private async Task<TextCipher> SeedPendingCipherForUserAsync(string userId)
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();

			var cipher = new TextCipher
			{
				Title = $"Submission_{Guid.NewGuid():N}".Substring(0, 30),
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
				CreatedByUserId = userId
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();
			return cipher;
		}

		private async Task<AnswerSuggestion> SeedPendingAnswerForUserAsync(string userId)
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();

			// Need an approved experimental cipher to attach the answer to
			var admin = await db.Users.FirstAsync(u => u.Email == "admin@cryptomind.com");

			var cipher = new TextCipher
			{
				Title = $"ExpForSub_{Guid.NewGuid():N}".Substring(0, 30),
				EncryptedText = $"Encrypted_{Guid.NewGuid():N}",
				DecryptedText = null,
				MLPrediction = @"{""Family"":""Substitution"",""Type"":""Caesar"",""Confidence"":0.9}",
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
				CreatedByUserId = admin.Id
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();

			var answer = new AnswerSuggestion
			{
				CipherId = cipher.Id,
				UserId = userId,
				DecryptedText = "my answer",
				Description = "my reasoning",
				Status = ApprovalStatus.Pending,
				UploadedTime = DateTime.UtcNow,
				PointsEarned = 0
			};

			db.AnswerSuggestions.Add(answer);
			await db.SaveChangesAsync();
			return answer;
		}

		// =========================================================================
		// GET ALL SUBMISSIONS
		// =========================================================================

		[Fact]
		public async Task GetAllSubmissions_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/submissions");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetAllSubmissions_Authenticated_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/submissions");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAllSubmissions_ResponseContainsExpectedFields()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/submissions");
			var body = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);

			json.TryGetProperty("ciphers", out _).Should().BeTrue();
			json.TryGetProperty("answers", out _).Should().BeTrue();
		}

		[Fact]
		public async Task GetAllSubmissions_EmptyForNewUser_ReturnsBothEmptyLists()
		{
			var freshClient = await RegisterAndGetClientAsync();

			var response = await freshClient.GetAsync("/api/submissions");
			var body = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);

			json.GetProperty("ciphers").GetArrayLength().Should().Be(0);
			json.GetProperty("answers").GetArrayLength().Should().Be(0);
		}

		[Fact]
		public async Task GetAllSubmissions_WithSeededCipher_CiphersListIsNotEmpty()
		{
			var email = $"sub_{Guid.NewGuid():N}@test.com";
			var authClient = await RegisterAndGetClientAsync(email: email);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == email);

			await SeedPendingCipherForUserAsync(user.Id);

			var response = await authClient.GetAsync("/api/submissions");
			var body = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);

			json.GetProperty("ciphers").GetArrayLength().Should().BeGreaterThan(0);
		}

		[Fact]
		public async Task GetAllSubmissions_WithSeededAnswer_AnswersListIsNotEmpty()
		{
			var email = $"sub_{Guid.NewGuid():N}@test.com";
			var authClient = await RegisterAndGetClientAsync(email: email);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == email);

			await SeedPendingAnswerForUserAsync(user.Id);

			var response = await authClient.GetAsync("/api/submissions");
			var body = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);

			json.GetProperty("answers").GetArrayLength().Should().BeGreaterThan(0);
		}

		[Fact]
		public async Task GetAllSubmissions_OnlyReturnsOwnSubmissions()
		{
			// Seed a cipher for the admin, then check that the regular user doesn't see it
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var admin = await db.Users.FirstAsync(u => u.Email == "admin@cryptomind.com");

			await SeedPendingCipherForUserAsync(admin.Id);

			var freshClient = await RegisterAndGetClientAsync();
			var response = await freshClient.GetAsync("/api/submissions");
			var body = await response.Content.ReadAsStringAsync();
			var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOpts);

			json.GetProperty("ciphers").GetArrayLength().Should().Be(0);
		}
	}
}