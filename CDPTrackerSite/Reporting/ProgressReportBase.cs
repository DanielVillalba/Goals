using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using CDPTrackerSite.DataAccessor;
using DataSource;
using Utils;
using CDPTrackerSite.Controllers;

namespace CDPTrackerSite.Reporting
{
    public class ProgressReportBase : System.Web.UI.Page
    {
        const string AdministrativeGroupName = "Overhead";
        const string OperationsGroupName = "Billable";
        static readonly Dictionary<string, string[]> Groups = new Dictionary<string, string[]>
                                                                  {
                                                                      {AdministrativeGroupName, new[] {"Hermosillo Staff", "Phoenix", "IT"}},
                                                                      {OperationsGroupName, new[] { "Developers", "Program Managers", "Testers" }}
                                                                  };

        protected IEnumerable<dynamic> GetGroupingQuery(string managerDomainName, DateTime maxMonthDate, DateTime minDate, DateTime minDateWithGracePeriod, DateTime maxDateWithGracePeriod)
        {
            List<string> filteredDomainNameList = new List<string>();
            bool isFilterByManager = false;
            if (!string.IsNullOrWhiteSpace(managerDomainName))
            {
                List<Resource> resources;
                if (ResourceDataAccessor.TryGetResourcesFromManager(managerDomainName, out resources))
                {
                    isFilterByManager = true;
                    filteredDomainNameList.AddRange(resources.Select(r => r.DomainName));
                }
            }
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var filtered = isFilterByManager
                                   ? (from r in context.Resources
                                      where filteredDomainNameList.Contains(r.DomainName)
                                      select r)
                                   : (from r in context.Resources
                                      select r);
                var query = from r in filtered
                            join g in context.GoalTrackings
                                on r.ResourceId equals g.ResourceId
                                into goalTrackings
                            from goalTracking in goalTrackings.DefaultIfEmpty()
                            group new { Goal = goalTracking } by new { r.ResourceId, r.DomainName, r.Name, }
                                into goalGroup
                                select new
                                           {
                                               goalGroup.Key.DomainName,
                                               goalGroup.Key.ResourceId,
                                               goalGroup.Key.Name,
                                               ExpectedToCompleteDuringPeriod =
                                               goalGroup.Count
                                               (
                                                   gg =>
                                                       //estimated to be completed during the period, but must not have been completed before the period
                                                   (
                                                       (gg.Goal.FinishDate.HasValue && gg.Goal.FinishDate.Value >= minDate &&
                                                        gg.Goal.FinishDate.Value <= maxMonthDate)
                                                       &&
                                                       (
                                                           !gg.Goal.VerifiedByManager
                                                           ||
                                                           (gg.Goal.LastUpdate.HasValue && gg.Goal.LastUpdate >= minDateWithGracePeriod &&
                                                            gg.Goal.LastUpdate <= maxDateWithGracePeriod)
                                                       )
                                                   )
                                                   ||
                                                   (
                                                       //or estimated to be completed before the period but completed during the period or not completed
                                                   gg.Goal.FinishDate.HasValue && gg.Goal.FinishDate.Value < minDate
                                                   &&
                                                   (
                                                       //completed in the period
                                                       (gg.Goal.VerifiedByManager && gg.Goal.LastUpdate.HasValue &&
                                                        gg.Goal.LastUpdate.Value >= minDateWithGracePeriod &&
                                                        gg.Goal.LastUpdate.Value <= maxDateWithGracePeriod)
                                                       ||
                                                       //not completed and pending from a previous period
                                                       !gg.Goal.VerifiedByManager
                                                       )
                                                   )
                                               ),
                                               Verified =
                                                   goalGroup.Count
                                                   (
                                                       gg => (gg.Goal.VerifiedByManager)
                                                             &&
                                                             (
                                                           //estimated for and completed during the period
                                                                 (
                                                                     (gg.Goal.FinishDate.HasValue && gg.Goal.FinishDate.Value >= minDate &&
                                                                      gg.Goal.FinishDate.Value <= maxMonthDate)
                                                                     &&
                                                                     (gg.Goal.LastUpdate.HasValue &&
                                                                      gg.Goal.LastUpdate.Value >= minDateWithGracePeriod &&
                                                                      gg.Goal.LastUpdate.Value <= maxDateWithGracePeriod)
                                                                 )
                                                                 ||
                                                           //or estimated to be completed before or after the period, but completed during the period
                                                                 (
                                                                     (
                                                                       gg.Goal.FinishDate.HasValue
                                                                       &&
                                                                       (
                                                                           gg.Goal.FinishDate.Value < minDate
                                                                           ||
                                                                           gg.Goal.FinishDate.Value > maxMonthDate
                                                                       )
                                                                     )
                                                                     &&
                                                                     gg.Goal.LastUpdate.HasValue &&
                                                                     gg.Goal.LastUpdate.Value >= minDateWithGracePeriod &&
                                                                     gg.Goal.LastUpdate.Value <= maxDateWithGracePeriod
                                                                 )
                                                             )
                                                   )
                                           };

                return query.ToList();
            }
        }

