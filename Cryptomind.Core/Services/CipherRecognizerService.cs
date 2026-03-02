using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryptomind.Core.Services
{
	public class CipherRecognizerService : ICipherRecognizerService
	{
		private readonly HttpClient httpClient;
		private readonly string mlApiUrl;

		private const int ApiTimeoutSeconds = Common.Constants.CipherRecognitionConstants.ApiTimeoutSeconds;

		public CipherRecognizerService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration)
		{
			httpClient = httpClientFactory.CreateClient();
			httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds);
			mlApiUrl = configuration["MLService:ApiUrl"] ?? "http://localhost:5002";
		}
		public async Task<CipherRecognitionResultViewModel> ClassifyCipher(string inputText)
		{
			if (string.IsNullOrWhiteSpace(inputText))
			{
				throw new CustomValidationException(CipherRecognitionConstants.InputTextCannotBeEmpty);
			}

			try
			{
				var requestPayload = new
				{
					text = inputText.ToUpper(), //Case-insensitive for the ML.
					return_top_k = true
				};

				var jsonRequest = JsonSerializer.Serialize(requestPayload);
				var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

				var response = await httpClient.PostAsync($"{mlApiUrl}/api/predict", content);

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					throw new Exception(string.Format(CipherRecognitionConstants.MLApiError, response.StatusCode, errorContent));
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
					throw new Exception(CipherRecognitionConstants.InvalidAnalysis);
				}

				if (pythonResponse.TopPrediction == null)
				{
					throw new Exception(CipherRecognitionConstants.BestPredictionMissing);
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
				throw new Exception(string.Format(CipherRecognitionConstants.MLServiceNotAvailable, mlApiUrl));
			}
			catch (TaskCanceledException ex)
			{
				throw new Exception(string.Format(CipherRecognitionConstants.MLTimeoutExpired, ApiTimeoutSeconds));
			}
			catch (JsonException ex)
			{
				throw new Exception(CipherRecognitionConstants.InvalidAnalysis);
			}
		}
		public async Task<bool> IsServiceHealthyAsync()
		{
			try
			{
				var response = await httpClient.GetAsync($"{mlApiUrl}/api/health");
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
		}
		private class PythonPrediction
		{
			[JsonPropertyName("family")]
			public string Family { get; set; }

			[JsonPropertyName("type")]
			public string Type { get; set; }

			[JsonPropertyName("confidence")]
			public double Confidence { get; set; }
		}
		#endregion
	}
}