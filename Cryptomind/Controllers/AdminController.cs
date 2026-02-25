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
		[HttpGet("pending-ciphers")]
		public async Task<IActionResult> GetPendingCiphers([FromQuery] string? filter)
		{
			var result = await adminCipherService.AllPendingCiphers(filter);
			return Ok(result);
		}

		[HttpGet("approved-ciphers")]
		public async Task<IActionResult> GetApprovedCiphers([FromQuery] CipherFilter filter)
		{
			var result = await adminCipherService.AllApprovedCiphers(filter);
			return Ok(result);
		}

		[HttpGet("deleted-ciphers")]
		public async Task<IActionResult> GetDeletedCiphers([FromQuery] CipherFilter filter)
		{
			var result = await adminCipherService.AllDeletedCiphers(filter);
			return Ok(result);
		}

		[HttpGet("cipher/{id}")]
		public async Task<IActionResult> GetCipher([FromRoute] int id)
		{
			var cipher = await adminCipherService.GetCipherById(id);
			if (cipher.Status != ApprovalStatus.Pending.ToString())
			{
				throw new ConflictException("Шифърът не очаква одобрение");
			}
			return Ok(cipher);
		}
		[HttpGet("cipher/{id}/analyze")]
		public async Task<IActionResult> AnalyzeCipher([FromRoute] int id)
		{
			var result = await adminCipherService.AnalyzeWithLLM(id);
			return Ok(result);
		}

		[HttpPut("cipher/{id}/approve")]
		public async Task<IActionResult> ApproveCipher([FromRoute] int id, [FromBody] ApproveCipherViewModel model)
		{
			string userId = await adminCipherService.ApproveCipherAsync(id, model);
			await badgeService.CheckBadgesByCategory(userId, BadgeCategory.OnUpload);
			return Ok();
		}

		[HttpPut("cipher/{id}/reject")]
		public async Task<IActionResult> RejectCipher([FromRoute] int id, [FromBody] string reason)
		{
			await adminCipherService.RejectCipherAsync(id, reason);
			return Ok();
		}

		[HttpPut("cipher/{id}/update")]
		public async Task<IActionResult> UpdateCipher([FromRoute] int id, [FromBody] UpdateCipherViewModel model)
		{
			await adminCipherService.UpdateApprovedCipher(id, model);
			return Ok();
		}


		[HttpPut("cipher/{id}/delete")]
		public async Task<IActionResult> DeleteCipher([FromRoute] int id)
		{
			await adminCipherService.SoftDeleteCipher(id);
			return Ok();
		}
		[HttpPut("cipher/{id}/restore")]
		public async Task<IActionResult> RestoreCipher([FromRoute] int id, [FromQuery] string? newTitle = null)
		{
			await adminCipherService.RestoreCipher(id, newTitle);
			return Ok();
		}
		#endregion

		#region Answer-specific
		[HttpGet("pending-answer-suggestions")]
		public async Task<IActionResult> GetSubmittedAnswers([FromQuery] string? cipherName, [FromQuery] string? username)
		{
			var result = await adminAnswerService.AllSubmittedAnswersAsync(cipherName, username);
			return Ok(result);

		}

		[HttpGet("answer/{id}")]
		public async Task<IActionResult> GetAnswer([FromRoute] int id)
		{
			var result = await adminAnswerService.GetAnswerById(id);
			return Ok(result);

		}

		[HttpPut("answer/{id}/approve")]
		public async Task<IActionResult> ApproveAnswer([FromRoute] int id)
		{
			List<string> userIds = await adminAnswerService.ApproveAnswerAsync(id);

			foreach (var userId in userIds)
			{
				await badgeService.CheckBadgesByCategory(userId, BadgeCategory.OnSuggesting);
			}
			return Ok();

		}

		[HttpPut("answer/{id}/reject")]
		public async Task<IActionResult> RejectAnswer([FromRoute] int id, [FromBody] string reason)
		{
			await adminAnswerService.RejectAnswerAsync(id, reason);
			return Ok();

		}
		#endregion

		#region User-specific
		[HttpGet("users")]
		public async Task<IActionResult> GetAllUsers([FromQuery] UserFilter filter)
		{
			var result = await adminUserService.GetAllUsers(filter);
			return Ok(result);

		}

		[HttpGet("user/{id}")]
		public async Task<IActionResult> GetUser([FromRoute] string id)
		{
			var result = await adminUserService.GetUser(id);
			return Ok(result);

		}

		[HttpPut("user/{id}/admin")]
		public async Task<IActionResult> MakeUserAdmin([FromRoute] string id)
		{
			await adminUserService.MakeAdmin(id);
			return Ok();
		}

		[HttpPut("user/{id}/ban")]
		public async Task<IActionResult> BanUser([FromRoute] string id, [FromForm] string reason)
		{
			await adminUserService.BanUserAsync(id, reason);
			return Ok();
		}

		[HttpPut("user/{id}/unban")]
		public async Task<IActionResult> UnbanUser([FromRoute] string id)
		{
			await adminUserService.UnbanUserAsync(id);
			return Ok();

		}
		#endregion
	}
}