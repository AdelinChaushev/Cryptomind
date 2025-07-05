using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Data.Entities;
using Crytomind.Core.Contracts;
using Crytomind.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cryptomind.Controllers
{
	[Route("api/cipherAdmin")]
	[ApiController]
	public class CipherAdminController : ControllerBase
	{
		private IAdminService adminService;
		public CipherAdminController(IAdminService service)
		{
			this.adminService = service;
		}

		[HttpGet("pendingCiphers")]
		//[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")] - this is correct everywhere
		public async Task<IActionResult> GetSubmittedCyphers()
		{
			try
			{
				var result = await adminService.AllSubmittedCyphers();
				return Ok(result);
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpGet("cipherAdmin/{id}")]
		//[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
		public async Task<IActionResult> GetCipher([FromRoute] int id)
		{
			try
			{
				var cipher = await adminService.GetCipherById(id);
				return Ok(cipher); //Should I return the whole DB entity??
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		} // The button approve is clicked here

		[HttpPut("approveCipher/{id}")]
		//[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
		public async Task<IActionResult> ApproveCipher([FromRoute] int id, [FromForm] ApproveCipherViewModel model)
		{
			try
			{
				if (!ModelState.IsValid) return BadRequest();

				await adminService.ApproveCipherAsync(id, model);
				//Here we can update some values for the user which submitted the cipher

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpDelete("rejectCipher/{id}")]
		//[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
		public async Task<IActionResult> RejectCipher([FromRoute] int id)
		{
			try
			{
				var cipher = await adminService.GetCipherById(id);
				if (cipher != null)
					await adminService.RejectCipherAsync(cipher.Id);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}
	}
}
