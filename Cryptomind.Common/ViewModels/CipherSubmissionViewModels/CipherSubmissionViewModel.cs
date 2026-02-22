using Cryptomind.Data.Entities;
namespace Cryptomind.Common.ViewModels.CipherSubmissionViewModels
{
	public class CipherSubmissionViewModel
	{
		//Common state (pending)

		public string Title { get; set; }
		public string CipherText { get; set; }
		public DateTime SubmittedTime { get; set; }
		public string Status { get; set; }
        public int? Id{ get; set; }
        //Deleted state
        public DateTime? DeletedTime { get; set; }
		//Approved state
		public DateTime? ApprovedTime { get; set; }
		public string? ApprovedAs { get; set; }
		public List<Tag>? AssignedTags { get; set; }
		public int SolvedByCount { get; set; }
		//Rejected state
		public DateTime? RejectionTime { get; set; }
		public string? RejectionReason { get; set; }
	}
}
