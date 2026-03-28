namespace Cryptomind.Common.Constants
{
	public class RoomConstants
	{
		public const int RoundsPerRace = 3;
		public const int RoundDurationSeconds = 300;
		public const int PreRoundSeconds = 3;

		public const string RoomNotFound = "Стаята не беше намерена";
		public const string PlayerNotInRoom = "Потрбителят с това ID: {0} не присъства в текущата стая или не съществува";
		public const string AlreadySubmitted = "Може да се подава само по един отговор на рунд";
		public const string RoomAlreadyFull = "Стаята вече е пълна";
		public const string PlayerAlreadyInRoom = "Вече сте в стаята";
		public const string RoundNotStarted = "Рундът все още не е започнал";
		public const string AnUnexpectedErrorOccured = "Възникна неочаквана грешка";
		public const string AlreadyInRoom = "Вече сте в друга стая"

		public const string WagerCannotBeNegative = "Залогът не може да бъде отрицателен";
		public const string NotEnoughPointsForWager = "Нямате достатъчно точки за този залог";
		public const string UserDoesNotHaveEnoughPoints = "{0} няма достатъчно точки за този залог";
	}
}
