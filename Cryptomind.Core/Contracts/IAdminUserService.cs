using Cryptomind.Common.DTOs;
using Cryptomind.Common.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IAdminUserService
	{
		Task<List<UserViewModel>> GetAllUsers(UserFilter? filter);
		Task<UserDetailViewModel> GetUser(string userId);
		Task MakeAdmin(string userId);
		Task BanUserAsync(string userId, string reason);
		Task UnbanUserAsync(string userId);
	}
}
