using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class AnswerSuggestionViewModel
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Description { get; set; }
		public int CipherId { get; set; }
	}
}
