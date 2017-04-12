using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class TasksModel
    {
        public int taskId { get; set; }
        public string taskDescription { get; set; }
        public int taskProgress { get; set; }
        public DateTime? taskFinishDate { get; set; }
        public int? taskTdu { get; set; }
        public int? sourceId { get; set; }
        public int? trainingCategoryId { get; set; }
        public bool taskVerified { get; set; }

        public string imgProgress { get; set; }
        public string statusTitle { get; set; }

        public string trainingCategoryDescription { get; set; }
    }
}