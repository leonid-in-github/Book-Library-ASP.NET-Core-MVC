using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyDisplayBook = Book_Library_EF_Core_Proxy_Class_Library.Models.Book.LibraryInterfaceBook.DisplayBook;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class DisplayBook : Book
    {
        public DisplayBook()
        {

        }

        public int ID { get; set; }

        public bool Availability { get; set; }

        public static explicit operator DisplayBook(ProxyDisplayBook model)
        {
            return new DisplayBook { Name = model.Name, Authors = model.Authors, Year = model.Year, ID = model.ID, Availability = model.Availability };
        }
    }
}
