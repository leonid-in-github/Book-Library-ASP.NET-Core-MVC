using BookLibrary.Storage.Models.Book;
using BookLibrary.Storage.Repositories;
using BookLibrary.WebServer.Models.Books;
using BookLibrary.WebServer.Models.JQueryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookLibrary.WebServer.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> BooksTableAjaxHandler([ModelBinder(BinderType = typeof(DataTableParametersBinder))]
            DataTableParameters parameters)
        {
            _ = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            var orderColumnName = parameters.Order.FirstOrDefault()?.Name;
            var orderColumnIndex = parameters.Order.FirstOrDefault()?.Column;
            var orderDirection = parameters.Order.FirstOrDefault()?.Dir?.ToLower();
            var isColumnOrderable = parameters.Columns.Find(column => column.Data == orderColumnIndex)?.Orderable;
            var booksList = await GetBooksList(
                userId,
                parameters.Search.Value,
                parameters.Start,
                parameters.Length,
                isColumnOrderable == true ? orderColumnName : null,
                orderDirection);
            var booksTotalCount = await GetBooksTotalCount(userId, parameters.Search.Value);
            var result = from a in booksList
                         select new ArrayList {

                            a.Name,
                            a.Authors,
                            a.Year.ToString("MM/dd/yyyy hh:mm tt"),
                            a.Availability.ToString(),
                            a.ID.ToString()
                        };

            return Json(new
            {
                draw = parameters.Draw,
                recordsTotal = booksTotalCount,
                recordsFiltered = booksTotalCount,
                data = result
            });
        }

        #region Private

        private async Task<IEnumerable<Book>> GetBooksList(
            int userId,
            string searchString,
            int from,
            int count,
            string orderColumnName = null,
            string orderDirection = "asc")
        {
            return Request.Cookies["TableSelectedMode"]?.ToString() switch
            {
                "all" => await booksRepository.GetBooks(searchString, false, -1, from, count, orderColumnName, orderDirection),
                "available" => await booksRepository.GetAvailableBooks(searchString, from, count, orderColumnName, orderDirection),
                "takenByUser" => await booksRepository.GetBooksByUser(userId, searchString, from, count, orderColumnName, orderDirection),
                _ => await booksRepository.GetBooks(searchString, false, -1, from, count, orderColumnName, orderDirection)
            };
        }

        private async Task<int> GetBooksTotalCount(int userId, string searchString)
        {
            return Request.Cookies["TableSelectedMode"]?.ToString() switch
            {
                "all" => await booksRepository.GetBooksTotalCount(searchString),
                "available" => await booksRepository.GetAvailableBooksTotalCount(searchString),
                "takenByUser" => await booksRepository.GetBooksByUserTotalCount(userId, searchString),
                _ => await booksRepository.GetBooksTotalCount(searchString)
            };
        }

        #endregion
    }
}