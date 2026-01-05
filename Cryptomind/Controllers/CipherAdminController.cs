using Cryptomind.Common.CipherAdminViewModels;
using Cryptomind.Common.CipherViewModels;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Services;
using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Cryptomind.Controllers
{
	[ApiController]
	[Route("api/admin")]
	[Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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
			catch (InvalidOperationException ex)
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
				var viewModel = new CipherReviewOutputViewModel();

				//string base64 = $"data:image/jpg;base64,{Convert.ToBase64String(await File.ReadAllBytesAsync(imageFolderPath))}";
				if (cipher is ImageCipher)
				{
					viewModel.Id = cipher.Id;
					viewModel.Title = cipher.Title;
					viewModel.DecryptedText = cipher.DecryptedText;
					viewModel.Points = cipher.Points;
					//viewModel.CipherText = base64;
					viewModel.AllowsAnswer = cipher.AllowSolution;
					viewModel.AllowsHint = cipher.AllowHint;
					viewModel.IsApproved = cipher.IsApproved;
					viewModel.IsImage = true;
				}
				else
				{
					TextCipher textCipher = cipher as TextCipher;
					viewModel.Id = cipher.Id;
					viewModel.Title = cipher.Title;
					viewModel.DecryptedText = cipher.DecryptedText;
					viewModel.Points = cipher.Points;
					viewModel.CipherText = textCipher.EncryptedText;
					viewModel.AllowsAnswer = cipher.AllowSolution;
					viewModel.AllowsHint = cipher.AllowHint;
					viewModel.IsApproved = cipher.IsApproved;
					viewModel.IsImage = true;
				}
				//Should I return the whole DB entity??
				//Samuil from December definetely not

				return Ok(viewModel);
			}
			catch (InvalidOperationException ex)
			{
				await Console.Out.WriteLineAsync(ex.Message);
			}
			return BadRequest();
		} 
		// The button approve is clicked here

		[HttpPut("cipher/{id}/approve")]
		public async Task<IActionResult> ApproveCipher([FromRoute] int id, [FromBody] ApproveUpdateCipherViewModel model)
		{
			try
			{
				

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
	}
}
