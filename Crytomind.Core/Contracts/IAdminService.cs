using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Data.Entities;

namespace Crytomind.Core.Contracts
{
    public interface IAdminService
    {
        Task<List<Cipher>> AllSubmittedCyphers();
        Task<Cipher> GetCipherById(int id);
        Task RejectCipherAsync(int id);
        Task ApproveCipherAsync(int id, ApproveCipherViewModel model);
    }
}
