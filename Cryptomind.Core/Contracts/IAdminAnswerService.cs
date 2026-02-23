using Cryptomind.Common.ViewModels.AdminViewModels;

namespace Cryptomind.Core.Contracts
{
	public interface IAdminAnswerService
	{
		Task<int> GetPendingAnswersCount();
		Task<int> GetApprovedAnswersCount();
		Task<List<AnswerSuggestionViewModel>> AllSubmittedAnswersAsync(string? cipherName, string? username);
		Task<AnswerSuggestionReviewViewModel> GetAnswerById(int id);
		Task<List<string>> ApproveAnswerAsync(int id);
		Task RejectAnswerAsync(int id, string reason);
	}
}
