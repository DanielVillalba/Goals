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
    public class TrainingProgramVisitsReport : ProgressReportBase
    {
        protected global::System.Web.UI.HtmlControls.HtmlForm form1;
        protected global::System.Web.UI.ScriptManager ScriptManager1;
        protected global::Microsoft.Reporting.WebForms.ReportViewer report;
        private const int GracePeriodDays = 5;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["mode"] != null)
                {
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "Application/pdf";
                    Response.AddHeader("content-disposition", "attachment; filename=TrainingProgramVisitsReport - " + DateTime.Now.ToString("MM-dd-yyyy") + ".pdf");

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
                if (string.IsNullOrWhiteSpace(dateEndString) || !DateTime.TryParse(dateEndString, out maxDate) || maxDate <= minDate)
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

                //julio escoboza

                string managerDomainName = Request.QueryString["mdn"];

                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var data = from Training in context.TrainingProgramVisitsView
                               where Training.VisitDate >= minDate && Training.VisitDate <= maxDate
                               select new { Training.Name, Training.Resource, Training.Visits, Training.Category};
                    data.ToList();
                    DataTable dtTP = new DataTable();
                    dtTP = new GeneralTrainingDataSet.GeneralTrainingProgramVisitsViewDataTable();
                    foreach (var visit in data)
                    {
                        DataRow row = dtTP.NewRow();
                        row["Name"] = visit.Name.ToString();
                        row["Category"] = visit.Category.ToString();
                        row["Resource"] = visit.Resource.ToString();
                        row["Visits"] = Convert.ToInt16(visit.Visits);
                        
                        dtTP.Rows.Add(row);
                    }
                    DataTable dt1 = new DataTable();
                    dt1 = dtTP;
                    report.LocalReport.DataSources.Add(new ReportDataSource("TrainingProgramDS", dt1));

                    var dataGeneral = from GeneralTraining in context.GeneralTrainingProgramVisitsView
                                      where GeneralTraining.VisitDate >= minDate && GeneralTraining.VisitDate <= maxDate
                                      select new { GeneralTraining.Name, GeneralTraining.Resource, GeneralTraining.Visits, GeneralTraining.Category };
                    dataGeneral.ToList();
                    DataTable dtGTP = new DataTable();
                    dtGTP = new GeneralTrainingDataSet.GeneralTrainingProgramVisitsViewDataTable();
                    foreach (var visit in dataGeneral)
                    {
                        DataRow row = dtGTP.NewRow();
                        row["Name"] = visit.Name.ToString();
                        row["Category"] = visit.Category.ToString();
                        row["Resource"] = visit.Resource.ToString();
                        row["Visits"] = Convert.ToInt16(visit.Visits);
                        dtGTP.Rows.Add(row);
                    }
                    DataTable dt2 = new DataTable();
                    dt2 = dtGTP;
                    report.LocalReport.DataSources.Add(new ReportDataSource("GeneralTrainingDS", dt2));
                    var dataOnDemand = from onDemand in context.TrainingProgramOnDemandVisitsView
                                       where onDemand.VisitDate >= minDate && onDemand.VisitDate <= maxDate
                                       select new { onDemand.Name, onDemand.Resource, onDemand.Visits, onDemand };
                    data.ToList();
                    DataTable dtTPOD = new DataTable();
                    dtTPOD = new GeneralTrainingDataSet.GeneralTrainingProgramVisitsViewDataTable();
                    foreach (var visit in data)
                    {
                        DataRow row = dtTPOD.NewRow();
                        row["Name"] = visit.Name.ToString();
                        row["Resource"] = visit.Resource.ToString();
                        row["Visits"] = Convert.ToInt16(visit.Visits);
                        dtTPOD.Rows.Add(row);
                    }
                    DataTable dt3 = new DataTable();
                    dt3 = dtTP;
                    report.LocalReport.DataSources.Add(new ReportDataSource("TrainingProgramOnDemandDS", dt3));

                    string reportTitle = "Training Program Visits Report from " + minDate.ToShortDateString() + " to " + maxDate.ToShortDateString();


                    report.LocalReport.ReportPath = Server.MapPath("~/Reporting/TrainingProgramVisitsReport.rdlc");
                    var parameters = new ReportParameterCollection
                                     {
                                         new ReportParameter("ReportTitle", reportTitle)
                                     };
                    report.LocalReport.SetParameters(parameters);
                    report.DataBind();
                }
            }
        }
    }
}