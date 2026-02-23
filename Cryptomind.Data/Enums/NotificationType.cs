namespace Cryptomind.Data.Enums
{
	public enum NotificationType
	{
		//Cipher related
		CipherApproved = 0,
		CipherRejected = 1,
		CipherDeleted = 2,
		CipherRestored = 3,
		CipherUpdated = 4,
		//Answer related
		AnswerApproved = 5,
		AnswerRejected = 6,
		AnswerCipherDeleted = 7,
		AnswerCipherRestored = 8,
		//Badges related
		BadgeEarned = 9,
	}
}
