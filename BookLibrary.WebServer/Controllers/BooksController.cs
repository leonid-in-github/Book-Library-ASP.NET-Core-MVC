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

        public async Task<IActionResult> BooksTableAjaxHandler([ModelBinder(BinderType = typeof(DataTableParametersBinder))]
            DataTableParameters parameters)
        {
            _ = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            var booksList = await GetBooksList(userId, parameters.Search.Value, parameters.Start, parameters.Length);
            var booksTotalCount = await GetBooksTotalCount(userId, parameters.Search.Value);
            var orderColumnIndex = parameters.Order.FirstOrDefault()?.Column;
            var orderDirection = parameters.Order.FirstOrDefault()?.Dir?.ToLower();
            var isColumnOrderable = parameters.Columns.Find(column => column.Data == orderColumnIndex)?.Orderable;
            if (isColumnOrderable == true)
            {
                switch (orderColumnIndex)
                {
                    case 0:
                        Func<Book, string> nameOrderingFunction = (a => a.Name);
                        if (orderDirection == "asc")
                            booksList = booksList.OrderBy(nameOrderingFunction, StringComparer.Ordinal);
                        else
                            booksList = booksList.OrderByDescending(nameOrderingFunction, StringComparer.Ordinal);
                        break;
                    case 1:
                        Func<Book, string> authorsOrderingFunction = (a => string.Join(", ", a.Authors));
                        if (orderDirection == "asc")
                            booksList = booksList.OrderBy(authorsOrderingFunction, StringComparer.Ordinal);
                        else
                            booksList = booksList.OrderByDescending(authorsOrderingFunction, StringComparer.Ordinal);
                        break;
                    case 2:
                        Func<Book, DateTime> yearOrderingFunction = (a => a.Year);
                        if (orderDirection == "asc")
                            booksList = booksList.OrderBy(yearOrderingFunction);
                        else
                            booksList = booksList.OrderByDescending(yearOrderingFunction);
                        break;
                    case 3:
                        Func<Book, bool?> availabilityOrderingFunction = (a => a.Availability);
                        if (orderDirection == "asc")
                            booksList = booksList.OrderBy(availabilityOrderingFunction);
                        else
                            booksList = booksList.OrderByDescending(availabilityOrderingFunction);
                        break;
                    case 4:
                        Func<Book, int?> IdOrderingFunction = (a => a.ID);
                        if (orderDirection == "asc")
                            booksList = booksList.OrderBy(IdOrderingFunction);
                        else
                            booksList = booksList.OrderByDescending(IdOrderingFunction);
                        break;
                    default:
                        throw new ArgumentException("Invalid order column index");
                }
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
                draw = parameters.Draw,
                recordsTotal = booksTotalCount,
                recordsFiltered = booksTotalCount,
                data = result
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