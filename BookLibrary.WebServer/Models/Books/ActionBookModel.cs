using BookLibrary.Storage.Models.Book;

namespace BookLibrary.WebServer.Models.Books
{
    public class ActionBookModel : BookDto
    {
        public ActionBookModel()
        {

        }

        public ActionBookModel(Book book) : base(book)
        {

        }
    }
}
