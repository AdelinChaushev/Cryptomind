using Cryptomind.Data.Entities;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.CipherViewModels
{
	public class CipherOutputViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public bool IsImage { get; set; }
		public string ChallengeTypeDisplay { get; set; }
		public bool AlreadySolved { get; set; }
	}
}
