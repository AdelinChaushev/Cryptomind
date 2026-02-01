using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.CipherViewModels
{
    public class CipherReviewOutputViewModel : CipherOutputViewModel
    {
        public string DecryptedText { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLLMRecommended { get; set; }
    }
}
