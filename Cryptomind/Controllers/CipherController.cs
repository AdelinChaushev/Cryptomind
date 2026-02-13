using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.CipherViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/ciphers")]
	[ApiController]
	public class CipherController : ControllerBase
	{
		private ICipherService cipherService;
		private ICipherRecognizerService recognizerService;
		private IBadgeService badgeService;
		private IHintService hintService;
		private ICipherSubmissionService cipherSubmissionService;
		private IAnswerSubmissionService answerService;
		public CipherController(
			ICipherService cipherService, 
			IUserService userService, 
			ICipherRecognizerService recognizerService, 
			IBadgeService badgeService, 
			IHintService hintService, 
			ICipherSubmissionService cipherSubmissionService, 
			IAnswerSubmissionService answerService)
		{
			this.cipherService = cipherService;
			this.recognizerService = recognizerService;
			this.badgeService = badgeService;
			this.hintService = hintService;
			this.cipherSubmissionService = cipherSubmissionService;
			this.answerService = answerService;
		}

		[HttpGet("all")]
		[Authorize(AuthenticationSchemes = "Bearer")] //You might not need this (read-only for users)
		public async Task<IActionResult> GetAllCiphers([FromQuery] CipherFilter filter)
		{
			var result = await cipherService.GetApprovedAsync(filter, GetUserId());
			return Ok(result);
		}

		[HttpGet("cipher/{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> GetCipherById([FromRoute] int id)
		{
			try
			{
				var result = await cipherService.GetCipherAsync(id, GetUserId());
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPost("cipher/{id}/solve")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> SolveCipher([FromRoute] int id, [FromBody] SolveCipherDTO dto)
		{
			try
			{
				bool result = await cipherService.SolveCipherAsync(GetUserId(), dto.UserSolution, id);
				await badgeService.CheckBadgesByCategory(GetUserId(), BadgeCategory.OnSolve);
				//Update user stats
				return Ok(result);
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPost("submit")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> SubmitCipher([FromForm] SubmitCipherViewModel model)
		{
			var userId = GetUserId();
			try
			{
				var cipher = await cipherSubmissionService.SubmitCipherAsync(model, userId);
				return Ok(cipher);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpPost("cipher/{id}/suggest-answer")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> SuggestAnswer([FromRoute] int id, [FromBody] SuggestAnswerDTO dto)
		{
			try
			{
				await answerService.SuggestAnswerAsync(dto, GetUserId(), id);
				return Ok("Your suggestion was recieved and will be reviewed by an admin.");
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPost("cipher/{id}/hint")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> RequestHint([FromRoute] int id, [FromBody] HintRequestDTO request)
		{
			try
			{
				string hintContent = await hintService.RequestHintAsync(GetUserId(), id, request.HintType);

				return Ok(new { hintContent });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("ml-health")]
		public async Task<IActionResult> CheckMLHealth()
		{
			try
			{
				var isHealthy = await recognizerService.IsServiceHealthyAsync();
				return Ok(new { isHealthy });
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}
		private string GetUserId()
		   => User.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
