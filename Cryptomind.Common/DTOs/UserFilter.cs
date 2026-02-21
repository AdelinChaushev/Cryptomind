using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.DTOs
{
	public class UserFilter
	{
		public bool? IsBanned { get; set; }
		public bool? IsDeactivated { get; set; }

        public string? Username { get; set; }
    }
}
