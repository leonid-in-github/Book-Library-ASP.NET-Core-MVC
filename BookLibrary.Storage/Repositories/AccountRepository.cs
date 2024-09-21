using BookLibrary.Storage.Contexts;
using BookLibrary.Storage.Exceptions;
using BookLibrary.Storage.Models.Account;
using BookLibrary.Storage.Models.Records.Account;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Storage.Repositories
{
    public class AccountRepository(ISessionRepository sessionRepository) : IAccountRepository
    {
        private readonly ISessionRepository sessionRepository = sessionRepository;

        public async Task<int> Login(string sessionId, string login, string password)
        {
            using var dbContext = new BookLibraryContext();
            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.Login == login && record.Password == password);
            if (accountRecord != null)
            {
                switch (await sessionRepository.CheckSessionExpiration(sessionId))
                {
                    case null:
                        if (!await sessionRepository.RegisterSession(accountRecord.ID, sessionId))
                            return 0;
                        break;
                    case true:
                        throw new SessionExpiredException("Application and DB session expiration time conflict.");
                }
                return accountRecord.ID;
            }

            return 0;
        }

        public async Task<bool> Logout(string sessionId)
        {
            return await sessionRepository.CloseSession(sessionId);
        }

        public async Task<int> Register(string sessionId, string login, string password, string firstName, string lastName, string email)
        {
            using var dbContext = new BookLibraryContext();
            using var transaction = dbContext.Database.BeginTransaction();

            var profileRecord = dbContext.Profiles.FirstOrDefault(record => record.FirstName == firstName && record.LastName == lastName && record.Email == email);
            if (profileRecord == null)
            {
                profileRecord = new ProfileRecord
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };
                dbContext.Profiles.Add(profileRecord);
                dbContext.SaveChanges();
            }

            var accountRecord = new AccountRecord
            {
                Login = login,
                Password = password,
                ProfileId = profileRecord.ID,
                ID = dbContext.Accounts.Max(record => record.ID) + 1
            };

            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Accounts] ON");
            dbContext.Accounts.Add(accountRecord);
            dbContext.SaveChanges();
            dbContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Accounts] OFF");
            transaction.Commit();

            switch (await sessionRepository.CheckSessionExpiration(sessionId))
            {
                case null:
                    if (!await sessionRepository.RegisterSession(accountRecord.ID, sessionId))
                        return 0;
                    break;
                case true:
                    throw new SessionExpiredException("Application and DB session expiration time conflict.");
            }
            return accountRecord.ID;
        }

        public Task<User> GetUser(int userId)
        {
            using var dbContext = new BookLibraryContext();

            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.ID == userId);
            if (accountRecord != null)
            {
                var profileRecord = dbContext.Profiles.FirstOrDefault(record => record.ID == accountRecord.ProfileId);
                if (profileRecord != null)
                {
                    return Task.FromResult(User.FromPersistence(
                        accountRecord.Login,
                        profileRecord.FirstName,
                        profileRecord.LastName,
                        profileRecord.Email
                    ));
                }
            }

            return Task.FromResult<User>(null);
        }

        public Task<bool> ChangeAccountPassword(int accountId, string accountPassword, string newAccountPassword)
        {
            using var dbContext = new BookLibraryContext();

            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.ID == accountId);
            if (accountRecord != null && accountRecord.Password == accountPassword)
            {
                accountRecord.Password = newAccountPassword;
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> DeleteAccount(int accountId, string accountPassword)
        {
            using var dbContext = new BookLibraryContext();

            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.ID == accountId);
            if (accountRecord != null && accountRecord.Password == accountPassword)
            {
                dbContext.Accounts.Remove(accountRecord);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
