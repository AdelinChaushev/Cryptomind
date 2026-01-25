using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.DTOs
{
	public class OCRValidationDTO
	{
		public bool IsValid { get; set; }
		public List<string> Warnings { get; set; }
		public string Recommendation { get; set; }
	}
}
