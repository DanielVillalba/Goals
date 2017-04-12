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
    public partial class TeamMembersInputReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string manager = Request.QueryString["manager"];
                string complete = Request.QueryString["complete"];
                string year = Request.QueryString["year"];
                string quarter = Request.QueryString["quarter"];
                List<TeamMemberInputReportModel> employeesTeamMemberInputStatus = getEmployeesTeamMemberInputStatus(manager, complete, Convert.ToInt32(year),Convert.ToInt32(quarter));
                report.LocalReport.DataSources.Add(new ReportDataSource("TeamMembersInputList", employeesTeamMemberInputStatus));
                report.LocalReport.ReportPath = Server.MapPath("~/Reporting/TeamMembersInputReport.rdlc");
            }
        }

        public static List<TeamMemberInputReportModel> getEmployeesTeamMemberInputStatus(string manager,string completed, Nullable<int> year, Nullable<int> quarter)
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
                //Get surveys type Team Member's Input
                surveys = context.Survey.Where(s => s.Year == year && s.Quarter == quarter && s.SurveyType == 1);


                //Obtain Resources

                var resources = context.Resources;
                IQueryable<Resource> resourcesFromManager;
                if (manager.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                {
                    resourcesFromManager = resources;
                }
                else
                {
                    //var managers = from mng in 
                    resourcesFromManager = from r in resources
                                           join mngr in context.Resources on r.Employee.ManagerId equals mngr.ActiveDirectoryId
                                           where mngr.Name.Equals(manager, StringComparison.InvariantCultureIgnoreCase)
                                           select r;
                }

                //IQueryable<TeamMemberInputReportModel> employeesTeamMeberStatus = Enumerable.Empty<TeamMemberInputReportModel>().AsQueryable();
                List<TeamMemberInputReportModel> employeesTeamMeberStatusList = new List<TeamMemberInputReportModel>();
                List<Survey> surveyList = surveys.ToList();
                if (surveyList.Count > 0)
                {
                    string surveyName = surveyList[0].Name;
                    int surveryId = surveyList[0].SurveyId;
                    var employeesTeamMeberStatus =
                        from resource in resourcesFromManager
                        join mang in context.Resources on resource.Employee.ManagerId equals mang.ActiveDirectoryId into mang_
                        from mangr in mang_.DefaultIfEmpty()
                        join surveyResource in context.SurveyResource on resource.ResourceId equals surveyResource.ResourceId into surveyResource_
                        from sr in surveyResource_.DefaultIfEmpty()
                            //join survey in surveys on sr.SurveyId equals survey.SurveyId into survey_
                            //from r in survey_.DefaultIfEmpty()
                                                where sr.SurveyId == surveryId &&
                            (sr != null && completed == "Yes") || (sr == null && completed == "No") || (completed == "All")
                        select
                        new TeamMemberInputReportModel
                        {
                            ResourceId = resource.ResourceId,
                            _DateAnswered = sr.DateAnswered,
                            Name = resource.Name,
                            SurveyName = surveyName,
                            DomainName = resource.DomainName,
                            Manager = mangr.Name,
                            Completed = (sr != null) ? "Yes" : "No"
                        };
                    if (surveyList.Count > 1)
                    {
                        for (int i = 1; i < surveys.Count(); i++)
                        {
                            surveyName = surveyList[i].Name;
                            surveryId = surveyList[i].SurveyId;
                            employeesTeamMeberStatus = employeesTeamMeberStatus.Union(
                                from resource in resourcesFromManager
                                join mang in context.Resources on resource.Employee.ManagerId equals mang.ActiveDirectoryId into mang_
                                from mangr in mang_.DefaultIfEmpty()
                                join surveyResource in context.SurveyResource on resource.ResourceId equals surveyResource.ResourceId into surveyResource_
                                from sr in surveyResource_.DefaultIfEmpty()
                                    //join survey in surveys on sr.SurveyId equals survey.SurveyId into survey_
                                    //from r in survey_.DefaultIfEmpty()
                            where sr.SurveyId == surveryId &&
                                (sr != null && completed == "Yes") || (sr == null && completed == "No") || (completed == "All")
                                select
                                new TeamMemberInputReportModel
                                {
                                    ResourceId = resource.ResourceId,
                                    _DateAnswered =  sr.DateAnswered,
                                    Name = resource.Name,
                                    SurveyName = surveyName,
                                    DomainName = resource.DomainName,
                                    Manager = mangr.Name,
                                    Completed = (sr != null) ? "Yes" : "No"
                                });
                            //employeesTeamMeberStatus = employeesTeamMeberStatus.Union(employeesTeamMeberStatus);
                        }
                    }

                    employeesTeamMeberStatusList = employeesTeamMeberStatus.OrderBy(x => x.Name).ToList();
                    employeesTeamMeberStatusList.ForEach(sr => sr.DateAnswered = sr._DateAnswered == null || sr._DateAnswered == default(DateTime) ? string.Empty : sr._DateAnswered.Value.ToShortDateString());
                    int val = employeesTeamMeberStatusList.Count;
                }
                

                return employeesTeamMeberStatusList;
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