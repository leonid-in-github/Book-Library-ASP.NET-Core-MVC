using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.WebServer.Controllers
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