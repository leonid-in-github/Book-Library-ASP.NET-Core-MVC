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

namespace BookLibrary.WebServer.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly IBookLibraryRepository repository;

        public BooksController(IBookLibraryRepository repository)
        {
            this.repository = repository;
        }

        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(AddBookModel book)
        {
            if (ModelState.IsValid)
            {
                repository.Books.AddBook(book);

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult DeleteBook(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            repository.Books.DeleteBook(bookId);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult EditBook(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            var book = new EditBookModel(repository.Books.GetBook(bookId));
            book.ID = bookId;
            return View(book);

        }

        [HttpPost]
        public IActionResult EditBook(EditBookModel book)
        {
            if (ModelState.IsValid)
            {
                repository.Books.UpdateBook(book);

                return RedirectToAction("Index", "Home");
            }
            return View(book);
        }

        public IActionResult BookTrack(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                var tracksCount =
                    Request.Cookies["BookTrackTableSelectedMode"] == null ? BookTrackTableModes.Default : Request.Cookies["BookTrackTableSelectedMode"].ToString();
                var bookTrackModel = (BookTrackModel)repository.Books.GetBookTrack(userId, (int)bookId, tracksCount);

                return View(bookTrackModel);
            }
            return new EmptyResult();
        }

        public IActionResult TakeBook(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                repository.Books.TakeBook(userId, bookId);
            }

            return RedirectToAction("BookTrack", "Books", new RouteValueDictionary(new { bookId = bookId }));
        }

        public IActionResult PutBook(int bookId)
        {
            if (bookId < 0)
            {
                return BadRequest();
            }

            if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                repository.Books.PutBook(userId, bookId);
            }

            return RedirectToAction("BookTrack", "Books", new RouteValueDictionary(new { bookId }));
        }

        public IActionResult MainBooksTableAjaxHandler(JQueryDataTableParamModel param)
        {
            Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            var booksList = GetBooksList(userId, param.sSearch, param.iDisplayStart, param.iDisplayLength);
            var booksTotalCount = GetBooksTotalCount(userId, param.sSearch);

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
                orderingFunction = (a => a.Authors);
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
                sEcho = param.sEcho,
                iTotalRecords = booksTotalCount,
                iTotalDisplayRecords = booksTotalCount,
                aaData = result
            });
        }

        #region Private

        private IEnumerable<Book> GetBooksList(int userId, string searchString, int from, int count)
        {
            return Request.Cookies["TableSelectedMode"]?.ToString() switch
            {
                "all" => repository.Books.GetBooks(searchString, from, count),
                "avaliable" => repository.Books.GetAvaliableBooks(searchString, from, count),
                "takenByUser" => repository.Books.GetBooksByUser(userId, searchString, from, count),
                _ => repository.Books.GetBooks(searchString, from, count)
            };
        }

        private int GetBooksTotalCount(int userId, string searchString)
        {
            return Request.Cookies["TableSelectedMode"]?.ToString() switch
            {
                "all" => repository.Books.GetBooksTotalCount(searchString),
                "avaliable" => repository.Books.GetAvaliableBooksTotalCount(searchString),
                "takenByUser" => repository.Books.GetBooksByUserTotalCount(userId, searchString),
                _ => repository.Books.GetBooksTotalCount(searchString)
            };
        }

        #endregion
    }
}