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
using CDPTrackerSite.Models;
using CDPTrackerSite.Controllers;

namespace CDPTrackerSite.Reporting
{
    public partial class NoPrioritiesReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string year = Request.QueryString["year"];
                string quarter = Request.QueryString["quarter"];
                List<NoPrioritiesReportModel> employeesWithoutPriorities = getEmployeesWithoutPriorities(Convert.ToInt32(year),Convert.ToInt32(quarter));
                report.LocalReport.DataSources.Add(new ReportDataSource("noPrioritiesReport", employeesWithoutPriorities));
                report.LocalReport.ReportPath = Server.MapPath("~/Reporting/NoPrioritiesReport.rdlc");
            }
        }

        public static List<NoPrioritiesReportModel> getEmployeesWithoutPriorities(Nullable<int> year, Nullable<int> quarter)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {

                EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
                List<EmployeeActiveDirectoryManager.UserData> userData = employeeActiveDirectoryManager.GetUserList();

                //const int UNASSIGNED_OBJECTIVE_CATEGORY_ID = 4;

                if (year == null || year == 0) year = Convert.ToInt16(DateTime.Now.Year);
                if (quarter == null || quarter == 0) quarter = getQuarter(DateTime.Now);

                var resourcesIdWithPriorities = context.Objectives.Where(x => x.Year == year && x.Quarter == quarter && x.Objective1 != "Unassign Objectives").Select(x => x.ResourceId);
               
                

                //Obtain Resources
                var allResourcesIDs = context.Resources.Select(x => new { x.ResourceId });

                var employeesIDWithoutPriorities = context.Resources
                    .Where(x => !resourcesIdWithPriorities.Contains(x.ResourceId))
                    .Select(x => new NoPrioritiesReportModel
                    {
                        ResourceId = x.ResourceId,
                        ActiveDirectoryID = x.ActiveDirectoryId,
                        //Lastlogin = x.LastLogin,
                        Name = x.Name,
                        DomainName = x.DomainName
                    });


                int val = employeesIDWithoutPriorities.Count();


                List<NoPrioritiesReportModel> employeesWithoutPriorities = new List<NoPrioritiesReportModel>();


                foreach (var noPriorityEmployee in employeesIDWithoutPriorities)
                {
                    noPriorityEmployee.Manager = GetManagerR(noPriorityEmployee.DomainName, userData);

                    if (GoalTrackingController.ExclusionList.Any(x => x.Contains(noPriorityEmployee.DomainName)))
                        continue;

                    if (GoalTrackingController.GetIfIsActiveEmployeeFromAD(noPriorityEmployee.DomainName))
                        employeesWithoutPriorities.Add(noPriorityEmployee);
                }

                return employeesWithoutPriorities.OrderBy(x=>x.Name).ToList();
            }
        }




        public static bool IsEnabledR(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userData)
        {
 
            EmployeeActiveDirectoryManager.UserData user = userData.SingleOrDefault(u => string.Compare(u.DomainName, domainName, StringComparison.OrdinalIgnoreCase) == 0);
            if (user == null)
            {
                return false;
            }

            return
                !user.OperationalUnits.Any(
                    ou =>
                    Constants.ExcludedOperationUnits.Any(eou => string.Compare(ou, eou, StringComparison.OrdinalIgnoreCase) == 0));
        }


        public static string GetManagerR(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userDataList)
        {
            string managerName = userDataList.Where(u => string.Compare(u.DomainName, domainName, StringComparison.OrdinalIgnoreCase) == 0).Select(u => u.Manager).FirstOrDefault();
            if (managerName == null) return string.Empty;

            return managerName.GetNameFromCommonNameMatch();
        }


        private static int getQuarter(DateTime date)
        {
            int month = date.Month;
            int quarter = 0;

            if (month >= 1 && month <= 3)
            {
                quarter = 1;
            }
            else if (month >= 4 && month <= 6)
            {
                quarter = 2;
            }
            else if (month >= 7 && month <= 9)
            {
                quarter = 3;
            }
            else if (month >= 10 && month <= 12)
            {
                quarter = 4;
            }

            return quarter;
        }
    }
}