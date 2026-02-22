using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Badges
{
	public interface IBadgeCriteria
	{
		Task<bool> IsSatisfied(string userId);
		BadgeCategory Category { get; }
	}
}
