using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class DashboardViewModel
	{
		public DashboardViewModel()
		{
			PendingCipherTitles = new List<string>();
		}
		public int PendingCiphersCount { get; set; }
		public int ApprovedCiphersCount { get; set; }
		public int PendingAnswersCount { get; set; }
		public int ApprovedAnswersCount { get; set; }
		public int DeletedCiphersCount { get; set; }
		public List<string> PendingCipherTitles { get; set; } 
	}
}
