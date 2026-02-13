using Cryptomind.Common.ViewModels.CipherViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class CipherDetailedReviewOutputViewModel : CipherReviewOutputViewModel
	{
		public int Points { get; set; }
		public string CipherText { get; set; }
		public bool AllowFullSolution { get; set; }
		public bool AllowType { get; set; }
		public bool AllowHint { get; set; }
		public string ImageBase64 { get; set; }
	}
}
