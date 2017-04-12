using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class MultipleSurveyData
    {
        public List<SurveyResponseReport> Q1 { get; set; }
        public List<SurveyResponseReport> Q2 { get; set; }
        public List<SurveyResponseReport> Q3 { get; set; }
        public List<SurveyResponseReport> Q4 { get; set; }

    }
}