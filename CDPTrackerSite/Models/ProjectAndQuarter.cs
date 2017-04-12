using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataSource;

namespace CDPTrackerSite.Models
{
    public class ProjectAndQuarter
    {
        public Project selectedProject { get; set; }
        public SelectedQuarter selectedQuarter { get; set; }
    }
}