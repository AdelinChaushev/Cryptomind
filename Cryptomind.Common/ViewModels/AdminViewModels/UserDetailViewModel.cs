using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class UserDetailViewModel : UserViewModel
	{
		public bool IsEmailConfirmed { get; set; }
		public bool IsBanned { get; set; }
		public string BanReason { get; set; }
		public DateTime? BannedAt { get; set; }
		public string BannedBy { get; set; }
		public DateTime RegisteredAt { get; set; }
		public int TotalScore { get; set; }
		public int CiphersSubmitted { get; set; }
		public int CiphersSolved { get; set; }
		public int HintsRequested { get; set; }
		public double SolveSuccessRate { get; set; }
		public int ApprovedCiphers { get; set; }
		public int PendingCiphers { get; set; }
		public List<UserCipherViewModel> SubmittedCiphers { get; set; }
		public List<UserCipherViewModel> SolvedCiphers { get; set; }
	}
}
