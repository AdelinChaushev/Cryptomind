using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Cryptomind.Controllers
{
	[Route("api/leaderboard")]
	[ApiController]
	public class LeaderboardController(ILeaderboardService leaderboardService) : ControllerBase
	{
		[HttpGet]
		[Route("")]
		public async Task<IActionResult> Leaderboard()
		{
			var result = await leaderboardService.GetPointLeaderboard();
			return Ok(result);
		}

		[HttpGet]
		[Route("rooms")]
		public async Task<IActionResult> RoomLeaderboard()
		{
			var result = await leaderboardService.GetRoomLeaderboard();
			return Ok(result);
		}

		[HttpGet]
		[Route("streaks")]
		public async Task<IActionResult> StreakLeaderboard()
		{
			var result = await leaderboardService.GetStreakLeaderboard();
			return Ok(result);
		}
	}
}
