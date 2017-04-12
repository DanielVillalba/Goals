using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class TeamMemberInputReportModel
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public string SurveyName { get; set; }
        public DateTime? _DateAnswered { get; set; }
        public string DateAnswered { get; set; }
        public string DomainName { get; set; }
        public string Completed { get; set; }
    }
}