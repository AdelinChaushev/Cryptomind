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
using System.IO;

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
        public async Task<IActionResult> SubmitCipher([FromForm] SubmitCipherViewModel model)
        {

                 string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                try
                {
                    await cipherService.SubmitCipherAsync(model,userId);
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
