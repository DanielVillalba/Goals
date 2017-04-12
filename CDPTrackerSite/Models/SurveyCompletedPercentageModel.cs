using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class SurveyCompletedPercentageModel
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
        public int Anwsered { get; set; }
        public int Total { get; set; }
        public decimal CompletionPercentage { get; set; }
    }
}