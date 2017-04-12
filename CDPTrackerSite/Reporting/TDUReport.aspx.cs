using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Reporting.WebForms;
using Utils;
using System.Configuration;
using CDPTrackerSite.RoleManagement;
using CDPTrackerSite.Controllers;
using CDPTrackerSite.Models;

namespace CDPTrackerSite.Reporting
{
    public partial class TDUReport : ProgressReportBase
    {
        private const int GracePeriodDays = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string year = Request.QueryString["year"];
                string quarter = Request.QueryString["quarter"];
                string managerDomainName = Request.QueryString["mdn"];
                /*try
                {*/
                    if (Request.QueryString["mode"] != null)
                    {
                        Response.Buffer = true;
                        Response.Clear();
                        Response.ContentType = "Application/pdf";
                        Response.AddHeader("content-disposition", "attachment; filename=TDUReport - " + DateTime.Now.ToString("MM-dd-yyyy") + ".pdf");
                   }

                    if (string.IsNullOrWhiteSpace(year)) year = "";
                    if (string.IsNullOrWhiteSpace(quarter)) quarter = "";
                    if (string.IsNullOrWhiteSpace(managerDomainName)) managerDomainName = "";

                    DateTime minDate;
                    DateTime maxDate;
                    int uQuarter;
                    List<TDUDetailModel> updatedData = new List<TDUDetailModel>();
                    if (!int.TryParse(quarter, out uQuarter)) uQuarter = getQuarter(DateTime.Now);
                    getMinMaxMonths(quarter, year, out minDate, out maxDate);

                    if (managerDomainName != "" || managerDomainName == string.Empty)
                        updatedData = GetEmployeesWithVerifiedTDU(quarter, year, string.Empty);
                    else
                        updatedData = GetEmployeesWithVerifiedTDU(quarter, year, managerDomainName);

                    report.LocalReport.DataSources.Add(new ReportDataSource("TDUDataSet1", updatedData.Where(d => !string.IsNullOrWhiteSpace(d.Manager))));
                    
                    #region Generate Title based on Date
                        string reportTitle;
                        if (minDate.Month == maxDate.Month)
                        {
                            reportTitle = "Tiempo Development Units earned by " +
                                          maxDate.ToString("MMMM", CultureInfo.InvariantCulture) + " " + maxDate.Year;
                        }
                        else
                        {
                            reportTitle = "Tiempo Development Units earned from Quarter " + uQuarter.ToString() + " - '" +
                                minDate.ToString("MMMM", CultureInfo.InvariantCulture) + (minDate.Year != maxDate.Year ? " " + minDate.Year : string.Empty) +
                                          " to " +
                                          maxDate.ToString("MMMM", CultureInfo.InvariantCulture) + "' " + maxDate.Year;
                        }
                    #endregion
                    report.LocalReport.ReportPath = Server.MapPath("~/Reporting/TDUReport.rdlc");
                    var parameters = new ReportParameterCollection
                                     {
                                         new ReportParameter("ReportTitle", reportTitle)
                                     };
                    report.LocalReport.SetParameters(parameters);
                    report.DataBind();
                /*}
                catch
                    (Exception a)
                {
                }*/
            }
        }

        private List<TDUDetailModel> GetEmployeesWithVerifiedTDU(string quarter, string year, string managerDomainName)
        {
            DateTime minDate = DateTime.Now;
            DateTime maxDate = DateTime.Now;
            int uQuarter;
            int uYear;
            if (!int.TryParse(quarter, out uQuarter)) uQuarter = getQuarter(DateTime.Now);
            if (!int.TryParse(year, out uYear)) uYear = DateTime.Now.Year;

            getMinMaxMonths(quarter, year, out minDate,out maxDate);

            EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
            List<EmployeeActiveDirectoryManager.UserData> userData = employeeActiveDirectoryManager.GetUserList();
            var allData = new List<dynamic>();
            if(managerDomainName == string.Empty)
                allData = GetGroupingMonthlyQuery(string.Empty,maxDate, minDate).ToList();
            else
                allData = GetGroupingMonthlyQuery(managerDomainName, maxDate, minDate).ToList();

            List<TDUDetailModel> updatedData = allData.Select(data =>
                                             new Models.TDUDetailModel
                                             {
                                                 DomainName = data.DomainName,
                                                 Location = GetLocation(data.DomainName, userData),
                                                 Verified = data.Verified,
                                                 ResourceId = data.ResourceId,
                                                 Employee = data.Employee,
                                                 EmployeeLocation = data.EmployeeLocation,
                                                 Manager = data.Manager,
                                                 ManagerLocation = data.ManagerLocation,
                                                 Progress = data.Progress,
                                                 FinishDate = data.FinishDate,
                                                 TDU = data.TDU
                                             }).Where(data => IsEnabled(data.DomainName, userData) && data.Verified == 1 && !(string.IsNullOrEmpty(data.Manager))).ToList();

            return updatedData;
        }

    }
}