using BookLibrary.Storage.Models.Book;

namespace BookLibrary.WebServer.Models.Books
{
    public class EditBookModel : ActionBookModel
    {
        public EditBookModel()
        {

        }

        public EditBookModel(Book bookItem) : base(bookItem)
        {

        }

        public string UpdateBookMessage { get; set; }
    }
}
