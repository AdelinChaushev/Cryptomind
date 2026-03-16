namespace Cryptomind.Common.Helpers
{
	public static class GenerateRoomCodeHelper
	{
		private static readonly Random _random = new();
		private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		public static string CodeGenerator()
		{
			return new string(Enumerable.Range(0, 6)
				.Select(_ => Chars[_random.Next(Chars.Length)])
				.ToArray());
		}
	}
}
