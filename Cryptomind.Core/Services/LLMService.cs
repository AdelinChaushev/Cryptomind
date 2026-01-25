using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cryptomind.Common.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cryptomind.Core.Services
{
	public class LLMService : ILLMService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiUrl;
		private readonly string _apiKey;
		private readonly string _model;
		private readonly ILogger<LLMService> _logger;

		private const int ApiTimeoutSeconds = 60;

		public LLMService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			ILogger<LLMService> logger)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds);

			_apiUrl = configuration["LLMService:ApiUrl"]
				?? throw new InvalidOperationException("LLMService:ApiUrl not configured");
			_apiKey = configuration["LLMService:ApiKey"]
				?? throw new InvalidOperationException("LLMService:ApiKey not configured");
			_model = configuration["LLMService:Model"] ?? "gpt-4o-mini";

			_logger = logger;

			// Set authorization header for OpenAI
			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", _apiKey);
		}

		public async Task<string> ValidateCipherTextAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var prompt = BuildValidationPrompt(encryptedText, decryptedText, mlResult);
			return await CallLLMAsync(prompt);
		}

		public async Task<string> SolveCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var prompt = BuildSolvingPrompt(encryptedText, decryptedText, mlResult);
			return await CallLLMAsync(prompt);
		}

		public async Task<string> GetHintAsync(
			string encryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var prompt = BuildHintPrompt(encryptedText, mlResult);
			return await CallLLMAsync(prompt);
		}

		public async Task<bool> IsServiceHealthyAsync()
		{
			try
			{
				// Simple test call to check if API is accessible
				var testPrompt = "Respond with 'OK' if you can read this.";
				var response = await CallLLMAsync(testPrompt, maxTokens: 10);
				return !string.IsNullOrWhiteSpace(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "LLM service health check failed");
				return false;
			}
		}

		#region Prompt Building

		private string BuildValidationPrompt(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var allPredictions = string.Join(", ",
				mlResult.AllPredictions.Take(3).Select(p =>
					$"{p.Type} ({p.Confidence:P0})"));

			var solutionSection = !string.IsNullOrWhiteSpace(decryptedText)
				? $@"

USER-PROVIDED SOLUTION:
{decryptedText}

ADDITIONAL TASK:
Verify if the provided solution appears to be correct for this cipher:
- Does it make linguistic sense? (proper English, coherent meaning)
- Does the length match expectations?
- Could this plausibly decrypt from the encrypted text using the predicted cipher type?
- Rate confidence: ""Highly likely correct"", ""Possibly correct"", ""Unlikely correct"", ""Definitely incorrect"""
				: "";

			return $@"You are a cryptography expert analyzing submitted cipher text for legitimacy.

ENCRYPTED TEXT:
{encryptedText}

ML ANALYSIS:
- Top Prediction: {mlResult.TopPrediction.Type} ({mlResult.TopPrediction.Confidence:P0} confidence)
- All Predictions: {allPredictions}
{solutionSection}

TASK:
Analyze if this appears to be legitimate cipher text or likely garbage/troll content.

Consider:
1. Does the text have cipher-like characteristics? (patterns, structure, reasonable length)
2. Does it match the statistical properties of the predicted cipher type?
3. Are there red flags suggesting random characters or spam?
4. Is the text length sufficient for cryptanalysis? (minimum 150 characters recommended)

Provide a concise analysis (3-4 sentences) with your assessment:
- ""Legitimate"" if it appears to be a real cipher
- ""Suspicious"" if uncertain or has some red flags  
- ""Likely garbage"" if it appears to be random/troll content

Include brief reasoning for your assessment.

Provide your analysis in this format:

**Cipher Legitimacy:** [Legitimate/Suspicious/Likely garbage]
**Reasoning:** [Brief explanation]
**Solution Assessment:** [If provided - Highly likely correct/Possibly correct/Unlikely correct/Definitely incorrect]
**Recommendation:** [Approve as Standard/Approve as Experimental/Reject]";
		}

		private string BuildSolvingPrompt(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var knownSolution = !string.IsNullOrWhiteSpace(decryptedText)
				? $"\n\nKNOWN SOLUTION:\n{decryptedText}"
				: "";

			return $@"You are a cryptography expert helping solve a classical cipher.

ENCRYPTED TEXT:
{encryptedText}

ML CLASSIFICATION:
- Predicted Type: {mlResult.TopPrediction.Type}
- Family: {mlResult.TopPrediction.Family}
- Confidence: {mlResult.TopPrediction.Confidence:P0}
{knownSolution}

TASK:
Attempt to decrypt this cipher and explain your solving process.

Your response should include:
1. **Cipher Type Confirmation**: Do you agree with the ML prediction? Why or why not?
2. **Solving Approach**: Explain the cryptanalysis method step-by-step
3. **Solution**: Provide the decrypted plaintext
4. **Verification**: How confident are you in this solution?

If the known solution is provided, verify it matches your analysis. If not, attempt to solve it using appropriate cryptanalysis techniques for the predicted cipher type.

Note: The ML model sometimes confuses similar cipher types (e.g., Columnar vs RailFence transposition). Consider alternative interpretations if the primary prediction doesn't yield sensible plaintext.";
		}

		private string BuildHintPrompt(
			string encryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			return $@"You are a cryptography tutor helping a student solve a cipher puzzle.

ENCRYPTED TEXT:
{encryptedText}

ML CLASSIFICATION:
- Predicted Type: {mlResult.TopPrediction.Type}
- Confidence: {mlResult.TopPrediction.Confidence:P0}

TASK:
Provide an educational hint without revealing the solution.

Your hint should:
1. Explain key characteristics of the {mlResult.TopPrediction.Type} cipher
2. Suggest what patterns or features to look for in the ciphertext
3. Recommend an approach or technique for solving this type
4. Encourage the student to apply cryptanalysis principles

Do NOT provide:
- The actual decrypted text
- Specific keys or shift values
- Step-by-step solution

Keep the hint educational and encouraging, helping them learn rather than just giving the answer.";
		}

		#endregion

		#region LLM API Communication

		private async Task<string> CallLLMAsync(string prompt, int maxTokens = 500)
		{
			try
			{
				var requestBody = new
				{
					model = _model,
					messages = new[]
					{
						new { role = "user", content = prompt }
					},
					max_tokens = maxTokens,
					temperature = 0.7
				};

				var jsonRequest = JsonSerializer.Serialize(requestBody);
				var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

				var response = await _httpClient.PostAsync(
					$"{_apiUrl}/chat/completions",
					content);

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("LLM API error: {StatusCode} - {Error}",
						response.StatusCode, errorContent);

					throw new InvalidOperationException(
						$"LLM API returned error (Status {response.StatusCode}): {errorContent}");
				}

				var responseJson = await response.Content.ReadAsStringAsync();
				var llmResponse = JsonSerializer.Deserialize<OpenAIResponse>(
					responseJson,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				if (llmResponse?.Choices == null || llmResponse.Choices.Count == 0)
				{
					throw new InvalidOperationException("LLM API returned empty response");
				}

				return llmResponse.Choices[0].Message.Content;
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "LLM service unavailable");
				throw new InvalidOperationException(
					$"LLM service is unavailable at {_apiUrl}", ex);
			}
			catch (TaskCanceledException ex)
			{
				_logger.LogError(ex, "LLM request timeout");
				throw new InvalidOperationException(
					$"LLM request timed out after {ApiTimeoutSeconds} seconds", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error during LLM call");
				throw new InvalidOperationException(
					$"Unexpected error during LLM call: {ex.Message}", ex);
			}
		}

		#endregion

		#region Internal Models for OpenAI API

		private class OpenAIResponse
		{
			[JsonPropertyName("choices")]
			public List<Choice> Choices { get; set; }

			[JsonPropertyName("usage")]
			public Usage Usage { get; set; }
		}

		private class Choice
		{
			[JsonPropertyName("message")]
			public Message Message { get; set; }

			[JsonPropertyName("finish_reason")]
			public string FinishReason { get; set; }
		}

		private class Message
		{
			[JsonPropertyName("role")]
			public string Role { get; set; }

			[JsonPropertyName("content")]
			public string Content { get; set; }
		}

		private class Usage
		{
			[JsonPropertyName("prompt_tokens")]
			public int PromptTokens { get; set; }

			[JsonPropertyName("completion_tokens")]
			public int CompletionTokens { get; set; }

			[JsonPropertyName("total_tokens")]
			public int TotalTokens { get; set; }
		}

		#endregion
	}
}