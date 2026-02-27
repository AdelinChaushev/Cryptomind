namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class PendingCipherTitleViewModels
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string CreatedBy { get; set; }

		public string? SubmittedAt { get; set; }
	}
}
