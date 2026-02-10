using Cryptomind.Common.AdminViewModels;
using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Common.DTOs;
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
		private IAdminService adminService;
		private IBadgeService badgeService;
		public AdminController(IAdminService service, IBadgeService badgeService)
		{
			this.adminService = service;
			this.badgeService = badgeService;
		}

		#region Cipher-specific
		[HttpGet("pending-ciphers")]
		public async Task<IActionResult> GetSubmittedCiphers()
		{
			try
			{
				var result = await adminService.AllSubmittedCiphers();
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("approved-ciphers")]
		public async Task<IActionResult> GetApprovedCiphers([FromQuery] CipherFilter filter)
		{
			try
			{
				var result = await adminService.AllApprovedCiphers(filter);
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("pending-answer-suggestions")]
		public async Task<IActionResult> GetSubmittedAnswers()
		{
			try
			{
				var result = await adminService.AllSubmittedAnswersAsync();
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
				var cipher = await adminService.GetCipherById(id);
				return Ok(cipher);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("cipler/{id}/analyze")]
		public async Task<IActionResult> AnalyzeCipher([FromRoute] int id)
		{
			try
			{
				var result = await adminService.AnalyzeWithLLM(id);
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
				string userID = await adminService.ApproveCipherAsync(id, model);
				await badgeService.CheckBadgesByCategory(userID, BadgeCategory.OnUpload);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpDelete("cipher/{id}/reject")]
		public async Task<IActionResult> RejectCipher([FromRoute] int id, [FromBody] string reason)
		{
			try
			{
				await adminService.RejectCipherAsync(id, reason);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("cipher/{id}/unapprove")]
		public async Task<IActionResult> UnapproveCipher([FromRoute] int id)
		{
			try
			{
				await adminService.UnapproveCipherAsync(id);
				//await badgeService.CheckBadgesByCategory(userID, BadgeCategory.OnUpload);
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
				await adminService.UpdateApprovedCipher(id, model);

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
				await adminService.DeleteApprovedCipher(id);

				return Ok();
			}
			catch (Exception ex)
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
				var result = await adminService.GetAnswerById(id);
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
				string userID = await adminService.ApproveAnswerAsync(id, points);
				await badgeService.CheckBadgesByCategory(userID, BadgeCategory.OnSuggesting);

				return Ok();
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpDelete("answer/{id}/reject")]
		public async Task<IActionResult> RejectAnswer([FromRoute] int id, [FromBody] string reason)
		{
			try
			{
				await adminService.RejectAnswerAsync(id, reason);

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
				var result = await adminService.GetAllUsers();

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
				var result = await adminService.GetUser(id);
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
				await adminService.MakeAdmin(id);
				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("user/{id}/ban")]
		public async Task<IActionResult> BanUser([FromRoute] string id, [FromForm] string reason)
		{
			try
			{
				await adminService.BanUserAsync(id, reason);
				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("user/{id}/unban")]
		public async Task<IActionResult> UnBanUser([FromRoute] string id)
		{
			try
			{
				await adminService.UnbanUserAsync(id);
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