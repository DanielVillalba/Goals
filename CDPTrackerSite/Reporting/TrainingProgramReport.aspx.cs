using CDPTrackerSite.Controllers;
using CDPTrackerSite.Models;
using DataSource;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;

namespace CDPTrackerSite.Reporting
{
    public partial class TrainingProgramReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var TrainingPrograms = getTrainings();
                report.LocalReport.DataSources.Add(new ReportDataSource("TrainingPrograms", TrainingPrograms));
                report.LocalReport.ReportPath = Server.MapPath("~/Reporting/TrainingProgramReport.rdlc");
            }
        }

        public static List<TrainingProgramReportModel> getTrainings()
        {
            List<TrainingProgramReportModel> list = new List<TrainingProgramReportModel>();

            EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
            List<EmployeeActiveDirectoryManager.UserData> userData = employeeActiveDirectoryManager.GetUserList();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                //Lista de General training Programa
                var GeneralTrainingPrograms = context.GeneralTrainingProgramDetails.ToList();
                var GeneralTrainingProgramsFiltered = GeneralTrainingPrograms.Where(x => IsEnabledR(x.Resource.DomainName, userData)).Select(data =>
                        new TrainingProgramReportModel
                        {
                            location = data.Resource.Location.Name,
                            type = "General Training Program",
                            manager = GetManagerR(data.Resource.DomainName, userData),
                            employee = data.Resource.DomainName,
                            category = data.GeneralTrainingProgram.TrainingProgramCategory.Name,
                            completed = data.Resource.GeneralTrainingProgramDetails.Where(x => x.Status == 2 && x.GeneralTrainingProgram.TrainingProgramCategory.Name == data.GeneralTrainingProgram.TrainingProgramCategory.Name).Count(),
                            total = data.Resource.GeneralTrainingProgramDetails.Where(x => x.GeneralTrainingProgram.TrainingProgramCategory.Name == data.GeneralTrainingProgram.TrainingProgramCategory.Name).Count()
                        }
                    );

                list = list.Union(GeneralTrainingProgramsFiltered).ToList();

                //Lista de Training Programs
                var TrainingPrograms = context.TrainingProgramDetails.ToList();
                var TrainingProgramsFiltered = TrainingPrograms.Where(x => IsEnabledR(x.Resource.DomainName, userData)).Select(data =>
                    new TrainingProgramReportModel
                    {
                        location = data.Resource.Location.Name,
                        type = "Training Program",
                        manager = GetManagerR(data.Resource.DomainName, userData),
                        employee = data.Resource.DomainName,
                        category = data.TrainingProgram.TrainingProgramCategory.Name,
                        completed = data.Resource.TrainingProgramDetails.Where(x => x.Status == 2 && x.TrainingProgram.TrainingProgramCategory.Name == data.TrainingProgram.TrainingProgramCategory.Name).Count(),
                        total = data.Resource.TrainingProgramDetails.Where(x => x.TrainingProgram.TrainingProgramCategory.Name == data.TrainingProgram.TrainingProgramCategory.Name).Count()
                    });

                list = list.Union(TrainingProgramsFiltered).ToList();

                //Lista de Training Programs On Demand
                var TrainingProgramOnDemand = context.TrainingProgramOnDemandDetails.ToList();
                var TrainingProgramOnDemandFiltered = TrainingProgramOnDemand.Where(x => IsEnabledR(x.Resource.DomainName, userData)).Select(data =>
                    new TrainingProgramReportModel
                    {
                        location = data.Resource.Location.Name,
                        type = "Training Program On Demand",
                        manager = GetManagerR(data.Resource.DomainName, userData),
                        employee = data.Resource.DomainName,
                        category = "No category",
                        completed = data.Resource.TrainingProgramOnDemandDetails.Where(x => x.Status == 2).Count(),
                        total = data.Resource.TrainingProgramOnDemandDetails.Count()
                    });

                list = list.Union(TrainingProgramOnDemandFiltered).ToList();

                return list;




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
    }
}