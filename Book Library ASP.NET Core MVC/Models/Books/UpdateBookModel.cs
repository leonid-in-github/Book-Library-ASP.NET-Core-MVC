using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ProxyUpdateBookModel = Book_Library_EF_Core_Proxy_Class_Library.Models.Book.LibraryInterfaceBook.UpdateBookModel;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class UpdateBookModel : ActionBookModel
    {
        public UpdateBookModel() { }

        public UpdateBookModel(ActionBookModel book)
        {
            Name = book.Name;
            Authors = book.Authors;
            Year = book.Year;
            UpdateBookMassege = null;
        }

        public int Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Year")]
        public override DateTime Year { get; set; }

        public string UpdateBookMassege { get; set; }

        public static explicit operator ProxyUpdateBookModel(UpdateBookModel model)
        {
            return new ProxyUpdateBookModel { Id = model.Id, Name = model.Name, Authors = model.Authors, Year = model.Year };
        }

    }
}
