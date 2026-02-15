using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class DashboardViewModel
	{
		public int PendingCiphersCount { get; set; }
		public int ApprovedCiphersCount { get; set; }
		public int PendingAnswersCount { get; set; }
		public int ApprovedAnswersCount { get; set; }
		public List<string> PendingCipherTitles { get; set; } 
	}
}
