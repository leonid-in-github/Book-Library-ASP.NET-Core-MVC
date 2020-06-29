using Book_Library_Repository_EF_Core.Models.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class AddBookModel : ActionBookModel
    {
        public AddBookModel() { }

        public string AddBookMassege { get; set; }

        public static explicit operator BookItem(AddBookModel model)
        {
            return new BookItem { Name = model.Name, Authors = model.Authors, Year = model.Year };
        }
    }
}
