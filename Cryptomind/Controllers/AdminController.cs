using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cryptomind.Controllers
{
	[ApiController]
	[Route("api/admin")]
	[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
	public class AdminController : ControllerBase
	{
		private IAdminCipherService adminCipherService;
		private IAdminAnswerService adminAnswerService;
		private IAdminUserService adminUserService;
		private IBadgeService badgeService;
		public AdminController(IAdminCipherService adminCipherService, IAdminAnswerService adminAnswerService, IAdminUserService adminUserService, IBadgeService badgeService)
		{
			this.adminCipherService = adminCipherService;
			this.adminAnswerService = adminAnswerService;
			this.adminUserService = adminUserService;
			this.badgeService = badgeService;
		}

		#region Cipher-specific
		[HttpGet("approved-ciphers")]
		public async Task<IActionResult> GetApprovedCiphers([FromQuery] CipherFilter filter)
		{
			try
			{
				var result = await adminCipherService.AllApprovedCiphers(filter);
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("pending-ciphers")]
		public async Task<IActionResult> GetSubmittedCiphers()
		{
			try
			{
				var result = await adminCipherService.AllSubmittedCiphers();
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("cipher/{id}")]
		public async Task<IActionResult> GetCipher([FromRoute] int id)
		{
			try
			{
				var cipher = await adminCipherService.GetCipherById(id);
				return Ok(cipher);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("cipher/{id}/analyze")]
		public async Task<IActionResult> AnalyzeCipher([FromRoute] int id)
		{
			try
			{
				var result = await adminCipherService.AnalyzeWithLLM(id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("cipher/{id}/approve")]
		public async Task<IActionResult> ApproveCipher([FromRoute] int id, [FromBody] ApproveCipherViewModel model)
		{
			try
			{
				string userID = await adminCipherService.ApproveCipherAsync(id, model);
				await badgeService.CheckBadgesByCategory(userID, BadgeCategory.OnUpload);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("cipher/{id}/reject")]
		public async Task<IActionResult> RejectCipher([FromRoute] int id, [FromBody] string reason)
		{
			try
			{
				await adminCipherService.RejectCipherAsync(id, reason);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("cipher/{id}/update")]
		public async Task<IActionResult> UpdateCipher([FromRoute] int id, [FromBody] UpdateCipherViewModel model)
		{
			try
			{
				await adminCipherService.UpdateApprovedCipher(id, model);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpDelete("cipher/{id}/delete")]
		public async Task<IActionResult> DeleteCipher([FromRoute] int id)
		{
			try
			{
				await adminCipherService.DeleteApprovedCipher(id);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}
		#endregion

		#region Answer-specific
		[HttpGet("pending-answer-suggestions")]
		public async Task<IActionResult> GetSubmittedAnswers()
		{
			try
			{
				var result = await adminAnswerService.AllSubmittedAnswersAsync();
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("answer/{id}")]
		public async Task<IActionResult> GetAnswer([FromRoute] int id)
		{
			try
			{
				var result = await adminAnswerService.GetAnswerById(id);
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("answer/{id}/approve")]
		public async Task<IActionResult> ApproveAnswer([FromRoute] int id, [FromForm] int points)
		{
			try
			{
				string userID = await adminAnswerService.ApproveAnswerAsync(id, points);
				await badgeService.CheckBadgesByCategory(userID, BadgeCategory.OnSuggesting);

				return Ok();
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("answer/{id}/reject")]
		public async Task<IActionResult> RejectAnswer([FromRoute] int id, [FromBody] string reason)
		{
			try
			{
				await adminAnswerService.RejectAnswerAsync(id, reason);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}
		#endregion

		#region User-specific
		[HttpGet("users")]
		public async Task<IActionResult> GetAllUsers()
		{
			try
			{
				var result = await adminUserService.GetAllUsers();

				return Ok(result);
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("user/{id}")]
		public async Task<IActionResult> GetUser([FromRoute] string id)
		{
			try
			{
				var result = await adminUserService.GetUser(id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("user/{id}/admin")]
		public async Task<IActionResult> MakeUserAdmin([FromRoute] string id)
		{
			try
			{
				await adminUserService.MakeAdmin(id);
				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("user/ban")]
		public async Task<IActionResult> BanUser([FromQuery] string id, [FromForm] string reason)
		{
			try
			{
				await adminUserService.BanUserAsync(id, reason);
				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("user/unban")]
		public async Task<IActionResult> UnBanUser([FromBody] string id)
		{
			try
			{
				await adminUserService.UnbanUserAsync(id);
				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}
		#endregion
	}
}