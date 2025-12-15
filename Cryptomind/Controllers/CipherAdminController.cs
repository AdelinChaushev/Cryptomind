using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Data.Entities;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cryptomind.Controllers
{
	[ApiController]
	[Route("api/admin")]
	//[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
	public class CipherAdminController : ControllerBase
	{
		private IAdminService adminService;
		public CipherAdminController(IAdminService service)
		{
			this.adminService = service;
		}

		[HttpGet("pending-ciphers")]
		public async Task<IActionResult> GetSubmittedCiphers()
		{
			try
			{
				var result = await adminService.AllSubmittedCiphers();
				return Ok(result);
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}
		[HttpGet("approved-ciphers")]
		public async Task<IActionResult> GetApprovedCiphers()
		{
			try
			{
				var result = await adminService.AllApprovedCiphers();
				return Ok(result);
			}
			catch (Exception ex)
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
				return Ok(cipher); //Should I return the whole DB entity??
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		} 
		// The button approve is clicked here

		[HttpPut("approve-cipher/{id}")]
		public async Task<IActionResult> ApproveCipher([FromRoute] int id, [FromBody] ApproveUpdateCipherViewModel model)
		{
            Console.WriteLine("In the method");
			try
			{
				if (!ModelState.IsValid) return BadRequest();

				await adminService.ApproveCipherAsync(id, model);
				//Here we can update some values for the user which submitted the cipher, like adding that he created a cipher

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}

		[HttpDelete("reject-cipher/{id}")]
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

		[HttpPut("update-cipher/{id}")]
		public async Task<IActionResult> UpdateCipher([FromRoute] int id, [FromForm] ApproveUpdateCipherViewModel model)
		{
			try
			{
				if (!ModelState.IsValid) return BadRequest();

				await adminService.UpdateApprovedCipher(id, model);

				return Ok();
			}
			catch (Exception ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		}
		[HttpDelete("delete-cipher/{id}")]
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
	}
}
