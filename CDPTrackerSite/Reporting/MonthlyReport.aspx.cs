using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Reporting.WebForms;
using Utils;
using System.Configuration;
using CDPTrackerSite.RoleManagement;
using CDPTrackerSite.Controllers;

namespace CDPTrackerSite.Reporting
{
#if !DEBUG
    [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
    public partial class MonthlyReport : ProgressReportBase
    {
        private const int GracePeriodDays = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Request.QueryString["mode"] != null)
                    {
                        Response.Buffer = true;
                        Response.Clear();
                        Response.ContentType = "Application/pdf";
                        Response.AddHeader("content-disposition", "attachment; filename=MonthlyReport - " + DateTime.Now.ToString("MM-dd-yyyy") + ".pdf");
                    }
                    DateTime minDate;
                    string dateString = Request.QueryString["date"];
                    //if date value not passed, or if passed but it is not a date, then use default
                    if (string.IsNullOrWhiteSpace(dateString) || !DateTime.TryParse(dateString, out minDate))
                    {
                        minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    }
                    DateTime minDateWithGracePeriod = minDate.AddDays(GracePeriodDays);

                    DateTime maxDate;
                    string dateEndString = Request.QueryString["dateEnd"];
                    //if date value not passed, or if passed but it is not a date, then use default
                    if (string.IsNullOrWhiteSpace(dateEndString) || !DateTime.TryParse(dateEndString, out maxDate))
                    {
                        maxDate = minDate.AddMonths(1).AddMilliseconds(-1);
                    }
                    else
                    {
                        maxDate = maxDate.AddMonths(1).AddMilliseconds(-1);
                    }

                    //obtain the max date-time of the current month, or the month requested
                    DateTime maxMonthDate = maxDate;
                    DateTime maxDateWithGracePeriod = maxMonthDate.AddDays(GracePeriodDays);

                    EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
                    List<EmployeeActiveDirectoryManager.UserData> userData = employeeActiveDirectoryManager.GetUserList();

                    string managerDomainName = Request.QueryString["mdn"];

                    IEnumerable<dynamic> allData = GetGroupingMonthlyQuery(managerDomainName, maxMonthDate, minDate);


                    var updatedData = allData.Select(data =>
                                                     new
                                                     {
                                                         data.DomainName,
                                                         Location = GetLocation(data.DomainName, userData),
                                                         data.ResourceId,
                                                         data.Name,
                                                         Manager = GetManager(data.DomainName, userData),
                                                         data.Verified,
                                                         Project = GetProject(data.DomainName, userData),
                                                         data.Progress,
                                                         data.Employee,
                                                         data.Goal,
                                                         data.FinishDate

                                                     }).Where(data => IsEnabled(data.DomainName, userData)).ToList();

                    report.LocalReport.DataSources.Add(new ReportDataSource("GoalsView", updatedData.Where(d => !string.IsNullOrWhiteSpace(d.Manager))));

                    string reportTitle;
                    if (minDate.Month == maxDate.Month)
                    {
                        reportTitle = "Monthly goals completed by " +
                                      maxMonthDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + maxMonthDate.Year;
                    }
                    else
                    {
                        reportTitle = "Monthly goals completed from " +
                            minDate.ToString("MMMM", CultureInfo.InvariantCulture) + (minDate.Year != maxDate.Year ? " " + minDate.Year : string.Empty) +
                                      " to " +
                                      maxDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + maxDate.Year;
                    }

                    report.LocalReport.ReportPath = Server.MapPath("~/Reporting/MonthlyReport.rdlc");
                    var parameters = new ReportParameterCollection
                                     {
                                         new ReportParameter("ReportTitle", reportTitle)
                                     };
                    report.LocalReport.SetParameters(parameters);
                    report.DataBind();
                }
                catch (Exception)
                { }
            }
        }
    }
}