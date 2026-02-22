using System.Text.Json.Serialization;

namespace Cryptomind.Common.ViewModels.EnglishValidationModels
{
	public class EnglishValidationResult
	{
		[JsonPropertyName("is_english")]
		public bool IsEnglish { get; set; }

		[JsonPropertyName("confidence")]
		public double Confidence { get; set; }

		[JsonPropertyName("metrics")]
		public ValidationMetrics Metrics { get; set; } = new();

		[JsonPropertyName("details")]
		public ValidationDetails Details { get; set; } = new();
	}
}
