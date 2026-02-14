using Cryptomind.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
