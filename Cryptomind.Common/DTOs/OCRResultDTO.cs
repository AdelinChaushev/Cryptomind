using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.DTOs
{
	public class OCRResultDTO
	{
		public bool Success { get; set; }
		public string ExtractedText { get; set; }
		public double Confidence { get; set; }
		public int CharCount { get; set; }
		public int WordCount { get; set; }
		public OCRValidation Validation { get; set; }
		public string ErrorMessage { get; set; }
	}
	public class OCRValidation
	{
		public bool IsValid { get; set; }
		public List<string> Warnings { get; set; }
		public string Recommendation { get; set; }
	}
}
