using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.EnglishValidationModels
{
	public class ValidationMetrics
	{
		[JsonPropertyName("frequency")]
		public double Frequency { get; set; }

		[JsonPropertyName("bigrams")]
		public double Bigrams { get; set; }

		[JsonPropertyName("trigrams")]
		public double Trigrams { get; set; }

		[JsonPropertyName("words")]
		public double Words { get; set; }

		[JsonPropertyName("ic")]
		public double Ic { get; set; }
	}
}
