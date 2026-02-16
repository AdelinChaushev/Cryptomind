using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.AnswerSubmissionViewModels
{
	public class AnswerSubmissionViewModel
	{
		//Common state
		public string CipherTitle { get; set; }
		public string SuggestedAnswer { get; set; }
		public string Status { get; set; }
		public DateTime SubmittedAt { get; set; }
		//Deleted state
		public DateTime? CipherDeletedAt { get; set; }
		//Approved state
		public int? PointsEarned { get; set; }
		public DateTime? ApprovedDate { get; set; }
		//Rejected date
		public DateTime? RejectionDate { get; set; }
		public string RejectionReason { get; set; }
	}
}
