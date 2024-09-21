using BookLibrary.Storage.Repositories;
using BookLibrary.WebServer.Models.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookLibrary.WebServer.Controllers
{
    public class BooksController(IBooksRepository booksRepository) : Controller
    {
        private readonly IBooksRepository booksRepository = booksRepository;

        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(AddBookModel addBookModel)
        {
            if (ModelState.IsValid)
            {
                var book = addBookModel.ToDomain();
                await booksRepository.AddBook(book);

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<IActionResult> DeleteBook(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            await booksRepository.DeleteBook(bookId);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> EditBook(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            var book = new EditBookModel(await booksRepository.GetBook(bookId))
            {
                ID = bookId
            };
            return View(book);

        }

        [HttpPost]
        public async Task<IActionResult> EditBook(EditBookModel editBookModel)
        {
            var book = editBookModel.ToDomain();
            if (ModelState.IsValid)
            {
                await booksRepository.UpdateBook(book);

                return RedirectToAction("Index", "Home");
            }
            return View(book);
        }

        public async Task<IActionResult> BookTrack(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                var tracksCount =
                    Request.Cookies["BookTrackTableSelectedMode"] == null ? BookTrackTableModes.Default : Request.Cookies["BookTrackTableSelectedMode"].ToString();
                var bookTrackModel = (BookTrackModel)await booksRepository.GetBookTrack(userId, (int)bookId, tracksCount);

                return View(bookTrackModel);
            }
            return new EmptyResult();
        }

        public async Task<IActionResult> TakeBook(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                await booksRepository.TakeBook(userId, bookId);
            }

            return RedirectToAction("BookTrack", "Books", new RouteValueDictionary(new { bookId }));
        }

        public async Task<IActionResult> PutBook(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                await booksRepository.PutBook(userId, bookId);
            }

            return RedirectToAction("BookTrack", "Books", new RouteValueDictionary(new { bookId }));
        }
    }
}
