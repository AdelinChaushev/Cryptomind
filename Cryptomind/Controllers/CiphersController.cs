using Cryptomind.Common.CipherRecognitionViewModels;
using Cryptomind.Common.CipherViewModels;
using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/ciphers")]
	[ApiController]
	public class CiphersController : ControllerBase
	{
		private ICipherService cipherService;
		private ICipherRecognizerService recognizerService;
		private IBadgeService badgeService;
		public CiphersController(ICipherService cipherService, IUserService userService, ICipherRecognizerService recognizerService, IBadgeService badgeService)
		{
			this.cipherService = cipherService;
			this.recognizerService = recognizerService;
			this.badgeService = badgeService;
		}

		[HttpGet("all")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> GetAllCiphers([FromQuery] CipherFilter filter)
		{
			var result = await cipherService.GetApprovedAsync(filter);
			return Ok(result);
		}

		[HttpGet("cipher/{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> GetCipherById([FromRoute] int id)
		{
			try
			{
				var result = await cipherService.GetCipherAsync(id);
				return Ok(result);
			}
			catch (InvalidOperationException ex)
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
				var cipher = await cipherService.SubmitCipherAsync(model, userId);
				return Ok(cipher);
			}
			catch (Exception ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

		[HttpPost("cipher/{id}/solve")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> SolveCipher([FromRoute] int id, [FromBody] SolveCipherDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			try
			{
				bool result = await cipherService.AnswerCipherAsync(GetUserId(), dto.UserSolution, id);
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

		[HttpPost("cipher/{id}/suggest-answer")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> SuggestAnswer([FromRoute] int id, [FromBody] SuggestAnswerDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			try
			{
				await cipherService.SuggestAnswerAsync(dto, GetUserId(), id);
				return Ok("Your suggestion was recieved and will be reviewed by an admin.");
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPost("cipher/{id}/classify")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> ClassifyCipher([FromRoute] int id)
		{
			var cipher = await cipherService.GetCipherAsync(id);
			try
			{
				var result = await recognizerService.ClassifyCipher(cipher.CipherText);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { error = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return StatusCode(503, new { error = ex.Message });
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
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
