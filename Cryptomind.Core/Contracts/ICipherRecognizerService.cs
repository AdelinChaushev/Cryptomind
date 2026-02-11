using Cryptomind.Common.ViewModels.CipherRecognitionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface ICipherRecognizerService
	{
		Task<CipherRecognitionResultViewModel> ClassifyCipher (string inputText);
		Task<bool> IsServiceHealthyAsync();
	}
}
