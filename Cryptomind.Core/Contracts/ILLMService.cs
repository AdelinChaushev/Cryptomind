using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Data.Enums;
using static Cryptomind.Core.Services.LLMService;

namespace Cryptomind.Core.Contracts
{
	public interface ILLMService
	{
		Task<CipherValidationResultDTO> ValidateCipherAsync(
			string encryptedText,
			string? decryptedText,
			CipherRecognitionResultViewModel mlResult,
			string? userProvidedType = null);

		Task<string> GetHint(Cipher cipher, HintType hintType);
	}
}
