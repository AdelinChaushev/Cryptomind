using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.EnglishValidationModels
{
	public class ValidationDetails
	{
		[JsonPropertyName("text_length")]
		public int TextLength { get; set; }

		[JsonPropertyName("letter_count")]
		public int LetterCount { get; set; }

		[JsonPropertyName("word_count")]
		public int WordCount { get; set; }

		[JsonPropertyName("ic_value")]
		public double IcValue { get; set; }

		[JsonPropertyName("error")]
		public string? Error { get; set; }
	}
}
