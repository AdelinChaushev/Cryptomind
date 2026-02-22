namespace Cryptomind.Data.Enums
{
	public enum CipherType
	{
		// Substitution Family (4 types)
		Caesar = 0,
		Atbash = 1,
		SimpleSubstitution = 2,
		ROT13 = 3,
		// Polyalphabetic Family (3 types)
		Vigenere = 4,
		Autokey = 5,
		Trithemius = 6,
		// Transposition Family (3 types)
		RailFence = 7,
		Columnar = 8,
		Route = 9,
		//Encoding (4 types)
		Base64 = 10,
		Morse = 11,
		Binary = 12,
		Hex = 13,
	}
}