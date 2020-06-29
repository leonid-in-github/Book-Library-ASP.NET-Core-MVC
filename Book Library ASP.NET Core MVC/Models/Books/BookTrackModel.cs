﻿using Book_Library_Repository_EF_Core.Models.Book;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class BookTrackModel
    {
        public BookTrackModel()
        {
            BookTrackList = new List<BookTrackItem>();

            SelectedMode = BookTrackTableModes.Default;

            TableModes = new List<SelectListItem>()
            {
                new SelectListItem {Value = BookTrackTableModes.Default, Text = "Default" },
                new SelectListItem {Value = BookTrackTableModes._100, Text = "100" },
                new SelectListItem {Value = BookTrackTableModes.All, Text = "All" }
            };

        }

        public int? BookId { get; set; }
        public String BookName { get; set; }
        public bool? BookAvailability { get; set; }
        public bool CanBePuted { get; set; }
        public string SelectedMode { get; set; }
        public List<BookTrackItem> BookTrackList { get; set; }

        public List<SelectListItem> TableModes { set; get; }

        public static explicit operator BookTrackModel(BookTrackList model)
        {
            return new BookTrackModel
            {
                BookId = model.BookId,
                BookName = model.BookName,
                BookAvailability = model.BookAvailability,
                CanBePuted = model.CanBePuted,
                BookTrackList = model.TracksList
            };
        }
    }

    public sealed class BookTrackTableModes
    {
        public const string Default = "10";
        public const string _100 = "100";
        public const string All = "All";
    }
}
