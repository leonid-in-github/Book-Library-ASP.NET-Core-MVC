using BookLibrary.Storage.Models.Book;
using System;

namespace BookLibrary.WebServer.Models.Books
{
    public class BookDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Authors { get; set; }
        public DateTime Year { get; set; }
        public bool? Availability { get; set; }

        public BookDto() { }

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
