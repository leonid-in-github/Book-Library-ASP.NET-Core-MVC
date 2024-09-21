using BookLibrary.Storage.Models.Book;
using BookLibrary.Storage.Repositories;
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
    [ApiController]
    [Route("api/books")]
    public class BooksApiController(IBooksRepository booksRepository) : ControllerBase
    {
        private readonly IBooksRepository booksRepository = booksRepository;

        [HttpGet]
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

            return Ok(new
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