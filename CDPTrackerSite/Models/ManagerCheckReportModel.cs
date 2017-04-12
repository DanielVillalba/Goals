using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class ManagerCheckReportModel
    {
        public string Manager { get; set; }
        public int ResourceId { get; set; }
        public string DomainName { get; set; }
        public DateTime? _DateAnswered { get; set; }
        public string DateAnswered { get; set; }
        public string EvaluatedEmployee { get; set; }
        public string Completed { get; set; }
        public string SurveyName { get; set; }
    }
}