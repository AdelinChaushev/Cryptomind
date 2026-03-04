
namespace Cryptomind.Common.Constants
{
	public class UserConstants
	{
		public const string AdminRole = "Admin";
		public const string UserRole = "User";

		//Error
		public const string UserAlreadyAdmin = "Потребителят вече е администратор.";
		public const string UserAlreadyDeactivated = "Този потребителски акаунт вече е деактивиран.";
		public const string UserAlreadyBanned = "Този потребител вече е блокиран.";
		public const string UserNotBanned = "Този потребител не е блокиран.";
		public const string AdminCannotBeBanned = "Администратори не могат да бъдат блокирани.";
		public const string UserNotFound = "Потребителят не беше намерен";
		public const string AdminsCannotDeactivate = "Администраторите не могат да деактивират собствения си акаунт";
		public const string UsernameCannotContainSpaces = "Потребителското име не може да съдържа интервали.";

		public const string InvalidCredentials = "Невалидни данни";
		public const string ThisAcountIsDeactivated = "Този акаунт е деактивиран";
		public const string ThisEmailAlreadyExists = "Потребител с този имейл вече съществува";
		public const string ThisUsernameAlreadyExists = "Вече съществува потребител с това име";
		public const string CannotCreateUsernameAnonymous = "Не може да създадете потребител с име: {0}";

		public const string KeepUsernameConstraints = "Спазвайте ограниченията за името";
		public const string KeepPasswordConstraints = "Спазвайте ограниченията на паролата";
	}
}