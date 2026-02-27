namespace Cryptomind.Data.Entities
{
	public class ImageCipher : Cipher
	{
		public string ImagePath { get; set; }
		public double OCRConfidence { get; set; }
	}
}
