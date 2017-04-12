using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class ResourceModel
    {
        public int resourceId { get; set; }
        public string resourceName { get; set; }
        public int managerId { get; set; }

        public List<CyclePrioritiesModel> priorities { get; set; }
    }
}