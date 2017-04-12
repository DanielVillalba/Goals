using DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDPTrackerSite.Models.QuarterlyPriorities;

namespace CDPTrackerSite.Services
{
    public class QuaterlyPrioritiesService
    {
        CDPTrackEntities context = new CDPTrackEntities();

        public QuarterlyPrioritiesModel GetQuaterlyPrioritiesDirector(int ResourceID, int quarterId)
        {/*
            var resourceGroupResult = context.Groups
                .Join(context.Employee_Groups,
                group => group.GroupName,
                egroup => egroup.GroupId,
                (group, egroup) => new { group, egroup })
                .Where(wgroup => wgroup.egroup.ResourceId == ResourceID && wgroup.group.GroupName == "tdg-moss-executive")
                .Single();

            var quartelyPrioritiesResult = context.QuarterlyPriorities
                .Where(quarterly => quarterly.ResourceId == resourceGroupResult.egroup.ResourceId
                                    && quarterly.QuarterId == quarterId)
                .Single();

            var kpiResult = context.KPI
                .Where(kpi => kpi.QuaterlyPrioritiesID == quartelyPrioritiesResult.QuarterlyPrioritiesId)
                .Join(context.Threshold,
                kpi => kpi.KpiID,
                threshold => threshold.KpiId,
                (kpi, threshold) => new { kpi, threshold })
                .Select(sel => new { sel.kpi, sel.threshold });



            var rankResult = context.RankCatalog
                .Where(rnk => rnk.GroupId == resourceGroupResult.group.GroupId
                        && rnk.IsActive == true
                        && rnk.IsVisible == true)
                .DefaultIfEmpty()
                .Join(context.RankTypeCatalog,
                        rnk => rnk.RankTypeID,
                        rnkT => rnkT.RankTypeID,
                        (rnk, rnkT) => new { rnk, rnkT }
                    )
                .Select(sel => new { sel.rnk.Description, sel.rnk.Score, sel.rnk.RankID, sel.rnkT.TypeName });





            var quarterlyPrioritiesResult = context.QuarterlyPriorities
                .Where(qp => qp.QuarterId == quarterId)
                .Single();

            var kpiResult = context.KPI
                .Where(k => k.QuaterlyPrioritiesID == quarterlyPrioritiesResult.QuarterId)
                .Single();

            var onePageResult = context.OnePagePlan
                .Where(o => o.OnePagePlanId == onepageID)
                .Select(o => new { o.SG, o.G, o.Y, o.R }).Single();

            var annualPrioritiesResult = context.AnnualPriorities
                .Where(a => a.OnePagePlanId == onepageID)
                .Select(a => a.AnnualPriorities1).ToList();

            var qpResult = context.QuarterlyPriorities
                .Where(o => o.ResourceId == userID )//&& o.QuarterId == quarterId)
                .Single();



            return new QuarterlyPrioritiesModel() {
                OnePagePlan = new Models.OnePagePlan() {
                    R = onePageResult.R,
                    Y = onePageResult.Y,
                    G = onePageResult.G,
                    SG = onePageResult.SG
                },

                Kpi = new List<KpiModel>(
                        quarterlyPrioritiesResult.KPI.Select(k => new KpiModel
                        {
                            Name = k.Name,
                            Weight = k.Weight,
                            Result = k.Result,
                            Comment = k.Comment,
                            Threshold = new List<ThresholdModel>(
                                kpiResult
                                .Select(t => new ThresholdModel
                                {
                                    Value = t.threshold.Value,
                                    //RankID = t.threshold.RankID,
                                    
                                    
                                })
                            )
                        })
                    ),

                KpiRank = new List<RankModel>(
                        rankResult.Where(r => r.TypeName == "KPIS")
                        .Select(rnk => new RankModel {
                            Description = rnk.Description,
                            Score = rnk.Score
                        })
                    ),

                StrategicPrioritiesRank = new List<RankModel>(
                        rankResult.Where(r => r.TypeName == "STRATEGIC_PRIORITIES")
                        .Select(rnk => new RankModel { 
                            Description = rnk.Description,
                            Score = rnk.Score
                        })
                    ),

                ValuesInfusionRank = new List<RankModel>(
                        rankResult.Where(r => r.TypeName == "VALUES_INFUSION")
                        .Select(rnk => new RankModel {
                            Description = rnk.Description,
                            Score = rnk.Score
                        })
                    ),
                
            };
        */
            return null;
        }

