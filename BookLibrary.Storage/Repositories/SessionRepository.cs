using BookLibrary.Storage.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Storage.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private TimeSpan SessionExpirationTimeSpan { get; }

        public SessionRepository()
        {
            SessionExpirationTimeSpan =
                new TimeSpan(TimeSpan.TicksPerMinute * StorageParameters.SessionTimeoutInMinutes);
        }

        public Task<bool> RegisterSession(Guid accountId, string sessionId)
        {
            using var dbContext = new BookLibraryContext();

            var sessionRecord = dbContext.Sessions.FirstOrDefault(record => record.SessionId == sessionId);
            if (sessionRecord == null)
            {
                sessionRecord = new Models.Records.Account.SessionRecord
                {
                    AccountId = accountId,
                    SessionId = sessionId,
                    OpenDate = DateTime.UtcNow
                };
                dbContext.Sessions.Add(sessionRecord);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> CloseSession(string sessionId)
        {
            using var dbContext = new BookLibraryContext();

            var sessionRecord = dbContext.Sessions.FirstOrDefault(record => record.SessionId == sessionId);
            if (sessionRecord != null)
            {
                sessionRecord.CloseDate = DateTime.UtcNow;
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool?> CheckSessionExpiration(string sessionId)
        {
            using var dbContext = new BookLibraryContext();

            var sessionRecord = dbContext.Sessions.FirstOrDefault(record => record.SessionId == sessionId);
            if (sessionRecord != null)
            {
                DateTime sessionLastRenewalDate = sessionRecord.CloseDate == null ? sessionRecord.LastRenewalDate ?? sessionRecord.OpenDate : DateTime.MinValue;
                if (DateTime.UtcNow - sessionLastRenewalDate > SessionExpirationTimeSpan)
                {
                    return Task.FromResult<bool?>(true);
                }
                else
                {
                    ContinueSession(sessionId);
                    return Task.FromResult<bool?>(false);
                }
            }

            return Task.FromResult<bool?>(null);
        }

        private static Task<bool> ContinueSession(string sessionId)
        {
            using var dbContext = new BookLibraryContext();

            var sessionRecord = dbContext.Sessions.FirstOrDefault(record => record.SessionId == sessionId);
            if (sessionRecord != null)
            {
                sessionRecord.LastRenewalDate = DateTime.UtcNow;
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
