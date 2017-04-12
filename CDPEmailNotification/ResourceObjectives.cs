using System.Collections.Generic;
using DataSource;

namespace CDPEmailNotification
{
    class ResourceObjectives
    {
        public string Name { get; set; }
        public string DomainName { get; set; }
        public string Email { get; set; }
        public List<Objective> Goals { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
    }
}
