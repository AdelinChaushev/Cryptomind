using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cryptomind.Core.Services.LLMService;

namespace Cryptomind.Core.Contracts
{
    public interface IAdminCipherService
    {
		Task<List<string>> GetRecentCipherSubmissionTitles();
		Task<int> GetPendingCiphersCount();
		Task<int> GetApprovedCiphersCount();
		Task<int> GetDeletedCiphersCount();
		Task<List<CipherReviewOutputViewModel>> AllSubmittedCiphers();
		Task<List<CipherReviewOutputViewModel>> AllApprovedCiphers(CipherFilter filter);
		Task<CipherDetailedReviewOutputViewModel> GetCipherById(int id);
		Task<string> ApproveCipherAsync(int id, ApproveCipherViewModel model);
		Task RejectCipherAsync(int id, string reason);
		Task UpdateApprovedCipher(int id, UpdateCipherViewModel model);
		Task<CipherValidationResult> AnalyzeWithLLM(int id);
        Task SoftDeleteCipher(int id);
		Task Restore(int id, string? newTitle = null);
	}
}
