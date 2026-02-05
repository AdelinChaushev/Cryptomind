using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cryptomind.Common.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cryptomind.Core.Services
{
	public class LLMService : ILLMService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiUrl;
		private readonly string _apiKey;
		private readonly string _validationModel;  // Cheap model for admin validation
		private readonly string _educationalModel; // Better model for user-facing content
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

			// Use cheap model for admin validation, better model for user content
			_validationModel = configuration["LLMService:ValidationModel"] ?? "gpt-4o-mini";
			_educationalModel = configuration["LLMService:EducationalModel"] ?? "gpt-4o";

			_logger = logger;

			_httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", _apiKey);
		}

		#region Admin Validation
		public async Task<CipherValidationResult> ValidateCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult,
			string? userProvidedType = null)
		{
			var prompt = BuildValidationPrompt(encryptedText, decryptedText, mlResult, userProvidedType);

			var jsonResponse = await CallLLMWithJsonAsync(
				prompt,
				model: _validationModel,
				maxTokens: 800,
				temperature: 0.3f  // Lower temperature for consistent structured output
			);

			try
			{
				var result = JsonSerializer.Deserialize<CipherValidationResult>(
					jsonResponse,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
				);

				if (result == null)
				{
					throw new InvalidOperationException("Failed to deserialize validation result");
				}

				return result;
			}
			catch (JsonException ex)
			{
				_logger.LogError(ex, "Failed to parse LLM validation response: {Response}", jsonResponse);

				// Fallback to manual review if JSON parsing fails
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
			string actualType = cipher.TypeOfCipher.ToString();

			string result = string.Empty;

			switch (hintType)
			{
				case HintType.Type:
						result = await GetTypeHintAsync(encryptedText, actualType, decryptedText);
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
			string actualType,
			string decryptedText)
		{
			var prompt = BuildTypeHintPrompt(encryptedText, actualType);

			return await CallLLMAsync(
				prompt,
				model: _educationalModel,
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
				model: _educationalModel,
				maxTokens: 600,
				temperature: 0.7f  // Higher temperature for more natural/creative hints
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
				model: _educationalModel,
				maxTokens: 1000,
				temperature: 0.7f
			);
		}
		#endregion

		#region Health Check

		public async Task<bool> IsServiceHealthyAsync()
		{
			try
			{
				var testPrompt = "Respond with 'OK' if you can read this.";
				var response = await CallLLMAsync(testPrompt, model: _validationModel, maxTokens: 10);
				return !string.IsNullOrWhiteSpace(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "LLM service health check failed");
				return false;
			}
		}

		#endregion

		#region Prompt Engineering
		private string BuildValidationPrompt(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult,
			string? userProvidedType)
			{
			var allPredictions = string.Join(", ",
				mlResult.AllPredictions.Take(5).Select(p =>
					$"{p.Type} ({p.Confidence:P0})"));
		
			var userTypeSection = !string.IsNullOrWhiteSpace(userProvidedType)
				? $"\nUSER-PROVIDED TYPE: {userProvidedType}"
				: "";
		
			var solutionSection = !string.IsNullOrWhiteSpace(decryptedText)
				? $"\nUSER-PROVIDED SOLUTION:\n{decryptedText}"
				: "";

			int textLength = encryptedText.Length;
			string lengthWarning = textLength < 150
				? "\n⚠️ CRITICAL: Text below 150 characters - ML reliability severely compromised"
				: textLength < 200
					? "\n⚠️ WARNING: Text below optimal 200 characters - expect reduced ML confidence"
					: "";

			return $@"You are a cryptanalysis validator analyzing a cipher submission for admin approval.

				ENCRYPTED TEXT ({textLength} characters):{lengthWarning}
				{encryptedText}
				
				ML ANALYSIS:
				- Top Prediction: {mlResult.TopPrediction.Type} ({mlResult.TopPrediction.Confidence:P0} confidence)
				- All Predictions: {allPredictions}{userTypeSection}{solutionSection}
				
				================================================================================
				CRITICAL: ML SYSTEM CONFUSION PATTERNS (based on 220-sample validation)
				================================================================================
				
				🔴 **HIGH RISK CONFUSIONS** (require extra scrutiny):
				Keep this confusions in mind if they are in the top prediction
				
				1. Vigenere and Trithemius
				2. Be careful in all the transposition ciphers (Route, Columnar, Railfence), one of the most commong ML mistakes is Columnar and Railfence
				3. Also sometimes in Caeasr and SimpleSubstitution

				Be extra careful in these, if you are not sure about something try decrypting it yourself.
				
				================================================================================
				TEXT LENGTH IMPACT ON RELIABILITY
				================================================================================
				
				- **Below 150 chars**: Statistical features unstable - DO NOT TRUST ML
				- **150-199 chars**: Expect 10-15% accuracy drop - increase scrutiny
				- **200-400 chars**: OPTIMAL RANGE - ML performs at 94.5% accuracy
				- **Above 400 chars**: Stable but diminishing returns
				
				Current text length: {textLength} characters
				Expected ML reliability: {(textLength < 150 ? "VERY LOW" : textLength < 200 ? "REDUCED" : "OPTIMAL")}
				
				================================================================================
				WHAT ML CANNOT DETECT (auto-reject these)
				================================================================================
				
				❌ **Compound/Nested Encryption**
				   - Example: Vigenere → RailFence, Caesar → Base64
				   - ML assumes single-layer encryption only
				   - Detection: Nonsensical patterns even after correct decryption
				
				❌ **Modern Cryptography**
				   - AES, RSA, any post-1950s encryption
				   - ML trained only on classical historical ciphers
				
				❌ **Non-English Text**
				   - System trained on English corpus (IC ≈ 0.065 assumption)
				   - Other languages have different statistical properties
				
				❌ **Encoding vs Cipher Confusion**
				   - Base64, Morse, Binary, Hex use RULE-BASED detection (not ML)
				   - Low confidence on these → likely garbage input
				
				================================================================================
				VALIDATION PROTOCOL
				================================================================================
				
				**STEP 1: AUTO-REJECT CHECK**
				□ Text length <150 chars AND confidence <75%
				□ Suspected compound encryption (mixed patterns)
				□ Modern encryption signatures
				□ Garbage/spam/inappropriate content
				□ Non-English text patterns
				
				**STEP 2: EVALUATE ML PREDICTION**
				
				IF prediction is **Vigenere**:
				  - Confidence <70% → HIGH PROBABILITY it's actually Trithemius
				  - Confidence 70-85% → MODERATE RISK, check alternative predictions
				  - Confidence >85% → Likely correct, but still check Trithemius alternative
				
				IF prediction is **RailFence**:
				  - Confidence >75% → STRONG POSSIBILITY it's actually Columnar
				  - Confidence >85% → 50/50 chance (model confidently wrong in 42% of cases)
				  - Always list Columnar as primary alternative
				
				IF prediction is **Columnar**:
				  - Confidence <90% → MIGHT BE RailFence instead
				  - Always list RailFence as alternative
				  - User disagreement → trust user over ML
				
				IF prediction is **Trithemius**:
				  - Confidence <80% → Check if it's Vigenere
				  - Higher confidence → Usually reliable
				
				IF prediction is **Route**:
				  - Generally reliable (95% accuracy)
				  - Only confuses with Columnar occasionally
				
				IF prediction is **Caesar**:
				  - May be ROT13 or generic SimpleSubstitution
				  - Functionally similar - low priority issue
				
				**STEP 3: CROSS-VALIDATE USER TYPE** (if provided)
				
				□ User type matches ML → Increases confidence significantly
				□ User type conflicts with ML:
				  - If conflict is Columnar/RailFence → TRUST USER (42% ML error rate here)
				  - If conflict is Vigenere/Trithemius → TRUST USER (58% ML error rate here)
				  - Other conflicts → Examine cryptographic characteristics manually
				
				**STEP 4: VERIFY SOLUTION** (if provided)
				
				□ Valid English text (coherent words, grammar, punctuation)
				□ Similar length to ciphertext (±10% acceptable)
				□ Apply English scoring: common words, natural flow
				□ If solution seems wrong but ML confident → Flag for manual review
				□ If no solution → set solution_correct: false
				
				**STEP 5: FINAL RECOMMENDATION**
				
				Choose ONE:
				- **approve**: High confidence (>85%) + no confusion risk + valid solution
				- **experimental**: Valid cipher but no solution OR moderate confidence (70-85%) OR in confusion zone
				- **reject**: Auto-reject criteria met, garbage, inappropriate, <150 chars with low confidence
				- **manual_review**: Conflicting signals, user disagrees with ML on high-risk confusion pair, or edge case
				
				================================================================================
				JSON RESPONSE FORMAT
				================================================================================
				
				{{
				  ""ml_prediction_valid"": boolean,
				  ""confidence"": ""high"" | ""medium"" | ""low"",
				  ""predicted_type"": ""string (your actual determination)"",
				  ""alternative_types"": [""max 2-3 plausible alternatives based on confusion matrix""],
				  ""solution_correct"": boolean,
				  ""decrypted_text"": ""string (verified plaintext or empty)"",
				  ""issues"": [""specific problems found - reference confusion patterns""],
				  ""recommendation"": ""approve"" | ""reject"" | ""experimental"",
				  ""reasoning"": ""2-3 sentences explaining decision with reference to confusion data""
				}}
				
				================================================================================
				EXAMPLE REASONING FORMATS
				================================================================================
				
				Good: ""ML predicts RailFence at 87% confidence, but this falls in the high-risk Columnar/RailFence confusion zone (42% of errors). Listed Columnar as primary alternative. Recommend manual review.""
				
				Good: ""ML predicts Vigenere at 68% confidence. This is below the 70% threshold where Vigenere commonly misclassifies as Trithemius (58% of errors). Predicted type: Trithemius.""
				
				Good: ""User provided type 'Columnar' conflicts with ML prediction 'RailFence' (94% confidence). Given 42% error rate in this bidirectional confusion, trusting user over ML.""
				
				Bad: ""Low confidence, needs review."" (too vague)
				Bad: ""This is probably Vigenere."" (ignores confusion data)
				
				================================================================================
				
				Be critical but data-driven. Reference the specific confusion patterns when making decisions. The goal is protecting platform quality while supporting legitimate submissions."; ;
		}
		private string BuildTypeHintPrompt(string encryptedText, string actualType)
		{
			return $@"You are a cryptography assistant helping a student identify a cipher type.

			ENCRYPTED TEXT:
			{encryptedText}
			
			ACTUAL CIPHER TYPE: {actualType}
			
			Your task is to reveal that this is a {actualType} cipher and briefly explain how to recognize it.
			
			Provide a short, direct response (2-3 sentences) that:
			1. States clearly: ""This is a {actualType} cipher""
			2. Mentions 1-2 key identifying features of {actualType} ciphers
			3. Is friendly and encouraging
			
			DO NOT:
			- Explain how to solve it (that's for the solving hint)
			- Use markdown formatting, headers, or bullet points
			- Write more than 3 sentences
			- Give the solution or key
			
			TONE: Direct but friendly, like quickly answering a question.
			
			Example good response: ""This is a Caesar cipher. You can tell because the letter frequencies are relatively uniform and the text preserves its original spacing and punctuation - classic signs of a simple shift cipher where every letter moves the same number of positions.""
			
			Example bad response: ""### Cipher Identification\n\nAfter analyzing the statistical properties...[long explanation]""
			
			Keep it short and sweet - just the type reveal with a quick explanation of why.";
		}
		private string BuildEducationalHintPrompt(
			string encryptedText,
			string actualType,
			string? decryptedText)
			{
				var solutionNote = !string.IsNullOrWhiteSpace(decryptedText)
					? $"\n\nACTUAL SOLUTION: {decryptedText}\n(DO NOT reveal the plaintext directly)"
					: "";
		
				return $@"You are a friendly cryptography tutor helping a student solve a cipher challenge.
		
					ENCRYPTED TEXT:
					{encryptedText}
					
					ACTUAL CIPHER TYPE: {actualType}{solutionNote}
					
					Your goal is to provide an educational hint that helps them solve this {actualType} cipher without giving away the final answer.
					
					Your hint should follow this structure (but write it naturally, not as separate sections):
					
					FIRST - Guide them to identify the cipher type:
					- Point out observable features in the ciphertext
					- Explain what patterns or characteristics to look for
					- Lead them to realize this is a {actualType} cipher (you can reveal the type, but explain WHY)
					
					THEN - Explain the solving approach:
					- Describe the general method for solving {actualType} ciphers
					- Suggest specific techniques (frequency analysis, pattern matching, etc.)
					- Give concrete things to look for in THIS specific ciphertext
					- Encourage them to try the approach themselves
					
					IMPORTANT RULES:
					- DO reveal the cipher type ({actualType}) and explain how to identify it
					- DO explain the solving methodology step-by-step
					- DO NOT reveal the actual plaintext solution
					- DO NOT give specific keys, shift values, or exact parameters
					- DO NOT provide the complete worked solution
					
					TONE AND FORMAT:
					- Write in natural, conversational paragraphs (3-4 paragraphs total)
					- NO markdown headers (###, ####, etc.)
					- NO bullet points or numbered lists
					- NO excessive bold or italic formatting
					- Sound like a helpful tutor explaining concepts, not a textbook
					
					Example good opening: ""Looking at this text, the first thing I notice is..."" 
					Example bad opening: ""### Cipher Identification\n\nThe text exhibits...""
					
					Keep it engaging and encouraging. Make them feel like they're learning, not just getting answers handed to them.";
			}
		private string BuildFullSolutionPrompt(
			string encryptedText,
			string actualType,
			string decryptedText)
		{
			return $@"You are a cryptography expert walking a student through solving a cipher puzzle from start to finish.
		
				ENCRYPTED TEXT:
				{encryptedText}
				
				CIPHER TYPE: {actualType}
				DECRYPTED TEXT: {decryptedText}
				
				Provide a complete solution walkthrough that shows your entire thought process, as if you're solving it in real-time.
				
				Your solution should flow naturally through these stages:
				
				1. INITIAL OBSERVATION
				   - What do you notice first about the ciphertext?
				   - What clues immediately stand out?
				   - What does this tell you about the cipher category?
				
				2. IDENTIFICATION PROCESS
				   - What specific features led you to identify this as {actualType}?
				   - What tests or analysis did you perform?
				   - Were there any other cipher types you considered? Why did you rule them out?
				
				3. SOLVING METHODOLOGY
				   - Walk through your solving process step-by-step
				   - Show any calculations, frequency analysis, or pattern matching
				   - Include trial and error if relevant (show wrong attempts that you corrected)
				   - Explain how you determined the key/shift/parameter
				
				4. VERIFICATION
				   - Show the decrypted plaintext: {decryptedText}
				   - How did you verify this is correct?
				   - What confirms this makes sense as English text?
				
				5. EDUCATIONAL CONTEXT
				   - Brief history of {actualType} cipher (who used it, when, why)
				   - Strengths and weaknesses of this cipher type
				   - How it compares to similar ciphers
				
				CRITICAL FORMATTING RULES:
				- Write in natural, flowing paragraphs
				- NO markdown headers (###, ####)
				- NO bullet points or numbered lists within the text
				- NO excessive bold/italic formatting
				- Sound like you're thinking out loud while solving
				
				TONE:
				- Conversational and engaging, like explaining to a friend
				- Show your reasoning process, including mistakes or corrections
				- Make it feel like a real problem-solving journey, not a polished essay
				- Use phrases like ""Let me try..."", ""Hmm, that doesn't work..."", ""Ah, I see now...""
				
				Example good start: ""Let me work through this step by step. First thing I notice is the text keeps its normal spacing and punctuation intact. That immediately tells me...""
				
				Example bad start: ""### Solution Overview\n\n**Step 1: Observation**\n\nThe ciphertext exhibits the following characteristics:""
				
				Aim for about 500-800 words. Make it educational and engaging, not dry and technical.";
		}

		#endregion

		#region LLM API Communication
		private async Task<string> CallLLMWithJsonAsync(
			string prompt,
			string model,
			int maxTokens = 500,
			float temperature = 0.7f)
		{
			try
			{
				var requestBody = new
				{
					model = model,
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
					temperature = temperature,
					response_format = new { type = "json_object" }  // FORCE JSON MODE
				};

				return await ExecuteLLMRequestAsync(requestBody);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during JSON LLM call");
				throw;
			}
		}
		private async Task<string> CallLLMAsync(
			string prompt,
			string model,
			int maxTokens = 500,
			float temperature = 0.7f)
		{
			try
			{
				var requestBody = new
				{
					model = model,
					messages = new[]
					{
						new { role = "user", content = prompt }
					},
					max_tokens = maxTokens,
					temperature = temperature
				};

				return await ExecuteLLMRequestAsync(requestBody);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during standard LLM call");
				throw;
			}
		}
		private async Task<string> ExecuteLLMRequestAsync(object requestBody)
		{
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

		#endregion

		#region Data Models

		// Models for OpenAI API communication (unchanged)
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

	#region Public Models for LLM Responses
	public class CipherValidationResult
	{
		[JsonPropertyName("confidence")]
		public string Confidence { get; set; }  // "high", "medium", "low"

		[JsonPropertyName("issues")]
		public List<string> Issues { get; set; } = new();

		[JsonPropertyName("recommendation")]
		public string Recommendation { get; set; } 

		[JsonPropertyName("reasoning")]
		public string Reasoning { get; set; }
	}

	#endregion
}