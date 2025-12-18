using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.DTOs
{
	public class SolveCipherDTO
	{
		public string UserSolution { get; set; }
		public bool UsedAiHint { get; set; }  // Track if they used AI
	}
}
