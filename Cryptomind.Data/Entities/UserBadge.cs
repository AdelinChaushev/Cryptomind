using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptomind.Data.Entities
{
	public class UserBadge
	{
		[Key]
		public int Id { get; set; }
		[ForeignKey(nameof(ApplicationUser))]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		[ForeignKey(nameof(Badge))]
		public int BadgeId { get; set; }
		public Badge Badge { get; set; }
		public DateTime EarnedAt { get; set; }
	}
}
