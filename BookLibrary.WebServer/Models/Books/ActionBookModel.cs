using BookLibrary.Storage.Models.Book;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.WebServer.Models.Books
{
    [ModelMetadataType(typeof(ActionBookModelMetadata))]
    public class ActionBookModel : BookItem
    {
        public ActionBookModel() { }

        public ActionBookModel(BookItem bookItem) : base(bookItem) { }
    }
}
