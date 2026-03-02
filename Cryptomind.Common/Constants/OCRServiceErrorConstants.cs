using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.Constants
{
	public static class OCRServiceErrorTexts
	{
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
