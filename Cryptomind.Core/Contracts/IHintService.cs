using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Contracts
{
	public interface IHintService
	{
		Task<string> RequestHintAsync(string userId, int cipherId, HintType hintType);
	}
}
