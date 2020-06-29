using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public class BookTrackActionModel
    {
        public BookTrackActionModel(
                int bookId,
                string bookName,
                string actionLogin,
                DateTime actionDateTime,
                string action
                )
        {
            BookId = bookId;
            Name = bookName;
            ActionLogin = actionLogin;
            ActionDateTime = actionDateTime;
            Action = action;
        }

        public int BookId { get; set; }

        public string Name { get; set; }

        public string ActionLogin { get; set; }

        public DateTime ActionDateTime { get; set; }

        public string Action { get; set; }
    }
}
