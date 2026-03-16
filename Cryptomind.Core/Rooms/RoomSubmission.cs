using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Rooms
{
	public class RoomSubmission
	{
		public string UserId { get; set; }
		public CipherType Answer { get; set; }
		public DateTime SubmissionTime { get; set; }
		public bool IsCorrect { get; set; }
	}
}
