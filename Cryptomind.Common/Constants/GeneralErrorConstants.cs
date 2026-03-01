namespace Cryptomind.Common.Constants
{
	public class GeneralErrorConstants
	{
		public const string DataIntegrity = "Грешка в целостта на данните: потребител {0}  няма отговор {1}.";
		public static string MatchDataIntegrityError(object arg1, object arg2)
		   => string.Format(DataIntegrity, arg1, arg2);
	}
}
