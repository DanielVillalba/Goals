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
    public partial class ManagersCheckReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string manager = Request.QueryString["manager"];
                string complete = Request.QueryString["complete"];
                string year = Request.QueryString["year"];
                string quarter = Request.QueryString["quarter"];
                List<ManagerCheckReportModel> employeesTeamMemberInputStatus = GetManagerCheckReportStatus(manager, complete, Convert.ToInt32(year), Convert.ToInt32(quarter));
                report.LocalReport.DataSources.Add(new ReportDataSource("ManagersCheckList", employeesTeamMemberInputStatus));
                report.LocalReport.ReportPath = Server.MapPath("~/Reporting/ManagersCheckReport.rdlc");
            }
        }

        public static List<ManagerCheckReportModel> GetManagerCheckReportStatus(string manager, string completed, Nullable<int> year, Nullable<int> quarter)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {

                EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
                List<EmployeeActiveDirectoryManager.UserData> userData = employeeActiveDirectoryManager.GetUserList();

                //const int UNASSIGNED_OBJECTIVE_CATEGORY_ID = 4;

                IQueryable<Survey> surveys;

                if (year == null || year == 0) year = Convert.ToInt16(DateTime.Now.Year);
                if (quarter == null || quarter == 0) quarter = getQuarter(DateTime.Now);
                if (string.IsNullOrWhiteSpace(manager)) manager = "All";
                if (string.IsNullOrWhiteSpace(completed)) completed = "All";
                //Get surveys type Managers Check Report
                surveys = context.Survey.Where(s => s.Year == year && s.Quarter == quarter && s.SurveyType == 2);


                //Obtain Resources

                var managers = from mngr 
                               in context.Resources
                               where context.Employee.Select(e => e.ManagerId).Contains(mngr.ActiveDirectoryId)
                               select mngr;
                IQueryable<Resource> filterManager;
                if (manager.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                {
                    filterManager = managers;
                }
                else
                {
                    //var managers = from mng in 
                    filterManager = from m in managers
                                      where m.Name.Equals(manager, StringComparison.InvariantCultureIgnoreCase)
                                           select m;
                }

                //IQueryable<ManagerCheckReportModel> managersCheckStatus = Enumerable.Empty<ManagerCheckReportModel>().AsQueryable();
                List<ManagerCheckReportModel> managersCheckStatusList = new List<ManagerCheckReportModel>();
                List<Survey> surveyList = surveys.ToList();
                if (surveyList.Count > 0)
                {
                    string surveyName = surveyList[0].Name;
                    int surveryId = surveyList[0].SurveyId;
                    var managersCheckStatus =
                        from manag in filterManager
                        join emp in context.Employee on manag.ActiveDirectoryId equals emp.ManagerId 
                        //from emp in manag.Employees
                        join surveyResource in context.SurveyResource 
                        on 
                        //new { a = emp.ResourceId } equals new { a = surveyResource.ResourceEvaluatedId }
                        emp.ResourceId equals surveyResource.ResourceEvaluatedId  
                        into surveyResource_
                        from sr in surveyResource_.DefaultIfEmpty()
                        //join survey in surveys on sr.SurveyId equals survey.SurveyId into survey_
                            //from r in survey_.DefaultIfEmpty()
                        where (sr == null || sr.SurveyId == surveryId)
                        && (sr != null && completed == "Yes") || (sr == null && completed == "No") || (completed == "All")
                        && (sr == null || manag.ResourceId == sr.ResourceId )
                        select
                        new ManagerCheckReportModel
                        {
                            Manager =  manag.Name
                            ,ResourceId = manag.ResourceId
                            ,_DateAnswered = sr.DateAnswered
                            ,DomainName = manag.DomainName
                            ,EvaluatedEmployee = emp.Resource.Name
                            ,SurveyName = surveyName
                            ,Completed = (sr != null) ? "Yes" : "No"

                        };
                    if (surveyList.Count > 1)
                    {
                        for (int i = 1; i < surveys.Count(); i++)
                        {
                            surveyName = surveyList[i].Name;
                            surveryId = surveyList[i].SurveyId;
                            managersCheckStatus =
                        from manag in filterManager
                        from emp in manag.Employees
                        join surveyResource in context.SurveyResource
                        on
                        //new { a = emp.ResourceId } equals new { a = surveyResource.ResourceEvaluatedId }
                        emp.ResourceId equals surveyResource.ResourceEvaluatedId
                        into surveyResource_
                        from sr in surveyResource_.DefaultIfEmpty()
                            //join survey in surveys on sr.SurveyId equals survey.SurveyId into survey_
                            //from r in survey_.DefaultIfEmpty()
                        where (sr == null || sr.SurveyId == surveryId)
                        && (sr != null && completed == "Yes") || (sr == null && completed == "No") || (completed == "All")
                        && (sr == null || manag.ResourceId == sr.ResourceId)
                        select
                        new ManagerCheckReportModel
                        {
                            Manager = manag.Name
                            , ResourceId = manag.ResourceId
                            ,_DateAnswered = sr.DateAnswered
                            , DomainName = manag.DomainName
                            , Completed = (sr != null) ? "Yes" : "No"

                        };
                        }
                    }
                    managersCheckStatusList = managersCheckStatus.OrderBy(x => x.Manager).ToList();
                    managersCheckStatusList.ForEach(sr => sr.DateAnswered = sr._DateAnswered == null || sr._DateAnswered == default(DateTime) ? string.Empty : sr._DateAnswered.Value.ToShortDateString());
                }
                
                int val = managersCheckStatusList.Count;

                return managersCheckStatusList;
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