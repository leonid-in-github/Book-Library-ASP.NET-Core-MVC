using BookLibrary.Storage.Models.Book;
using BookLibrary.Storage.Repositories;
using BookLibrary.WebServer.Models.Books;
using BookLibrary.WebServer.Models.JQueryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
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

        public async Task<IActionResult> MainBooksTableAjaxHandler(JQueryDataTableParamModel param)
        {
            _ = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            var booksList = await GetBooksList(userId, param.sSearch, param.iDisplayStart, param.iDisplayLength);
            var booksTotalCount = await GetBooksTotalCount(userId, param.sSearch);

            var isNameSortable = Convert.ToBoolean(Request.Query["bSortable_0"]);
            var isAuthorsSortable = Convert.ToBoolean(Request.Query["bSortable_1"]);
            var isYearSortable = Convert.ToBoolean(Request.Query["bSortable_2"]);
            var isAvailabilitySortable = Convert.ToBoolean(Request.Query["bSortable_3"]);
            var isIdSortable = Convert.ToBoolean(Request.Query["bSortable_4"]);

            var sortColumnIndex = Convert.ToInt32(Request.Query["iSortCol_0"]);


            var sortDirection = Request.Query["sSortDir_0"]; // asc or desc
            if (sortColumnIndex == 0 && isNameSortable)
            {
                Func<Book, string> orderingFunction;
                orderingFunction = (a => a.Name);

                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction, StringComparer.Ordinal);
                else
                    booksList = booksList.OrderByDescending(orderingFunction, StringComparer.Ordinal);
            }
            else if (sortColumnIndex == 1 && isAuthorsSortable)
            {
                Func<Book, string> orderingFunction;
                orderingFunction = (a => string.Join(", ", a.Authors));
                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction, StringComparer.Ordinal);
                else
                    booksList = booksList.OrderByDescending(orderingFunction, StringComparer.Ordinal);
            }
            else if (sortColumnIndex == 2 && isYearSortable)
            {
                Func<Book, DateTime> orderingFunction;
                orderingFunction = (a => a.Year);
                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction);
                else
                    booksList = booksList.OrderByDescending(orderingFunction);
            }
            else if (sortColumnIndex == 3 && isAvailabilitySortable)
            {
                Func<Book, bool?> orderingFunction;
                orderingFunction = (a => a.Availability);
                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction);
                else
                    booksList = booksList.OrderByDescending(orderingFunction);
            }
            else if (sortColumnIndex == 4 && isIdSortable)
            {
                Func<Book, int?> orderingFunction;
                orderingFunction = (a => a.ID);
                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction);
                else
                    booksList = booksList.OrderByDescending(orderingFunction);
            }

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
                param.sEcho,
                iTotalRecords = booksTotalCount,
                iTotalDisplayRecords = booksTotalCount,
                aaData = result
            });
        }

        #region Private

        private async Task<IEnumerable<Book>> GetBooksList(int userId, string searchString, int from, int count)
        {
            return Request.Cookies["TableSelectedMode"]?.ToString() switch
            {
                "all" => await booksRepository.GetBooks(searchString, false, -1, from, count),
                "available" => await booksRepository.GetAvailableBooks(searchString, from, count),
                "takenByUser" => await booksRepository.GetBooksByUser(userId, searchString, from, count),
                _ => await booksRepository.GetBooks(searchString, false, -1, from, count)
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