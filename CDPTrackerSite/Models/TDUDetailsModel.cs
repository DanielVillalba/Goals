using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class TDUDetailModel
    {
        public int GoalId { get; set; }
        public string EmployeeLocation { get; set; }
        public int ManagerId { get; set; }
        public string Manager { get; set; }
        public string ManagerLocation { get; set; }
        public int ResourceId { get; set; }
        public string Employee { get; set; }
        public string Goal { get; set; }
        public string Objective { get; set; }
        public string TrainingCategory { get; set; }
        public DateTime FinishDate { get; set; }
        public int Verified { get; set; }
        public int MustCheck { get; set; }
        public int Progress { get; set; }
        public int TDU { get; set; }
        public string Location { get; set; }
        public string Project { get; set; }
        public string DomainName { get; set; }
        public int CategoryId { get; set; }
    }
}