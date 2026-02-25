using Cryptomind.Data.Entities;
namespace Cryptomind.Common.ViewModels.CipherSubmissionViewModels
{
	public class CipherSubmissionViewModel
	{
		//Common state (pending)

		public string Title { get; set; }
		public string CipherText { get; set; }
		public string SubmittedTime { get; set; }
		public string Status { get; set; }
        public int? Id{ get; set; }
        //Deleted state
        public string? DeletedTime { get; set; }
		//Approved state
		public string? ApprovedTime { get; set; }
		public string? ApprovedAs { get; set; }
		public List<Tag>? AssignedTags { get; set; }
		public int SolvedByCount { get; set; }
		//Rejected state
		public string? RejectionTime { get; set; }
		public string? RejectionReason { get; set; }
	}
}
