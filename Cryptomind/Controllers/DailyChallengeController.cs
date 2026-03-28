using Cryptomind.Common.DTOs;
using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/daily-challenge")]
	[ApiController]
	public class DailyChallengeController(IDailyChallengeService dailyChallengeService) : ControllerBase
	{
		[HttpGet("")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> GetTodaysChallenge()
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			var result = await dailyChallengeService.GetTodaysChallengeAsync(userId);
			return Ok(result);
		}

		[HttpPost("solve")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> SubmitAnswer([FromBody] DailyChallengeSubmitDTO dto)
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			var result = await dailyChallengeService.SubmitAnswerAsync(userId, dto.Answer);
			return Ok(result);
		}

		[HttpGet("leaderboard")]
		public async Task<IActionResult> GetStreakLeaderboard()
		{
			var result = await dailyChallengeService.GetStreakLeaderboardAsync();
			return Ok(result);
		}
	}
}
