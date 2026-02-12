using Cryptomind.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cryptomind.Controllers
{
	[Route("api/submissions")]
	[ApiController]
	public class SubmissionController : ControllerBase
	{
		private ICipherSubmissionService cipherSubmissionService;
		private IAnswerSubmissionService answerSubmissionService;
	}
}
