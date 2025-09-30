using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.CipherViewModels
{
    public class CipherOutputViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
       
        public string CipherText { get; set; }
        public int Points { get; set; }
        public bool AllowsHint { get; set; }

        public bool AllowsAnswer { get; set; }
        public bool IsApproved { get; set; }

        public bool IsImage { get; set; }
    }
}
