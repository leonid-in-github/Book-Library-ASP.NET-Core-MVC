using Book_Library_Repository_EF_Core.Models.Book;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class ActionBookModel : Book
    {
        public ActionBookModel() { }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public override string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Authors (sep-tor \',\')")]
        public override string Authors { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Year")]
        public override DateTime Year { get; set; }

        public static explicit operator ActionBookModel(BookItem model)
        {
            return new ActionBookModel { Name = model.Name, Authors = model.Authors, Year = model.Year };
        }
    }
}
