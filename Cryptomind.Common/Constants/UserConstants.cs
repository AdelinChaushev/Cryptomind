
namespace Cryptomind.Common.Constants
{
	public class UserConstants
	{
		public const string AdminRole = "Admin";
		public const string UserRole = "User";

		// Error Messages
		public const string UserAlreadyAdmin = "Потребителят вече е администратор.";
		public const string UserAlreadyDeactivated = "Този потребителски акаунт вече е деактивиран.";
		public const string UserAlreadyBanned = "Този потребител вече е блокиран.";
		public const string UserNotBanned = "Този потребител не е блокиран.";
		public const string AdminCannotBeBanned = "Администратори не могат да бъдат блокирани.";
	}
}