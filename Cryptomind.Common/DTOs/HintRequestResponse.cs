using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.DTOs
{
	public class HintRequestResponse
	{
		public string? DecryptedText { get; set; }
		public string Explanataion { get; set; }
		public bool isSuccess { get; set; }
		public HintType HintType { get; set; }
	}
}
