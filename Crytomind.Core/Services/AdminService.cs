using Cryptomind.Data.Entities;
using Crytomind.Core.Contracts;
using Cryptomind.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptomind.Common.CipherAdminViewModels;

namespace Crytomind.Core.Services
{
	public class AdminService (
		IRepository<Cipher, int> cipherRepo) : IAdminService
	{

		public async Task<List<Cipher>> AllSubmittedCyphers()
		{
			var result = (await cipherRepo.GetAllAsync()).Where(c => c.IsApproved == false).ToList();
			if (result == null) throw new InvalidOperationException("Wasn't able to retrieve submitted ciphers");
			return result;
		}
		public async Task<Cipher> GetCipherById(int id) //Should I return cipher the current logic is to return it so it can be shown to the next view
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");
			return cipher; //Go to next
		}

		public async Task ApproveCipherAsync(int id, ApproveCipherViewModel model)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);
			if (cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");

			List<CipherTag> cipherTags = new List<CipherTag>();
			foreach (var tag in model.Tags)
			{
				var cipherTag = new CipherTag()
				{
					TagId = tag.Id,
					CipherId = cipher.Id,
				};

				cipherTags.Add(cipherTag);
			}

			cipher.CipherTags = cipherTags;

			cipher.Title = model.Title;
			cipher.DecryptedText = model.DecryptedText;
			//result.TypeOfCipher = model.TypeOfCipher;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;
			cipher.IsApproved = true;

			bool isSuccessfull = await cipherRepo.UpdateAsync(cipher);
            await Console.Out.WriteLineAsync(isSuccessfull.ToString());
            //Here there might be name collision with other cipher
        }

		public async Task RejectCipherAsync(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);
			if (cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");

			bool isDeleted = await cipherRepo.DeleteAsync(cipher);

			if (!isDeleted) throw new InvalidOperationException("Wasn't able to delete the cipher");
		}
	}
}
