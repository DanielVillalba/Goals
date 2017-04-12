using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SurveyResponseReport
    {
        public int SurveyId { get; set; }
        public string Quarter { get; set; }
        public string Text { get; set; }
        public int ResponseId { get; set; }
        public int ResourceId { get; set; }

    }
}