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
    public partial class TDUDetailsReport : ProgressReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                string year = Request.QueryString["year"];
                string quarter = Request.QueryString["quarter"];
                if (Request.QueryString["mode"] != null)
                {
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "Application/pdf";
                    Response.AddHeader("content-disposition", "attachment; filename=TDUReport - " + DateTime.Now.ToString("MM-dd-yyyy") + ".pdf");
                }
                if (string.IsNullOrWhiteSpace(year)) year = "";
                if (string.IsNullOrWhiteSpace(quarter)) quarter = "";
                DateTime minDate;
                DateTime maxDate;
                int uQuarter;
                if (!int.TryParse(quarter, out uQuarter)) uQuarter = getQuarter(DateTime.Now);
                getMinMaxMonths(quarter, year, out minDate, out maxDate);
                List<TDUDetailModel> updatedData = GetEmployeeGoalDetails(quarter,year);
                report.LocalReport.DataSources.Add(new ReportDataSource("TDUDetailDataset", updatedData.Where(d => !string.IsNullOrWhiteSpace(d.Manager))));
                report.LocalReport.ReportPath = Server.MapPath("~/Reporting/TDUDetailsReport.rdlc");
                
                report.DataBind();
        }
        }
        private List<TDUDetailModel> GetEmployeeGoalDetails(string quarter, string year)
        {
            DateTime minDate = DateTime.Now;
            DateTime maxDate = DateTime.Now;
            int uQuarter;
            int uYear;
            if (!int.TryParse(quarter, out uQuarter)) uQuarter = getQuarter(DateTime.Now);
            if (!int.TryParse(year, out uYear)) uYear = DateTime.Now.Year;

            getMinMaxMonths(quarter, year, out minDate, out maxDate);

            EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
            List<EmployeeActiveDirectoryManager.UserData> userData = employeeActiveDirectoryManager.GetUserList();

            IEnumerable<dynamic> allData = GetGroupingMonthlyTDUDetails(maxDate, minDate);

            List<TDUDetailModel> updatedData = allData.Select(data =>
                                             new Models.TDUDetailModel
                                             {
                                                 DomainName = data.DomainName,
                                                 Location = GetLocation(data.DomainName, userData),
                                                 ResourceId = data.ResourceId,
                                                 Objective = data.Objective,
                                                 TrainingCategory = data.TrainingCategory,
                                                 EmployeeLocation = data.EmployeeLocation,
                                                 Manager = GetManager(data.DomainName, userData),
                                                 ManagerLocation = data.ManagerLocation,
                                                 Verified = data.Verified,
                                                 Project = GetProject(data.DomainName, userData),
                                                 Progress = data.Progress,
                                                 Employee = data.Employee,
                                                 Goal = data.Goal,
                                                 FinishDate = data.FinishDate,
                                                 TDU = data.TDU
                                             }).Where(data => IsEnabled(data.DomainName, userData)).ToList();

            return updatedData;
        }
    }
}