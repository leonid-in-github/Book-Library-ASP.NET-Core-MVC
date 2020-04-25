using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProxyDisplayBook = Book_Library_EF_Core_Proxy_Class_Library.Models.Book.LibraryInterfaceBook.DisplayBook;
using ProxyBookTrackActionModel = Book_Library_EF_Core_Proxy_Class_Library.Models.Book.LibraryInterfaceBook.BookTrackActionModel;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books.Extensions
{
    public static class ListExtensions
    {
        public static List<DisplayBook> ConvertToMVCListDisplayBook(this List<ProxyDisplayBook> list)
        {
            return list.ConvertAll(new Converter<ProxyDisplayBook, DisplayBook>(ProxyDisplayBookToDisplayBook));
        }

        public static DisplayBook ProxyDisplayBookToDisplayBook(ProxyDisplayBook model)
        {
            return new DisplayBook { Name = model.Name, Authors = model.Authors, Year = model.Year, ID = model.ID, Availability = model.Availability };
        }

        public static List<BookTrackActionModel> ConvertToProxyListDisplayBook(this List<ProxyBookTrackActionModel> list)
        {
            return list.ConvertAll(new Converter<ProxyBookTrackActionModel, BookTrackActionModel>(ProxyBookTrackActionModelToBookTrackActionModel));
        }

        public static BookTrackActionModel ProxyBookTrackActionModelToBookTrackActionModel(ProxyBookTrackActionModel model)
        {
            return new BookTrackActionModel(model.BookId, model.Name, model.ActionLogin, model.ActionDateTime, model.Action);
        }
    }
}
