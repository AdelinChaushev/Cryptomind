using Cryptomind.Common.DTOs;
using Cryptomind.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytomind.Core.Contracts
{
	public interface ICipherService
	{
		Task<List<Cipher>> GetApprovedAsync(CipherFilter? filter); // Implement functionality to be able to filter by tags
		Task<Cipher?> GetCipherAsync(int id);
		Task SubmitCipherAsync(Cipher cipher);
		Task ApproveCipherAsync(Cipher cipher);
		Task<string> AnswerCipherAsync(string input, int cipherId);
		Task<HintRequestResponse> RequestHintAsync(HintRequest request);
	}
}
