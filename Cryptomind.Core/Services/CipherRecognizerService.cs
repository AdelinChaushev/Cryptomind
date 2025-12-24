using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cryptomind.Common.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;
using Microsoft.Extensions.Configuration;

namespace Cryptomind.Core.Services
{
	public class CipherRecognizerService : ICipherRecognizerService
	{
		private readonly HttpClient _httpClient;
		private readonly string _mlApiUrl;

		private const int ApiTimeoutSeconds = Common.Constants.CipherRecognitionConstants.ApiTimeoutSeconds;

		public CipherRecognizerService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds);

			_mlApiUrl = configuration["MLService:ApiUrl"] ?? "http://localhost:5002";
		}
		public async Task<CipherRecognitionResultViewModel> ClassifyCipherAsync(string inputText)
		{
			if (string.IsNullOrWhiteSpace(inputText))
			{
				throw new ArgumentException("Input text cannot be empty", nameof(inputText));
			}

			try
			{
				var requestPayload = new
				{
					text = inputText.ToUpper(),
					return_top_k = true
				};

				var jsonRequest = JsonSerializer.Serialize(requestPayload);
				var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

				var response = await _httpClient.PostAsync($"{_mlApiUrl}/api/predict", content);

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					throw new InvalidOperationException(
						$"ML API returned error (Status {response.StatusCode}): {errorContent}"
					);
				}

				var responseJson = await response.Content.ReadAsStringAsync();

				var pythonResponse = JsonSerializer.Deserialize<PythonApiResponse>(
					responseJson,
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					}
				);

				if (pythonResponse == null)
				{
					throw new InvalidOperationException("Failed to parse ML API response");
				}

				if (pythonResponse.TopPrediction == null)
				{
					throw new InvalidOperationException("Invalid response from ML API - missing top prediction");
				}

				var result = new CipherRecognitionResultViewModel
				{
					TopPrediction = ConvertToViewModel(pythonResponse.TopPrediction),
					AllPredictions = pythonResponse.AllPredictions?
						.Select(ConvertToViewModel)
						.ToList() ?? new List<PredictionViewModel>(),
				};

				return result;
			}
			catch (HttpRequestException ex)
			{
				throw new InvalidOperationException(
					$"ML service is unavailable. Please ensure the Python ML API is running at {_mlApiUrl}",
					ex
				);
			}
			catch (TaskCanceledException ex)
			{
				throw new InvalidOperationException(
					$"ML service request timed out after {ApiTimeoutSeconds} seconds",
					ex
				);
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException(
					"Failed to parse response from ML service. The service may be returning invalid data.",
					ex
				);
			}
			catch (InvalidOperationException ex)
			{
				throw new InvalidOperationException ("Invalid Operation Exception", ex);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(
					$"An unexpected error occurred during classification: {ex.Message}",
					ex
				);
			}
		}
		public async Task<bool> IsServiceHealthyAsync()
		{
			try
			{
				var response = await _httpClient.GetAsync($"{_mlApiUrl}/api/health");
				return response.IsSuccessStatusCode;
			}
			catch
			{
				return false;
			}
		}
		private PredictionViewModel ConvertToViewModel(PythonPrediction pythonPrediction)
		{
			return new PredictionViewModel
			{
				Family = pythonPrediction.Family,
				Type = pythonPrediction.Type,
				Confidence = pythonPrediction.Confidence
			};
		}

		#region Internal Models for Python API
		private class PythonApiResponse
		{
			[JsonPropertyName("top_prediction")]
			public PythonPrediction TopPrediction { get; set; }

			[JsonPropertyName("all_predictions")]
			public List<PythonPrediction> AllPredictions { get; set; }

			[JsonPropertyName("letter_count")]
			public int LetterCount { get; set; }

			[JsonPropertyName("text_length")]
			public int TextLength { get; set; }
		}
		private class PythonPrediction
		{
			[JsonPropertyName("family")]
			public string Family { get; set; }

			[JsonPropertyName("type")]
			public string Type { get; set; }

			[JsonPropertyName("confidence")]
			public double Confidence { get; set; }

			[JsonPropertyName("family_confidence")]
			public double FamilyConfidence { get; set; }

			[JsonPropertyName("type_confidence")]
			public double TypeConfidence { get; set; }
		}
		#endregion
	}
}