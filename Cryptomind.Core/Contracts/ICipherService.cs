using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.CipherViewModels;

namespace Cryptomind.Core.Contracts
{
	public interface ICipherService
	{
		Task<List<CipherOutputViewModel>> GetApprovedAsync(CipherFilter filter, string userId); // Implement functionality to be able to filter by tags
		Task<CipherDetailedOutputViewModel?> GetCipherAsync(int id, string userId);
		Task<bool> SolveCipherAsync(string userId, string input, int cipherId);
	}
}