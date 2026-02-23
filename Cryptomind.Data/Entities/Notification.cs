using Cryptomind.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptomind.Data.Entities
{
	public class Notification
	{
		[Key]
		public int Id { get; set; }
		[ForeignKey(nameof(ApplicationUser))]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public NotificationType Type { get; set; }
		[MaxLength(500)]
		public string Message { get; set; }
		[MaxLength(200)]
		public string Link { get; set; } // URL to navigate to
		public bool IsRead { get; set; } = false;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}