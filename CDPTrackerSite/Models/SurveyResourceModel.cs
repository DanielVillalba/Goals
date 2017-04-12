using CDPTrackerSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SurveyResourceModel
    {
        public int? resourceId { get; set; }
        public int surveyId { get; set; }
        public string dateAnswered { get; set; }
        public int surveyType { get; set; }
        public int? resourceEvaluatedId { get; set; }

        public List<SurveyResponseModel> answers { get; set; }
    }
}