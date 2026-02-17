using Cryptomind.Common.ViewModels.CipherViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
    public class CipherReviewOutputViewModel
    {
		public int Id { get; set; }
		public string Title { get; set; }
		public bool IsImage { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int PercentageOfConfidence { get; set; }
        public string MlPrediction { get; set; }
        public bool IsLLMRecommended { get; set; }
    }
}
