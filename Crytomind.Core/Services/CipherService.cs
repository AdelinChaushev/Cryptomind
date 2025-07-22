using Cryptomind.Common.CipherViewModels;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Repositories;
using Crytomind.Core.Contracts;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Crytomind.Core.Services
{
    public class CipherService(IRepository<Cipher, int> cipherRepo, IRepository<UserSolution, int> solutionRepository) : ICipherService
	{
      
        public async Task<string> AnswerCipherAsync(string userId ,string input, int cipherId)
		{
			Cipher cipher = await cipherRepo.GetByIdAsync(cipherId);
			if (cipher == null) throw new InvalidOperationException("There is no cipher with the given Id");

			string correctAnswer = cipher.DecryptedText;

			if (correctAnswer == input) //The answer is correct
			{
                await solutionRepository.AddAsync(new UserSolution()
                {

                });
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
		public async Task<CipherOutputViewModel?> GetCipherAsync(int id)
		{
			Cipher? cipher = await cipherRepo.GetByIdAsync(id);
			if (cipher == null)
				throw new InvalidOperationException("There is no cipher with the given Id");

			return await ToOuputViewModel(cipher);
		}
		public Task<HintRequestResponse> RequestHintAsync(HintRequest request)
		{
			throw new NotImplementedException();
		}
		public async Task<Cipher> SubmitCipherAsync(SubmitCipherViewModel model,string userId)
		{
            if (await cipherRepo.GetAllAttached().AnyAsync(c => c.Title == model.Title))
				throw new InvalidOperationException("Cannot create two ciphers with the same name");

            Cipher? cipher = null;
           
            if (model.CipherDefinition == CipherDefinition.TextCipher)
            {
                cipher = new TextCipher()
                {
                    Title = model.Title,
                    DecryptedText = model.DecryptedText,
                    EncryptedText = model.EncryptedText,
                    //TypeOfCipher = model.Type,
                    AllowHint = false,
                    AllowSolution = false,
                    IsApproved = false,
                    CreatedByUserId = userId,
                    CipherTags = new List<CipherTag>(),
                    HintsRequested = new List<HintRequest>()
                };
            }          
            else if (model.CipherDefinition == CipherDefinition.ImageCipher)
            {
                string solutionRoot = Directory.GetParent(Directory.GetCurrentDirectory())!
    .Parent!.Parent!.Parent!.FullName;

                string imageFolderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Images"));
                Directory.CreateDirectory(imageFolderPath); // Make sure it exists

                string safeTitle = MakeSafeFilename(model.Title);
                string imageFilePath = Path.Combine(imageFolderPath, safeTitle + ".jpg");

                // Save the image
                using (var ms = new MemoryStream())
                {
                    await model.Image.CopyToAsync(ms);
                    byte[] bytes = ms.ToArray();
                    await File.WriteAllBytesAsync(imageFilePath, bytes);
                }

                // This is the relative path you'll store in the DB
                string relativePath = Path.Combine("Images", safeTitle + ".jpg");

                Console.WriteLine(relativePath);

                cipher = new ImageCipher()
                {
                    Title = model.Title,
                    DecryptedText = model.DecryptedText,
                    ImagePath = relativePath, // relative path for DB
                    AllowHint = false,
                    AllowSolution = false,
                    IsApproved = false,
                    CreatedByUserId = userId,
                    CipherTags = new List<CipherTag>(),
                    HintsRequested = new List<HintRequest>()
                };

              
            }
           

            await cipherRepo.AddAsync(cipher);
			return cipher;
		}
        private string MakeSafeFilename(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }

        private async Task<CipherOutputViewModel> ToOuputViewModel(Cipher cipher)
        {
            if (cipher is ImageCipher)
            {
                ImageCipher cipherImage = cipher as ImageCipher;
                string imageFolderPath = Path.Combine(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")), cipherImage.ImagePath);
                string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";


                return(new CipherOutputViewModel
                {
                    Id = cipher.Id,
                    Title = cipher.Title,
                   
                    Points = cipher.Points,
                    CipherText = base64,
                    AllowsAnswer = cipher.AllowSolution,
                    AllowsHint = cipher.AllowHint,
                    IsImage = true,


                });

            }
            else
            {
                TextCipher cipherText = cipher as TextCipher;
                return (new CipherOutputViewModel
                {
                    Id = cipher.Id,
                    Title = cipher.Title,
                    
                    CipherText = cipherText.EncryptedText,
                    Points = cipher.Points,
                    AllowsAnswer = cipher.AllowSolution,
                    AllowsHint = cipher.AllowHint,
                    IsImage = false,


                });
            }
        }
    }
}
