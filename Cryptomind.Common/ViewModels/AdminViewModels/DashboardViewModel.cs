namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class DashboardViewModel
	{
		public DashboardViewModel()
		{
			PendingCipherTitles = new List<PendingCipherTitleViewModels>();
		}
		public int PendingCiphersCount { get; set; }
		public int ApprovedCiphersCount { get; set; }
		public int DeletedCiphersCount { get; set; }
		public int PendingAnswersCount { get; set; }
		public int ApprovedAnswersCount { get; set; }
		public List<PendingCipherTitleViewModels> PendingCipherTitles { get; set; } 
	}
}
