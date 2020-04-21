using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Library_EF_Core_Proxy_Class_Library.Adapters.Interfaces
{
    internal interface ISessionAdapter
    {
        bool RegisterSession(int accountId, string sessionId);

        bool CloseSession(string sessionId);

        bool? CheckSessionExpiration(string sessionId);
    }
}
