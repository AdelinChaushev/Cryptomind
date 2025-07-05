using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data.Enums
{
   public enum CipherType
    {
        Caesar = 0,
        Substitution = 1,
		Atbash = 2,
		Affine = 3,
		Vigenere = 4,
		Playfair = 5,
		RailFence = 6,
		ColumnarTransposition = 7,
		ROT13 = 8,
		Baconian = 9,
		PolybiusSquare = 10,
		MorseCode = 11,
		ADFGVX = 12,
		Hill = 13,
		Beaufort = 14,
		Keyword = 15,
		Nihilist = 16,
		OneTimePad = 17,
		Autokey = 18,
		XOR = 19,
	}
}
