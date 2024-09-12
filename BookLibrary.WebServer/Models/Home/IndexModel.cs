using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BookLibrary.WebServer.Models.Home
{
    public class IndexModel
    {
        public IndexModel()
        {
            TableModes =
            [
                new SelectListItem {Value = "all", Text = "All" },
                new SelectListItem {Value = "available", Text = "Available" },
                new SelectListItem {Value = "takenByUser", Text = "Taken by user" }
            ];

        }

        public List<SelectListItem> TableModes { set; get; }

        public int? SelectedMode { get; set; }
    }
}
