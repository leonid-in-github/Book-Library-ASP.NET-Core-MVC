using BookLibrary.Repository.Models.Book;

namespace BookLibrary.WebServer.Models.Books
{
    public class UpdateBookModel : ActionBookModel
    {
        public string UpdateBookMassege { get; set; }

        public UpdateBookModel() { }

        public UpdateBookModel(BookItem bookItem) : base(bookItem) { }
    }
}
