using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models.QuarterlyPriorities
{
    public class StrategicPrioritiesModel
    {
        public string Action { get; set; }
        public string Weight { get; set; }
        public string DefinitionOfDone { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public string Result { get; set; }
        public string Comment { get; set; }
        public int AnnualPrioritiesID { get; set; }
        public int RankID { get; set; }
    }
}