        public QuarterlyPrioritiesModel GetQuarterlyPrioritiesByDirector(int ResourceID, int quarterID)
        {
            var resoruceGroupResult = context.Groups
                .Join(context.Employee_Groups,
                group => group.GroupName,
                egroup => egroup.GroupId,
                (group, egroup) => new { group, egroup })
                .Where(wgroup => wgroup.egroup.ResourceId == ResourceID && wgroup.group.GroupName == "tdg-moss-executive")
                //.Select(sgroup => sgroup.group.GroupId)
                .FirstOrDefault();

            var onePageResult = context.OnePagePlan
                .Where(wopp => wopp.QuarterId == quarterID)
                .Single();

            var keyInitiativesResult = context.AnnualPriorities
                .Where(wkt => wkt.OnePagePlanId == onePageResult.OnePagePlanId)
                .Select(sel => new Models.AnnualPriority {
                    annual_priority = sel.AnnualPriorities1
                });

            var quarterlyPrioritiesResult = context.QuarterlyPriorities
                .Where(wqp => wqp.ResourceId == resoruceGroupResult.egroup.ResourceId
                    && wqp.QuarterId == quarterID)
                .FirstOrDefault();

            var kpiResult = context.KPI
                .Join(context.Threshold,
                k => k.KpiID,
                t => t.KpiID,
                (k, t) => new { k, t })
                .Where(wk => wk.k.QuaterlyPrioritiesID == quarterlyPrioritiesResult.QuarterlyPrioritiesId)
                .Select(sel => new KpiModel {
                    Name = sel.k.Name,
                    Result = sel.k.Result,
                    RankID = sel.k.RankID,
                    Threshold = new ThresholdModel {
                        RankID = sel.t.RankID,
                        Value = sel.t.Value,
                    }
                });

            var strategicPrioritiesResult = context.StrategicPriority
                .Where(wsp => wsp.QuarterlyPrioritiesId == quarterlyPrioritiesResult.QuarterlyPrioritiesId)
                .Select(sel => new StrategicPrioritiesModel {
                    Action = sel.Action,
                    DefinitionOfDone = sel.DefinitionOfDone,
                    DueDate = sel.DueDate,
                    AnnualPrioritiesID = sel.AnnualPrioritiesID,
                    Result = sel.Result,
                    RankID = sel.RankID,
                });

            var valuesInfusionResult = context.ValuesInfusion
                .Where(wvi => wvi.QuarterlyPrioritiesId == quarterlyPrioritiesResult.QuarterlyPrioritiesId)
                .Select(sel => new ValuesInfusionModel
                {
                    Value = sel.Value,
                    DefinitionOfDone = sel.DefinitionOfDone,
                    IsDone = sel.IsDone,
                });

            return new QuarterlyPrioritiesModel {
                OnePagePlan = new Models.OnePagePlan
                {
                    SG = onePageResult.SG,
                    G = onePageResult.G,
                    Y = onePageResult.Y,
                    R = onePageResult.R,
                    annual_priorities = new List<Models.AnnualPriority>(
                        keyInitiativesResult
                        )
                },
                CEOComment = quarterlyPrioritiesResult.CEOComment,
                ClosingComment = quarterlyPrioritiesResult.ClosingComment,
                ManagetComment = quarterlyPrioritiesResult.ManagerComment,
                Kpi = new List<Models.QuarterlyPriorities.KpiModel>(
                    kpiResult
                    ),
                StrategicPriorities = new List<StrategicPrioritiesModel>(
                    strategicPrioritiesResult
                    ),
            };
        }
    }
}