namespace Cryptomind.Common.Constants
{
	public static class EnglishValidationConstants
	{
		public const int ApiTimeoutSeconds = 10;

		//Error
		public const string DecryptionTextCannotBeEmpty = "Декриптириният текст не може да бъде празен";
		public const string APIError = "API за валидиране на английски език върна грешка (Статус{0}): {1}";
		public const string InvalidAnalysis = "Неуспешен анализ на отговора за валидиране на английски език";

		public const string EnglishServiceNotAvailable = "Услугата за валидиране на английски език не е налична. Моля, уверете се, че Python ML API работи на {0}";
		public const string TimeoutExpired = "Проверката на английския език не получи навременен отговор в рамките на  {0} секунди";
	}
}
