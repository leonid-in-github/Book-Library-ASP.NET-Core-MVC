﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Library_EF_Core_Proxy_Class_Library.Constants
{
    public sealed class LibraryConstants
    {
        public const string CONNECTIONSTRING = @"Data Source=DESKTOP-3JLM130\SQLEXPRESS;Initial Catalog=test1;Integrated Security=True;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public const int SESSIONEXPIRATIONTIMEINMINUTES = 20;
    }
}
