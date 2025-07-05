using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Data.Entities
{
	public class UserSolution
	{
		[ForeignKey(nameof(Cipher))]
		public int CipherId { get; set; }
		public Cipher Cipher { get; set; }
		[ForeignKey(nameof(ApplicationUser))]
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
		public DateTime TimeSolved { get; set; }
	}
}
