using BookLibrary.Storage.Models.Book;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.WebServer.Models.Books
{
    [ModelMetadataType(typeof(ActionBookModelMetadata))]
    public class ActionBookModel : Book
    {
        public ActionBookModel() { }

        public ActionBookModel(Book bookItem) : base(bookItem) { }
    }
}
