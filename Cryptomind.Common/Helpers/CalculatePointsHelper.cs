namespace Cryptomind.Common.Helpers
{
	public static class CalculatePointsHelper
	{
		private const double TypeHintPenalty = 0.30;
		private const double SolutionHintPenalty = 0.50;
		private const double FullSolutionHintPenalty = 0.95;
		public static int CalculateAvailablePointsWithPenalty(
			int basePoints,
			bool usedTypeHint,
			bool usedSolutionHint,
			bool usedFullSolution)
		{
			double multiplier = 1.0;

			if (usedTypeHint)
				multiplier -= TypeHintPenalty; //-30%

			if (usedSolutionHint)
				multiplier -= SolutionHintPenalty; //-50%

			if (usedFullSolution)
				multiplier -= FullSolutionHintPenalty; //-95%

			if (multiplier < 0.05)
				multiplier = 0.05;

			return (int)Math.Ceiling(Math.Max(0, basePoints * multiplier));
		}
	}
}
