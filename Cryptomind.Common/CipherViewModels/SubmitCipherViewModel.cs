using Cryptomind.Common.Enums;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.CipherViewModels
{
	public class SubmitCipherViewModel
	{
		public string Title { get; set; }
		public string DecryptedText { get; set; }
		public string EncryptedText { get; set; }
		public string ImagePath { get; set; }
		public CipherType Type { get; set; }
		public CipherDefinition CipherDefinition { get; set; }
	}
}
