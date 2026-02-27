using Cryptomind.Data.Enums;

namespace Cryptomind.Common.Helpers
{
	public static class CipherTypeMapperHelper
	{
		private static readonly Dictionary<CipherType, string> BulgarianNames = new()
		{
			{ CipherType.Caesar, "Цезар" },
			{ CipherType.Atbash, "Атбаш" },
			{ CipherType.SimpleSubstitution, "Проста замяна" },
			{ CipherType.ROT13, "ROT13" },
			{ CipherType.Vigenere, "Виженер" },
			{ CipherType.Autokey, "Автоключ" },
			{ CipherType.Trithemius, "Тритемий" },
			{ CipherType.RailFence, "Железопътна ограда" },
			{ CipherType.Columnar, "Колонна" },
			{ CipherType.Route, "Маршрут" },
			{ CipherType.Base64, "Base64" },
			{ CipherType.Morse, "Морзов" },
			{ CipherType.Binary, "Двоичен" },
			{ CipherType.Hex, "Шестнадесетичен" },
		};

		public static string ToDisplayName(CipherType cipherType)
		{
			var bulgarian = BulgarianNames[cipherType];
			var english = cipherType.ToString();
			return $"{bulgarian} ({english})";
		}
		//public static string ToDisplayName(int cipherType)
		//{
		//	return ToDisplayName((CipherType)cipherType);
		//}
	}
}
