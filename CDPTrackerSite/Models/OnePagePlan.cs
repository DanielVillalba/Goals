using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class OnePagePlan
    {
        public List<CoreValue> core_values { get; set; }
        public List<KeyThrust> key_thrusts { get; set; }
        public List<AnnualPriority> annual_priorities { get; set; }
        public List<CriticalNumber> critical_numbers { get; set; }
        public int onePagePlanID { get; set; }

        public string SG { get; set; }
        public string G { get; set; }
        public string Y { get; set; }
        public string R { get; set; }
    }
    public class CoreValue
    {
        public int id { get; set; }
        public string core_value { get; set; }
    }

    public class KeyThrust
    {
        public int id { get; set; }
        public string key_thrust { get; set; }
    }

    public class AnnualPriority
    {
        public int id { get; set; }
        public string annual_priority { get; set; }
    }

    public class CriticalNumber
    {
        public int id { get; set; }
        public string critical_number { get; set; }
    }

   

}