using CDPTrackerSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class CategoriesModel
    {
        public int categoryId { get; set; }
        public string categoryDescription { get; set; }
        public bool categoryVisibility { get; set; }
    }
}