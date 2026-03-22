using Cryptomind.Common.ViewModels.LeaderboardPlaceViewModel;

namespace Cryptomind.Core.Contracts
{
	public interface ILeaderboardService
	{
		Task<List<LeaderboardPlaceViewModel>> GetPointLeaderboard();
		Task<List<LeaderboardPlaceViewModel>> GetRoomLeaderboard();
	}
}
