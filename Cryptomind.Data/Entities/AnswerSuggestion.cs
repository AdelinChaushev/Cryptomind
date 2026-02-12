using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public ApprovalStatus Status { get; set; }
		public string Description { get; set; }
		public string DecryptedText { get; set; }
		public DateTime UplodaedTime { get; set; }
	}
}
