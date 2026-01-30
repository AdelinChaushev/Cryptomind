using Cryptomind.Common.Enums;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Http;
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
		public string? EncryptedText { get; set; }
		public IFormFile? Image { get; set; }
		public CipherTypeDTO Type { get; set; }
		public CipherDefinition CipherDefinition { get; set; }
	}
}
