using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cryptomind.Controllers
{
	[Route("api/leaderboard")]
	[ApiController]
	public class LeaderboardController : ControllerBase
	{
		private ILeaderboardService leaderboardService;
		public LeaderboardController(ILeaderboardService leaderboardService)
		{
			this.leaderboardService = leaderboardService;
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> LeaderBoard()
		{
			var result = await leaderboardService.GetLeaderboard();
			return Ok(result);
		}
	}
}
