namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class AnswerSuggestionReviewViewModel : AnswerSuggestionViewModel
	{
		public string DecryptedText { get; set; }
		public string CipherEncryptedText { get; set; }

        public string Family { get; set; }
    }
}
