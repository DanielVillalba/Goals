using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.Models.QuarterlyPriorities
{
    public class QuarterlyPrioritiesModel
    {
        public OnePagePlan OnePagePlan { get; set; }
        public List<AnnualPriority> AnualPriority {get;set;}
        public List<KpiModel> Kpi { get; set; }
        public List<StrategicPrioritiesModel> StrategicPriorities { get; set; }
        public List<ValuesInfusion> ValuesInfusion { get; set; }
        public List<PersonalDevelopmentModel> PersonalDevelopment { get; set; }
        public string ClosingComment { get; set; }
        public string CEOComment { get; set; }
        public string ManagetComment { get; set; }

        #region Ranks
        public List<RankModel> KpiRank { get; set; }
        public List<RankModel> StrategicPrioritiesRank { get; set; }
        public List<RankModel> ValuesInfusionRank { get; set; }
        #endregion

    }
}