using Cryptomind.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cryptomind.Data.Entities
{
	public class Badge
	{
		[Key]
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		//public string? ImagePath { get; set; }
		public BadgeCategory Category { get; set; }
		public int EarnedBy { get; set; }
		public ICollection<UserBadge> UserBadges { get; set; }
	}
}
