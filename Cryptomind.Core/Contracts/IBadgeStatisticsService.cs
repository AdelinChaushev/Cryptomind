using Cryptomind.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IBadgeStatisticsService
	{
		Task<int> GetSolvedCount(string userId);
		Task<int> GetApprovedCount(string userId);
		Task<int> GetDestinctCipherTypesSolved(string userId);
		Task<int> GetApprovedAnswersCount(string userId);
		Task<ApplicationUser> GetUser(string userId);
	}
}
