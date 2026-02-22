using Cryptomind.Common.ViewModels.LeaderboardPlaceViewModel;

namespace Cryptomind.Core.Contracts
{
	public interface ILeaderboardService
	{
		Task<List<LeaderboardPlaceViewModel>> GetLeaderboard();
	}
}
