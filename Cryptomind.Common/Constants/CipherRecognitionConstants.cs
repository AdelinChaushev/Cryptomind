namespace Cryptomind.Common.Constants
{
	public static class CipherRecognitionConstants
	{
		public const double ReliableConfidenceThreshold = 0.7;
		public const int ApiTimeoutSeconds = 30;

		//Errors
		public const string InputTextCannotBeEmpty = "Въведеният текст не може да бъде празен";
		public const string MLApiError = "ML API върна грешка (Статус{0}): {1}";
		public const string InvalidAnalysis = "Неуспешно анализиране на отговора на ML API";
		public const string BestPredictionMissing = "Невалиден отговор от ML API - липсва най-добрата прогноза";
		public const string MLServiceNotAvailable = "Услугата за машинно обучение не е налична. Моля, уверете се, че Python ML API работи на {0}";
		public const string MLTimeoutExpired = "ML услугата не отговори в рамките на {0} секунди";
	}
}
