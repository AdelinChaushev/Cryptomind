using Cryptomind.Data.Entities;

namespace Cryptomind.Core.Contracts
{
	public interface IAuthService
	{
		Task<ApplicationUser> Authenticate(string email, string password);
		Task<string> GenerateJSONWebToken(ApplicationUser user);
		Task<ApplicationUser> CreateUser(string userName, string email, string password);
		Task DeactivateAccount(string userId);
	}
}
