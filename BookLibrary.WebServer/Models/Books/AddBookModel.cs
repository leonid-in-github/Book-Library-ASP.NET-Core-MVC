using BookLibrary.Storage.Models.Book;

namespace BookLibrary.WebServer.Models.Books
{
    public class AddBookModel : ActionBookModel
    {
        public AddBookModel()
        {

        }

        public AddBookModel(Book book) : base(book)
        {

        }

        public string AddBookMessage { get; set; }
    }
}
