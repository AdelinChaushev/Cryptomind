using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.AnswerSubmissionViewModels;

namespace Cryptomind.Core.Contracts
{
	public interface IAnswerSubmissionService
	{
		Task SuggestAnswerAsync(SuggestAnswerDTO answer, string userId, int cipherId);
		Task<List<AnswerSubmissionViewModel>> SubmittedAnswers(string userId);
	}
}