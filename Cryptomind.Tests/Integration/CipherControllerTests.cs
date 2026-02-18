using Cryptomind.Tests.Integration.Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public class CipherControllerTests : IntegrationTestBase
	{
		public CipherControllerTests(CryptomindWebApplicationFactory factory) : base(factory) { }

		#region Authorization

		[Fact]
		public async Task GetAllCiphers_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/ciphers/all");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task GetAllCiphers_AuthenticatedUser_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/ciphers/all");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetCipherById_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/ciphers/cipher/1");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		#endregion

		#region Get Ciphers

		[Fact]
		public async Task GetAllCiphers_WithFilters_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/ciphers/all?sortBy=date&order=asc");

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Fact]
		public async Task GetCipherById_NonExistentId_Returns400()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/ciphers/cipher/99999");

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Solve Cipher

		[Fact]
		public async Task SolveCipher_Unauthenticated_Returns401()
		{
			var response = await Client.PostAsJsonAsync("/api/ciphers/cipher/1/solve", new { userSolution = "hello" });

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task SolveCipher_NonExistentCipher_Returns400()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.PostAsJsonAsync("/api/ciphers/cipher/99999/solve", new { userSolution = "hello" });

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Submit Cipher

		[Fact]
		public async Task SubmitCipher_Unauthenticated_Returns401()
		{
			using var content = new MultipartFormDataContent();
			content.Add(new StringContent("Some cipher text that is definitely longer than one hundred and fifty characters so it passes the minimum length check required by the ML system here"), "EncryptedText");
			content.Add(new StringContent("Test Title"), "Title");
			content.Add(new StringContent("0"), "CipherDefinition");

			var response = await Client.PostAsync("/api/ciphers/submit", content);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task SubmitCipher_NoTypeAndNoSolution_Returns400()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			// Business rule: submitting without both CipherType and DecryptedText is rejected
			using var content = new MultipartFormDataContent();
			content.Add(new StringContent("Some cipher text that is definitely longer than one hundred and fifty characters so it passes the minimum length check required by the ML system here"), "EncryptedText");
			content.Add(new StringContent("Test Title"), "Title");
			content.Add(new StringContent("0"), "CipherDefinition");

			var response = await userClient.PostAsync("/api/ciphers/submit", content);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		[Fact]
		public async Task SubmitCipher_WithTypeAndSolution_Returns200()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			using var content = new MultipartFormDataContent();
			content.Add(new StringContent("Khoor zruog wklv lv d whvw flskhu wkdw lv orqj hqrxjk wr sdvv wkh plqlpxp ohqjwk uhtxluhphqw zklfk lv rqh kxqguhg dqg iliwb fkdudfwhuv"), "EncryptedText");
			content.Add(new StringContent("Hello world this is a test cipher that is long enough to pass the minimum length requirement which is one hundred and fifty characters here"), "DecryptedText");
			content.Add(new StringContent("Test Cipher Title"), "Title");
			content.Add(new StringContent("0"), "CipherType");   // Caesar = 0
			content.Add(new StringContent("0"), "CipherDefinition");  // TextCipher = 0

			var response = await userClient.PostAsync("/api/ciphers/submit", content);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		#endregion

		#region Suggest Answer

		[Fact]
		public async Task SuggestAnswer_Unauthenticated_Returns401()
		{
			var payload = new { answer = "hello world", explanation = "I think this is Caesar cipher shifted by 3" };

			var response = await Client.PostAsJsonAsync("/api/ciphers/cipher/1/suggest-answer", payload);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task SuggestAnswer_NonExistentCipher_Returns400()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var payload = new { answer = "hello world", explanation = "I think this is Caesar cipher shifted by 3" };

			var response = await userClient.PostAsJsonAsync("/api/ciphers/cipher/99999/suggest-answer", payload);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region Hint

		[Fact]
		public async Task RequestHint_Unauthenticated_Returns401()
		{
			var payload = new { hintType = "General" };

			var response = await Client.PostAsJsonAsync("/api/ciphers/cipher/1/hint", payload);

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[Fact]
		public async Task RequestHint_NonExistentCipher_Returns400()
		{
			var userClient = await GetAuthenticatedUserClientAsync();
			var payload = new { hintType = "General" };

			var response = await userClient.PostAsJsonAsync("/api/ciphers/cipher/99999/hint", payload);

			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		}

		#endregion

		#region ML Health

		[Fact]
		public async Task CheckMLHealth_AuthenticatedUser_Returns200Or400()
		{
			var userClient = await GetAuthenticatedUserClientAsync();

			var response = await userClient.GetAsync("/api/ciphers/ml-health");

			// ML service is not running in tests so 400 is acceptable
			((int)response.StatusCode).Should().BeOneOf(200, 400);
		}

		[Fact]
		public async Task CheckMLHealth_Unauthenticated_Returns401()
		{
			var response = await Client.GetAsync("/api/ciphers/ml-health");

			response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		#endregion
	}
}