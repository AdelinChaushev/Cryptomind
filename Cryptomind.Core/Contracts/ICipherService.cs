using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.CipherViewModels;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface ICipherService
	{
		Task<List<CipherOutputViewModel>> GetApprovedAsync(CipherFilter? filter, string userId); // Implement functionality to be able to filter by tags
		Task<CipherOutputViewModel?> GetCipherAsync(int id, string userId);
		Task<bool> SolveCipherAsync(string userId, string input, int cipherId);
	}
}
