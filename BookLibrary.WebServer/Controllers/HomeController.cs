using BookLibrary.WebServer.Models;
using BookLibrary.WebServer.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookLibrary.WebServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var viewmodel = new IndexModel();
            return View(viewmodel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
