using BookLibrary.Storage.Models.Account;
using System;
using System.Threading.Tasks;

namespace BookLibrary.Storage.Repositories
{
    public interface IAccountRepository
    {
        Task<bool> ChangeAccountPassword(Guid accountId, string accountPassword, string newAccountPassword);
        Task<bool> DeleteAccount(Guid accountId, string accountPassword);
        Task<User> GetUser(Guid userId);
        Task<Guid?> Login(string sessionId, string login, string password);
        Task<bool> Logout(string sessionId);
        Task<Guid?> Register(string sessionId, string login, string password, string firstName, string lastName, string email);
    }
}