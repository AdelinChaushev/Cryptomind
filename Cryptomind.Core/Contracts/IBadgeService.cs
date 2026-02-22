using Cryptomind.Data.Enums;

namespace Cryptomind.Core.Contracts
{
	public interface IBadgeService
	{
		Task CheckBadgesByCategory(string userId, BadgeCategory category);
	}
}
