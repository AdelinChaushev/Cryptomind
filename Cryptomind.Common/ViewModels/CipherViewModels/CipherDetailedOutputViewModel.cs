using Cryptomind.Common.ViewModels.UserViewModels;
using Cryptomind.Data.Enums;

namespace Cryptomind.Common.ViewModels.CipherViewModels
{
	public class CipherDetailedOutputViewModel : CipherOutputViewModel
	{
		public CipherDetailedOutputViewModel()
		{
			PreviousHints = new List<HintData>();
		}
		public string CipherText { get; set; }
		public int Points { get; set; }
		public int SolvedUsersCount { get; set; }
		public int TimesSolved { get; set; }
		public string DateSubmitted { get; set; }
		public double SuccessRate { get; set; }
		public bool AllowsAnswer { get; set; }
		public bool AllowsHint { get; set; }
		public string ImageBase64 { get; set; }
		public bool AllowsTypeHint { get; set; }
		public bool AllowsSolutionHint { get; set; }
		public bool AllowsFullSolution { get; set; }
		public bool TypeHintUsed { get; set; }
		public bool SolutionHintUsed { get; set; }
		public bool FullSolutionUsed { get; set; }

		public int AllSubmissions { get; set; }
		public int SuccessfulSubmissions { get; set; }
		public List<CipherSolverViewModel> RecentSolvers { get; set; }
		public List<HintData> PreviousHints { get; set; }
		public List<string> Tags { get; set; }
	}

	public class HintData
	{
		public HintType Type { get; set; }
		public string Content { get; set; }
		public DateTime RequestedAt { get; set; }
	}
}
