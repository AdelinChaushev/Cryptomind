namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class DashboardViewModel
	{
		public DashboardViewModel()
		{
			PendingCipherTitles = new List<PendingCipher>();
		}
		public int PendingCiphersCount { get; set; }
		public int ApprovedCiphersCount { get; set; }
		public int PendingAnswersCount { get; set; }
		public int ApprovedAnswersCount { get; set; }
		public int DeletedCiphersCount { get; set; }
		public List<PendingCipher> PendingCipherTitles { get; set; } 
	}
	public class PendingCipher
	{
        public int Id { get; set; }
        public string Title { get; set; }

		public string CreatedBy { get; set; }

        public DateTime? SubmittedAt { get; set; }
    }
}
