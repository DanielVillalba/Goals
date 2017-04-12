using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SiteQuarterAndSequenceQuestionModel
    {
        public QuarterSiteAndEmployeeTypeModel LocationQuarterAndType { get; set; }
        public int QuestionSequence { get; set; }
    }
}