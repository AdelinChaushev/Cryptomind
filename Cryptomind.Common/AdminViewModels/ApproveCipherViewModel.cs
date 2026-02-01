using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.CipherAdminViewModels
{
	public class ApproveCipherViewModel
	{
		public string Title { get; set; }
		public bool AllowHint { get; set; }
		public bool AllowSolution { get; set; }
		public bool AllowTypeHint { get; set; }
		public CipherType CipherType { get; set; }
		public ChallengeType ChallengeType { get; set; }
		public ICollection<int>? TagIds { get; set; }
	}
}
