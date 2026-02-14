using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.CipherViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/ciphers")]
	[Authorize(AuthenticationSchemes = "Bearer")]
	[ApiController]
	public class CipherController(
		ICipherService cipherService,
		ICipherRecognizerService recognizerService,
		IBadgeService badgeService,
		IHintService hintService,
		ICipherSubmissionService cipherSubmissionService,
		IAnswerSubmissionService answerService) : ControllerBase
	{
		[HttpGet("all")]
		public async Task<IActionResult> GetAllCiphers([FromQuery] CipherFilter filter)
		{
			string? userId = GetUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return BadRequest(new { error = "User ID not found in token" });
			}

			var result = await cipherService.GetApprovedAsync(filter, userId);
			return Ok(result);
		}

		[HttpGet("cipher/{id}")]
		public async Task<IActionResult> GetCipherById([FromRoute] int id)
		{
			try
			{
				string? userId = GetUserId();
				if (string.IsNullOrEmpty(userId))
				{
					return BadRequest(new { error = "User ID not found in token" });
				}

				var result = await cipherService.GetCipherAsync(id, userId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpPost("cipher/{id}/solve")]
		public async Task<IActionResult> SolveCipher([FromRoute] int id, [FromBody] SolveCipherDTO dto)
		{
			try
			{
				string? userId = GetUserId();
				if (string.IsNullOrEmpty(userId))
				{
					return BadRequest(new { error = "User ID not found in token" });
				}

				bool result = await cipherService.SolveCipherAsync(userId, dto.UserSolution, id);
				await badgeService.CheckBadgesByCategory(userId, BadgeCategory.OnSolve);

				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpPost("submit")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> SubmitCipher([FromForm] SubmitCipherViewModel model)
		{
			string? userId = GetUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return BadRequest(new { error = "User ID not found in token" });
			}

			try
			{
				var cipher = await cipherSubmissionService.SubmitCipherAsync(model, userId);
				return Ok("Your cipher was received and will be reviewed by admin");
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpPost("cipher/{id}/suggest-answer")]
		public async Task<IActionResult> SuggestAnswer([FromRoute] int id, [FromBody] SuggestAnswerDTO dto)
		{
			try
			{
				string? userId = GetUserId();
				if (string.IsNullOrEmpty(userId))
				{
					return BadRequest(new { error = "User ID not found in token" });
				}

				await answerService.SuggestAnswerAsync(dto, userId, id);
				return Ok("Your suggestion was received and will be reviewed by admin.");
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpPost("cipher/{id}/hint")]
		public async Task<IActionResult> RequestHint([FromRoute] int id, [FromBody] HintRequestDTO request)
		{
			try
			{
				string? userId = GetUserId();
				if (string.IsNullOrEmpty(userId))
				{
					return BadRequest(new { error = "User ID not found in token" });
				}

				string hintContent = await hintService.RequestHintAsync(userId, id, request.HintType);
				return Ok(new { hintContent });
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
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
				return BadRequest(new { error = ex.Message });
			}
		}

		#region Private methods
		private string? GetUserId()
			=> User.FindFirstValue(ClaimTypes.NameIdentifier);
		#endregion
	}
}