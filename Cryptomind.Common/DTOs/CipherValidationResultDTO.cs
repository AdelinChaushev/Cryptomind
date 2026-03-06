using System.Text.Json.Serialization;

namespace Cryptomind.Common.DTOs
{
	public class CipherValidationResultDTO
	{
		[JsonPropertyName("predicted_type")]
		public string? PredictedType { get; set; }

		[JsonPropertyName("confidence")]
		public string? Confidence { get; set; }

		[JsonPropertyName("solution_correct")]
		public bool? SolutionCorrect { get; set; }

		[JsonPropertyName("is_appropriate")]
		public bool IsAppropriate { get; set; }

		[JsonPropertyName("is_solvable")]
		public bool? IsSolvable { get; set; }

		[JsonPropertyName("issues")]
		public List<string> Issues { get; set; } = new();

		[JsonPropertyName("recommendation")]
		public string Recommendation { get; set; } = string.Empty;

		[JsonPropertyName("reasoning")]
		public string Reasoning { get; set; } = string.Empty;

		[JsonPropertyName("type_reasoning")]
		public string TypeReasoning { get; set; } = string.Empty;
	}
}
