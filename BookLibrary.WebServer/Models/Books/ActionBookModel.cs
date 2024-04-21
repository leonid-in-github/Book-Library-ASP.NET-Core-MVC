using BookLibrary.Storage.Models.Book;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.WebServer.Models.Books
{
    [ModelMetadataType(typeof(ActionBookModelMetadata))]
    public class ActionBookModel : BookDto
    {
        public ActionBookModel() { }

        public ActionBookModel(Book book) : base(book) { }
    }
}
