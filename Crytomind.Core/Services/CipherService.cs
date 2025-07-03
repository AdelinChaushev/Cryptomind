using Cryptomind.Common.DTOs;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Crytomind.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Crytomind.Core.Services
{
	public class CipherService (IRepository<Cipher, int> cipherRepo) : ICipherService
	{
        public Task<string> AnswerCipherAsync(string input, int cipherId)
		{
			throw new NotImplementedException();
		}

		public Task ApproveCipherAsync(Cipher cipher)
		{
			throw new NotImplementedException();
		}

		public async Task<List<Cipher>> GetApprovedAsync(CipherFilter? filter)
		{
			List<Cipher> approved = cipherRepo.GetAllAttached().ToList();
			if (string.IsNullOrWhiteSpace(filter.SearchTerm))
				throw new ArgumentException("Invalid Search Term");

			//if (filter.Tags.Any() && filter.Tags != null);
			approved.Where(c => c.Title.Contains(filter.SearchTerm) && c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type)));
			return approved;
		}

		public Task<Cipher?> GetCipherAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<HintRequestResponse> RequestHintAsync(HintRequest request)
		{
			throw new NotImplementedException();
		}

		public Task SubmitCipherAsync(Cipher cipher)
		{
			throw new NotImplementedException();
		}
	}
}
