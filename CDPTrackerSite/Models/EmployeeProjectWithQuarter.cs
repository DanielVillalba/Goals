using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class EmployeeProjectWithQuarter
    {

        public int ResourceId { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public int Quarter { get; set; }
        public int Year { get; set; }
    }
}