using System.ComponentModel.DataAnnotations;

namespace BookLibrary.WebServer.Models.Books
{
    public class ActionBookModelMetadata
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public object Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Authors")]
        public object Authors { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Year")]
        public object Year { get; set; }
    }
}
