using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IHintService
	{
		Task<string> RequestHintAsync(string userId, int cipherId, HintType hintType);
	}
}
