using System;

namespace BookLibrary.Storage.Exceptions
{
    [Serializable]
    public class SessionExpiredException(string message) : Exception(message)
    {
    }
}
