using System.Collections.Generic;
using DataSource;

namespace CDPEmailNotification
{
    class ResourceGoals
    {
        public bool IsRegisteredUser { get; set; }
        public string Name { get; set; }
        public string DomainName { get; set; }
        public string Email { get; set; }
        public List<GoalTracking> Goals { get; set; }
        public List<Objective> Objectives { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
    }
}
