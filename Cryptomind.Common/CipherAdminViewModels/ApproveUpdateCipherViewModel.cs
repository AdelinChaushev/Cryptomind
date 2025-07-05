using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.CipherAdminViewModels
{
	public class ApproveUpdateCipherViewModel
	{
		public string Title { get; set; }
		//public CipherType TypeOfCipher { get; set; }
		public bool AllowHint { get; set; }
		public bool AllowSolution { get; set; }
		public ICollection<Tag> Tags { get; set; }
	}
}
