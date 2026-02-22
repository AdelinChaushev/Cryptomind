using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using Cryptomind.Common.Constants;
using Microsoft.Extensions.Configuration;
using Cryptomind.Common.Exceptions;

namespace Cryptomind.Core.Services
{
	public class LLMService : ILLMService
	{
		private readonly HttpClient httpClient;
		private readonly string apiUrl;
		private readonly string apiKey;
		private readonly string validationModel;  // Cheap model for admin validation
		private readonly string educationalModel; // Better model for user-facing content

		private const int ApiTimeoutSeconds = LLMConstants.ApiTimeoutSeconds;

		public LLMService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration)
		{
			httpClient = httpClientFactory.CreateClient();
			httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds);

			apiUrl = configuration["LLMService:ApiUrl"]
				?? throw new Exception("LLMService:ApiUrl not configured");
			apiKey = configuration["LLMService:ApiKey"]
				?? throw new Exception("LLMService:ApiKey not configured");

			// Use cheap model for admin validation, better model for user content
			validationModel = configuration["LLMService:ValidationModel"] ?? "gpt-4o-mini";
			educationalModel = configuration["LLMService:EducationalModel"] ?? "gpt-4o";

			httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", apiKey);
		}

		#region Admin Validation
		public async Task<CipherValidationResult> ValidateCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult,
			string? userProvidedType = null)
		{
			bool hasType = !string.IsNullOrWhiteSpace(userProvidedType);
			bool hasSolution = !string.IsNullOrWhiteSpace(decryptedText);

			string prompt;

			if (hasType && hasSolution)
				prompt = BuildCase1Prompt(encryptedText, decryptedText!, mlResult, userProvidedType!);
			else if (hasType && !hasSolution)
				prompt = BuildCase2Prompt(encryptedText, userProvidedType!);
			else
				prompt = BuildCase3Prompt(encryptedText, decryptedText!, mlResult);

			var jsonResponse = await CallLLMWithJsonAsync(
				prompt,
				model: validationModel,
				maxTokens: 800,
				temperature: 0.3f
			);

			try
			{
				var result = JsonSerializer.Deserialize<CipherValidationResult>(
					jsonResponse,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
				);

				if (result == null)
					throw new Exception("Failed to deserialize validation result");

				return result;
			}
			catch (JsonException)
			{
				return new CipherValidationResult
				{
					Issues = new List<string> { "LLM response parsing failed - manual review required" },
					Recommendation = "manual_review",
					Reasoning = "Could not parse LLM response into structured format"
				};
			}
		}

		#endregion

		#region User-facing content
		public async Task<string> GetHint(Cipher cipher, HintType hintType)
		{
			string encryptedText = cipher.EncryptedText;
			string decryptedText = cipher.DecryptedText;

			if (cipher.TypeOfCipher == null)
				throw new ConflictException("Cannot generate hint for cipher without a type");

			string actualType = cipher.TypeOfCipher.ToString();

			string result = string.Empty;

			switch (hintType)
			{
				case HintType.Type:
					result = await GetTypeHintAsync(encryptedText, actualType);
					break;
				case HintType.Hint:
					result = await GetHintAsync(encryptedText, actualType, decryptedText);
					break;
				case HintType.FullSolution:
					result = await GetFullSolutionAsync(encryptedText, actualType, decryptedText);
					break;
			}

			return result;
		}
		#endregion

		#region Private methods
		private async Task<string> GetTypeHintAsync(
			string encryptedText,
			string actualType)
		{
			var prompt = BuildTypeHintPrompt(encryptedText, actualType);

			return await CallLLMAsync(
				prompt,
				model: educationalModel,
				maxTokens: 400,
				temperature: 0.7f
			);
		}
		private async Task<string> GetHintAsync(
			string encryptedText,
			string actualType,
			string decryptedText)
		{
			var prompt = BuildEducationalHintPrompt(encryptedText, actualType, decryptedText);

			return await CallLLMAsync(
				prompt,
				model: educationalModel,
				maxTokens: 600,
				temperature: 0.7f
			);
		}
		private async Task<string> GetFullSolutionAsync(
			string encryptedText,
			string actualType,
			string decryptedText)
		{
			var prompt = BuildFullSolutionPrompt(encryptedText, actualType, decryptedText);

			return await CallLLMAsync(
				prompt,
				model: educationalModel,
				maxTokens: 1000,
				temperature: 0.7f
			);
		}
		#endregion

		#region Prompt Engineering/Validation
		private string BuildCase1Prompt(
			string encryptedText,
			string decryptedText,
			CipherRecognitionResultViewModel mlResult,
			string userProvidedType)
		{
			var allPredictions = string.Join(", ",
				mlResult.AllPredictions.Take(5).Select(p => $"{p.Type} ({p.Confidence:P0})"));
			int textLength = encryptedText.Length;
			return $@"You are a cryptanalysis expert validating a cipher submission for an educational platform.

			ENCRYPTED TEXT ({textLength} characters):
			{encryptedText}

			USER-PROVIDED TYPE: {userProvidedType}
			USER-PROVIDED SOLUTION: {decryptedText}

			ML ANALYSIS:
			- Top Prediction: {mlResult.TopPrediction.Type} ({mlResult.TopPrediction.Confidence:P0} confidence)
			- All Predictions: {allPredictions}

			SUPPORTED CIPHER TYPES (the only valid types for this platform):
			Substitution family: Caesar, ROT13, Atbash, SimpleSubstitution
			Polyalphabetic family: Vigenere, Autokey, Trithemius
			Transposition family: RailFence, Columnar, Route
			Encoding family: Base64, Morse, Binary, Hex

			================================================================================
			YOUR THREE TASKS
			================================================================================
			TASK 1 - APPROPRIATENESS CHECK:
			After decrypting, is this legitimate cipher content suitable for an educational platform?
			Reject if: spam, inappropriate content, random gibberish that is clearly not a real cipher, offensive material.

			TASK 2 - VERIFY THE TYPE:
			Evaluate whether the user-provided type ({userProvidedType}) is correct.
			The predicted type in your response MUST be one of the 14 supported types listed above.
			Cross-check it against the ML prediction using this logic:

			- If ML AND your own analysis independently agree on the same type → commit to that
			  type confidently, do not let the user override it, set recommendation to approve
			- If ML is uncertain OR your own analysis disagrees with ML → trust the user over ML

			Known confusion pairs and ML confidence thresholds where ML is considered uncertain:
			- Columnar/RailFence: ML unreliable below 90% confidence
			- Vigenere/Trithemius: ML unreliable below 70% confidence
			- Caesar/SimpleSubstitution: low priority, functionally similar

			TEXT LENGTH IMPACT ON ML RELIABILITY:
			- Below 150 chars: ML very unreliable
			- 150-199 chars: Reduced ML reliability
			- 200-400 chars: Optimal range
			Current length: {textLength} chars

			TASK 3 - VERIFY THE SOLUTION:
			Check that the solution is genuinely valid English plaintext.
			Look for coherent words, natural grammar, and reasonable length relative to the ciphertext.
			A correct solution should be similar in length to the ciphertext (within ~10%).

			================================================================================
			JSON RESPONSE FORMAT
			================================================================================

			{{
			  ""predicted_type"": ""must be one of the 14 supported types listed above"",
			  ""confidence"": ""high"" | ""medium"" | ""low"",
			  ""solution_correct"": true | false,
			  ""is_appropriate"": true | false,
			  ""issues"": [""list any specific problems found. If text is below 150 characters always add: 'Text below 150 characters — ML prediction reliability is reduced'""],
			  ""recommendation"": ""approve"" | ""reject"" | ""manual_review"",
			  ""reasoning"": ""2-3 sentences explaining your decision, referencing confusion patterns if relevant""
			}}

			Recommendation rules:
			- ""approve"": content is appropriate, type is identifiable, solution is valid
			- ""manual_review"": use ONLY when you are genuinely uncertain between two or more of the 14 supported types after your own analysis
			- ""reject"": use when content is inappropriate, spam, clearly not a real cipher, or the cipher cannot be matched to any of the 14 supported types";
		}

		private string BuildCase2Prompt(
			string encryptedText,
			string userProvidedType)
		{
			int textLength = encryptedText.Length;

			return $@"You are a cryptanalysis expert reviewing a cipher submission for an educational platform.
			The user has submitted a cipher with a type but no solution.

			ENCRYPTED TEXT ({textLength} characters):
			{encryptedText}

			USER-PROVIDED TYPE: {userProvidedType}

			SUPPORTED CIPHER TYPES (the only valid types for this platform):
			Substitution family: Caesar, ROT13, Atbash, SimpleSubstitution
			Polyalphabetic family: Vigenere, Autokey, Trithemius
			Transposition family: RailFence, Columnar, Route
			Encoding family: Base64, Morse, Binary, Hex

			================================================================================
			YOUR TWO TASKS
			================================================================================

			TASK 1 - APPROPRIATENESS CHECK:
			Is this legitimate cipher content suitable for an educational platform?
			Reject if: spam, inappropriate content, random gibberish that is clearly not a real cipher, offensive material.
			Also reject if the encrypted text cannot be matched to any of the 14 supported types listed above.

			TASK 2 - SOLVABILITY ASSESSMENT:
			Given the cipher type ({userProvidedType}), can this cipher be solved without the original key?

			Examples of ciphers solvable without a key:
			- Caesar (brute force 26 shifts)
			- ROT13 (fixed shift)
			- Atbash (fixed substitution)
			- Rail Fence with short text (limited rails to try)
			- Base64, Hex, Binary, Morse (deterministic encoding)

			Examples of ciphers that require a key:
			- Vigenere (needs the keyword)
			- Autokey (needs the key)
			- Trithemius (structured but complex)
			- SimpleSubstitution (26! combinations)
			- Columnar/Route (needs column order or route pattern)

			TASK 3 - TYPE VERIFICATION:
			Does the ciphertext actually match the user-provided type ({userProvidedType})?
			Analyze the statistical properties of the ciphertext and assess whether it is consistent with the claimed type.
			Your confidence reflects how certain you are that the ciphertext matches the user-provided type.

			This assessment is informational for the admin — they make the final decision on how to categorize it.

			is_solvable MUST always be true or false. Never null.
			- true: cipher can be solved without the original key (Caesar, ROT13, Atbash, Base64, Hex, Binary, Morse)
			- false: cipher requires the original key to decrypt (Vigenere, Autokey, SimpleSubstitution, Columnar, Route, RailFence with long text)

			================================================================================
			JSON RESPONSE FORMAT
			================================================================================

			{{
			  ""predicted_type"": ""must be one of the 14 supported types listed above"",
			  ""confidence"": ""high"" | ""medium"" | ""low"",
			  ""is_appropriate"": true | false,
			  ""is_solvable"": true | false,
			  ""issues"": [""list any specific problems found""],
			  ""recommendation"": ""approve"" | ""reject"",
			  ""reasoning"": ""2-3 sentences. If rejecting, explain why. If approving, briefly note the solvability assessment and type confidence.""
			}}

			Recommendation rules:
			- ""approve"": content is appropriate and matches one of the 14 supported types
			- ""reject"": content is inappropriate, spam, clearly not a real cipher, or cannot be matched to any of the 14 supported types";
		}

		private string BuildCase3Prompt(
			string encryptedText,
			string decryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var allPredictions = string.Join(", ",
				mlResult.AllPredictions.Take(5).Select(p => $"{p.Type} ({p.Confidence:P0})"));

			int textLength = encryptedText.Length;

			return $@"You are a cryptanalysis expert validating a cipher submission for an educational platform.
			The user has provided a solution but no cipher type. Your job is to determine the correct type.

			ENCRYPTED TEXT ({textLength} characters):
			{encryptedText}

			USER-PROVIDED SOLUTION: {decryptedText}

			ML ANALYSIS:
			- Top Prediction: {mlResult.TopPrediction.Type} ({mlResult.TopPrediction.Confidence:P0} confidence)
			- All Predictions: {allPredictions}

			SUPPORTED CIPHER TYPES (the only valid types for this platform):
			Substitution family: Caesar, ROT13, Atbash, SimpleSubstitution
			Polyalphabetic family: Vigenere, Autokey, Trithemius
			Transposition family: RailFence, Columnar, Route
			Encoding family: Base64, Morse, Binary, Hex

			================================================================================
			YOUR THREE TASKS
			================================================================================
			TASK 1 - APPROPRIATENESS CHECK:
			After decrypting, is this legitimate cipher content suitable for an educational platform?
			Reject if: spam, inappropriate content, random gibberish that is clearly not a real cipher, offensive material.

			TASK 2 - DETERMINE THE CIPHER TYPE:
			The predicted type in your response MUST be one of the 14 supported types listed above.
			If after your own analysis the cipher cannot be matched to any of the 14 supported types, set recommendation to reject.
			Evaluate the ML prediction critically using this logic:

			- If ML AND your own analysis independently agree on the same type → commit to that
			  type confidently
			- If ML is uncertain OR your own analysis disagrees with ML → rely on your own analysis

			Known confusion pairs and ML confidence thresholds where ML is considered uncertain:
			- Columnar/RailFence: ML unreliable below 90% confidence
			- Vigenere/Trithemius: ML unreliable below 70% confidence
			- Caesar/SimpleSubstitution: low priority, functionally similar

			TEXT LENGTH IMPACT ON ML RELIABILITY:
			- Below 150 chars: ML very unreliable, rely more on your own analysis
			- 150-199 chars: Reduced ML reliability
			- 200-400 chars: Optimal range
			Current length: {textLength} chars

			TASK 3 - VERIFY THE SOLUTION:
			Check that the solution is genuinely valid English plaintext.
			Look for coherent words, natural grammar, and reasonable length relative to the ciphertext (within ~10%).

			================================================================================
			JSON RESPONSE FORMAT
			================================================================================
			{{
			  ""predicted_type"": ""must be one of the 14 supported types listed above"",
			  ""confidence"": ""high"" | ""medium"" | ""low"",
			  ""solution_correct"": true | false,
			  ""is_appropriate"": true | false,
			  ""issues"": [""list any specific problems found. If text is below 150 characters always add: 'Text below 150 characters — ML prediction reliability is reduced'""],
			  ""recommendation"": ""approve"" | ""reject"" | ""manual_review"",
			  ""reasoning"": ""2-3 sentences. State whether you agree with ML, reference confusion patterns if relevant, and note solution validity.""
			}}

			Recommendation rules:
			- ""approve"": content is appropriate, type is identifiable from the supported list, solution is valid
			- ""manual_review"": use ONLY when you are genuinely uncertain between two or more of the 14 supported types after your own analysis
			- ""reject"": use when content is inappropriate, spam, the solution clearly does not match the ciphertext, or the cipher cannot be matched to any of the 14 supported types";
		}
		#endregion

		#region Prompt Engineering/Hint
		private string BuildTypeHintPrompt(
			string encryptedText,
			string actualType)
		{
			return $@"You are a cryptography assistant helping a student identify a cipher.

			ENCRYPTED TEXT:
			{encryptedText}
			
			ACTUAL CIPHER TYPE: {actualType}
			
			In 2-3 sentences maximum:
			1. State clearly this is a {actualType} cipher
			2. Mention one key identifying feature
			
			No markdown, no headers, no bullet points. Be direct and friendly.";
		}
		private string BuildEducationalHintPrompt(
			string encryptedText,
			string actualType,
			string? decryptedText)
		{
			var solutionNote = !string.IsNullOrWhiteSpace(decryptedText)
				? $"\n\nACTUAL SOLUTION: {decryptedText}\n(DO NOT reveal the plaintext directly)"
				: "";

			return $@"You are a cryptography tutor giving a hint for a {actualType} cipher.

			ENCRYPTED TEXT:
			{encryptedText}{solutionNote}
			
			Write 2 short paragraphs:
			- First: What features in the ciphertext reveal it is a {actualType} cipher
			- Second: How to approach solving it without giving the answer
			
			DO NOT reveal the plaintext, the key, shift values, or any specific parameters.
			No markdown, no headers, no bullet points. No more than 150 words total.";
		}

		private string BuildFullSolutionPrompt(
			string encryptedText,
			string actualType,
			string decryptedText)
		{
			return $@"You are a cryptography expert explaining a solved cipher to a student.

			ENCRYPTED TEXT:
			{encryptedText}
			
			CIPHER TYPE: {actualType}
			SOLUTION: {decryptedText}
			
			Write a concise walkthrough in 3 short paragraphs:
			- First: What features identify this as a {actualType} cipher
			- Second: How the decryption works, key steps only
			- Third: One interesting fact about {actualType} ciphers
			Do not invent specific examples, only explain the general process.
			
			No markdown, no headers, no bullet points. No more than 150 words total. 
			Write naturally like you are explaining to a friend, not writing an essay.";
		}
		#endregion

		#region LLM API Communication
		private async Task<string> CallLLMWithJsonAsync(
			string prompt,
			string model,
			int maxTokens = 500,
			float temperature = 0.7f)
		{
			var requestBody = new
			{
				model,
				messages = new[]
				{
					new
					{
						role = "system",
						content = "You are a cryptanalysis expert. Always respond with valid JSON only."
					},
					new { role = "user", content = prompt }
				},
				max_tokens = maxTokens,
				temperature,
				response_format = new { type = "json_object" }
			};

			return await ExecuteLLMRequestAsync(requestBody);
		}
		private async Task<string> CallLLMAsync(
			string prompt,
			string model,
			int maxTokens = 500,
			float temperature = 0.7f)
		{
			var requestBody = new
			{
				model,
				messages = new[]
				{
						new { role = "user", content = prompt }
				},
				max_tokens = maxTokens,
				temperature
			};

			return await ExecuteLLMRequestAsync(requestBody);
		}
		private async Task<string> ExecuteLLMRequestAsync(object requestBody)
		{
			var jsonRequest = JsonSerializer.Serialize(requestBody);
			var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

			var response = await httpClient.PostAsync(
				$"{apiUrl}/chat/completions",
				content);

			if (!response.IsSuccessStatusCode)
			{
				var errorContent = await response.Content.ReadAsStringAsync();

				throw new Exception($"LLM API returned error (Status {response.StatusCode}): {errorContent}");
			}

			var responseJson = await response.Content.ReadAsStringAsync();
			var llmResponse = JsonSerializer.Deserialize<OpenAIResponse>(
				responseJson,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (llmResponse?.Choices == null || llmResponse.Choices.Count == 0)
				throw new Exception("LLM API returned empty response");


			return llmResponse.Choices[0].Message.Content;
		}

		#endregion

		#region Data Models
		private class OpenAIResponse
		{
			[JsonPropertyName("choices")]
			public List<Choice> Choices { get; set; }
		}

		private class Choice
		{
			[JsonPropertyName("message")]
			public Message Message { get; set; }
		}

		private class Message
		{
			[JsonPropertyName("content")]
			public string Content { get; set; }
		}

		#endregion

		#region Public Models for LLM Responses
		public class CipherValidationResult
		{
			[JsonPropertyName("predicted_type")]
			public string? PredictedType { get; set; }

			[JsonPropertyName("confidence")]
			public string? Confidence { get; set; }

			[JsonPropertyName("solution_correct")]
			public bool? SolutionCorrect { get; set; }

			[JsonPropertyName("is_appropriate")]
			public bool IsAppropriate { get; set; }

			[JsonPropertyName("is_solvable")]
			public bool? IsSolvable { get; set; }

			[JsonPropertyName("issues")]
			public List<string> Issues { get; set; } = new();

			[JsonPropertyName("recommendation")]
			public string Recommendation { get; set; } = string.Empty;

			[JsonPropertyName("reasoning")]
			public string Reasoning { get; set; } = string.Empty;
		}
		#endregion
	}
}