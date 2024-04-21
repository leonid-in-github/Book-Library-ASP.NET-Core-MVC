using System;
using System.Collections.Generic;

namespace BookLibrary.Storage.Models.Book
{
    public class Book
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public DateTime Year { get; set; }
        public bool? Availability { get; set; }

        public Book(string name, IEnumerable<string> authors, DateTime year, bool availability) : this(0, name, authors, year, availability) { }

        public static Book FromPersistance(int id, string name, IEnumerable<string> authors, DateTime year, bool availability)
        {
            return new Book(id, name, authors, year, availability);
        }

        private Book(int id, string name, IEnumerable<string> authors, DateTime year, bool availability)
        {
            ID = id;
            Name = name;
            Authors = authors;
            Year = year;
            Availability = availability;
        }
    }
}
