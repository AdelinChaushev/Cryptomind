using System.Text;

namespace Cryptomind.Common.Helpers
{
	public static class CipherGeneratorHelper
	{
		private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private static readonly Random Random = new();

		private static readonly Dictionary<char, string> MorseDict = new()
		{
			{'A',".-"},{'B',"-..."},{'C', "-.-."},{'D',"-.."},{'E',"."},
			{'F',"..-."},{'G',"--."},{'H',"...."},{'I',".."},{'J',".---"},
			{'K',"-.-"},{'L',".-.."},{'M',"--"},{'N',"-."},{'O',"---"},
			{'P',".--."},{'Q',"--.-"},{'R',".-."},{'S',"..."},{'T',"-"},
			{'U',"..-"},{'V',"...-"},{'W',".--"},{'X',"-..-"},{'Y',"-.--"},
			{'Z',"--.."},{'/',"/"}
		};

		#region Substitution
		public static string Caesar(string plaintext, int? shift = null)
		{
			int s = shift ?? Random.Next(1, 26);
			return new string(plaintext.ToUpper().Select(c =>
				Alphabet.Contains(c) ? Alphabet[(Alphabet.IndexOf(c) + s) % 26] : c).ToArray());
		}

		public static string Rot13(string plaintext) => Caesar(plaintext, 13);

		public static string Atbash(string plaintext)
		{
			return new string(plaintext.ToUpper().Select(c =>
				Alphabet.Contains(c) ? Alphabet[25 - Alphabet.IndexOf(c)] : c).ToArray());
		}

