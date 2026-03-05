namespace Cryptomind.Common.Constants
{
	public class CipherErrorConstants
	{
		public const string MySubmissionsPath = "my_submissions";

		public const string CipherNotFoundMessage = "Шифърът не е намерен.";
		public const string CipherDeletedConflict = "Шифърът е изтрит.";
		public const string UserNotFoundMessage = "Потребителят не е намерен.";
		public const string TitleRequiredMessage = "Заглавието е задължително.";
		public const string DuplicateTitleMessage = "Вече има шифър с това заглавие.";
		public const string HintExperimentalConflict = "Подсказките не могат да се използват за експериментални шифри.";
		public const string NotDeleted = "Шифърът не е изтрит";
		public const string EncryptedTextShouldNotBeEmpty = "Шифрованият текст е задължителен";

		public const string AlreadyHasAnswer = "Шифърът вече има официален отговор.";
		public const string StandardCipherSuggestionConflict = "Не може да се предлага отговор за стандартен шифър.";
		public const string EmptyAnswerSuggestion = "Не можете да изпратите празен отговор.";
		public const string OwnCipherSuggestionConflict = "Не можете да предлагате отговори на шифри, създадени от Вас.";
		public const string DuplicateSuggestion = "Вече сте предложили този отговор.";
		public const string AlreadyTriedWithThatAnswer = "Вече пробвахте с този отговор";

		public const string OwnCipherSolveConflict = "Потребителят не може да решава собствените си шифри.";
		public const string ExperimentalSolveConflict = "Експерименталните шифри не могат да бъдат решавани.";
		public const string AlreadySolvedConflict = "Не можете да решавате един и същ шифър два пъти.";
		public const string AnonymousUser = "Anonymous";

		public const string DuplicateCipherContent = "Вече съществува идентичен шифър.";
		public const string UnknownCipherSolutionConflict = "Не може да се изпрати шифър с неизвестен дешифриран текст и тип едновременно.";
		public const string MaxLengthExceeded = "Максималната дължина на шифрирания текст е 450 символа.";
		public const string OCRFailedMessage = "OCR не успя да извлече текст от изображението.";
		public const string PlaintextNotAllowed = "Текстът изглежда вече е дешифриран. Разрешени са само шифрирани текстове.";
		public const string StatusCipherDeleted = "CipherDeleted";
		public const string ImageRequired = "За този тип шифър е необходимо изображение.";
		public const string InvalidFileType = "Невалиден тип файл. Разрешени: .jpg, .jpeg, .png, .webp";
		public const string EmptyFileError = "Файлът не може да бъде празен.";
		public const string FileTooLarge = "Размерът на файла не може да надвишава 5MB.";

		public const string CipherIsResolved = "Шифърът не очаква одобрение";
		public const string CannotAnalyzeResolvedCiphers = "Можете да анализирате само шифри, които са в състояние на чакане";
		public const string CanApproveOnlyPendingCiphers = "Само чакащи шифри могат да бъдат одобрявани";
		public const string CanApproveOnlyCiphersWithType = "Шифър с неизвестен тип не може да бъде одобрен, защото точките за всеки шифър се базират на неговия тип.";
		public const string InvalidCipherType = "Невалиден тип шифър";
		public const string CipherIsAlreadyApproved = "Шифърът вече е одобрен";
		public const string CipherIsAlreadyRejected = "Шифърът е вече отхвърлен";

		public const string CipherIsAlreadyDeleted = "Шифърът вече е изтрит";
		public const string CipherIsNotApproved = "Шифърът не е одобрен";
	}
}
