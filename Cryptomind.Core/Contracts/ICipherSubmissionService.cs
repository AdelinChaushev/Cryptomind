using Cryptomind.Common.Enums;
using Cryptomind.Common.ViewModels.CipherSubmissionViewModels;
using Cryptomind.Common.ViewModels.CipherViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface ICipherSubmissionService
	{
		Task<Cipher> SubmitCipherAsync(SubmitCipherViewModel cipher, string userId);
		Task<List<CipherSubmissionViewModel>> SubmittedCiphers(string userId);
	}
}
