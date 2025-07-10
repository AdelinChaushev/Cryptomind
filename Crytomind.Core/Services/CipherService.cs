using Cryptomind.Common.DTOs;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Crytomind.Core.Contracts;
using Microsoft.EntityFrameworkCore;
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
        public async Task<string> AnswerCipherAsync(string input, int cipherId)
		{
			Cipher cipher = await cipherRepo.GetByIdAsync(cipherId);
			if (cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");

			string correctAnswer = cipher.DecryptedText;

			if (correctAnswer == input) //The answer is correct
			{
				return "Правилен отговор!";
				//Some user
			}

			return "Грешен отговор";
		}
		public async Task<List<Cipher>> GetApprovedAsync(CipherFilter? filter)
		{
			List<Cipher> approved = cipherRepo.GetAllAttached().ToList();

			if(!string.IsNullOrEmpty(filter.SearchTerm))
			 approved =	approved.Where(c => c.Title.Contains(filter.SearchTerm)).ToList();

			if (filter.Tags != null)
                approved = approved.Where(c => c.CipherTags.Any(t => filter.Tags.Contains(t.Tag.Type))).ToList();

			return approved;
		}
		public async Task<Cipher?> GetCipherAsync(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);
			if (cipher == null)
				throw new InvalidOperationException("There is no cipher with the given Id");

			return cipher;
		}
		public Task<HintRequestResponse> RequestHintAsync(HintRequest request)
		{
			throw new NotImplementedException();
		}
		public async Task<Cipher> SubmitCipherAsync(Cipher cipher)
		{
            if (await cipherRepo.GetAllAttached().AnyAsync(c => c.Title == cipher.Title))
				throw new InvalidOperationException("Cannot create two ciphers with the same name");

			await cipherRepo.AddAsync(cipher);
			return cipher;
		}
	}
}
