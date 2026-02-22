using Cryptomind.Common.ViewModels.CipherSubmissionViewModels;
using Cryptomind.Common.ViewModels.CipherViewModels;

namespace Cryptomind.Core.Contracts
{
	public interface ICipherSubmissionService
	{
		Task<Cipher> SubmitCipherAsync(SubmitCipherViewModel cipher, string userId);
		Task<List<CipherSubmissionViewModel>> SubmittedCiphers(string userId);
	}
}
