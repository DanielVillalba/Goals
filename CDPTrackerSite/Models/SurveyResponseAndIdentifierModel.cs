using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SurveyResponseAndIdentifierModel
    {
        public string Identifier { get; set; }
        public List<SurveyResponseReport>  SurveyResponse { get; set; }
    }
}