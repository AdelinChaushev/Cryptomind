using Cryptomind.Common.Enums;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.DTOs
{
	public class CipherFilter
	{
		public string? SearchTerm { get; set; }
		public List<TagType>? Tags { get; set; }
		public ChallengeType? ChallengeType { get; set; }
		public CipherDefinition? CipherDefinition { get; set; }
		public CipherOrderTerm? OrderTerm { get; set; }
	}
}
