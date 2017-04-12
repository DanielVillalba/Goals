using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class ManagerCheck
    {
        public string Quarter { set; get; }
        public string Text { set; get; }
        public int ResourceId { set; get; }
        public int ResponseId { set; get; }
        public string ResponseText { set; get; }
        public int ResourceEvaluatedId { set; get; }
    }
}