using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.CipherViewModels
{
	public class CipherOutputViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string CipherText { get; set; }
		public int Points { get; set; }
		public int SolvedUsersCount { get; set; }
		public bool AllowsAnswer { get; set; }
		public bool AllowsHint { get; set; }
		public string ChallengeTypeDisplay { get; set; }
		public bool IsImage { get; set; }
		public string ImageBase64 { get; set; } //For image
		public bool AllowsTypeHint { get; set; }
		public bool AllowsSolutionHint { get; set; }
		public bool AllowsFullSolution { get; set; }
		public bool TypeHintUsed { get; set; }
		public bool SolutionHintUsed { get; set; }
		public bool FullSolutionUsed { get; set; }
		public List<HintData> PreviousHints { get; set; } = new List<HintData>();
		public bool AlreadySolved { get; set; }
	}

	public class HintData
	{
		public HintType Type { get; set; }
		public string Content { get; set; }
		public DateTime RequestedAt { get; set; }
	}
}
