using Cryptomind.Common.ViewModels.EnglishValidationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IEnglishValidationService
	{
		Task<EnglishValidationResult> ValidatePlaintextAsync(string plaintext);
		Task<bool> IsLikelyEnglishAsync(string plaintext, double minConfidence = 0.5);
	}
}
