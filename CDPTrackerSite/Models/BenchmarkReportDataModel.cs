using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class BenchmarkReportDataModel
    {
        public EmployeeProject employeeProjectData { get; set; }
        //public int Manager { get; set; }
        public int ProjectId { get; set; }
        public int LocationId { get; set; }
        public int ResourceId { get; set; }
        public int quarter { get; set; }
        public int year { get; set; }
    }
}