namespace Cryptomind.Data.Entities
{
	public class DailyChallengeParticipation
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public int DailyChallengeEntryId { get; set; }
		public DailyChallengeEntry Entry { get; set; }
		public DateTime ChallengeDate { get; set; }
		public bool IsCompleted { get; set; }
		public DateTime? SolvedAt { get; set; }
		public int AttemptCount { get; set; }
	}
}
