namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class AnswerSuggestionViewModel
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Description { get; set; }
        public string CipherName { get; set; }
        public int CipherId { get; set; }
	}
}
