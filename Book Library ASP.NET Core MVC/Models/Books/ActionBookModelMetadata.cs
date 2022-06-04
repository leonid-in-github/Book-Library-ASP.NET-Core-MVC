using System.ComponentModel.DataAnnotations;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class ActionBookModelMetadata
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public object Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Authors (sep-tor \',\')")]
        public object Authors { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Year")]
        public object Year { get; set; }
    }
}
