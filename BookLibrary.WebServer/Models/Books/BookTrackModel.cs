using BookLibrary.Storage.Models.Book;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookLibrary.WebServer.Models.Books
{
    public class BookTrackModel
    {
        public int? BookId { get; set; }
        public string BookName { get; set; }
        public bool? BookAvailability { get; set; }
        public bool CanBePut { get; set; }
        public string SelectedMode { get; set; } = BookTrackTableModes.Default;
        public List<BookTrack> BookTrackList { get; set; } = [];

        public List<SelectListItem> TableModes { set; get; } =
        [
            new SelectListItem {Value = BookTrackTableModes.Default, Text = "10" },
            new SelectListItem {Value = BookTrackTableModes._100, Text = "100" },
            new SelectListItem {Value = BookTrackTableModes.All, Text = "All" }
        ];

        public static explicit operator BookTrackModel(BookTrackList model)
        {
            return new BookTrackModel
            {
                BookId = model.BookId,
                BookName = model.BookName,
                BookAvailability = model.BookAvailability,
                CanBePut = model.CanBePut,
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
