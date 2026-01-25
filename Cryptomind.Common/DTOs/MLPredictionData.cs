using System.Text.Json.Serialization;

namespace Cryptomind.Common.DTOs
{
	public class MlPredictionData
	{
		[JsonPropertyName("family")]
		public string Family { get; set; }

		[JsonPropertyName("type")]
		public string Type { get; set; }

		[JsonPropertyName("confidence")]
		public double Confidence { get; set; }

		[JsonPropertyName("allPredictions")]
		public List<MlPredictionItem> AllPredictions { get; set; }
	}

	public class MlPredictionItem
	{
		[JsonPropertyName("family")]
		public string Family { get; set; }

		[JsonPropertyName("type")]
		public string Type { get; set; }

		[JsonPropertyName("confidence")]
		public double Confidence { get; set; }
	}
}