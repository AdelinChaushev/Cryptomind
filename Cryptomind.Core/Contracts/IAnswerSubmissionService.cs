using Cryptomind.Common.DTOs;
using Cryptomind.Common.Enums;
using Cryptomind.Common.ViewModels.AdminViewModels;
using Cryptomind.Common.ViewModels.AnswerSubmissionViewModels;
using Cryptomind.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IAnswerSubmissionService
	{
		Task SuggestAnswerAsync(SuggestAnswerDTO answer, string userId, int cipherId);
		Task<List<AnswerSubmissionViewModel>> SubmittedAnswers(string userId);
	}
}