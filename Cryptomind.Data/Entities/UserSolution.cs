using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptomind.Data.Entities
{
	public class UserSolution
	{
		[Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Cipher))]
		public int CipherId { get; set; }
		public Cipher Cipher { get; set; }
		[ForeignKey(nameof(ApplicationUser))]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public DateTime TimeSubmitted { get; set; }
		public bool IsCorrect { get; set; }
		public bool UsedTypeHint { get; set; } = false;
		public bool UsedSolutionHint { get; set; } = false;
		public bool UsedFullSolution { get; set; } = false;
		public int PointsEarned { get; set; }
	}
}
