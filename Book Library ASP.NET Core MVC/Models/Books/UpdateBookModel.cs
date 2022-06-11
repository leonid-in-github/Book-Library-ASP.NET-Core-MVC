using Book_Library_Repository_EF_Core.Models.Book;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class UpdateBookModel : ActionBookModel
    {
        public string UpdateBookMassege { get; set; }

        public UpdateBookModel() { }

        public UpdateBookModel(BookItem bookItem) : base(bookItem) { }
    }
}
