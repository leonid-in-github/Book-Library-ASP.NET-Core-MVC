using System;
using System.Threading.Tasks;

namespace BookLibrary.Storage.Repositories
{
    public interface ISessionRepository
    {
        Task<bool?> CheckSessionExpiration(string sessionId);
        Task<bool> CloseSession(string sessionId);
        Task<bool> RegisterSession(Guid accountId, string sessionId);
    }
}