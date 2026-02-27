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
		IBadgeService badgeService,
		IHintService hintService,
		IOCRService ocrService,
		ICipherSubmissionService cipherSubmissionService,
		IAnswerSubmissionService answerService) : ControllerBase
	{
		[HttpGet("all")]
		public async Task<IActionResult> GetAllCiphers([FromQuery] CipherFilter filter)
		{
			string? userId = GetUserId();

			var result = await cipherService.GetApprovedAsync(filter, userId);
			return Ok(result);
		}

		[HttpGet("cipher/{id}")]
		public async Task<IActionResult> GetCipherById([FromRoute] int id)
		{
			string? userId = GetUserId();

			var result = await cipherService.GetCipherAsync(id, userId);
			return Ok(result);
		}

		[HttpPost("cipher/{id}/solve")]
		public async Task<IActionResult> SolveCipher([FromRoute] int id, [FromBody] SolveCipherDTO dto)
		{
			string? userId = GetUserId();

			bool result = await cipherService.SolveCipherAsync(userId, dto.UserSolution, id);
			await badgeService.CheckBadgesByCategory(userId, BadgeCategory.OnSolve);
			return Ok(result);
		}

		[HttpPost("submit")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> SubmitCipher([FromForm] SubmitCipherViewModel model)
		{
			string? userId = GetUserId();

			var cipher = await cipherSubmissionService.SubmitCipherAsync(model, userId);
			return Ok("Your cipher was received and will be reviewed by admin");
		}

		[HttpPost("ocr-preview")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> PreviewOCR([FromForm] IFormFile image)
		{
			var result = await ocrService.ExtractTextFromImageAsync(image);
			return Ok(new { extractedText = result.ExtractedText, confidence = result.Confidence });
		}

		[HttpPost("cipher/{id}/suggest-answer")]
		public async Task<IActionResult> SuggestAnswer([FromRoute] int id, [FromBody] SuggestAnswerDTO dto)
		{
			string? userId = GetUserId();

			await answerService.SuggestAnswerAsync(dto, userId, id);
			return Ok("Your suggestion was received and will be reviewed by admin.");
		}

		[HttpPost("cipher/{id}/hint")]
		public async Task<IActionResult> RequestHint([FromRoute] int id, [FromBody] HintRequestDTO request)
		{
			string? userId = GetUserId();

			var hintResult = await hintService.RequestHintAsync(userId, id, request.HintType);
			return Ok(new { hintResult });
		}
		#region Private methods
		private string? GetUserId()
			=> User.FindFirstValue(ClaimTypes.NameIdentifier);

		#endregion
	}
}