        protected IEnumerable<dynamic> GetGroupingQueryByProjectGoals(string managerDomainName, DateTime maxMonthDate, DateTime minDate, DateTime minDateWithGracePeriod, DateTime maxDateWithGracePeriod)
        {
            List<string> filteredDomainNameList = new List<string>();
            bool isFilterByManager = false;
            if (!string.IsNullOrWhiteSpace(managerDomainName))
            {
                List<Resource> resources;
                if (ResourceDataAccessor.TryGetResourcesFromManager(managerDomainName, out resources))
                {
                    isFilterByManager = true;
                    filteredDomainNameList.AddRange(resources.Select(r => r.DomainName));
                }
            }
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var filtered = isFilterByManager
                                   ? (from r in context.Resources
                                      where filteredDomainNameList.Contains(r.DomainName)
                                      select r)
                                   : (from r in context.Resources
                                      select r);
                var query = from r in filtered
                            join g in context.GoalTrackings
                                on r.ResourceId equals g.ResourceId
                            select new { r.ResourceId, Location = r.Location.Name, Goal = g.Goal, r.DomainName, Verified = g.VerifiedByManager, r.Name, g.FinishDate };

                return query.ToList();
            }
        }

        protected IEnumerable<dynamic> GetGroupingMonthlyQuery(string managerDomainName, DateTime maxMonthDate, DateTime minDate)
        {
            List<string> filteredDomainNameList = new List<string>();
            bool isFilterByManager = false;
            if (!string.IsNullOrWhiteSpace(managerDomainName))
            {
                List<Resource> resources;
                if (ResourceDataAccessor.TryGetResourcesFromManager(managerDomainName, out resources))
                {
                    isFilterByManager = true;
                    filteredDomainNameList.AddRange(resources.Select(r => r.DomainName));
                }
            }
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var filtered = isFilterByManager
                                   ? (from r in context.Resources
                                      where filteredDomainNameList.Contains(r.DomainName)
                                      select r)
                                   : (from r in context.Resources
                                      select r);
                var query = from r in filtered
                            join g in context.GoalsView
                            on r.ResourceId equals g.ResourceId
                            into goalTrackings
                            from goalTracking in goalTrackings.DefaultIfEmpty()
                            where goalTracking.FinishDate.Value <= maxMonthDate && goalTracking.FinishDate.Value >= minDate && goalTracking.Progress == 2
                            group new { Goal = goalTracking } by new {goalTracking.GoalId, goalTracking.Verified, goalTracking.Manager, goalTracking.ManagerLocation, r.ResourceId, r.DomainName, goalTracking.EmployeeLocation, goalTracking.Employee, goalTracking.Goal, goalTracking.FinishDate, goalTracking.Progress, goalTracking.TDU}
                                into goalGroup
                                select new
                                {
                                    goalGroup.Key.DomainName,
                                    goalGroup.Key.ResourceId,
                                    goalGroup.Key.EmployeeLocation,
                                    goalGroup.Key.Employee,
                                    goalGroup.Key.Goal,
                                    goalGroup.Key.GoalId,
                                    goalGroup.Key.FinishDate,
                                    goalGroup.Key.Verified,
                                    goalGroup.Key.Progress,
                                    goalGroup.Key.ManagerLocation,
                                    goalGroup.Key.Manager,
                                    goalGroup.Key.TDU
                                };

                return query.ToList();
            }
        }

        protected IEnumerable<dynamic> GetGroupingMonthlyTDUDetails(DateTime maxMonthDate, DateTime minDate)
        {
            List<string> filteredDomainNameList = new List<string>();
            bool isFilterByManager = false;
            
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var filtered = isFilterByManager
                                   ? (from r in context.Resources
                                      where filteredDomainNameList.Contains(r.DomainName)
                                      select r)
                                   : (from r in context.Resources
                                      select r);
                var query = from r in filtered
                            join g in context.GoalsView
                            on r.ResourceId equals g.ResourceId
                            into goalTrackings
                            from goalTracking in goalTrackings.DefaultIfEmpty()
                            where goalTracking.FinishDate.Value <= maxMonthDate && goalTracking.FinishDate.Value >= minDate
                            group new { Goal = goalTracking } by new {goalTracking.Manager, goalTracking.Objective, goalTracking.TrainingCategory, r.ResourceId, r.DomainName, goalTracking.EmployeeLocation, goalTracking.Employee, goalTracking.Goal, goalTracking.FinishDate, goalTracking.Verified, goalTracking.Progress, goalTracking.TDU,goalTracking.ManagerLocation }
                                into goalGroup
                                select new
                                {
                                    goalGroup.Key.Manager,
                                    goalGroup.Key.DomainName,
                                    goalGroup.Key.ResourceId,
                                    goalGroup.Key.EmployeeLocation,
                                    goalGroup.Key.Employee,
                                    goalGroup.Key.Goal,
                                    goalGroup.Key.FinishDate,
                                    goalGroup.Key.Verified,
                                    goalGroup.Key.Progress,
                                    goalGroup.Key.ManagerLocation,
                                    goalGroup.Key.TrainingCategory,
                                    goalGroup.Key.Objective,
                                    goalGroup.Key.TDU
                                };
                return query.ToList();
            }
        }

        new public IPrincipal User
        {
            get
            {
                return HttpContext.Current.User;
            }
        }
        protected string ResourceUser
        {
            get
            {
                return User.Identity.Name;
            }
        }
        private EmployeeActiveDirectoryManager mDomainBrowser;

        protected EmployeeActiveDirectoryManager DomainBrowser
        {
            get
            {
                if (mDomainBrowser == null && Session["DomainController"] != null)
                {
                    mDomainBrowser = (EmployeeActiveDirectoryManager)Session["DomainController"];
                }
                else
                {
                    string activeDirectoryPath = ConfigurationManager.AppSettings["ADPath"];
                    string activeDirectoryUser = ConfigurationManager.AppSettings["ADUsr"];
                    string activeDirectoryPass = ConfigurationManager.AppSettings["ADPass"];
                    mDomainBrowser = new EmployeeActiveDirectoryManager(activeDirectoryPath, activeDirectoryUser, activeDirectoryPass);
                }
                return mDomainBrowser;
            }
        }

        protected string GetLocation(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userDataList)
        {
            IEnumerable<Location> locationsList = GetLocationsList(); 
            string location =
                userDataList.Where(u => string.Compare(u.DomainName, domainName, StringComparison.OrdinalIgnoreCase) == 0).Select(u => u.Location).FirstOrDefault();
            if (location == null) return string.Empty;
            return locationsList.Where(x=> x.abbreviation == location).Select(x=> x.Name).FirstOrDefault();
        }

        protected string GetManager(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userDataList)
        {
            string managerName = userDataList.Where(u => string.Compare(u.DomainName, domainName, StringComparison.OrdinalIgnoreCase) == 0).Select(u => u.Manager).FirstOrDefault();
            if (managerName == null) return string.Empty;

            return managerName.GetNameFromCommonNameMatch();
        }

        protected string GetProject(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userDataList)
        {
            
            var organizationUnit = (from p in userDataList
                                    where StringComparer.CurrentCultureIgnoreCase.Equals(domainName, p.DomainName)
                                    select p.OperationalUnits).FirstOrDefault();
            if (organizationUnit != null)
            {
                return organizationUnit.FirstOrDefault();
            }
            return "Unknown Project";
        }
        
        protected string GetArea(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userData)
        {
            EmployeeActiveDirectoryManager.UserData user = userData.SingleOrDefault(u => string.Compare(u.DomainName, domainName, StringComparison.OrdinalIgnoreCase) == 0);
            if (user == null)
            {
                return string.Empty;
            }
            if (Groups[OperationsGroupName].Any(g => user.OperationalUnits.Any(ou => string.Compare(ou, g, StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return OperationsGroupName;
            }
            if (Groups[AdministrativeGroupName].Any(g => user.OperationalUnits.Any(ou => string.Compare(ou, g, StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return AdministrativeGroupName;
            }
            return string.Empty;
        }


        protected bool IsEnabled(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userData)
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

        private IEnumerable<Location> GetLocationsList()
        {
            const string sessionLocationEnums = "LocationEnums";
            IEnumerable<Location> locationEnums = Session[sessionLocationEnums] as IEnumerable<Location>;
            if (locationEnums != null) return locationEnums;

            if (!ResourceDataAccessor.TryGetLocations(out locationEnums))
            {
                return new List<Location>();
            }

            Session[sessionLocationEnums] = locationEnums;
            return locationEnums;
        }

        protected static void getMinMaxMonths(string quarter, string year, out DateTime minDate, out DateTime maxDate)
        {
            minDate = DateTime.Now;
            int uQuarter;
            int uYear;
            if (!int.TryParse(quarter, out uQuarter)) uQuarter = getQuarter(DateTime.Now);
            if (!int.TryParse(year, out uYear)) uYear = DateTime.Now.Year;

            if (uQuarter == 1)
            {
                minDate = new DateTime(uYear, 1, 1);
            }
            else if (uQuarter == 2)
            {
                minDate = new DateTime(uYear, 4, 1);
            }
            else if (uQuarter == 3)
            {
                minDate = new DateTime(uYear, 7, 1);
            }
            else if (uQuarter == 4)
            {
                minDate = new DateTime(uYear, 10, 1);
            }
            maxDate = minDate.AddMonths(3).AddMilliseconds(-1);
        }

        protected static int getQuarter(DateTime date)
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