using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using Cryptomind.Core.Services;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
