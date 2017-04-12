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
#if !DEBUG
    [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
    public partial class KPIReport : System.Web.UI.Page
    {
        private static EmployeeActiveDirectoryManager activeDirectoryInstance;

        protected void Page_Load(object sender, EventArgs e)
        {         
            if (!IsPostBack)
            {
                report.LocalReport.DataSources.Clear();
                report.LocalReport.DataSources.Add(new ReportDataSource("KPIDS", getDataKPI()));
                report.LocalReport.DataSources.Add(new ReportDataSource("KPIManagerDSHMO", createDTForLocation("Hermosillo")));
                report.LocalReport.DataSources.Add(new ReportDataSource("KPIManagerDSMTY", createDTForLocation("Monterrey")));
                report.LocalReport.DataSources.Add(new ReportDataSource("KPIManagerDSPHX", createDTForLocation("Phoenix")));
                report.LocalReport.DataSources.Add(new ReportDataSource("KPIManagerDSGDL", createDTForLocation("Guadalajara")));
                report.LocalReport.ReportPath = Server.MapPath("~/Reporting/KPIReport.rdlc");
            }
        }

        public List<KPIReportModel> getDataKPI()
        {
            int[] dataHMO = getDataForLocation("Hermosillo");
            int[] dataMTY = getDataForLocation("Monterrey");
            int[] dataPHX = getDataForLocation("Phoenix");
            int[] dataGDL = getDataForLocation("Guadalajara");

            List<KPIReportModel> KPI = new List<KPIReportModel>();
            KPIReportModel data = new KPIReportModel
            {
                shortTermGoalsHMO = dataHMO[0],
                middleTermGoalsHMO = dataHMO[1],
                longTermGoalsHMO = dataHMO[2],
                shortToDoHMO = dataHMO[3],
                middleToDoHMO = dataHMO[4],
                longToDoHMO = dataHMO[5],
                shortInProgressHMO = dataHMO[6],
                middleInProgressHMO = dataHMO[7],
                longInProgressHMO = dataHMO[8],
                shortCompletedHMO = dataHMO[9],
                middleCompletedHMO = dataHMO[10],
                longCompletedHMO = dataHMO[11],
                peopleCountHMO = dataHMO[12],
                shortTermGoalsMTY = dataMTY[0],
                middleTermGoalsMTY = dataMTY[1],
                longTermGoalsMTY = dataMTY[2],
                shortToDoMTY = dataMTY[3],
                middleToDoMTY = dataMTY[4],
                longToDoMTY = dataMTY[5],
                shortInProgressMTY = dataMTY[6],
                middleInProgressMTY = dataMTY[7],
                longInProgressMTY = dataMTY[8],
                shortCompletedMTY = dataMTY[9],
                middleCompletedMTY = dataMTY[10],
                longCompletedMTY = dataMTY[11],
                peopleCountMTY = dataMTY[12],
                shortTermGoalsPHX = dataPHX[0],
                middleTermGoalsPHX = dataPHX[1],
                longTermGoalsPHX = dataPHX[2],
                shortToDoPHX = dataPHX[3],
                middleToDoPHX = dataPHX[4],
                longToDoPHX = dataPHX[5],
                shortInProgressPHX = dataPHX[6],
                middleInProgressPHX = dataPHX[7],
                longInProgressPHX = dataPHX[8],
                shortCompletedPHX = dataPHX[9],
                middleCompletedPHX = dataPHX[10],
                longCompletedPHX = dataPHX[11],
                peopleCountPHX = dataPHX[12],
                shortTermGoalsGDL = dataGDL[0],
                middleTermGoalsGDL = dataGDL[1],
                longTermGoalsGDL = dataGDL[2],
                shortToDoGDL = dataGDL[3],
                middleToDoGDL = dataGDL[4],
                longToDoGDL = dataGDL[5],
                shortInProgressGDL = dataGDL[6],
                middleInProgressGDL = dataGDL[7],
                longInProgressGDL = dataGDL[8],
                shortCompletedGDL = dataGDL[9],
                middleCompletedGDL = dataGDL[10],
                longCompletedGDL = dataGDL[11],
                peopleCountGDL = dataGDL[12]
            };
            KPI.Add(data);
            return KPI;
        }

        public static bool GetIfIsActiveEmployeeFromAD(string logonName)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            IList<String> OUFromEmployee;

            try
            {
                //if employee is part of an OU different from Ex-User then is an ActiveEmployee.
                OUFromEmployee = ActiveDirectory.GetEmployeeOrganizationalUnitList(logonName).ToList();

                foreach (var OU in OUFromEmployee)
                {
                    if (OU.Equals("Ex-User") || OU.Equals("Customers"))
                        return false;
                }
            }
            catch (ArgumentException e)
            {
                return false;
            }

            return (OUFromEmployee.Count > 0);
        }

        private static EmployeeActiveDirectoryManager getActiveDirectoryInstance()
        {
            //SINGLETON PATTERN
            if (activeDirectoryInstance == null)
            {
                string activeDirectoryPath = ConfigurationManager.AppSettings["ADPath"];
                string activeDirectoryUser = ConfigurationManager.AppSettings["ADUsr"];
                string activeDirectoryPass = ConfigurationManager.AppSettings["ADPass"];
                activeDirectoryInstance = new EmployeeActiveDirectoryManager(activeDirectoryPath, activeDirectoryUser, activeDirectoryPass);
            }

            return activeDirectoryInstance;
        }

        private static int getQuarterForKPI(DateTime date)
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

        private static int getGoalsProgress(List<Objective> objectives, int progress, int counter)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                if (objectives.Count != 0)
                {
                    int goalProgress = objectives.Select(x => counter += x.GoalTracking.Where(y => y.Progress == progress).Count()).Last();
                    counter = 0;
                    return goalProgress;
                }
                else
                {
                    return 0;
                }
            }
        }

        private static int[] getDataForLocation(string location)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                DateTime dateForKPI = DateTime.Now.AddMonths(-6);
                EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
                List<Resource> resourcesActiveInCDP = new List<Resource>();
                int ActualQuarter = getQuarterForKPI(dateForKPI);
                int ActualYear = dateForKPI.Year;
                var allResources = context.Resources.ToList();

                var userData = ActiveDirectory.GetUserList().Where(x => x.OperationalUnits.Any(OU => OU == "Interns")).ToList();
                foreach (var resource in allResources)
                {
                    if (GetIfIsActiveEmployeeFromAD(resource.DomainName) && !userData.Any(x => x.Email == resource.Email))
                    {
                        //All resources
                        resourcesActiveInCDP.Add(resource);
                    }
                }

                //People with CDP
                var personsWithCDP = context.Resources.Where(x => x.GoalTrackings.Any(y => y.Progress >= 0 && y.FinishDate >= dateForKPI)).ToList();

                //People with CDP (Hermosillo)
                var TotalPersonsWithCDPLocation = personsWithCDP.Where(x => x.Location != null && x.Location.Name == location).ToList();

                //create a list with all the employee objectives
                List<Objective> assessmentListByLocation = new List<Objective>();
                foreach (Resource resource in TotalPersonsWithCDPLocation)
                {
                    assessmentListByLocation.AddRange(resource.Objective.ToList());

                }
                // List of shortTerm assesments  (Short is current 0-6 months)
                List<Objective> shortTermObjectives = new List<Objective>();
                if (ActualQuarter != 4)
                {
                    shortTermObjectives = assessmentListByLocation.Where(x => (x.Quarter == ActualQuarter && x.Year == ActualYear) || (x.Quarter == ActualQuarter + 1 && x.Year == ActualYear)).ToList();
                }
                else
                {
                    // List of all assessments
                    shortTermObjectives = assessmentListByLocation.Where(x => (x.Quarter == ActualQuarter && x.Year == ActualYear) || (x.Quarter == 1 && x.Year == (ActualYear + 1))).ToList();
                }

                // List of long assesments (Long term is 18 months and beyond)
                List<Objective> longTermObjectives = assessmentListByLocation.Where(x => x.Quarter >= ActualQuarter && x.Year == (ActualYear + 2)).ToList();

                //Exclude Elements from long term and short term
                //List of middle assesments  (Middle term is 6-18 months)               
                List<Objective> middleTermObjectives = assessmentListByLocation.Except(longTermObjectives).Except(shortTermObjectives).ToList();
                List<Objective> submiddleTermList = middleTermObjectives.Where(x => x.Quarter < ActualQuarter && x.Year == ActualYear).ToList();
                middleTermObjectives = middleTermObjectives.Except(submiddleTermList).ToList();

                int goals = assessmentListByLocation.Count();

                int counter = 0;
                int shortGoals = 0;
                if (shortTermObjectives.Count() != 0)
                {
                    shortGoals = shortTermObjectives.Select(x => counter += x.GoalTracking.Count).Last();
                    counter = 0;
                }
                else
                {
                    shortGoals = 0;
                }

                int middleGoals = 0;
                if (middleTermObjectives.Count() != 0)
                {
                    middleGoals = middleTermObjectives.Select(x => counter += x.GoalTracking.Count).Last();
                    counter = 0;
                }

                int longGoals = 0;
                if (longTermObjectives.Count() != 0)
                {
                    longGoals = longTermObjectives.Select(x => counter += x.GoalTracking.Count).Last();
                }

                int[] data = { shortGoals, middleGoals, longGoals, getGoalsProgress(shortTermObjectives, 0, 0), getGoalsProgress(middleTermObjectives, 0, 0), getGoalsProgress(longTermObjectives, 0, 0), getGoalsProgress(shortTermObjectives, 1, 0), getGoalsProgress(middleTermObjectives, 1, 0), getGoalsProgress(longTermObjectives, 1, 0), getGoalsProgress(shortTermObjectives, 2, 0), getGoalsProgress(middleTermObjectives, 2, 0), getGoalsProgress(longTermObjectives, 2, 0), TotalPersonsWithCDPLocation.Count() };
                return data;
            }
        }

        private static List<string> getManagersString(string location)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            List<Resource> managersList = new List<Resource>();
            int quarter = getQuarterForKPI(DateTime.Now);
            List<string> managers = new List<string>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var allEmployees = context.Employee.ToList();
                var activeEmployees = allEmployees.Where(x => GetIfIsActiveEmployeeFromAD(x.Resource.DomainName)).ToList();

                foreach (var employee in activeEmployees)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(employee.ManagerId)))
                    {
                        Resource item = context.Resources.Where(x => x.ActiveDirectoryId == employee.ManagerId).FirstOrDefault();
                        managersList.Add(item);
                    }
                }

                var managersFilteredList = managersList.GroupBy(x => x.ResourceId).Select(x => x.First());
                //var locationList = context.Locations.ToList<Location>();

                foreach (var manager in managersFilteredList.Where(x => x.Location.Name == location).OrderBy(x => x.DomainName).ToList())
                {
                    managers.Add(manager.Name);
                }
                return managers;
            }
        }

        private static List<Resource> getDataManagers(string location)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            List<Resource> managersList = new List<Resource>();
            int quarter = getQuarterForKPI(DateTime.Now);
            List<Resource> managers = new List<Resource>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var allEmployees = context.Employee.ToList();
                var activeEmployees = allEmployees.Where(x => GetIfIsActiveEmployeeFromAD(x.Resource.DomainName)).ToList();

                foreach (var employee in activeEmployees)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(employee.ManagerId)))
                    {
                        Resource item = context.Resources.Where(x => x.ActiveDirectoryId == employee.ManagerId).FirstOrDefault();
                        managersList.Add(item);
                    }
                }

                var managersFilteredList = managersList.GroupBy(x => x.ResourceId).Select(x => x.First());
                //var locationList = context.Locations.ToList<Location>();
                managers = managersFilteredList.Where(x => x.Location.Name == location).OrderBy(x => x.DomainName).ToList();
                return managers;
            }
        }         

        private static List<string> getEmployeesCDP(List<Resource> managers, bool output)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            int quarter = getQuarterForKPI(DateTime.Now);
            List<string> CDP = new List<string>();
            List<string> CDPNames = new List<string>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                foreach (var manager in managers)
                {
                    int teamMembers = 0;

                    int teamMemberswithCDP = 0;

                    var team = context.Employee.Where(x => x.ManagerId == manager.ActiveDirectoryId).ToList();

                    foreach (var teamMember in team)
                    {
                        string EmployeeName = teamMember.Resource.DomainName;

                        var OUS = ActiveDirectory.GetEmployeeOrganizationalUnitList(EmployeeName);

                        if (GetIfIsActiveEmployeeFromAD(teamMember.Resource.DomainName) && !OUS.Contains("Interns"))
                        {
                            teamMembers++;
                            var activeGoalTeamMember = context.Objectives
                            .Where(x => x.Quarter == quarter && x.Year == DateTime.Now.Year && x.ResourceId == teamMember.ResourceId && x.Objective1 != "Unassign Objectives")
                            .Select(y => y.GoalTracking.Any(t => t.Progress == 1)).Count();

                            if (activeGoalTeamMember > 0)
                            {
                                CDPNames.Add(teamMember.Resource.Name);
                                teamMemberswithCDP++;
                            }
                        }

                    }
                    CDP.Add(teamMemberswithCDP.ToString());
                }
                if (output)
                {
                    return CDPNames;
                }
                else
                {
                    return CDP;
                }
            }
        }

        private static List<int> getNumberMembers(List<Resource> managers)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            int quarter = getQuarterForKPI(DateTime.Now);
            List<int> members = new List<int>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                foreach (var manager in managers)
                {
                    int teamMembers = 0;
                    var team = context.Employee.Where(x => x.ManagerId == manager.ActiveDirectoryId).ToList();

                    foreach (var teamMember in team)
                    {
                        string EmployeeName = teamMember.Resource.DomainName;

                        var OUS = ActiveDirectory.GetEmployeeOrganizationalUnitList(EmployeeName);

                        if (GetIfIsActiveEmployeeFromAD(teamMember.Resource.DomainName) && !OUS.Contains("Interns"))
                        {
                            teamMembers++;
                        }

                    }
                    members.Add(teamMembers);
                }
                return members;
            }
        }

        private static DataTable createDTForLocation(string location)
        {
            List<string> managers = new List<string>();
            List<string> CDP = new List<string>();
            List<string> namesCDP = new List<string>();
            List<string> namesTrimmed = new List<string>();
            List<int> members = new List<int>();
            DataTable dtKPI = new DataTable();

            switch(location)
            {
                case "Hermosillo":
                    dtKPI = new KPIDataSet.KPIHMODataTable();
                    break;
                case "Monterrey":
                    dtKPI= new KPIDataSet.KPIMTYDataTable();
                    break;
                case "Phoenix":
                    dtKPI= new KPIDataSet.KPIPHXDataTable();
                    break;
                case "Guadalajara":
                    dtKPI = new KPIDataSet.KPIGDLDataTable();
                    break;
            }
            
            managers = getManagersString(location);
            CDP = getEmployeesCDP(getDataManagers(location), false);
            namesCDP = getEmployeesCDP(getDataManagers(location), true);
            members = getNumberMembers(getDataManagers(location));

            int counterManager = 0;
            int intCDP;
            int counterMember = 0;

            foreach (var manager in managers)
            {
                intCDP = Convert.ToInt16(CDP[counterManager]);
                counterMember = 0;
                foreach (var member in namesCDP)
                {
                    var row = dtKPI.NewRow();
                    row["manager"] = manager;
                    row["members"] = members[counterManager];
                    row["membersCDP"] = CDP[counterManager];
                    int index = namesCDP.IndexOf(member);
                    if (intCDP > 0)
                    {
                        row["nameCDP"] = member;
                    }
                    dtKPI.Rows.Add(row);
                    if (counterMember == intCDP - 1)
                    {                     
                        break;
                    }
                    counterMember++;
                }
                namesCDP.RemoveRange(0, intCDP);
                counterManager++;
            }
            DataTable dt = new DataTable();
            dt = dtKPI;
            return dtKPI;
        }
    }
}