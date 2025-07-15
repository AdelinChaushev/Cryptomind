using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Common.CipherViewModels;
using Cryptomind.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crytomind.Core.Contracts
{
    public interface IAdminService
    {
        Task<List<CipherOutputViewModel>> AllSubmittedCiphers();
		Task<List<Cipher>> AllApprovedCiphers();
		Task<Cipher> GetCipherById(int id);
        Task RejectCipherAsync(int id);
        Task ApproveCipherAsync(int id, ApproveUpdateCipherViewModel model);
        Task DeleteApprovedCipher(int id);
        Task UpdateApprovedCipher(int id, ApproveUpdateCipherViewModel model);
    }
}
