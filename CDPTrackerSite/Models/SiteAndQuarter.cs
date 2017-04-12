using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SiteAndQuarter
    {
        public SiteLocation selectedSite { get; set; }
        public SelectedQuarter selectedQuarter { get; set; }
    }
}