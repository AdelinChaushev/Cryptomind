using Cryptomind.Core.Contracts;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Badges
{
	public interface IBadgeCriteria
	{
		Task<bool> IsSatisfied(string userId);
		BadgeCategory Category { get; }
	}
}
