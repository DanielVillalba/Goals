using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SurveyResponseProjectVsAllProjectsModel
    {
        public string Project { get; set; }
        public List<SurveyResponseReport> SurveyResponse { get; set; }
    }
}