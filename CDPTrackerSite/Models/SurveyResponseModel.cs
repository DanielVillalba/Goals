using CDPTrackerSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SurveyResponseModel
    {
        public int questionId { get; set; }
        public int? resourceId { get; set; }
        public int? responseId { get; set; }
        public string responseText { get; set; }
    }
}