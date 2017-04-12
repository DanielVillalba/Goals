using CDPTrackerSite.Controllers;
using DataSource;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Utils;

namespace CDPTrackerSite.Reporting
{
    public partial class ProgressByProjectReport : ProgressReport
    {
        private const int GracePeriodDays = 5;
        public static IEnumerable projects;
        public static IEnumerable areas;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var employeesGoalsByProject = getEmployeesGoalsByProject();
                report.LocalReport.DataSources.Add(new ReportDataSource("ProgressByProject", employeesGoalsByProject));
                report.LocalReport.ReportPath = Server.MapPath("~/Reporting/ProgressByProjectReport.rdlc");
               

            }
        }

        public IEnumerable getEmployeesGoalsByProject()
        {
            /*
                if (Request.QueryString["mode"] != null)
                {
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "Application/pdf";
                    Response.AddHeader("content-disposition", "attachment; filename=ProgressReport - " + DateTime.Now.ToString("MM-dd-yyyy") + ".pdf");
                }
                DateTime minDate;
                string dateString = Request.QueryString["date"];
             * 
                //if date value not passed, or if passed but it is not a date, then use default
                if (string.IsNullOrWhiteSpace(dateString) || !DateTime.TryParse(dateString, out minDate))
                {
                    minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
                DateTime minDateWithGracePeriod = minDate.AddDays(GracePeriodDays);

                DateTime maxDate;
                string dateEndString = Request.QueryString["dateEnd"];
                //if date value not passed, or if passed but it is not a date, then use default
             * */
                DateTime minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime maxDate;
                DateTime minDateWithGracePeriod = minDate.AddDays(GracePeriodDays);

                maxDate = minDate.AddMonths(1).AddMilliseconds(-1);



                //obtain the max date-time of the current month, or the month requested
                DateTime maxMonthDate = maxDate;
                DateTime maxDateWithGracePeriod = maxMonthDate.AddDays(GracePeriodDays);

                EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
                List<EmployeeActiveDirectoryManager.UserData> userData = employeeActiveDirectoryManager.GetUserList();

                string managerDomainName = null;
                
                IEnumerable<dynamic> groupingQuery = GetGroupingQueryByProjectGoals(managerDomainName, maxMonthDate, minDate, minDateWithGracePeriod,
                                                 maxDateWithGracePeriod);


                var updatedDataGrouping = groupingQuery.Select(data=>
                                                        new
                                                        {
                                                            data.DomainName,
                                                            Employee = data.DomainName,
                                                            data.Location,
                                                            data.ResourceId,
                                                            data.Name,
                                                            Manager = GetManager(data.DomainName, userData),
                                                            data.Goal,
                                                            Project = GetProject(data.DomainName, userData),
                                                            Verified = Convert.ToInt32(data.Verified),
                                                            CompletedDate = data.FinishDate ?? "NO ESTABLISHED",
                                                            Area = GetArea(data.DomainName, userData)
                                                        }).Where(data => IsEnabled(data.DomainName, userData)).ToList();

                projects = updatedDataGrouping.GroupBy(x => x.Project).Select(x => x.Key);
                areas = updatedDataGrouping.GroupBy(x => x.Area).Select(x => x.Key);
                
                return updatedDataGrouping;

        }

        public static string GetManager(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userDataList)
        {
            string managerName = userDataList.Where(u => string.Compare(u.DomainName, domainName, StringComparison.OrdinalIgnoreCase) == 0).Select(u => u.Manager).FirstOrDefault();
            if (managerName == null) return string.Empty;

            return managerName.GetNameFromCommonNameMatch().ToString();
        }
    }
}