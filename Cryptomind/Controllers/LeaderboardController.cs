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
		public async Task<IActionResult> LeaderBoard()
		{
			var result = await leaderboardService.GetLeaderboard();
			return Ok(result);
		}
	}
}
