using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class MultipleManagerCheckData
    {
        public List<ManagerCheck> Q1 { get; set; }
        public List<ManagerCheck> Q2 { get; set; }
        public List<ManagerCheck> Q3 { get; set; }
        public List<ManagerCheck> Q4 { get; set; }
    }
}