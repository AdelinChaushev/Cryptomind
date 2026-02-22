namespace Cryptomind.Data.Enums
{
	public enum NotificationType
	{
		//Cipher related
		CipherApproved = 0,
		CipherRejected = 1,
		CipherDeleted = 2,
		CipherRestored = 3,
		//Answer related
		AnswerApproved = 4,
		AnswerRejected = 5,
		AnswerCipherDeleted = 6,
		AnswerCipherRestored = 7,
		//Badges related
		BadgeEarned = 8,
	}
}
