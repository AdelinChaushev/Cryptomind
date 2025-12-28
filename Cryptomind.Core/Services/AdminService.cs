using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Common.CipherViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Services
{
	public class AdminService (
		IRepository<Cipher, int> cipherRepo,
		IRepository<Tag, int> tagRepo) : IAdminService
	{
		public async Task<List<CipherReviewOutputViewModel>> AllSubmittedCiphers()
		{
			var result = (await cipherRepo.GetAllAsync()).Where(c => c.IsApproved == false).ToList();
			if (result == null) throw new InvalidOperationException("Wasn't able to retrieve submitted ciphers");
			return await ToReviewOutputViewModelMany(result);
			
		}
		public async Task<List<CipherReviewOutputViewModel>> AllApprovedCiphers()
		{
			var result = (await cipherRepo.GetAllAsync()).Where(c => c.IsApproved == true).ToList();
			if (result == null) throw new InvalidOperationException("Wasn't able to retrieve approved ciphers");
            return await ToReviewOutputViewModelMany(result);
        }
		public async Task<Cipher> GetCipherById(int id) 
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);

			if (cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");
			return cipher; //Go to next
		}
		public async Task ApproveCipherAsync(int id, ApproveUpdateCipherViewModel model) 
		{
			Cipher? cipher = await GetCipherById(id);
			if (cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");
			else if (cipher.IsApproved) throw new InvalidOperationException("The cipher with the given Id is already approved");
			if (model.TagIds != null && model.TagIds.Count > 0)
			{
                await DefineTagsAsync(cipher, model.TagIds.ToList());
            }
			

			cipher.Title = model.Title;
			//result.TypeOfCipher = model.TypeOfCipher;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;
			cipher.IsApproved = true;

			await cipherRepo.UpdateAsync(cipher);
			//await Console.Out.WriteLineAsync(await cipherRepo.UpdateAsync(cipher);.ToString());
			
			//Here there might be name collision with other cipher
		}
		public async Task RejectCipherAsync(int id)
		{
			Cipher? cipher = await GetCipherById(id);
			if(cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");
			else if (cipher.IsApproved) throw new InvalidOperationException("The cipher with the given Id is already approved");

			bool isDeleted = await cipherRepo.DeleteAsync(cipher);

			if (!isDeleted) throw new InvalidOperationException("Wasn't able to delete the cipher");
		}
		public async Task DeleteApprovedCipher(int id)
		{
			Cipher? cipher = await GetCipherById(id);
			if(cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");
			else if (!cipher.IsApproved) throw new InvalidOperationException("The cipher with the given Id is not approved");

			bool result = await cipherRepo.DeleteAsync(cipher);
			if (!result) throw new InvalidOperationException("Wasn't able to delete the cipher");
		}
		public async Task UpdateApprovedCipher(int id, ApproveUpdateCipherViewModel model)
		{
			Cipher? cipher = await GetCipherById(id);

			if (cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");
			else if (!cipher.IsApproved) throw new InvalidOperationException("The cipher with the given Id is not approved");

			cipher.Title = model.Title;
			cipher.AllowHint = model.AllowHint;
			cipher.AllowSolution = model.AllowSolution;
			if (model.TagIds != null && model.TagIds.Count > 0)
			{
				await DefineTagsAsync(cipher, model.TagIds.ToList());
			}

			await cipherRepo.UpdateAsync(cipher);
		}

		//Common methods
		private async Task DefineTagsAsync (Cipher? cipher, List<int> tagIds)
		{
			if (cipher.CipherTags != null)
				cipher.CipherTags?.Clear();
			else
				cipher.CipherTags = new List<CipherTag>();

			List<Tag> existingTags = (await tagRepo.GetAllAsync()).Where(x => tagIds.Contains(x.Id)).ToList();

			//if (existingTags.Count != tagIds.Count)
			//	throw new InvalidOperationException("One or more tag IDs are invalid");

			foreach(var tag in existingTags)
			{
				cipher.CipherTags.Add(new CipherTag
				{
					TagId = tag.Id,
					CipherId = cipher.Id,
				});
			}
		}
		private async Task<List<CipherReviewOutputViewModel>> ToReviewOutputViewModelMany(List<Cipher> result)
		{
            List<CipherReviewOutputViewModel> output = new List<CipherReviewOutputViewModel>();
            foreach (var cipher in result)
            {

                if (cipher is ImageCipher)
                {
                    ImageCipher cipherImage = cipher as ImageCipher;
                    string imageFolderPath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), cipherImage.ImagePath);
                    string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";


                    output.Add(new CipherReviewOutputViewModel
                    {
                        Id = cipher.Id,
                        Title = cipher.Title,
                        DecryptedText = cipher.DecryptedText,
                        Points = cipher.Points,
                        CipherText = base64,
                        AllowsAnswer = cipher.AllowSolution,
                        AllowsHint = cipher.AllowHint,
                        IsApproved = cipher.IsApproved,
                        IsImage = true,


                    });

                }
                else
                {
                    TextCipher cipherText = cipher as TextCipher;
                    output.Add(new CipherReviewOutputViewModel
                    {
                        Id = cipher.Id,
                        Title = cipher.Title,
                        DecryptedText = cipher.DecryptedText,
                        CipherText = cipherText.EncryptedText,
                        Points = cipher.Points,
                        AllowsAnswer = cipher.AllowSolution,
                        AllowsHint = cipher.AllowHint,
                        IsApproved = cipher.IsApproved,
                        IsImage = false,


                    });
                }



            }
            return output;
        }
	}
}
