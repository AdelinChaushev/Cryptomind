namespace Cryptomind.Common.ViewModels.AdminViewModels
{
	public class UpdateCipherViewModel
	{
		public string Title { get; set; }
		public bool AllowTypeHint { get; set; }
		public bool AllowHint { get; set; }
		public bool AllowSolution { get; set; }
		public ICollection<int>? TagIds { get; set; }
	}
}
