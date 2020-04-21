using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Book_Library_ASP.NET_Core_MVC.Controllers
{
    public class BookLibraryController : Controller
    {
        public bool IsLoged
        {
            get
            {
                if(HttpContext.Session.TryGetValue("AccountLogin", out byte[] sLoginBArr))
                {
                    if (sLoginBArr == null) return false;
                    return sLoginBArr.Length > 0;
                }
                return false;
            }
        }
    }
}