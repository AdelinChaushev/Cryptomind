using Cryptomind.Common.Constants;
using Cryptomind.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
