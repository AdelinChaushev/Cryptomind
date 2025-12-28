using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data.Enums
{
	public enum CipherType
	{
		// Substitution Family (4 types)
		Caesar = 0,
		ROT13 = 1,
		Atbash = 2,
		SimpleSubstitution = 3,

		// Polyalphabetic Family (3 types)
		Vigenere = 4,
		Autokey = 5,
		Trithemius = 6,

		// Transposition Family (3 types)
		RailFence = 7,
		Columnar = 8,
		Route = 9,

		// Encoding Types (4 types)
		Base64 = 10,
		Morse = 11,
		Binary = 12,
		Hex = 13,

		// Special
		Plaintext = 14  // For plain English text detection
	}
}