using Cryptomind.Common.CipherRecognitionViewModels;
using Cryptomind.Core.Contracts;

namespace Cryptomind.Core.Services
{
	public class MockLLMService : ILLMService
	{
		public Task<string> ValidateCipherTextAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var hasDecrypted = !string.IsNullOrWhiteSpace(decryptedText);

			var response = hasDecrypted
				? $@"**MOCK LLM VALIDATION**

Cipher Analysis:
- Encrypted text appears legitimate
- Predicted type: {mlResult.TopPrediction.Type} ({mlResult.TopPrediction.Confidence:P0} confidence)
- Text length: {encryptedText.Length} characters

Solution Verification:
- User provided decrypted text: ""{decryptedText?.Substring(0, Math.Min(50, decryptedText.Length))}...""
- Assessment: **Highly likely correct** - text appears coherent and matches expected pattern
- The solution is grammatically sound and appropriate length

Recommendation: LEGITIMATE - Approve as Standard cipher"
				: $@"**MOCK LLM VALIDATION**

Cipher Analysis:
- Encrypted text appears legitimate
- Predicted type: {mlResult.TopPrediction.Type} ({mlResult.TopPrediction.Confidence:P0} confidence)
- Text length: {encryptedText.Length} characters
- Shows typical cipher characteristics (patterns detected)

No Solution Provided:
- User did not provide decrypted text
- This appears to be an experimental submission

Recommendation: LEGITIMATE - Approve as Experimental cipher for community solving";

			return Task.FromResult(response);
		}

		public Task<string> SolveCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var response = $@"**MOCK LLM SOLVING ATTEMPT**

Cipher Type Confirmation:
- ML predicted: {mlResult.TopPrediction.Type} ({mlResult.TopPrediction.Family} family)
- Confidence: {mlResult.TopPrediction.Confidence:P0}
- Assessment: Prediction appears reasonable based on text patterns

Solving Approach:
- For {mlResult.TopPrediction.Type} cipher, typical approach involves:
  * Frequency analysis
  * Pattern recognition
  * Statistical methods appropriate for {mlResult.TopPrediction.Family} ciphers

Attempted Solution:
- [MOCK] This is where the actual decrypted text would appear
- [MOCK] Real LLM would attempt cryptanalysis here
- [MOCK] Example: ""THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG""

Confidence:
- [MOCK] Solution confidence would be rated here (High/Medium/Low)
- Real LLM would verify linguistic coherence

Note: This is a MOCK response for testing. Real LLM would provide actual cryptanalysis.";

			return Task.FromResult(response);
		}

		public Task<string> GetHintAsync(
			string encryptedText,
			CipherRecognitionResultViewModel mlResult)
		{
			var response = $@"**MOCK LLM HINT**

Cipher Type Detected: {mlResult.TopPrediction.Type}

Educational Hint:
- {mlResult.TopPrediction.Type} ciphers work by [mock explanation]
- Look for patterns in the encrypted text
- Pay attention to letter frequencies
- Consider the characteristics of {mlResult.TopPrediction.Family} family ciphers

Suggested Approach:
- Try analyzing the Index of Coincidence
- Look for repeated sequences
- Consider the key length (if applicable)

This is a MOCK hint. Real LLM would provide detailed cryptographic guidance.";

			return Task.FromResult(response);
		}

		public Task<bool> IsServiceHealthyAsync()
		{
			// Mock is always "healthy"
			return Task.FromResult(true);
		}
	}
}