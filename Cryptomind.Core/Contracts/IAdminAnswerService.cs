using Cryptomind.Common.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IAdminAnswerService
	{
		Task<List<AnswerSuggestionViewModel>> AllSubmittedAnswersAsync();
		Task<AnswerSuggestionReviewViewModel> GetAnswerById(int id);
		Task<string> ApproveAnswerAsync(int id, int points);
		Task RejectAnswerAsync(int id, string reason);
	}
}
