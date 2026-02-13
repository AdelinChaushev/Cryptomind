using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cryptomind.Controllers
{
	[Route("api/submissions")]
	[ApiController]
	public class SubmissionController : ControllerBase
	{
		private ICipherSubmissionService cipherSubmissionService;
		private IAnswerSubmissionService answerSubmissionService;

		public SubmissionController(ICipherSubmissionService cipherSubmissionService, IAnswerSubmissionService answerSubmissionService)
		{
			this.cipherSubmissionService = cipherSubmissionService;
			this.answerSubmissionService = answerSubmissionService;
		}

		[HttpGet("")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> GetAllSubmissions()
		{
			string userId = GetUserId();

			var ciphers = await cipherSubmissionService.SubmittedCiphers(userId);
			var answers = await answerSubmissionService.SubmittedAnswers(userId);

			return Ok(new { ciphers, answers });
		}

		private string GetUserId()
		   => User.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}
