namespace Cryptomind.Common.DTOs
{
	public class OCRResultDTO
	{
		public bool Success { get; set; }
		public string ExtractedText { get; set; }
		public double Confidence { get; set; }
		public int CharCount { get; set; }
		public int WordCount { get; set; }
		public OCRValidationDTO Validation { get; set; }
		public string ErrorMessage { get; set; }
	}
}
