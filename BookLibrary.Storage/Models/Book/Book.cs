using System;

namespace BookLibrary.Storage.Models.Book
{
    public class Book
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string Authors { get; set; }
        public DateTime Year { get; set; }
        public bool? Availability { get; set; }

        public Book() { }

        public Book(Book bookItem)
        {
            ID = bookItem.ID;
            Name = bookItem.Name;
            Authors = bookItem.Authors;
            Year = bookItem.Year;
            Availability = bookItem.Availability;
        }
    }
}
