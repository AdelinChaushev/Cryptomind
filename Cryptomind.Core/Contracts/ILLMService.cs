using Cryptomind.Common.CipherRecognitionViewModels;
using Cryptomind.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface ILLMService
	{
		Task<CipherValidationResult> ValidateCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult,
			string? userProvidedType = null);

		Task<string> GetTypeHintAsync(string encryptedText, string actualType);
		Task<string> GetHintAsync(
			string encryptedText,
			string actualType,
			string? decryptedText = null);
		Task<string> GetFullSolutionAsync(
			string encryptedText,
			string actualType,
			string decryptedText);
		Task<bool> IsServiceHealthyAsync();
	}
}
