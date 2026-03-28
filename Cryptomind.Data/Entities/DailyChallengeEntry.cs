using Cryptomind.Data.Enums;

namespace Cryptomind.Data.Entities
{
	public class DailyChallengeEntry
	{
		public int Id { get; set; }
		public string PlainText { get; set; }
		public string EncryptedText { get; set; }
		public CipherType CipherType { get; set; }
		public bool IsUsed { get; set; }
		public DateTime? AssignedDate { get; set; }
		public ICollection<DailyChallengeParticipation> Participations { get; set; } = new List<DailyChallengeParticipation>();
	}
}
