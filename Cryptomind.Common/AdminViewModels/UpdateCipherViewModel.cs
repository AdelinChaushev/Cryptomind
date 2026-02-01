using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.AdminViewModels
{
	public class UpdateCipherViewModel
	{
		public string Title { get; set; }
		public bool AllowHint { get; set; }
		public bool AllowSolution { get; set; }
		public ICollection<int>? TagIds { get; set; }
	}
}
