using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.AdminViewModels;

namespace Cryptomind.Core.Contracts
{
	public interface IAdminCipherService
	{
		Task<List<PendingCipherTitleViewModels>> GetRecentCipherSubmissionTitles();
		Task<int> GetPendingCiphersCount();
		Task<int> GetApprovedCiphersCount();
		Task<int> GetDeletedCiphersCount();
		Task<List<CipherReviewOutputViewModel>> AllPendingCiphers(string? filter);
		Task<List<CipherReviewOutputViewModel>> AllApprovedCiphers(CipherFilter filter);
		Task<List<CipherReviewOutputViewModel>> AllDeletedCiphers(CipherFilter filter);
		Task<CipherDetailedReviewOutputViewModel> GetCipherById(int id);
		Task<string> ApproveCipherAsync(int id, ApproveCipherViewModel model);
		Task RejectCipherAsync(int id, string reason);
		Task UpdateApprovedCipher(int id, UpdateCipherViewModel model);
		Task<CipherValidationResultDTO> AnalyzeWithLLM(int id);
		Task SoftDeleteCipher(int id);
		Task RestoreCipher(int id, string? newTitle = null);
	}
}
