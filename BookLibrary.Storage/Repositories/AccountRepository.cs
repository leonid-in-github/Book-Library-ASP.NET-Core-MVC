using BookLibrary.Storage.Contexts;
using BookLibrary.Storage.Exceptions;
using BookLibrary.Storage.Models.Account;
using BookLibrary.Storage.Models.Records.Account;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Storage.Repositories
{
    public class AccountRepository(ISessionRepository sessionRepository) : IAccountRepository
    {
        private readonly ISessionRepository sessionRepository = sessionRepository;

        public async Task<Guid?> Login(string sessionId, string login, string password)
        {
            using var dbContext = new BookLibraryContext();
            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.Login == login && record.Password == password);
            if (accountRecord != null)
            {
                switch (await sessionRepository.CheckSessionExpiration(sessionId))
                {
                    case null:
                        if (!await sessionRepository.RegisterSession(accountRecord.Id, sessionId))
                            return null;
                        break;
                    case true:
                        throw new SessionExpiredException("Application and DB session expiration time conflict.");
                }
                return accountRecord.Id;
            }

            return null;
        }

        public async Task<bool> Logout(string sessionId)
        {
            return await sessionRepository.CloseSession(sessionId);
        }

        public async Task<Guid?> Register(string sessionId, string login, string password, string firstName, string lastName, string email)
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
                ProfileId = profileRecord.Id,
                Id = Guid.NewGuid()
            };

            dbContext.Accounts.Add(accountRecord);
            dbContext.SaveChanges();
            transaction.Commit();

            switch (await sessionRepository.CheckSessionExpiration(sessionId))
            {
                case null:
                    if (!await sessionRepository.RegisterSession(accountRecord.Id, sessionId))
                        return null;
                    break;
                case true:
                    throw new SessionExpiredException("Application and DB session expiration time conflict.");
            }
            return accountRecord.Id;
        }

        public Task<User> GetUser(Guid userId)
        {
            using var dbContext = new BookLibraryContext();
            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.Id == userId);
            if (accountRecord != null)
            {
                var profileRecord = dbContext.Profiles.FirstOrDefault(record => record.Id == accountRecord.ProfileId);
                if (profileRecord != null)
                {
                    return Task.FromResult(User.FromPersistence(
                        accountRecord.Login,
                        profileRecord.Email,
                        profileRecord.FirstName,
                        profileRecord.LastName
                    ));
                }
            }

            return null;
        }

        public Task<bool> ChangeAccountPassword(Guid accountId, string accountPassword, string newAccountPassword)
        {
            using var dbContext = new BookLibraryContext();
            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.Id == accountId);
            if (accountRecord != null && accountRecord.Password == accountPassword)
            {
                accountRecord.Password = newAccountPassword;
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> DeleteAccount(Guid accountId, string accountPassword)
        {
            using var dbContext = new BookLibraryContext();
            var accountRecord = dbContext.Accounts.FirstOrDefault(record => record.Id == accountId);
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
