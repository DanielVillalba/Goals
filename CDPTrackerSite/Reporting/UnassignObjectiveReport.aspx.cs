using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Reporting.WebForms;
using Utils;
using System.Configuration;
using CDPTrackerSite.RoleManagement;
using System.Data;
using DataSource;
using System.Collections;
using CDPTrackerSite.Controllers;

namespace CDPTrackerSite.Reporting
{
    public partial class UnassignObjectiveReport : System.Web.UI.Page
    {
        public List<object> employeesWithUnassignObjectives;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<dynamic> employeesWithUnassignObjectives = getEmployeesWithUnassignObjectives();
                report.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", employeesWithUnassignObjectives));
                report.LocalReport.ReportPath = Server.MapPath("~/Reporting/UnassignObjectiveReport.rdlc");

            }
        }  

        public static List<dynamic> getEmployeesWithUnassignObjectives()
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {                   
                var query = from t1 in context.GoalTrackings
                        join t2 in context.Objectives on t1.ObjectiveId equals t2.ObjectiveId
                        join t3 in context.Resources on t1.ResourceId equals t3.ResourceId
                        where t2.CategoryId == 4 && t1.VerifiedByManager == false
                        group t3 by new {t3.DomainName, t3.ResourceId, t3.Name} into g
                        //dName is the domain name, we dont changed the field to  'Name' because in the report is no updating the parameters available
                        select new { ResourceId = g.Key.ResourceId, DomainName = g.Key.Name, RecordsPerGroup = g.Count(), dName=g.Key.DomainName};
                    
                var orderedQuery = query.OrderBy(x=>x.DomainName);

                List<dynamic> employeesWithUnassignObjectives = new List<dynamic>();

                foreach (var item in orderedQuery)
                {
                    if (GoalTrackingController.ExclusionList.Any(x => x.Contains(item.dName)))
                        continue;

                    if (GoalTrackingController.GetIfIsActiveEmployeeFromAD(item.dName))
                        employeesWithUnassignObjectives.Add(item);   
                }

                return employeesWithUnassignObjectives;
            }   
        }


    }
}