using Cryptomind.Data.Enums;

namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class ApproveCipherViewModel
	{
		public string Title { get; set; }
		public bool AllowTypeHint { get; set; }
		public bool AllowHint { get; set; }
		public bool AllowSolution { get; set; }
		public CipherType? TypeOfCipher { get; set; }
		public ICollection<int>? TagIds { get; set; }
	}
}
