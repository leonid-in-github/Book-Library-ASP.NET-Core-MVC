﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Storage.Models.Records.Book
{
    public class BookRecord
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Year { get; set; }
        public bool Availability { get; set; }

        public static BookRecord FromDomain(Models.Book.Book book)
        {
            return new BookRecord { Id = book.Id, Name = book.Name, Year = book.Year, Availability = book.Availability ?? true };
        }
    }
}
