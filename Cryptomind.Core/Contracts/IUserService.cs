using Cryptomind.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
    public interface IUserService
    {
        Task<ApplicationUser> Authenticate(string email, string password);
        string GenerateJSONWebToken(ApplicationUser user);
        Task<ApplicationUser> CreateUser(string userName, string email, string password);
        Task<IEnumerable<string>> GetRolesUsers(string id);
        Task RemoveUserFromRole(string userId, string role);
        Task AddUserToRole(string userId, string role);
        Task DeactivateAccount(string userId);

    }
}
