using BookLibrary.Storage.Models.Account;
using System.Threading.Tasks;

namespace BookLibrary.Storage.Repositories
{
    public interface IAccountRepository
    {
        Task<bool> ChangeAccountPassword(int accountId, string accountPassword, string newAccountPassword);
        Task<bool> DeleteAccount(int accountId, string accountPassword);
        Task<User> GetUser(int userId);
        Task<int> Login(string sessionId, string login, string password);
        Task<bool> Logout(string sessionId);
        Task<int> Register(string sessionId, string login, string password, string firstName, string lastName, string email);
    }
}