		public static string SimpleSubstitution(string plaintext, string? key = null)
		{
			if (key == null)
			{
				var shuffled = Alphabet.ToCharArray();
				for (int i = shuffled.Length - 1; i > 0; i--)
				{
					int j = Random.Next(i + 1);
					(shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
				}
				key = new string(shuffled);
			}

			return new string(plaintext.ToUpper().Select(c =>
				Alphabet.Contains(c) ? key[Alphabet.IndexOf(c)] : c).ToArray());
		}

		#endregion

		#region Polyalphabetic
		public static string Vigenere(string plaintext, string? key = null)
		{
			key ??= new string(Enumerable.Range(0, Random.Next(3, 13))
				.Select(_ => Alphabet[Random.Next(26)]).ToArray());

			var result = new StringBuilder();
			int keyIndex = 0;

			foreach (char c in plaintext.ToUpper())
			{
				if (Alphabet.Contains(c))
				{
					int encIdx = (Alphabet.IndexOf(c) + Alphabet.IndexOf(key[keyIndex % key.Length])) % 26;
					result.Append(Alphabet[encIdx]);
					keyIndex++;
				}
				else result.Append(c);
			}
			return result.ToString();
		}

		public static string Autokey(string plaintext, string? key = null)
		{
			key ??= new string(Enumerable.Range(0, Random.Next(3, 9))
				.Select(_ => Alphabet[Random.Next(26)]).ToArray());

			string upper = plaintext.ToUpper();
			string lettersOnly = new string(upper.Where(c => Alphabet.Contains(c)).ToArray());
			string fullKey = key + lettersOnly[..^1];

			var result = new StringBuilder();
			int keyIndex = 0;

			foreach (char c in upper)
			{
				if (Alphabet.Contains(c))
				{
					int encIdx = (Alphabet.IndexOf(c) + Alphabet.IndexOf(fullKey[keyIndex])) % 26;
					result.Append(Alphabet[encIdx]);
					keyIndex++;
				}
				else result.Append(c);
			}
			return result.ToString();
		}

		public static string Trithemius(string plaintext)
		{
			var result = new StringBuilder();
			int shift = 0;

			foreach (char c in plaintext.ToUpper())
			{
				if (Alphabet.Contains(c))
				{
					result.Append(Alphabet[(Alphabet.IndexOf(c) + shift) % 26]);
					shift++;
				}
				else result.Append(c);
			}
			return result.ToString();
		}

		#endregion

		#region Transposition
		public static string RailFence(string plaintext, int? rails = null)
		{
			int r = rails ?? Random.Next(2, 7);
			string text = string.Concat(plaintext.Where(c => c != ' '));

			if (text.Length < r) return text;

			var fence = new List<char>[r];
			for (int i = 0; i < r; i++) fence[i] = new List<char>();

			int rail = 0, direction = 1;
			foreach (char c in text)
			{
				fence[rail].Add(c);
				rail += direction;
				if (rail == 0 || rail == r - 1) direction *= -1;
			}

			return string.Concat(fence.SelectMany(row => row));
		}
		public static string ColumnarTransposition(string plaintext, string? key = null)
		{
			if (key == null)
			{
				int keyLength = Random.Next(3, 9);
				key = new string(Enumerable.Range(0, keyLength)
					.Select(_ => Alphabet[Random.Next(26)]).ToArray());
			}

			string text = string.Concat(plaintext.Where(c => c != ' '));
			int cols = key.Length;
			int rows = (int)Math.Ceiling((double)text.Length / cols);

			char?[,] grid = new char?[rows, cols];
			for (int i = 0; i < text.Length; i++)
				grid[i / cols, i % cols] = text[i];

			var colOrder = key.Select((ch, i) => (ch, i))
				.OrderBy(x => x.ch).ThenBy(x => x.i)
				.Select(x => x.i).ToList();

			var result = new StringBuilder();
			foreach (int col in colOrder)
				for (int row = 0; row < rows; row++)
					if (grid[row, col].HasValue)
						result.Append(grid[row, col]!.Value);

			return result.ToString();
		}

		public static string RouteCipher(string plaintext)
		{
			string text = string.Concat(plaintext.Where(c => c != ' '));
			int cols = (int)Math.Ceiling(Math.Sqrt(text.Length));
			int rows = (int)Math.Ceiling((double)text.Length / cols);

			char?[,] grid = new char?[rows, cols];
			for (int i = 0; i < text.Length; i++)
				grid[i / cols, i % cols] = text[i];

			var result = new StringBuilder();
			int top = 0, bottom = rows - 1, left = 0, right = cols - 1;

			while (top <= bottom && left <= right)
			{
				for (int c = left; c <= right; c++)
					if (grid[top, c].HasValue) result.Append(grid[top, c]!.Value);
				top++;

				for (int r = top; r <= bottom; r++)
					if (grid[r, right].HasValue) result.Append(grid[r, right]!.Value);
				right--;

				if (top <= bottom)
				{
					for (int c = right; c >= left; c--)
						if (grid[bottom, c].HasValue) result.Append(grid[bottom, c]!.Value);
					bottom--;
				}

				if (left <= right)
				{
					for (int r = bottom; r >= top; r--)
						if (grid[r, left].HasValue) result.Append(grid[r, left]!.Value);
					left++;
				}
			}

			return result.ToString();
		}
		#endregion

		private static readonly List<string> CipherTypes = new()
		{
			"Caesar", "ROT13", "Atbash", "SimpleSubstitution",
			"Vigenere", "Autokey", "Trithemius",
			"RailFence", "Columnar", "Route",
		};

		private static readonly List<string> Sentences = new()
		{
			"Meet me at dawn by the old bridge",
			"The enemy moves at midnight",
			"Trust no one in the palace",
			"Burn this letter after reading",
			"The gold is hidden beneath the floor",
			"He was never seen again after that night",
			"Leave the city before the gates close",
			"The king does not know about the plot",
			"She passed the message through the baker",
			"Three men waited in the shadows",
			"The package will arrive on Thursday",
			"Do not speak his name in public",
			"The map leads to the northern tower",
			"We sail at first light tomorrow",
			"The spy was caught at the border",
			"Beware the man with the silver ring",
			"The code changes every seven days",
			"He left a mark on the eastern wall",
			"No one must know we were here",
			"The captain gave the order at dusk",
			"Follow the river until you reach the mill",
			"Two guards stand at the western gate",
			"The chest was empty when they arrived",
			"She knew the truth but said nothing",
			"The signal will be three short knocks",
			"Danger approaches from the southern road",
			"The letter was written in invisible ink",
			"He changed his name and fled to the coast",
			"The meeting is cancelled until further notice",
			"Watch the clock tower at noon",
			"They found the body near the old chapel",
			"The ambassador carries a secret message",
			"Strike when the bell rings twice",
			"The prisoner escaped through the east tunnel",
			"Her silence was the loudest confession",
			"The documents must not fall into their hands",
			"A storm is coming from the north",
			"He memorized the route and burned the map",
			"The vault opens only on the winter solstice",
			"Someone in this room is a traitor",
			"The artifact was stolen before sunrise",
			"We have been compromised abort the mission",
			"The lighthouse keeper knows the way in",
			"Four horses were found without riders",
			"The witness disappeared before the trial",
			"Retrieve the package from the third pillar",
			"The old scholar held the final piece",
			"Every door in the fortress was locked",
			"The general trusted only his shadow",
			"Silence is the first rule of survival",
		};

		public static string GetRandomSentence()
			=> Sentences[Random.Next(Sentences.Count)];
		public static (string CipherType, string EncryptedText) GenerateRandom()
		{
			var plaintext = GetRandomSentence();

			string cipherType = CipherTypes[Random.Next(CipherTypes.Count)];
			string encrypted = cipherType switch
			{
				"Caesar" => Caesar(plaintext),
				"ROT13" => Rot13(plaintext),
				"Atbash" => Atbash(plaintext),
				"SimpleSubstitution" => SimpleSubstitution(plaintext),
				"Vigenere" => Vigenere(plaintext),
				"Autokey" => Autokey(plaintext),
				"Trithemius" => Trithemius(plaintext),
				"RailFence" => RailFence(plaintext),
				"Columnar" => ColumnarTransposition(plaintext),
				"Route" => RouteCipher(plaintext),
				_ => throw new ArgumentException($"Unknown cipher: {cipherType}")
			};

			return (cipherType, encrypted);
		}
	}
}