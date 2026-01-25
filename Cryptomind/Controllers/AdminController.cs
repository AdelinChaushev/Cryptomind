using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Common.CipherViewModels;
using Cryptomind.Common.DTOs;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using static System.Net.Mime.MediaTypeNames;
using Cryptomind.Data.Enums;

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
		[HttpPost("cipher/{id}/solve")]
		public async Task<IActionResult> SolveCipher(int id)
		{
			try
			{
				var solution = await adminService.SolveCipherWithLLM(id);

				return Ok(new
				{
					success = true,
					analysis = solution
				});
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = "An error occurred while solving the cipher" });
			}
		}

		[HttpPut("cipher/{id}/approve")]
		public async Task<IActionResult> ApproveCipher([FromRoute] int id, [FromBody] ApproveUpdateCipherViewModel model)
		{
			try
			{
				string userID = await adminService.ApproveCipherAsync(id, model);
				await badgeService.CheckBadgesByCategory(userID, BadgeCategory.OnUpload);
				//Here we can update some values for the user which submitted the cipher, like adding that he created a cipher

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}
		[HttpPut("cipher/{id}/unapprove")]
		public async Task<IActionResult> UnpproveCipher([FromRoute] int id)
		{
			try
			{
				string userID = await adminService.UnapproveCipherAsync(id);
				//await badgeService.CheckBadgesByCategory(userID, BadgeCategory.OnUpload);
				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpDelete("cipher/{id}/reject")]
		public async Task<IActionResult> RejectCipher([FromRoute] int id)
		{
			try
			{
				await adminService.RejectCipherAsync(id);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpPut("cipher/{id}/update")]
		public async Task<IActionResult> UpdateCipher([FromRoute] int id, [FromBody] ApproveUpdateCipherViewModel model)
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
		[HttpGet("test-llm-config")]
		public IActionResult TestLLMConfig([FromServices] IConfiguration config)
		{
			var apiUrl = config["LLMService:ApiUrl"];
			var model = config["LLMService:Model"];
			var apiKey = config["LLMService:ApiKey"];

			return Ok(new
			{
				ApiUrl = apiUrl,
				Model = model,
				ApiKeyExists = !string.IsNullOrEmpty(apiKey),
				ApiKeyPreview = apiKey?.Substring(0, 10) + "..." // Show first 10 chars only
			});
		}
	}
}
