using System;
using System.Collections.Generic;
using System.Text;

namespace Book_Library_Repository_EF_Core.Models.Book
{
    public class BookItem
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public string Authors { get; set; }
        public DateTime Year { get; set; }
        public bool? Availability { get; set; }

        public BookItem() { }

        public BookItem(BookItem bookItem) 
        {
            ID = bookItem.ID;
            Name = bookItem.Name;
            Authors = bookItem.Authors;
            Year = bookItem.Year;
            Availability = bookItem.Availability;
        }
    }
}
