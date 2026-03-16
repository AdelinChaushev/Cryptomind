using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Rooms
{
	public class Round
	{
		public Round()
		{
			Submissions = new List<RoomSubmission>();
		}
		public bool IsFinished { get; set; }
		public string EncryptedText { get; set; }
		public CipherType CorrectAnswer { get; set; }
		public string? WinnerId { get; set; }
		public List<RoomSubmission> Submissions { get; set; }
	}
}
