using CDPTrackerSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class CyclePrioritiesModel
    {
        public int priorityId { get; set; }
        public string priorityDescription { get; set; }
        public int resourceId { get; set; }
        public int categoryId { get; set; }
        public int? priorityYear { get; set; }
        public int? priorityQuarter { get; set; }
        public int? priorityProgress { get; set; }
        public bool? priorityDuplicated { get; set; }

        public string statusTitle { get; set; }

        public List<TasksModel> tasks { get; set; }
    }
}