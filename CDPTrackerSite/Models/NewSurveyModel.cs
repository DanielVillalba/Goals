using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class NewSurveyModel
    {
        public string Cycle { get; set; }
        public int Type { get; set; }
        public DateTime EnableDate { get; set; }
    }
}