using Cryptomind.Common.CipherViewModels;
using Cryptomind.Common.DTOs;
using Cryptomind.Data.Entities;
using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cryptomind.Data.Enums;
using Cryptomind.Common.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Cryptomind.Common.CipherRecognitionViewModels;

namespace Cryptomind.Controllers
{
	[Route("api/ciphers")]
	[ApiController]
	public class CiphersController : ControllerBase
	{
		private ICipherService cipherService;
		private IUserService userService;
		private ICipherRecognizerService recognizerService;

		public CiphersController(ICipherService cipherService, IUserService userService, ICipherRecognizerService recognizerService)
		{
			this.cipherService = cipherService;
			this.userService = userService;
			this.recognizerService = recognizerService;
		}

		[HttpGet("all")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> GetAllCiphers([FromQuery] CipherFilter filter)
		{
			await Console.Out.WriteLineAsync(filter.SearchTerm);
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

		[HttpPost("submit-cipher")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> SubmitCipher([FromForm] SubmitCipherViewModel model)
		{
			string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			try
			{
				await cipherService.SubmitCipherAsync(model, userId);
				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPost("solve-cipher/{id}")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> SolveCipher([FromRoute] int id, [FromBody] SolveCipherDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			try
			{
				bool result = await cipherService.AnswerCipherAsync(GetUserId(), dto.UserSolution, id);
				//Update user stats
				return Ok(result);
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPost("classify")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> ClassifyCipher([FromBody] ClassifyRequest request)
		{
			try
			{
				var result = await recognizerService.ClassifyCipherAsync(request.CipherText);
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

	public class ClassifyRequest
	{
		public string CipherText { get; set; }
	}
}
