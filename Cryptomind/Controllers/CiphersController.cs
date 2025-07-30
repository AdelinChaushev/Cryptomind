using Cryptomind.Common.CipherViewModels;
using Cryptomind.Common.DTOs;
using Cryptomind.Data.Entities;
using Crytomind.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cryptomind.Data.Enums;
using Cryptomind.Common.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Cryptomind.Controllers
{
	[Route("api/ciphers")]
	[ApiController]
	public class CiphersController : ControllerBase
	{
        private ICipherService cipherService;
        public CiphersController(ICipherService cipherService)
        {
            this.cipherService = cipherService;
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
        [HttpPost("submitCipher")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> SubmitCipher ([FromBody] SubmitCipherViewModel model)
        {
            Cipher? cipher = null;
            if (model.CipherDefinition == CipherDefinition.TextCipher)
            {
                cipher = new TextCipher()
                {
                    Title = model.Title,
                    DecryptedText = model.DecryptedText,
                    EncryptedText = model.EncryptedText,
                    //TypeOfCipher = model.Type,
                    AllowHint = false,
                    AllowSolution = false,
                    IsApproved = false,
                    CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CipherTags = new List<CipherTag>(),
                    HintsRequested = new List<HintRequest>()
			    };
            }
            else if (model.CipherDefinition == CipherDefinition.ImageCipher)
            {
				cipher = new ImageCipher()
				{
					Title = model.Title,
					DecryptedText = model.DecryptedText,
					ImagePath = model.ImagePath,
					//TypeOfCipher = model.Type,
					AllowHint = false,
					AllowSolution = false,
					IsApproved = false,
					CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
					CipherTags = new List<CipherTag>(),
					HintsRequested = new List<HintRequest>()
				};
			}
            try
            {
                await cipherService.SubmitCipherAsync(cipher);
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
