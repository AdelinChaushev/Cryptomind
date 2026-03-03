using Cryptomind.Common.Constants;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

		public LLMService(IHttpClientFactory httpClientFactory)
		{
			httpClient = httpClientFactory.CreateClient();
			httpClient.Timeout = TimeSpan.FromSeconds(ApiTimeoutSeconds);

			apiUrl = Environment.GetEnvironmentVariable("OPENAI_API_URL")
				?? throw new Exception("LLMService: ApiUrl not configured");
			apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
				?? throw new Exception("LLMService: ApiKey not configured");

			// Use cheap model for admin validation, better model for user content
			validationModel = Environment.GetEnvironmentVariable("VALIDATION_MODEL") 
				?? throw new Exception("LLMService: Validation model not configured");
			educationalModel = Environment.GetEnvironmentVariable("EDUCATIONAL_MODEL") 
				?? throw new Exception("LLMService: Educational model not configured");

			httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", apiKey);
		}

		#region Admin Validation
		private static readonly HashSet<string> SupportedCipherTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Caesar", "ROT13", "Atbash", "SimpleSubstitution",
			"Vigenere", "Autokey", "Trithemius",
			"RailFence", "Columnar", "Route",
			"Base64", "Morse", "Binary", "Hex"
		};

		public async Task<CipherValidationResultDTO> ValidateCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult,
			string? userProvidedType = null)
		{
			encryptedText = encryptedText.Replace("#", "");

			bool hasType = !string.IsNullOrWhiteSpace(userProvidedType);
			bool hasSolution = !string.IsNullOrWhiteSpace(decryptedText);

			if (!hasType && !hasSolution)
			{
				return new CipherValidationResultDTO
				{
					Issues = new List<string> { "Не е предоставен нито тип, нито решение — заявката е автоматично отхвърлена." },
					Recommendation = "reject",
					Reasoning = "Подаденият шифър не съдържа нито тип, нито решение."
				};
			}

			string prompt;
			if (hasType && hasSolution)
				prompt = BuildCase1Prompt(encryptedText, decryptedText!, mlResult, userProvidedType!);
			else if (hasType && !hasSolution)
				prompt = BuildCase2Prompt(encryptedText, userProvidedType!, mlResult);
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
				var result = JsonSerializer.Deserialize<CipherValidationResultDTO>(
					jsonResponse,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
				);

				if (result == null)
					throw new Exception(LLMConstants.InvalidDeserialization);

				if (!SupportedCipherTypes.Contains(result.PredictedType ?? string.Empty))
				{
					result.Recommendation = "reject";
					result.Issues ??= new List<string>();
					result.Issues.Add($"Предсказаният тип '{result.PredictedType}' не съответства на нито един от 14-те поддържани типа шифри — автоматично отхвърлен.");
				}

				if (result.Recommendation != "approve" && result.Recommendation != "reject")
				{
					result.Recommendation = "reject";
					result.Issues ??= new List<string>();
					result.Issues.Add("Невалидна препоръка от LLM — автоматично отхвърлен.");
				}

				return result;
			}
			catch (JsonException)
			{
				return new CipherValidationResultDTO
				{
					Issues = new List<string> { "Неуспешно разчитане на отговора от LLM — необходим е ръчен преглед." },
					Recommendation = "reject",
					Reasoning = "Отговорът на LLM не можа да бъде десериализиран в структуриран формат."
				};
			}
		}

		#endregion

		#region User-facing content
		public async Task<string> GetHint(Cipher cipher, HintType hintType)
		{
			string encryptedText = cipher.EncryptedText.Replace("#", "");
			string decryptedText = cipher.DecryptedText;

			if (cipher.TypeOfCipher == null)
				throw new ConflictException("Не може да се генерира подсказка за шифър без тип");

			if ((hintType == HintType.Hint || hintType == HintType.FullSolution) && string.IsNullOrWhiteSpace(cipher.DecryptedText))
				throw new ConflictException("Не може да се генерира подсказка за шифър без решение");

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
				default:
					throw new ArgumentException($"Unsupported hint type: {hintType}");
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
				maxTokens: 150,
				temperature: 0.3f
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
				temperature: 0.3f
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
				maxTokens: 600,
				temperature: 0.3f
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

Short text length is never a reason to reject — it only affects ML reliability. You can add it as issue though.
IMPORTANT: All ""issues"" array entries and the ""reasoning"" field MUST be written in Bulgarian.

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
Reject if the decrypted text contains: spam, advertising, promotional content (e.g. ""buy cheap viagra"", 
""click here"", ""limited offer""), offensive language, sexual content, violence, random gibberish 
that is clearly not a real cipher, or any content unsuitable for students.

TASK 2 - VERIFY THE TYPE:
Evaluate whether the user-provided type ({userProvidedType}) is correct.
The predicted type in your response MUST be one of the 14 supported types listed above.
If you cannot match the cipher to any of the 14 supported types, set recommendation to reject.
Cross-check against the ML prediction using this logic:

- If ML AND your own analysis independently agree on the same type → commit to that type confidently, do not let the user override it, set recommendation to approve
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
Look for coherent words, natural grammar, and reasonable length relative to the ciphertext (within ~10%).
If the solution is identical or nearly identical to the ciphertext, it is invalid — reject it.
A correct solution must be meaningfully different from the encrypted text.

================================================================================
JSON RESPONSE FORMAT
================================================================================

{{
  ""predicted_type"": ""must be one of the 14 supported types listed above"",
  ""confidence"": ""висока"" | ""средна"" | ""ниска"",
  ""solution_correct"": true | false,
  ""is_appropriate"": true | false,
  ""issues"": [""List ALL problems found in Bulgarian. Always include specific issues like inappropriate content, solution mismatch, wrong type etc. ONLY add 'Текстът е под 150 символа — надеждността на ML предсказването е намалена.' if the current text length is actually below 150 characters. Current length is {textLength} chars (counting all characters including spaces).""],
  ""recommendation"": ""approve"" | ""reject"",
  ""reasoning"": ""2-3 sentences in Bulgarian explaining your decision, referencing confusion patterns if relevant.""
}}

Recommendation rules:
- ""approve"": content is appropriate, type matches one of the 14 supported types, solution is valid
- ""reject"": content is inappropriate, spam, solution does not match ciphertext, or cipher cannot be matched to any of the 14 supported types";
		}

		private string BuildCase2Prompt(
			string encryptedText,
			string userProvidedType,
			CipherRecognitionResultViewModel mlResult)
		{
			var allPredictions = string.Join(", ",
				mlResult.AllPredictions.Take(5).Select(p => $"{p.Type} ({p.Confidence:P0})"));
			int textLength = encryptedText.Length;

			return $@"You are a cryptanalysis expert reviewing a cipher submission for an educational platform.
The user has submitted a cipher with a type but no solution.

NEVER reject based on text length alone. If the content is appropriate and matches a supported type, 
you MUST return approve regardless of text length.
Only reject if content is inappropriate, spam, or cannot be matched to any of the 14 supported types.

IMPORTANT: All ""issues"" array entries and the ""reasoning"" field MUST be written in Bulgarian.

ENCRYPTED TEXT ({textLength} characters):
{encryptedText}

USER-PROVIDED TYPE: {userProvidedType}

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
Reject if the decrypted text contains: spam, advertising, promotional content (e.g. ""buy cheap viagra"", 
""click here"", ""limited offer""), offensive language, sexual content, violence, random gibberish 
that is clearly not a real cipher, or any content unsuitable for students.

TASK 2 - TYPE VERIFICATION:
Does the ciphertext actually match the user-provided type ({userProvidedType})?
Analyze the statistical properties of the ciphertext and assess whether it is consistent with the claimed type.
Cross-check against the ML prediction using this logic:

- If ML AND your own analysis independently agree on the same type → commit to that type confidently
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

TASK 3 - SOLVABILITY ASSESSMENT:
Given the cipher type ({userProvidedType}), can this cipher be solved without the original key?

Examples of ciphers solvable without a key:
- Caesar (brute force 26 shifts), ROT13 (fixed shift), Atbash (fixed substitution)
- Rail Fence with short text (limited rails to try)
- Base64, Hex, Binary, Morse (deterministic encoding)

Examples of ciphers that require a key:
- Vigenere, Autokey, Trithemius, SimpleSubstitution, Columnar, Route, RailFence with long text

is_solvable MUST always be true or false, never null.

================================================================================
JSON RESPONSE FORMAT
================================================================================
IMPORTANT: DO NOT include a ""solution_correct"" field in your response. This submission has no solution and that field does not exist for this case.

{{
  ""predicted_type"": ""must be one of the 14 supported types listed above"",
  ""confidence"": ""висока"" | ""средна"" | ""ниска"",
  ""is_appropriate"": true | false,
  ""is_solvable"": true | false,
  ""issues"": [""List ALL problems found in Bulgarian. Always include specific issues like inappropriate content, solution mismatch, wrong type etc. ONLY add 'Текстът е под 150 символа — надеждността на ML предсказването е намалена.' if the current text length is actually below 150 characters. Current length is {textLength} chars (counting all characters including spaces).""],
  ""recommendation"": ""approve"" | ""reject"",
  ""reasoning"": ""2-3 sentences in Bulgarian. If rejecting, explain why. If approving, briefly note the solvability assessment and type confidence.""
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

IMPORTANT: All ""issues"" array entries and the ""reasoning"" field MUST be written in Bulgarian.
Short text length is never a reason to reject — it only affects ML reliability. You can add it as issue though.

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
Reject if the decrypted text contains: spam, advertising, promotional content (e.g. ""buy cheap viagra"", 
""click here"", ""limited offer""), offensive language, sexual content, violence, random gibberish 
that is clearly not a real cipher, or any content unsuitable for students.

TASK 2 - DETERMINE THE CIPHER TYPE:
The predicted type in your response MUST be one of the 14 supported types listed above.
If after your own analysis the cipher cannot be matched to any of the 14 supported types, set recommendation to reject.
Evaluate the ML prediction critically using this logic:

- If ML AND your own analysis independently agree on the same type → commit to that type confidently
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
If the solution is identical or nearly identical to the ciphertext, it is invalid — reject it.
A correct solution must be meaningfully different from the encrypted text.

================================================================================
JSON RESPONSE FORMAT
================================================================================
{{
  ""predicted_type"": ""must be one of the 14 supported types listed above"",
  ""confidence"": ""висока"" | ""средна"" | ""ниска"",
  ""solution_correct"": true | false,
  ""is_appropriate"": true | false,
  ""issues"": [""List ALL problems found in Bulgarian. Always include specific issues like inappropriate content, solution mismatch, wrong type etc. ONLY add 'Текстът е под 150 символа — надеждността на ML предсказването е намалена.' if the current text length is actually below 150 characters. Current length is {textLength} chars (counting all characters including spaces).""],
  ""recommendation"": ""approve"" | ""reject"",
  ""reasoning"": ""2-3 sentences in Bulgarian. The user provided NO type — do not reference the user providing a wrong type. State whether you agree with ML, reference confusion patterns if relevant, and note solution validity.""
}}

Recommendation rules:
- ""approve"": content is appropriate, type is identifiable from the supported list, solution is valid
- ""reject"": content is inappropriate, spam, solution does not match ciphertext, or cipher cannot be matched to any of the 14 supported types";
		}
		#endregion

		#region Prompt Engineering/Hint
		private string BuildTypeHintPrompt(
			string encryptedText,
			string actualType)
		{
			return $@"You are a cryptography assistant helping a student identify a cipher.

IMPORTANT: Respond only in Bulgarian.

ENCRYPTED TEXT:
{encryptedText}

CIPHER TYPE: {actualType}

In 2-3 sentences, tell the student this is a {actualType} cipher and explain the key features 
in the ciphertext that reveal this — what should they look for to identify it themselves next time.

No markdown, no headers, no bullet points. Be direct and friendly.";
		}
		private string BuildEducationalHintPrompt(
			string encryptedText,
			string actualType,
			string decryptedText)
		{
			return $@"You are a cryptography tutor giving a hint for a {actualType} cipher.

IMPORTANT: Respond only in Bulgarian.

ENCRYPTED TEXT:
{encryptedText}

ACTUAL SOLUTION: {decryptedText}
(Use this only as internal context to guide your hints — DO NOT reveal the plaintext, the key, shift values, or any specific parameters)

Write 3 paragraphs:
- First: Point out specific visual or statistical features in THIS ciphertext that reveal it is a {actualType} cipher. Reference actual patterns or letter distributions you can observe in the text itself.
- Second: Give a concrete approach for solving THIS specific ciphertext — what the student should try first, what to look for, and what tools or techniques apply to {actualType} ciphers.
- Third: Begin the solving process with the student. Show the first concrete step applied to THIS ciphertext — for example the first letter or first few letters being worked through, or the first calculation — without completing the solution or revealing the final answer.

No markdown, no headers, no bullet points. No more than 250 words total.";
		}
		private string BuildFullSolutionPrompt(
			string encryptedText,
			string actualType,
			string decryptedText)
		{
			return $@"You are an expert cryptography instructor reviewing a fully solved cipher with a student.
IMPORTANT: Respond only in Bulgarian.

The solution is already known.
Your explanation MUST be tightly grounded in the provided texts.
Generic or purely theoretical explanations are NOT acceptable.

ENCRYPTED TEXT (full):
{encryptedText}

CIPHER TYPE:
{actualType}

DECRYPTED TEXT (full):
{decryptedText}

Requirements for your explanation:
- Explicitly reference multiple concrete fragments from the encrypted text (beginning, middle, and end)
- Explain how these specific fragments transform into the corresponding parts of the decrypted text
- Describe the decryption logic in sequence, making clear how the key or rule evolves during the process in THIS exact ciphertext
- If a key or starting word exists, explain how it is applied and then how the decrypted text itself continues the process
- Demonstrate why the decrypted text is correct by tying meaning and structure back to the encrypted text
- Do NOT rely on abstract descriptions alone — the explanation must be impossible to write without having this exact ciphertext

You are allowed to paraphrase fragments if the text is long, but you must clearly indicate which part of the ciphertext you are referring to.
Assume the student wants to fully reconstruct the solution from your explanation.
Do not hide steps.
Do not use markdown, headers, or bullet points.
Maximum 300 words.
Write naturally, like a careful teacher reviewing the entire solution.";
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
					new { role = "system", content = "You are a cryptography tutor. Always respond in Bulgarian." },
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

				throw new Exception($"LLM API върна грешка (Статус{response.StatusCode}): {errorContent}");
			}

			var responseJson = await response.Content.ReadAsStringAsync();
			var llmResponse = JsonSerializer.Deserialize<OpenAIResponse>(
				responseJson,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			if (llmResponse?.Choices == null || llmResponse.Choices.Count == 0)
				throw new Exception("LLM API върна празен отговор");


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
	}
}