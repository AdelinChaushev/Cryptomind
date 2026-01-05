using Cryptomind.Common.CipherViewModels;
using Cryptomind.Common.DTOs;
using Cryptomind.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface ICipherService
	{
		Task<List<CipherOutputViewModel>> GetApprovedAsync(CipherFilter? filter); // Implement functionality to be able to filter by tags
		Task<CipherOutputViewModel?> GetCipherAsync(int id);
		Task<Cipher> SubmitCipherAsync(SubmitCipherViewModel cipher, string userId);
		Task<bool> AnswerCipherAsync(string userId,string input, int cipherId);
		Task<HintRequestResponse> RequestHintAsync(HintRequest request);
	}
}
