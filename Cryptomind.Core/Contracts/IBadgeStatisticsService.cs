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
		Task<int> GetDistinctCipherTypesSolved(string userId);
		Task<int> GetApprovedAnswersCount(string userId);
		Task<int> GetSolvedWithoutHintCount(string userId);
		Task<int> GetSolvedOnFirstAttemptCount(string userId);
		Task<int> GetUsedHints(string userId);
		Task<int> GetRareSolves(string userId);
	}
}
