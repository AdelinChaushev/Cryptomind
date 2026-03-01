using Cryptomind.Data;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class AdminControllerTests : IntegrationTestBase
	{
		private static readonly JsonSerializerOptions JsonOpts = new()
		{
			PropertyNameCaseInsensitive = true
		};

		public AdminControllerTests(CryptomindWebApplicationFactory factory) : base(factory)
		{
		}

		#region Seed Helpers

		private async Task<TextCipher> SeedExperimentalCipherAsync()
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

		private async Task<TextCipher> SeedPendingCipherAsync(string? title = null)
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();

			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var cipher = new TextCipher
			{
				Title = title ?? $"Pending_{Guid.NewGuid():N}".Substring(0, 30),
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
				CreatedByUserId = user.Id,
				TypeOfCipher = CipherType.Caesar,
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();
			return cipher;
		}

		private async Task<TextCipher> SeedApprovedCipherAsync(string? title = null)
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();

			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var cipher = new TextCipher
			{
				Title = title ?? $"Approved_{Guid.NewGuid():N}".Substring(0, 30),
				EncryptedText = $"Encrypted_{Guid.NewGuid():N}",
				DecryptedText = "This is the decrypted text",
				MLPrediction = @"{""Family"":""Substitution"",""Type"":""Caesar"",""Confidence"":0.9}",
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
				CreatedByUserId = user.Id
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();
			return cipher;
		}

		private async Task<TextCipher> SeedDeletedCipherAsync()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();

			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var cipher = new TextCipher
			{
				Title = $"Deleted_{Guid.NewGuid():N}".Substring(0, 30),
				EncryptedText = $"Encrypted_{Guid.NewGuid():N}",
				MLPrediction = @"{""Family"":""Substitution"",""Type"":""Caesar"",""Confidence"":0.9}",
				ChallengeType = ChallengeType.Standard,
				Status = ApprovalStatus.Approved,
				CreatedAt = DateTime.UtcNow,
				ApprovedAt = DateTime.UtcNow,
				IsDeleted = true,
				DeletedAt = DateTime.UtcNow,
				IsPlaintextValid = true,
				IsLLMRecommended = false,
				AllowTypeHint = true,
				AllowHint = true,
				AllowSolution = true,
				Points = 10,
				CreatedByUserId = user.Id
			};

			db.TextCiphers.Add(cipher);
			await db.SaveChangesAsync();
			return cipher;
		}

		private async Task<AnswerSuggestion> SeedPendingAnswerAsync()
		{
			var cipher = await SeedExperimentalCipherAsync();

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();

			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var answer = new AnswerSuggestion
			{
				CipherId = cipher.Id,
				UserId = user.Id,
				DecryptedText = "some answer",
				Description = "some reasoning",
				Status = ApprovalStatus.Pending,
				UploadedTime = DateTime.UtcNow,
				PointsEarned = 0
			};

			db.AnswerSuggestions.Add(answer);
			await db.SaveChangesAsync();
			return answer;
		}

		#endregion

		#region Dashboard

		[Fact]
		public async Task Dashboard_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/admin/dashboard");
			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task Dashboard_RegularUser_Returns403()
		{
			var userClient = await RegisterAndGetClientAsync();
			var response = await userClient.GetAsync("/api/admin/dashboard");
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task Dashboard_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/dashboard");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task Dashboard_Admin_ResponseContainsExpectedFields()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/dashboard");
			var json = JsonSerializer.Deserialize<JsonElement>(
				await response.Content.ReadAsStringAsync(),
				JsonOpts);

			json.TryGetProperty("approvedCiphersCount", out _).Should().BeTrue();
			json.TryGetProperty("pendingCiphersCount", out _).Should().BeTrue();
			json.TryGetProperty("deletedCiphersCount", out _).Should().BeTrue();
			json.TryGetProperty("pendingAnswersCount", out _).Should().BeTrue();
			json.TryGetProperty("approvedAnswersCount", out _).Should().BeTrue();
		}

		#endregion

		#region Pending Ciphers

		[Fact]
		public async Task GetPendingCiphers_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/pending-ciphers");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetPendingCiphers_RegularUser_Returns403()
		{
			var userClient = await RegisterAndGetClientAsync();
			var response = await userClient.GetAsync("/api/admin/pending-ciphers");
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task GetPendingCiphers_WithFilter_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/pending-ciphers?filter=caesar");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		#endregion

		#region Approved Ciphers

		[Fact]
		public async Task GetApprovedCiphers_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/approved-ciphers");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetApprovedCiphers_WithFilters_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/approved-ciphers?sortBy=date&order=desc");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		#endregion

		#region Deleted Ciphers

		[Fact]
		public async Task GetDeletedCiphers_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/deleted-ciphers");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetDeletedCiphers_RegularUser_Returns403()
		{
			var userClient = await RegisterAndGetClientAsync();
			var response = await userClient.GetAsync("/api/admin/deleted-ciphers");
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		#endregion

		#region Get Single Cipher

		[Fact]
		public async Task GetCipher_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/cipher/99999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task GetCipher_PendingCipher_Returns200()
		{
			var cipher = await SeedPendingCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync($"/api/admin/cipher/{cipher.Id}");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetCipher_ApprovedCipher_Returns409()
		{
			var cipher = await SeedApprovedCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync($"/api/admin/cipher/{cipher.Id}");
			response.StatusCode.Should().Be(HttpStatusCode.Conflict);
		}

		#endregion

		#region Approve Cipher

		[Fact]
		public async Task ApproveCipher_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var model = new { challengeType = "Standard", title = "Test Cipher", description = "A test cipher" };
			var response = await adminClient.PutAsJsonAsync("/api/admin/cipher/99999/approve", model);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task ApproveCipher_PendingCipher_Returns200()
		{
			var cipher = await SeedPendingCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var model = new { challengeType = "Standard", title = cipher.Title, description = "desc", typeOfCipher = CipherType.Caesar };
			var response = await adminClient.PutAsJsonAsync($"/api/admin/cipher/{cipher.Id}/approve", model);
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task ApproveCipher_PendingCipher_StatusBecomesApproved()
		{
			var cipher = await SeedPendingCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var model = new { challengeType = "Standard", title = cipher.Title, description = "desc", typeOfCipher = CipherType.Caesar };
			var response = await adminClient.PutAsJsonAsync($"/api/admin/cipher/{cipher.Id}/approve", model);

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var updated = await db.TextCiphers.FindAsync(cipher.Id);
			updated!.Status.Should().Be(ApprovalStatus.Approved);
		}

		#endregion

		#region Reject Cipher

		[Fact]
		public async Task RejectCipher_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsJsonAsync("/api/admin/cipher/99999/reject", "Not valid content");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task RejectCipher_PendingCipher_Returns200()
		{
			var cipher = await SeedPendingCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsJsonAsync($"/api/admin/cipher/{cipher.Id}/reject", "Low quality");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task RejectCipher_PendingCipher_StatusBecomesRejected()
		{
			var cipher = await SeedPendingCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsJsonAsync($"/api/admin/cipher/{cipher.Id}/reject", "Low quality");

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var updated = await db.TextCiphers.FindAsync(cipher.Id);
			updated!.Status.Should().Be(ApprovalStatus.Rejected);
		}

		#endregion

		#region Update Cipher

		[Fact]
		public async Task UpdateCipher_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var model = new { title = "New Title" };
			var response = await adminClient.PutAsJsonAsync("/api/admin/cipher/99999/update", model);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task UpdateCipher_ApprovedCipher_Returns200()
		{
			var cipher = await SeedApprovedCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var model = new { title = $"Updated_{Guid.NewGuid():N}".Substring(0, 20) };
			var response = await adminClient.PutAsJsonAsync($"/api/admin/cipher/{cipher.Id}/update", model);
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task UpdateCipher_RegularUser_Returns403()
		{
			var cipher = await SeedApprovedCipherAsync();
			var userClient = await RegisterAndGetClientAsync();
			var model = new { title = "Hacked Title" };
			var response = await userClient.PutAsJsonAsync($"/api/admin/cipher/{cipher.Id}/update", model);
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		#endregion

		#region Soft Delete Cipher

		[Fact]
		public async Task DeleteCipher_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync("/api/admin/cipher/99999/delete", null);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task DeleteCipher_ApprovedCipher_Returns200()
		{
			var cipher = await SeedApprovedCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync($"/api/admin/cipher/{cipher.Id}/delete", null);
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task DeleteCipher_ApprovedCipher_IsDeletedBecomesTrue()
		{
			var cipher = await SeedApprovedCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync($"/api/admin/cipher/{cipher.Id}/delete", null);

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var updated = await db.TextCiphers.FindAsync(cipher.Id);
			updated!.IsDeleted.Should().BeTrue();
		}

		#endregion

		#region Restore Cipher

		[Fact]
		public async Task RestoreCipher_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync("/api/admin/cipher/99999/restore", null);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task RestoreCipher_DeletedCipher_Returns200()
		{
			var cipher = await SeedDeletedCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync($"/api/admin/cipher/{cipher.Id}/restore", null);
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task RestoreCipher_DeletedCipher_IsDeletedBecomesFalse()
		{
			var cipher = await SeedDeletedCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync($"/api/admin/cipher/{cipher.Id}/restore", null);

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var updated = await db.TextCiphers.FindAsync(cipher.Id);
			updated!.IsDeleted.Should().BeFalse();
		}

		[Fact]
		public async Task RestoreCipher_WithNewTitle_Returns200()
		{
			var cipher = await SeedDeletedCipherAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync($"/api/admin/cipher/{cipher.Id}/restore?newTitle=RestoredTitle", null);
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		#endregion

		#region Pending Answers

		[Fact]
		public async Task GetPendingAnswers_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/pending-answer-suggestions");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetPendingAnswers_WithFilters_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/pending-answer-suggestions?cipherName=test&username=admin");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetPendingAnswers_RegularUser_Returns403()
		{
			var userClient = await RegisterAndGetClientAsync();
			var response = await userClient.GetAsync("/api/admin/pending-answer-suggestions");
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		#endregion

		#region Get Single Answer

		[Fact]
		public async Task GetAnswer_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/answer/99999");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task GetAnswer_ExistingAnswer_Returns200()
		{
			var answer = await SeedPendingAnswerAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync($"/api/admin/answer/{answer.Id}");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		#endregion

		#region Approve Answer

		[Fact]
		public async Task ApproveAnswer_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync("/api/admin/answer/99999/approve", null);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task ApproveAnswer_PendingAnswer_Returns200()
		{
			var answer = await SeedPendingAnswerAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync($"/api/admin/answer/{answer.Id}/approve", null);
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task ApproveAnswer_PendingAnswer_StatusBecomesApproved()
		{
			var answer = await SeedPendingAnswerAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync($"/api/admin/answer/{answer.Id}/approve", null);

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var updated = await db.AnswerSuggestions.FindAsync(answer.Id);
			updated!.Status.Should().Be(ApprovalStatus.Approved);
		}

		#endregion

		#region Reject Answer

		[Fact]
		public async Task RejectAnswer_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsJsonAsync("/api/admin/answer/99999/reject", "Wrong answer");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task RejectAnswer_PendingAnswer_Returns200()
		{
			var answer = await SeedPendingAnswerAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsJsonAsync($"/api/admin/answer/{answer.Id}/reject", "Incorrect reasoning");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task RejectAnswer_PendingAnswer_StatusBecomesRejected()
		{
			var answer = await SeedPendingAnswerAsync();
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsJsonAsync($"/api/admin/answer/{answer.Id}/reject", "Incorrect reasoning");

			var body = await response.Content.ReadAsStringAsync();
			response.StatusCode.Should().Be(HttpStatusCode.OK, because: body);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var updated = await db.AnswerSuggestions.FindAsync(answer.Id);
			updated!.Status.Should().Be(ApprovalStatus.Rejected);
		}

		#endregion

		#region Users

		[Fact]
		public async Task GetAllUsers_Admin_Returns200()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/users");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetAllUsers_RegularUser_Returns403()
		{
			var userClient = await RegisterAndGetClientAsync();
			var response = await userClient.GetAsync("/api/admin/users");
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		[Fact]
		public async Task GetUser_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync("/api/admin/user/nonexistent-id-000");
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task GetUser_SeededUser_Returns200()
		{
			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var user = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.GetAsync($"/api/admin/user/{user.Id}");
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		#endregion

		#region Make Admin

		[Fact]
		public async Task MakeUserAdmin_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync("/api/admin/user/nonexistent-id/admin", null);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task MakeUserAdmin_ExistingUser_Returns200()
		{
			var email = $"fresh_{Guid.NewGuid():N}@test.com";
			await RegisterAndGetClientAsync(email: email);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var freshUser = await db.Users.FirstAsync(u => u.Email == email);

			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync($"/api/admin/user/{freshUser.Id}/admin", null);
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task MakeUserAdmin_RegularUser_Returns403()
		{
			var userClient = await RegisterAndGetClientAsync();

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var target = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var response = await userClient.PutAsync($"/api/admin/user/{target.Id}/admin", null);
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		#endregion

		#region Ban User

		[Fact]
		public async Task BanUser_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("reason", "Test ban reason")
			});
			var response = await adminClient.PutAsync("/api/admin/user/nonexistent-user-id/ban", content);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task BanUser_ExistingUser_Returns200()
		{
			var email = $"fresh_{Guid.NewGuid():N}@test.com";
			await RegisterAndGetClientAsync(email: email);

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var freshUser = await db.Users.FirstAsync(u => u.Email == email);

			var adminClient = await GetAuthenticatedAdminClientAsync();
			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("reason", "Test ban")
			});
			var response = await adminClient.PutAsync($"/api/admin/user/{freshUser.Id}/ban", content);
			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task BanUser_RegularUser_Returns403()
		{
			var userClient = await RegisterAndGetClientAsync();

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var target = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("reason", "Trying to ban")
			});
			var response = await userClient.PutAsync($"/api/admin/user/{target.Id}/ban", content);
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		#endregion

		#region Unban User

		[Fact]
		public async Task UnbanUser_NonExistentId_Returns404()
		{
			var adminClient = await GetAuthenticatedAdminClientAsync();
			var response = await adminClient.PutAsync("/api/admin/user/nonexistent-user-id/unban", null);
			response.StatusCode.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public async Task UnbanUser_RegularUser_Returns403()
		{
			var userClient = await RegisterAndGetClientAsync();

			using var scope = Factory.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<CryptomindDbContext>();
			var target = await db.Users.FirstAsync(u => u.Email == "user@cryptomind.com");

			var response = await userClient.PutAsync($"/api/admin/user/{target.Id}/unban", null);
			response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
		}

		#endregion
	}
}