using Cryptomind.Common.ViewModels.UserViewModels;

namespace Cryptomind.Core.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<string>> GetRolesUsers(string id);
        Task<AccountViewModel?> GetUserAccountInfo(string id);
    }
}
