using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.AdminViewModels
{
	public class UserCipherViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public CipherType TypeOfCipher { get; set; }
		public string Status { get; set; }
		public int Points { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? SolvedAt { get; set; }  // If in solved list
		public ChallengeType ChallengeType { get; set; }
	}
}
