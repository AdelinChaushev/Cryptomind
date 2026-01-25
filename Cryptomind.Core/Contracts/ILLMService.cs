using Cryptomind.Common.CipherRecognitionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface ILLMService
	{
		Task<string> ValidateCipherTextAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult);

		Task<string> SolveCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult);

		Task<string> GetHintAsync(
			string encryptedText,
			CipherRecognitionResultViewModel mlResult);

		Task<bool> IsServiceHealthyAsync();
	}
}
