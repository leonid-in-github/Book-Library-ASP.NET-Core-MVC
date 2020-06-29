using Book_Library_Repository_EF_Core.Models.Book;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class UpdateBookModel : ActionBookModel
    {
        public UpdateBookModel() { }

        public int? Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Year")]
        public override DateTime Year { get; set; }

        public string UpdateBookMassege { get; set; }

        public static explicit operator BookItem(UpdateBookModel model)
        {
            return new BookItem { ID = model.Id, Name = model.Name, Authors = model.Authors, Year = model.Year };
        }

        public static explicit operator UpdateBookModel(BookItem model)
        {
            return new UpdateBookModel { Id = model.ID, Name = model.Name, Authors = model.Authors, Year = model.Year };
        }

    }
}
