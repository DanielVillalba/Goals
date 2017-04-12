using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SurveyResponseAndProjectModel
    {
        public string Project { get; set; }
        public List<SurveyResponseReport> SurveyResponse { get; set; }
    }
}