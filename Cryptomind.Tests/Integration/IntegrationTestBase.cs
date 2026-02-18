using Cryptomind.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cryptomind.Tests.Integration
{
	public abstract class IntegrationTestBase : IClassFixture<CryptomindWebApplicationFactory>, IDisposable
	{
		protected readonly HttpClient Client;
		protected readonly CryptomindWebApplicationFactory Factory;

		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		private static WebApplicationFactoryClientOptions DefaultOptions => new()
		{
			HandleCookies = false,
			AllowAutoRedirect = false
		};

		protected IntegrationTestBase(CryptomindWebApplicationFactory factory)
		{
			Factory = factory;
			Client = factory.CreateSeededClient(DefaultOptions);
		}

		protected async Task<HttpClient> GetAuthenticatedAdminClientAsync()
		{
			return await CreateAuthenticatedClientAsync("admin@cryptomind.com", "Admin123!");
		}

		protected async Task<HttpClient> GetAuthenticatedUserClientAsync()
		{
			return await CreateAuthenticatedClientAsync("user@cryptomind.com", "User123!");
		}

		protected async Task<HttpClient> RegisterAndGetClientAsync(
			string? email = null,
			string? username = null,
			string password = "Test123!")
		{
			var client = Factory.CreateSeededClient(DefaultOptions);
			email ??= $"test_{Guid.NewGuid():N}@test.com";
			username ??= $"u_{Guid.NewGuid():N}".Substring(0, 16);

			var response = await client.PostAsJsonAsync("/api/auth/register",
				new { email, username, password, confirmPassword = password });

			var rawBody = await response.Content.ReadAsStringAsync();
			Console.WriteLine("REGISTER RESPONSE: " + rawBody);

			if (!response.IsSuccessStatusCode)
				throw new InvalidOperationException($"Register failed ({response.StatusCode}): {rawBody}");

			response.EnsureSuccessStatusCode();

			var body = JsonSerializer.Deserialize<JsonElement>(rawBody, JsonOptions);
			var token = body.GetProperty("token").GetString()
				?? throw new InvalidOperationException("Token was null in register response");

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			return client;
		}

		private async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
		{
			var client = Factory.CreateSeededClient(DefaultOptions);

			var response = await client.PostAsJsonAsync("/api/auth/login",
				new { email, password });

			var rawBody = await response.Content.ReadAsStringAsync();
			Console.WriteLine("LOGIN RESPONSE: " + rawBody);

			response.EnsureSuccessStatusCode();

			var body = JsonSerializer.Deserialize<JsonElement>(rawBody, JsonOptions);
			var token = body.GetProperty("token").GetString()
				?? throw new InvalidOperationException("Token was null in login response");

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			return client;
		}

		protected static StringContent JsonContent(object obj)
		{
			var json = JsonSerializer.Serialize(obj);
			return new StringContent(json, Encoding.UTF8, "application/json");
		}

		protected static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
		{
			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<T>(content, JsonOptions);
		}

		public void Dispose()
		{
			Client.Dispose();
		}
	}
}