using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Data.Enums;
using static Cryptomind.Core.Services.LLMService;

namespace Cryptomind.Core.Contracts
{
	public interface ILLMService
	{
		Task<CipherValidationResult> ValidateCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult,
			string? userProvidedType = null);

		Task<string> GetHint(Cipher cipher, HintType hintType);
	}
}
