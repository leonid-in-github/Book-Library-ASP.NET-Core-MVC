using Book_Library_EF_Core_Proxy_Class_Library.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Library_EF_Core_Proxy_Class_Library.Proxy
{
    public static class dbBookLibraryProxy
    {
        public static AccountAdapter Account
        {
            get
            {
                return new AccountAdapter();
            }
        }

        public static BookAdapter Books
        {
            get
            {
                return new BookAdapter();
            }
        }
    }
}
