using DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class multipleModelsOnePagecs
    {
        public IEnumerable<AnnualPriority> annualPrioritiesDB { get; set; }
        public IEnumerable<KeyThrust> keyThrustDB { get; set; }
        public IEnumerable<CoreValues> coreValuesDB { get; set; }
     
    }
}