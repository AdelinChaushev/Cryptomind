using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.DTOs
{
	public class CipherDetailDTO
	{
		public int Id { get; set; }
		public string Title { get; set; }
		//public CipherType TypeOfCipher { get; set; }
		public string CreatedByUserId { get; set; }
	}
}
