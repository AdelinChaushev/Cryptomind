using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IBadgeService
	{
		Task CheckBadgesByCategory(string userId, BadgeCategory category);
	}
}
