using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Common.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IAnswerSubmissionService
	{
		Task SuggestAnswerAsync(SuggestAnswerDTO answer, string userId, int cipherId);
		Task<List<AnswerSuggestionReviewViewModel>> SubmittedAnswers(SubmittedOrderTerm orderingTerm, string userId);
	}
}