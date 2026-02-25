using Cryptomind.Common.DTOs;
using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Contracts
{
	public interface IHintService
	{
		Task<HintResultDTO> RequestHintAsync(string userId, int cipherId, HintType hintType);
	}
}
