using Book_Library_ASP.NET_Core_MVC.Models.Books.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyBookTrackModel = Book_Library_EF_Core_Proxy_Class_Library.Models.Book.LibraryInterfaceBook.BookTrackModel;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class BookTrackModel
    {
        public BookTrackModel()
        {
            BookTrackList = new List<BookTrackActionModel>();

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
        public List<BookTrackActionModel> BookTrackList { get; set; }

        public List<SelectListItem> TableModes { set; get; }

        public static explicit operator BookTrackModel(ProxyBookTrackModel model)
        {
            return new BookTrackModel
            {
                BookId = model.BookId,
                BookName = model.BookName,
                BookAvailability = model.BookAvailability,
                CanBePuted = model.CanBePuted,
                BookTrackList = model.BookTrackList.ConvertToProxyListDisplayBook()
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
