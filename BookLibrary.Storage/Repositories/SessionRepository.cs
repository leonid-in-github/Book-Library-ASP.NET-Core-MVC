using BookLibrary.Repository.Repositories;
using BookLibrary.Storage.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookLibrary.Storage.Repositories
{
    public class SessionRepository
    {
        private TimeSpan SessionExpirationTimeSpan { get; }

        public SessionRepository()
        {
            SessionExpirationTimeSpan =
                new TimeSpan(TimeSpan.TicksPerMinute * RepositoryParameters.SESSIONEXPIRATIONTIMEINMINUTES);
        }

        public bool RegisterSession(int accountId, string sessionId)
        {
            using (var dbContext = new BookLibraryContext())
            {
                var inAccountId = new SqlParameter
                {
                    ParameterName = "AccountId",
                    Value = accountId,
                    DbType = System.Data.DbType.Int32,
                    Direction = System.Data.ParameterDirection.Input
                };
                var inOpenDate = new SqlParameter
                {
                    ParameterName = "OpenDate",
                    Value = DateTime.Now,
                    DbType = System.Data.DbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input
                };
                var inSessionId = new SqlParameter
                {
                    ParameterName = "SessionId",
                    Value = sessionId,
                    DbType = System.Data.DbType.String,
                    Direction = System.Data.ParameterDirection.Input
                };
                var outResult = new SqlParameter
                {
                    ParameterName = "Result",
                    DbType = System.Data.DbType.Int32,
                    Direction = System.Data.ParameterDirection.Output
                };

                var sql = "exec OpenSession @AccountId, @OpenDate, @SessionId, @Result OUT";
                _ = dbContext.Database.ExecuteSqlRaw(sql, inAccountId, inOpenDate, inSessionId, outResult);

                if (int.TryParse(outResult.Value.ToString(), out int openSessionResult))
                    if (openSessionResult == 1)
                    {
                        return true;
                    }
            }
            return false;
        }

        public bool CloseSession(string sessionId)
        {
            using (var dbContext = new BookLibraryContext())
            {
                var inSessionId = new SqlParameter
                {
                    ParameterName = "SessionId",
                    Value = sessionId,
                    DbType = System.Data.DbType.String,
                    Direction = System.Data.ParameterDirection.Input
                };
                var inCloseDate = new SqlParameter
                {
                    ParameterName = "CloseDate",
                    Value = DateTime.Now,
                    DbType = System.Data.DbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input
                };
                var outResult = new SqlParameter
                {
                    ParameterName = "Result",
                    DbType = System.Data.DbType.Int32,
                    Direction = System.Data.ParameterDirection.Output
                };

                var sql = "exec CloseSession @SessionId, @CloseDate, @Result OUT";
                _ = dbContext.Database.ExecuteSqlRaw(sql, inSessionId, inCloseDate, outResult);

                if (int.TryParse(outResult.Value.ToString(), out int spResult))
                    if (spResult == 1)
                    {
                        return true;
                    }
            }
            return false;
        }

        public bool? CheckSessionExpiration(string sessionId)
        {
            using (var dbContext = new BookLibraryContext())
            {
                var inSessionId = new SqlParameter
                {
                    ParameterName = "SessionId",
                    Value = sessionId,
                    DbType = System.Data.DbType.String,
                    Direction = System.Data.ParameterDirection.Input
                };
                var outResult = new SqlParameter
                {
                    ParameterName = "Result",
                    DbType = System.Data.DbType.DateTime,
                    Direction = System.Data.ParameterDirection.Output
                };

                var sql = "exec GetSessionLastRenewalDate @SessionId, @Result OUT";
                _ = dbContext.Database.ExecuteSqlRaw(sql, inSessionId, outResult);

                if (DateTime.TryParse(outResult.Value.ToString(), out DateTime spDateTimeResult))
                {
                    if (DateTime.Now - spDateTimeResult > SessionExpirationTimeSpan)
                    {
                        return true;
                    }
                    else
                    {
                        ContinueSession(sessionId);
                    }
                }
                else
                    return null;
            }
            return false;
        }

        private bool ContinueSession(string sessionId)
        {
            using (var dbContext = new BookLibraryContext())
            {
                var inSessionId = new SqlParameter
                {
                    ParameterName = "SessionId",
                    Value = sessionId,
                    DbType = System.Data.DbType.String,
                    Direction = System.Data.ParameterDirection.Input
                };
                var inRenewDate = new SqlParameter
                {
                    ParameterName = "RenewDate",
                    Value = DateTime.Now,
                    DbType = System.Data.DbType.DateTime,
                    Direction = System.Data.ParameterDirection.Input
                };
                var outResult = new SqlParameter
                {
                    ParameterName = "Result",
                    DbType = System.Data.DbType.Int32,
                    Direction = System.Data.ParameterDirection.Output
                };

                var sql = "exec RenewSession @SessionId, @RenewDate, @Result OUT";
                _ = dbContext.Database.ExecuteSqlRaw(sql, inSessionId, inRenewDate, outResult);

                if (int.TryParse(outResult.Value.ToString(), out int spResult))
                    if (spResult == 1)
                    {
                        return true;
                    }
            }
            return false;
        }
    }
}
