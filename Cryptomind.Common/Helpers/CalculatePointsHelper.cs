using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Common.Helpers
{
	public static class CalculatePointsHelper
	{
		private const double TypeHintPenalty = 0.20;
		private const double SolutionHintPenalty = 0.30;
		private const double FullSolutionHintPenalty = 0.40;
		public static int CalculateAvailablePointsWithPenalty(
			int basePoints,
			bool usedTypeHint,
			bool usedSolutionHint,
			bool usedFullSolution)
		{
			double multiplier = 1.0;

			if (usedTypeHint)
				multiplier -= TypeHintPenalty; //-20%

			if (usedSolutionHint)
				multiplier -= SolutionHintPenalty; //-30%

			if (usedFullSolution)
				multiplier -= FullSolutionHintPenalty; //-40%

			return (int)Math.Max(0, basePoints * multiplier);
		}
	}
}
