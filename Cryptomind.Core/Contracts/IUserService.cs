using Cryptomind.Common.ViewModels.UserViewModels;
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
        Task<IEnumerable<string>> GetRolesUsers(string id);
        Task<AccountViewModel?> GetUserAccountInfo(string id);

    }
}
