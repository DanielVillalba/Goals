using CDPTrackerSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SurveyAnsweredModel
    {
        public List<SurveyResponseModel> surveyResponses { get; set; }
        public String managerName { get; set; }
        public int surveyQuarterId { get; set; }
        public int? resourceId { get; set; }
    }
}