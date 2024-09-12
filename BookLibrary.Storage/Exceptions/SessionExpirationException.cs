using System;

namespace BookLibrary.Storage.Exceptions
{
    [Serializable]
    public class SessionExpirationException(string message) : Exception(message)
    {
    }
}
