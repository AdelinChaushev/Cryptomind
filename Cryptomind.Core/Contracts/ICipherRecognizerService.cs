using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;

namespace Cryptomind.Core.Contracts
{
	public interface ICipherRecognizerService
	{
		Task<CipherRecognitionResultViewModel> ClassifyCipher(string inputText);
		Task<bool> IsServiceHealthyAsync();
	}
}
