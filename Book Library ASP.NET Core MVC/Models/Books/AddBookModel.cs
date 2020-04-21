using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyAddBookModel = Book_Library_EF_Core_Proxy_Class_Library.Models.Book.LibraryInterfaceBook.AddBookModel;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class AddBookModel : ActionBookModel
    {
        public AddBookModel() { }

        public string AddBookMassege { get; set; }

        public static explicit operator ProxyAddBookModel(AddBookModel model)
        {
            return new ProxyAddBookModel { Name = model.Name, Authors = model.Authors, Year = model.Year, AddBookMassege = model.AddBookMassege };
        }
    }
}
