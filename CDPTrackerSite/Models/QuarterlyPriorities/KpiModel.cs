using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models.QuarterlyPriorities
{
    public class KpiModel
    {
        public string Name { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public ThresholdModel Threshold { get; set; }
        public string Result { get; set; }
        public string Comment { get; set; }
        public int RankID { get; set; }
     }
}