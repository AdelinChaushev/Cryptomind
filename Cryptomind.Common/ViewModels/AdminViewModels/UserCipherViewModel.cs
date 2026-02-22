using Cryptomind.Data.Enums;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class UserCipherViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string TypeOfCipher { get; set; }
		public string Status { get; set; }
		public int Points { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? SolvedAt { get; set; }  // If in solved list
		public ChallengeType ChallengeType { get; set; }
	}
}
