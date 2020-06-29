using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Book_Library_ASP.NET_Core_MVC.Models.Books;
using Book_Library_ASP.NET_Core_MVC.Models.JQueryModels;
using Book_Library_Repository_EF_Core.Models.Book;
using Book_Library_Repository_EF_Core.Repositories;
using Book_Library_Repository_EF_Core.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace Book_Library_ASP.NET_Core_MVC.Controllers
{
    public class BooksController : BookLibraryController
    {
        private IDataStore DataStore => RepositoryService.Get<BookLibraryRepository>();

        public IActionResult AddBook()
        {
            if (!IsLoged) return RedirectToAction("Index", "Home");
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult AddBook(AddBookModel book)
        {
            if (ModelState.IsValid)
            {
                DataStore.Books.AddBook((BookItem)book);

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
                    DataStore.Books.DeleteBook(bookId);
                }
                catch (Exception) { }
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult EditBook(int bookId)
        {
            if (!IsLoged) return RedirectToAction("Index", "Home");

            if (bookId >= 0)
            {
                try
                {
                    var book = (UpdateBookModel)DataStore.Books.GetBook(bookId);
                    book.Id = bookId;
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
                DataStore.Books.UpdateBook((BookItem)book);

                return RedirectToAction("Index", "Home");
            }
            return View(book);
        }

        public IActionResult BookTrack(int? bookId)
        {
            if (bookId == null || !IsLoged) return RedirectToAction("Index", "Home");

            if (bookId >= 0)
            {
                try
                {
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        var tracksCount = 
                            Request.Cookies["BookTrackTableSelectedMode"] == null ? BookTrackTableModes.Default : Request.Cookies["BookTrackTableSelectedMode"].ToString();
                        var bookTrackModel = (BookTrackModel)DataStore.Books.GetBookTrack(
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
            if (bookId == null || !IsLoged) return RedirectToAction("Index", "Home");

            if (bookId >= 0)
            {
                try
                {
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        DataStore.Books.TakeBook(aId, bookId);
                    }
                }
                catch (Exception) { }
            }

            return RedirectToAction("BookTrack", "Books", new RouteValueDictionary(new { bookId = bookId }));
        }

        public IActionResult PutBook(int? bookId)
        {
            if (bookId == null || !IsLoged) return RedirectToAction("Index", "Home");

            if (bookId >= 0)
            {
                try
                {
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        DataStore.Books.PutBook(aId, bookId);
                    }
                }
                catch (Exception) { }
            }

            return RedirectToAction("BookTrack", "Books", new RouteValueDictionary(new { bookId = bookId }));
        }

        public IActionResult MainBooksTableAjaxHandler(JQueryDataTableParamModel param)
        {
            List<BookItem> BooksList = null;

            switch (Request.Cookies["TableSelectedMode"]?.ToString())
            {
                case null:
                    BooksList = DataStore.Books.GetBooks();//.ConvertAll(new Converter<ProxyDisplayBook, DisplayBook>());
                    break;
                case "1":
                    BooksList = DataStore.Books.GetBooks();
                    break;
                case "2":
                    BooksList = DataStore.Books.GetAvaliableBooks();
                    break;
                case "3":
                    if (Int32.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int aId))
                    {
                        BooksList = DataStore.Books.GetBooksByUser(aId);
                    }
                    break;
            }

            IEnumerable<BookItem> filteredBooks;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                //Used if particulare columns are filtered 

                var nameFilter = Convert.ToString(Request.Query["sSearch_0"]);
                var authorsFilter = Convert.ToString(Request.Query["sSearch_1"]);
                var yearFilter = Convert.ToString(Request.Query["sSearch_2"]);
                var availabilityFilter = Convert.ToString(Request.Query["sSearch_3"]);
                var idFilter = Convert.ToString(Request.Query["sSearch_4"]);

                //Optionally check whether the columns are searchable at all 

                var isNameSearchable = Convert.ToBoolean(Request.Query["bSearchable_0"]);
                var isAuthorsSearchable = Convert.ToBoolean(Request.Query["bSearchable_1"]);
                var isYearSearchable = Convert.ToBoolean(Request.Query["bSearchable_2"]);
                var isAvailabilitySearchable = Convert.ToBoolean(Request.Query["bSearchable_3"]);
                var isIdSearchable = Convert.ToBoolean(Request.Query["bSearchable_4"]);

                filteredBooks = BooksList.Where(a =>
                                   isIdSearchable && a.ID.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   isNameSearchable && a.Name.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   isAuthorsSearchable && a.Authors.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   isYearSearchable && a.Year.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   isAvailabilitySearchable && a.Availability.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredBooks = BooksList;
            }


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
                    filteredBooks = filteredBooks.OrderBy(orderingFunction, StringComparer.Ordinal);
                else
                    filteredBooks = filteredBooks.OrderByDescending(orderingFunction, StringComparer.Ordinal);
            }
            else if (sortColumnIndex == 1 && isAuthorsSortable)
            {
                Func<BookItem, string> orderingFunction;
                orderingFunction = (a => a.Authors);
                if (sortDirection == "asc")
                    filteredBooks = filteredBooks.OrderBy(orderingFunction, StringComparer.Ordinal);
                else
                    filteredBooks = filteredBooks.OrderByDescending(orderingFunction, StringComparer.Ordinal);
            }
            else if (sortColumnIndex == 2 && isYearSortable)
            {
                Func<BookItem, DateTime> orderingFunction;
                orderingFunction = (a => a.Year);
                if (sortDirection == "asc")
                    filteredBooks = filteredBooks.OrderBy(orderingFunction);
                else
                    filteredBooks = filteredBooks.OrderByDescending(orderingFunction);
            }
            else if (sortColumnIndex == 3 && isAvailabilitySortable)
            {
                Func<BookItem, bool?> orderingFunction;
                orderingFunction = (a => a.Availability);
                if (sortDirection == "asc")
                    filteredBooks = filteredBooks.OrderBy(orderingFunction);
                else
                    filteredBooks = filteredBooks.OrderByDescending(orderingFunction);
            }
            else if (sortColumnIndex == 4 && isIdSortable)
            {
                Func<BookItem, int?> orderingFunction;
                orderingFunction = (a => a.ID);
                if (sortDirection == "asc")
                    filteredBooks = filteredBooks.OrderBy(orderingFunction);
                else
                    filteredBooks = filteredBooks.OrderByDescending(orderingFunction);
            }

            var displayedBooks = filteredBooks.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from a in displayedBooks
                         select new ArrayList {

                            a.Name,
                            a.Authors,
                            a.Year.ToString(),
                            a.Availability.ToString(),
                            a.ID.ToString()
                        };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = BooksList.Count(),
                iTotalDisplayRecords = filteredBooks.Count(),
                aaData = result
            });
        }
    }
}