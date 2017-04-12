using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models.GoalTracking
{
    public class ManagerResource
    {
        public string sResourceName { get; set; }
        public int iResourceActiveDirectoryId { get; set; }
        public bool bResourceEvaluated { get; set; }
        public bool bResourceGoals { get; set; }
        public bool bResourceTeamMembersInput { get; set; }
        public int iManagerActiveDirectoryId { get; set; }
        //public bool bSurveyAvailable { get; set; } 
    }
}