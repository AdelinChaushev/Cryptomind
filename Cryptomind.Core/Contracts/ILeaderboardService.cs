using Cryptomind.Common.ViewModels.LeaderboardPlaceViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface ILeaderboardService
	{
		Task<List<LeaderboardPlaceViewModel>> GetLeaderboard();
	}
}
