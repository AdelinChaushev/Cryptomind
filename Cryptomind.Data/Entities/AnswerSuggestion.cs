using Cryptomind.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptomind.Data.Entities
{
	public class AnswerSuggestion
	{
		[Key]
		public int Id { get; set; }
		[ForeignKey(nameof(Cipher))]
		public int CipherId { get; set; }
		public Cipher Cipher { get; set; }
		[ForeignKey(nameof(ApplicationUser))]
		public string UserId { get; set; }
		public ApplicationUser ApplicationUser { get; set; }
		public string Description { get; set; }
		public string DecryptedText { get; set; }
		public ApprovalStatus Status { get; set; }
		public string? RejectionReason { get; set; }
		public int PointsEarned { get; set; }
		public DateTime UploadedTime { get; set; }
		public DateTime? ApprovalDate { get; set; }
		public DateTime? RejectionDate { get; set; }
	}
}
