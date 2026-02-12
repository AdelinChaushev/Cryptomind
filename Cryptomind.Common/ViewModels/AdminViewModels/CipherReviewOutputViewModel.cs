using Cryptomind.Common.ViewModels.CipherViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
    public class CipherReviewOutputViewModel : CipherOutputViewModel
    {
        public string DecryptedText { get; set; }
        public string Status { get; set; }
        public bool IsLLMRecommended { get; set; }
    }
}
