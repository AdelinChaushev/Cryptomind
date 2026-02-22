namespace Cryptomind.Common.ViewModels.CipherRecognitionViewModels
{
	public class CipherRecognitionResultViewModel
	{
		public PredictionViewModel TopPrediction { get; set; }
		public List<PredictionViewModel> AllPredictions { get; set; }
		public bool HasAlternatives => AllPredictions.Count > 1;
	}
}
