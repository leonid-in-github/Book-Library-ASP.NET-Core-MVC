using System;

namespace BookLibrary.Storage.Exceptions
{
    [Serializable]
    public class SessionExpirationException : Exception
    {
        public SessionExpirationException(string message) : base(message) { }
    }
}
