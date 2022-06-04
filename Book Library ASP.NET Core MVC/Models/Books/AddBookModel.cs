using Book_Library_Repository_EF_Core.Models.Book;
using Microsoft.AspNetCore.Mvc;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    [ModelMetadataType(typeof(ActionBookModelMetadata))]
    public class AddBookModel : BookItem
    {
        public string AddBookMassege { get; set; }
    }
}
