using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class TrainingProgramReportModel
    {
        public string type { get; set; }
        public string location { get; set; }
        public string manager { get; set; }
        public string employee { get; set; }
        public string category { get; set; }
        public int completed { get; set; }
        public int total { get; set; }
    }
}