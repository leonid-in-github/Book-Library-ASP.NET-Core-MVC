using BookLibrary.Storage.Models.Book;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.WebServer.Models.Books
{
    public class BookDto
    {
        public int ID { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Authors")]
        public string Authors { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Year")]
        public DateTime Year { get; set; }
        public bool? Availability { get; set; }

        public BookDto()
        {

        }

        public BookDto(Book book)
        {
            ID = book.ID;
            Name = book.Name;
            Authors = string.Join(", ", book.Authors);
            Year = book.Year;
            Availability = book.Availability;
        }

        public Book ToDomain()
        {
            if (ID == 0)
            {
                return new Book(Name, Authors?.Split(", "), Year, Availability ?? true);
            }
            else
            {
                return Book.FromPersistence(ID, Name, Authors?.Split(", "), Year, Availability ?? true);
            }
        }
    }
}
