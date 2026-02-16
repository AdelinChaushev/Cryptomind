using Cryptomind.Common.DTOs;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cryptomind.Controllers
{
	[ApiController]
	[Route("api/admin")]
	[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
	public class AdminController(
		IAdminCipherService adminCipherService,
		IAdminAnswerService adminAnswerService,
		IAdminUserService adminUserService,
		IBadgeService badgeService) : ControllerBase
	{
		[HttpGet("dashboard")]
		public async Task<IActionResult> GetDashboard()
		{
			return Ok(new DashboardViewModel
			{
				ApprovedCiphersCount = await adminCipherService.GetApprovedCiphersCount(),
				PendingCiphersCount = await adminCipherService.GetPendingCiphersCount(),
				DeletedCiphersCount = await adminCipherService.GetDeletedCiphersCount(),
				PendingCipherTitles = await adminCipherService.GetRecentCipherSubmissionTitles(),
				ApprovedAnswersCount = await adminAnswerService.GetApprovedAnswersCount(),
				PendingAnswersCount = await adminAnswerService.GetPendingAnswersCount(),
			});
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
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("pending-ciphers")]
		public async Task<IActionResult> GetSubmittedCiphers()
		{
			try
			{
				var result = await adminCipherService.AllSubmittedCiphers();
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("cipher/{id}")]
		public async Task<IActionResult> GetCipher([FromRoute] int id)
		{
			try
			{
				var cipher = await adminCipherService.GetCipherById(id);
				return Ok(cipher);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("cipher/{id}/approve")]
		public async Task<IActionResult> ApproveCipher([FromRoute] int id, [FromBody] ApproveCipherViewModel model)
		{
			try
			{
				string userId = await adminCipherService.ApproveCipherAsync(id, model);
				await badgeService.CheckBadgesByCategory(userId, BadgeCategory.OnUpload);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("cipher/{id}/delete")]
		public async Task<IActionResult> DeleteCipher([FromRoute] int id)
		{
			try
			{
				await adminCipherService.SoftDeleteCipher(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPut("cipher/{id}/restore")]
		public async Task<IActionResult> Restore([FromRoute]int id, [FromQuery]string? newTitle = null)
		{
			try
			{
				await adminCipherService.Restore(id, newTitle);
				return Ok();
			}
			catch (TitleConflictException ex)
			{
				return Conflict(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
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
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("answer/{id}")]
		public async Task<IActionResult> GetAnswer([FromRoute] int id)
		{
			try
			{
				var result = await adminAnswerService.GetAnswerById(id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("answer/{id}/approve")]
		public async Task<IActionResult> ApproveAnswer([FromRoute] int id, [FromForm] int points)
		{
			try
			{
				List<string> userIds = await adminAnswerService.ApproveAnswerAsync(id, points);

				foreach (var userId in userIds)
				{
					await badgeService.CheckBadgesByCategory(userId, BadgeCategory.OnSuggesting);	
				}
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
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
				return BadRequest(ex.Message);
			}
		}
		#endregion
	}
}