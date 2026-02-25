namespace Cryptomind.Common.ViewModels.AnswerSubmissionViewModels
{
	public class AnswerSubmissionViewModel
	{
		//Common state
		public string CipherTitle { get; set; }
		public string SuggestedAnswer { get; set; }
		public string Status { get; set; }
		public string SubmittedAt { get; set; }
		//Deleted state
		public string? CipherDeletedAt { get; set; }
		//Approved state
		public int? CipherId { get; set; }
		public int? PointsEarned { get; set; }
		public string? ApprovedDate { get; set; }
		//Rejected date
		public string? RejectionDate { get; set; }
		public string? RejectionReason { get; set; }
	}
}
