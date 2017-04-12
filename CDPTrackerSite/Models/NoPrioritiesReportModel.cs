using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class NoPrioritiesReportModel{
    
        public int ResourceId {get; set;}
        public int? ActiveDirectoryID { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public System.DateTime Lastlogin { get; set; }
        public string DomainName { get; set; }

    }
}