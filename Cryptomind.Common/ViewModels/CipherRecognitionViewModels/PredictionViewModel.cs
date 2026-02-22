using Cryptomind.Common.Constants;
namespace Cryptomind.Common.ViewModels.CipherRecognitionViewModels
{
	public class PredictionViewModel
	{
		public string Family { get; set; }
		public string Type { get; set; }
		public double Confidence { get; set; }
		
		// Helper properties
		public string ConfidencePercentage => $"{Confidence * 100:F1}%";
		public bool IsReliable => Confidence >= CipherRecognitionConstants.ReliableConfidenceThreshold;
	}
}
