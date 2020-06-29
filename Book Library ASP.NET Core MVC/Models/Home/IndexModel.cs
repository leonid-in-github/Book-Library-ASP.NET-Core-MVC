﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Library_ASP.NET_Core_MVC.Models.Home
{
    public class IndexModel
    {
        public IndexModel()
        {
            TableModes = new List<SelectListItem>()
            {
                new SelectListItem {Value = "1", Text = "All" },
                new SelectListItem {Value = "2", Text = "Avaliable" },
                new SelectListItem {Value = "3", Text = "Taked by user" }
            };

        }

        public List<SelectListItem> TableModes { set; get; }

        public int? SelectedMode { get; set; }
    }
}
