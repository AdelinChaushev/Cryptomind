using Cryptomind.Common.ViewModels.EnglishValidationModels;

namespace Cryptomind.Core.Contracts
{
	public interface IEnglishValidationService
	{
		Task<EnglishValidationResult> ValidatePlaintextAsync(string plaintext);
		Task<bool> IsLikelyEnglishAsync(string plaintext, double minConfidence = 0.5);
	}
}
