using Cryptomind.Common.Enums;
using Cryptomind.Data.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.CipherViewModels
{
	public class SubmitCipherViewModel
	{
		public string? Title { get; set; }
		public string? DecryptedText { get; set; }
		public string? EncryptedText { get; set; }
		public IFormFile? Image { get; set; }
		public CipherType? CipherType { get; set; } //Can just send null values instead of having a None value, because what is that cipher type - "none"
		public CipherDefinition CipherDefinition { get; set; }
	}
}
