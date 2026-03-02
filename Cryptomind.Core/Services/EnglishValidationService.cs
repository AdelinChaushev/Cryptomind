using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.EnglishValidationModels;
using Cryptomind.Core.Contracts;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Cryptomind.Core.Services
{
	public class EnglishValidationService : IEnglishValidationService
	{
		private readonly HttpClient httpClient;
		private readonly string mlApiUrl;
		private const int ApiTimeoutSeconds = EnglishValidationConstants.ApiTimeoutSeconds;
		public EnglishValidationService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration)
		{
			httpClient = httpClientFactory.CreateClient();
			httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds);
			mlApiUrl = configuration["MLService:ApiUrl"] ?? "http://localhost:5002";
		}

		public async Task<EnglishValidationResult> ValidatePlaintextAsync(string plaintext)
		{
			if (string.IsNullOrWhiteSpace(plaintext))
			{
				throw new CustomValidationException(EnglishValidationConstants.DecryptionTextCannotBeEmpty);
			}

			try
			{
				var requestPayload = new
				{
					text = plaintext
				};

				var jsonRequest = JsonSerializer.Serialize(requestPayload);
				var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

				var response = await httpClient.PostAsync(
					$"{mlApiUrl}/api/validate-english",
					content
				);

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					throw new Exception(string.Format(EnglishValidationConstants.APIError, response.StatusCode, errorContent));
				}

				var responseJson = await response.Content.ReadAsStringAsync();

				var result = JsonSerializer.Deserialize<EnglishValidationResult>(
					responseJson,
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					}
				);

				if (result == null)
				{
					throw new Exception(EnglishValidationConstants.InvalidAnalysis);
				}

				return result;
			}
			catch (HttpRequestException ex)
			{
				throw new Exception(string.Format(EnglishValidationConstants.EnglishServiceNotAvailable, mlApiUrl));
			}
			catch (TaskCanceledException ex)
			{
				throw new Exception(string.Format(EnglishValidationConstants.TimeoutExpired, ApiTimeoutSeconds));
			}
			catch (JsonException ex)
			{
				throw new Exception(EnglishValidationConstants.InvalidAnalysis);
			}
		}

		public async Task<bool> IsLikelyEnglishAsync(string plaintext, double minConfidence = 0.5)
		{
			var result = await ValidatePlaintextAsync(plaintext);
			return result.IsEnglish && result.Confidence >= minConfidence;
		}
	}
}