using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Common.CipherViewModels;
using Cryptomind.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
    public interface IAdminService
    {
        Task<List<CipherReviewOutputViewModel>> AllSubmittedCiphers();
		Task<List<CipherReviewOutputViewModel>> AllApprovedCiphers();
		Task<Cipher> GetCipherById(int id);
        Task RejectCipherAsync(int id);
        Task ApproveCipherAsync(int id, ApproveUpdateCipherViewModel model);
        Task DeleteApprovedCipher(int id);
        Task UpdateApprovedCipher(int id, ApproveUpdateCipherViewModel model);
    }
}
