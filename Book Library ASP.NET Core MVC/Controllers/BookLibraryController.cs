using Microsoft.AspNetCore.Mvc;

namespace Book_Library_ASP.NET_Core_MVC.Controllers
{
    public class BookLibraryController : Controller
    {
        public bool IsLoged
        {
            get
            {
                return User.Identity.IsAuthenticated;
            }
        }
    }
}