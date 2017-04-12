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
    public partial class TrendByLevelReport : ProgressReportBase
    {
        //private const int GracePeriodDays = 5;

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
                        Response.AddHeader("content-disposition", "attachment; filename=TrendByLevelReport - " + DateTime.Now.ToString("MM-dd-yyyy") + ".pdf");
                    }
                    DateTime minDate;
                    string dateString = Request.QueryString["date"];
                    //if date value not passed, or if passed but it is not a date, then use default
                    if (string.IsNullOrWhiteSpace(dateString) || !DateTime.TryParse(dateString, out minDate))
                    {
                        minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    }
                    //DateTime minDateWithGracePeriod = minDate.AddDays(GracePeriodDays);

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
                    //maxMonthDate.DateTime maxDateWithGracePeriod = maxMonthDate.AddDays(GracePeriodDays);

                    EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
                    List<EmployeeActiveDirectoryManager.UserData> userData = employeeActiveDirectoryManager.GetUserList();

                    string managerDomainName = Request.QueryString["mdn"];
                    GoalTrackingController goal = new GoalTrackingController();
                    int quarter = goal.GetQuarter(maxMonthDate.Month);
                    int year = maxMonthDate.Year;
                    using (CDPTrackEntities context = new CDPTrackEntities())
                    {
                        var data = from cdp in context.CDPView
                                   where cdp.Year == year && cdp.Quarter == quarter
                                   select new { cdp.PositionName, cdp.CDP, cdp.Name, cdp.Quarter, cdp.Year };
                        data.ToList();
                        DataTable dtTBL = new DataTable();
                        dtTBL = new TBLDataSet.CDPViewDataTable();
                        foreach (var position in data)
                        {
                            DataRow row = dtTBL.NewRow();
                            row["PositionName"] = position.PositionName.ToString();
                            row["CDP"] = Convert.ToInt16(position.CDP);
                            row["Name"] = position.Name.ToString();
                            row["Year"] = Convert.ToInt16(position.Year);
                            row["Quarter"] = Convert.ToInt16(position.Quarter);
                            dtTBL.Rows.Add(row);
                        }
                        DataTable dt = new DataTable();
                        dt = dtTBL;
                        report.LocalReport.DataSources.Add(new ReportDataSource("TBLDataSet", dt));

                        string reportTitle = "Trend By Level Report. Quarter: " + quarter.ToString() + ". Year: " + year.ToString();
                        

                        report.LocalReport.ReportPath = Server.MapPath("~/Reporting/TrendByLevelReport.rdlc");
                        var parameters = new ReportParameterCollection
                                     {
                                         new ReportParameter("ReportTitle", reportTitle)
                                     };
                        report.LocalReport.SetParameters(parameters);
                        report.DataBind();
                    }
                }
                catch (Exception)
                { }           
            }
        }
    }
}