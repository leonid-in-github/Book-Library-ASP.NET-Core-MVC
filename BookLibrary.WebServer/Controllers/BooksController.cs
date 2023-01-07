using BookLibrary.Repository.Models.Book;
using BookLibrary.Repository.Repositories;
using BookLibrary.WebServer.Models.Books;
using BookLibrary.WebServer.Models.JQueryModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace BookLibrary.WebServer.Controllers
{
    public class BooksController : Controller
    {
        private readonly IDataStore dataStore;

        public BooksController(IDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public IActionResult AddBook()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult AddBook(AddBookModel book)
        {
            if (ModelState.IsValid)
            {
                dataStore.Books.AddBook(book);

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult DeleteBook(int bookId)
        {
            if (bookId >= 0)
            {
                try
                {
                    dataStore.Books.DeleteBook(bookId);
                }
                catch (Exception) { }
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult EditBook(int bookId)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            if (bookId >= 0)
            {
                try
                {
                    var book = new UpdateBookModel(dataStore.Books.GetBook(bookId));
                    book.ID = bookId;
                    return View(book);
                }
                catch (Exception) { }
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public IActionResult EditBook(UpdateBookModel book)
        {
            if (ModelState.IsValid)
            {
                dataStore.Books.UpdateBook(book);

                return RedirectToAction("Index", "Home");
            }
            return View(book);
        }

        public IActionResult BookTrack(int? bookId)
        {
            if (bookId == null || !User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            if (bookId >= 0)
            {
                try
                {
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        var tracksCount =
                            Request.Cookies["BookTrackTableSelectedMode"] == null ? BookTrackTableModes.Default : Request.Cookies["BookTrackTableSelectedMode"].ToString();
                        var bookTrackModel = (BookTrackModel)dataStore.Books.GetBookTrack(
                        aId, (int)bookId, tracksCount);

                        return View(bookTrackModel);
                    }
                }
                catch (Exception) { }
            }
            return new EmptyResult();
        }

        public IActionResult TakeBook(int? bookId)
        {
            if (bookId == null || !User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            if (bookId >= 0)
            {
                try
                {
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        dataStore.Books.TakeBook(aId, bookId);
                    }
                }
                catch (Exception) { }
            }

            return RedirectToAction("BookTrack", "Books", new RouteValueDictionary(new { bookId = bookId }));
        }

        public IActionResult PutBook(int? bookId)
        {
            if (bookId == null || !User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");

            if (bookId >= 0)
            {
                try
                {
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        dataStore.Books.PutBook(aId, bookId);
                    }
                }
                catch (Exception) { }
            }

            return RedirectToAction("BookTrack", "Books", new RouteValueDictionary(new { bookId = bookId }));
        }

        public IActionResult MainBooksTableAjaxHandler(JQueryDataTableParamModel param)
        {
            var booksList = GetBooksList(param.sSearch, param.iDisplayStart, param.iDisplayLength);

            var isNameSortable = Convert.ToBoolean(Request.Query["bSortable_0"]);
            var isAuthorsSortable = Convert.ToBoolean(Request.Query["bSortable_1"]);
            var isYearSortable = Convert.ToBoolean(Request.Query["bSortable_2"]);
            var isAvailabilitySortable = Convert.ToBoolean(Request.Query["bSortable_3"]);
            var isIdSortable = Convert.ToBoolean(Request.Query["bSortable_4"]);

            var sortColumnIndex = Convert.ToInt32(Request.Query["iSortCol_0"]);


            var sortDirection = Request.Query["sSortDir_0"]; // asc or desc
            if (sortColumnIndex == 0 && isNameSortable)
            {
                Func<BookItem, string> orderingFunction;
                orderingFunction = (a => a.Name);

                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction, StringComparer.Ordinal);
                else
                    booksList = booksList.OrderByDescending(orderingFunction, StringComparer.Ordinal);
            }
            else if (sortColumnIndex == 1 && isAuthorsSortable)
            {
                Func<BookItem, string> orderingFunction;
                orderingFunction = (a => a.Authors);
                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction, StringComparer.Ordinal);
                else
                    booksList = booksList.OrderByDescending(orderingFunction, StringComparer.Ordinal);
            }
            else if (sortColumnIndex == 2 && isYearSortable)
            {
                Func<BookItem, DateTime> orderingFunction;
                orderingFunction = (a => a.Year);
                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction);
                else
                    booksList = booksList.OrderByDescending(orderingFunction);
            }
            else if (sortColumnIndex == 3 && isAvailabilitySortable)
            {
                Func<BookItem, bool?> orderingFunction;
                orderingFunction = (a => a.Availability);
                if (sortDirection == "asc")
                    booksList = booksList.OrderBy(orderingFunction);
                else
                    booksList = booksList.OrderByDescending(orderingFunction);
            }
            else if (sortColumnIndex == 4 && isIdSortable)
            {
                Func<BookItem, int?> orderingFunction;
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
                iTotalRecords = booksList.Count(),
                iTotalDisplayRecords = booksList.Count(),
                aaData = result
            });
        }

        private IEnumerable<BookItem> GetBooksList(string searchString, int from, int count)
        {
            Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId);
            return Request.Cookies["TableSelectedMode"]?.ToString() switch
            {
                "all" => dataStore.Books.GetBooks(searchString, from, count),
                "avaliable" => dataStore.Books.GetAvaliableBooks(searchString, from, count),
                "takenByUser" => dataStore.Books.GetBooksByUser(aId, searchString, from, count),
                _ => dataStore.Books.GetBooks(searchString, from, count)
            };
        }
    }
}