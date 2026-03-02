namespace Cryptomind.Common.Constants
{
	public class OCRServiceConstants
	{
		public const int ApiTimeoutSeconds = 30;

		//Errors
		public const string OCRServiceUnavailable =
			"Услугата за OCR не е налична. Моля, уверете се, че Python OCR работи на {0}";

		public const string OCRApiError =
			"OCR API върна грешка (Статус {0}): {1}";

		public const string OCRApiInvalidResponse =
			"Неуспешен анализ на отговора на OCR API";

		public const string OCRExtractionFailed =
			"OCR извличането е неуспешно: {0}";

		public const string OCRNoTextExtracted =
			"OCR не успя да извлече текст от изображението.";

		public const string OCRServiceTimeout = "OCR услугата не отговаря в момента.Моля, опитайте отново по-късно.";
	}
}
