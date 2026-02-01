using Cryptomind.Common.AdminViewModels;
using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Common.CipherViewModels;
using Cryptomind.Common.DTOs;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Identity;
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
		Task<List<CipherReviewOutputViewModel>> AllApprovedCiphers(CipherFilter filter);
		Task<CipherReviewOutputViewModel> GetCipherById(int id);
		Task<CipherValidationResult> AnalyzeWithLLM(int id);

		Task RejectCipherAsync(int id, string reason);
        Task<string> ApproveCipherAsync(int id, ApproveCipherViewModel model);
		Task UnapproveCipherAsync(int id);
        Task DeleteApprovedCipher(int id);
        Task UpdateApprovedCipher(int id, UpdateCipherViewModel model);
		Task<List<UserViewModel>> GetAllUsers();
		Task<UserDetailViewModel> GetUser(string userId);
		Task MakeAdmin(string userId);
		Task BanUserAsync(string userId, string reason);
		Task UnbanUserAsync(string userId);
	}
}
