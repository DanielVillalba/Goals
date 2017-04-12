using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models
{
    public class QuartelyPriorities
    {
        public List<Kpi> Kpi { get; set; }
        public string KpiResultTitle { get; set; }
        public List<QuarterlyAction> quarterly_actions { get; set; }
        public List<ValuesInfusion> values_infusion { get; set; }
        public List<PersonalDevelopment> personal_development { get; set; }
        public int quarterlyPriorityId { set; get; }
        public int onePagePlanId { get; set; }
        public string cycle_comments { get; set; }
    }
    /*
    public class Kpi
    {
        public int id { get; set; }
        public string name { get; set; }
        public string goal { get; set; }
    }

    public class QuarterlyAction
    {
        public int id { get; set; }
        public string action { get; set; }
        public string due_date { get; set; }
        public string key_initiative { get; set; }
    }

    public class ValuesInfusion
    {
        public int id { get; set; }
        public string action { get; set; }
    }

    public class PersonalDevelopment
    {
        public int id { get; set; }
        public string skill { get; set; }
        public string definition_of_success { get; set; }
    }
    */
    public class Kpi
    {
        public int id { get; set; }
        public string name { get; set; }
        public string goal { get; set; }
        public string results { get; set; }

    }

    public class QuarterlyAction
    {
        public int id { get; set; }
        public string action { get; set; }
        public string due_date { get; set; }
        public string key_initiative { get; set; }
        public string outcome { get; set; }
    }

    public class ValuesInfusion
    {
        public int id { get; set; }
        public string action { get; set; }
        public int value_checked { get; set; }
    }

    public class PersonalDevelopment
    {
        public int id { get; set; }
        public string skill { get; set; }
        public string definition_of_success { get; set; }
        public string outcome { get; set; }
    }
    
}