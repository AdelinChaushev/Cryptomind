using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.CipherRecognitionViewModels
{
	public class CipherRecognitionResultViewModel
	{
		public PredictionViewModel TopPrediction { get; set; }
		public List<PredictionViewModel> AllPredictions { get; set; }
		public bool HasAlternatives => AllPredictions.Count > 1;
	}
}
