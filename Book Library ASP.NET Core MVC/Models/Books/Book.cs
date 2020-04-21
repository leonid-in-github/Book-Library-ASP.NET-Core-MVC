using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Library_ASP.NET_Core_MVC.Models.Books
{
    public abstract class Book
    {
        public virtual string Name { get; set; }

        public virtual string Authors { get; set; }

        public virtual DateTime Year { get; set; }
    }
}
