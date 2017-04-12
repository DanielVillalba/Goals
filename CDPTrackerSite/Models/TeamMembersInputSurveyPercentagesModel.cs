using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class TeamMembersInputSurveyPercentagesModel
    {
        public string Text { get; set; }
        public decimal? Strongly_Agree { get; set; }
        public decimal? Agree { get; set; }
        public decimal? Neutral { get; set; }
        public decimal? Disagree { get; set; }
        public decimal? Strongly_Disagree { get; set; }
    }
}