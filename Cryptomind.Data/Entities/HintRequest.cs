using Cryptomind.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptomind.Data.Entities
{
	public class HintRequest
	{
		[Key]
		public int Id { get; set; }
		[ForeignKey(nameof(Cipher))]
		public int CipherId { get; set; }
		public Cipher Cipher { get; set; }
		[ForeignKey(nameof(ApplicationUser))]
		public string UserId { get; set; }
		public ApplicationUser ApplicationUser { get; set; }
		public HintType HintType { get; set; }
		public DateTime RequestedAt { get; set; }
		public string HintContent { get; set; }
	}
}
