using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using CDPTrackerSite.Controllers;
using DataSource;
using Utils;
using System.DirectoryServices.AccountManagement;

namespace CDPTrackerSite.DataAccessor
{
    public class ResourceDataAccessor
    {
        private static readonly EmployeeActiveDirectoryManager ActiveDirectory;

        internal static void TryGetResourceByUserName(object name, out Resource resource)
        {
            throw new NotImplementedException();
        }

        static ResourceDataAccessor()
        {
            string activeDirectoryPath = ConfigurationManager.AppSettings["ADPath"];
            string activeDirectoryUser = ConfigurationManager.AppSettings["ADUsr"];
            string activeDirectoryPass = ConfigurationManager.AppSettings["ADPass"];
            ActiveDirectory = new EmployeeActiveDirectoryManager(activeDirectoryPath, activeDirectoryUser, activeDirectoryPass);
        }

        protected string GetPropertyValue(DirectoryEntry entry, string propertyName)
        {
            if (!entry.Properties.Contains(propertyName)) return string.Empty;
            object value = entry.Properties[propertyName].Value;
            return value is string ? value.ToString() : string.Empty;
        }
       
        public static bool TryGetResourceByUserName(string loginName, out Resource resource)
       { 
            
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    string userName = loginName.StripDomain();
                    context.Configuration.LazyLoadingEnabled = false;
                    var matchResources = context.Resources.Include("Employee").Include("GoalTrackings").Include("GoalTrackings.ProgressEnum").Include("Objective").Include("Objective.ProgressEnum").Where(r => userName.Contains(r.DomainName));
                    resource = matchResources.FirstOrDefault();
                    return resource != null;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                resource = null;
                return false;
            }
        }

        private static readonly Dictionary<string, List<String>> SessionManagerDomainNames = new Dictionary<string, List<String>>();
        private static bool TryGetManagerDomainNameList(string userName, out List<String> domainNameList)
        {
            if (SessionManagerDomainNames.TryGetValue(userName, out domainNameList))
            {
                return true;
            }

            try
            {
                domainNameList = ActiveDirectory.GetManagerResourcesDomainNames(userName);
                if(SessionManagerDomainNames.ContainsKey(userName))
                {
                    SessionManagerDomainNames[userName] = domainNameList;
                }
                else
                {
                    SessionManagerDomainNames.Add(userName, domainNameList);
                }
                return true;
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                domainNameList = null;
                return false;
            }
        }

        public static bool IsManager(string userName)
        {
            List<string> domainNames;
            if(!TryGetManagerDomainNameList(userName, out domainNames))
            {
                return false;
            }

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    return context.Resources.Any(r => domainNames.Contains(r.DomainName));
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                return false;
            }
        }

        public static bool TryGetVerifiableResourcesFromManager(string managerDomainName, out List<Resource> resources)
        {
            List<string> domainNames;
            if (!TryGetManagerDomainNameList(managerDomainName.StripDomain(), out domainNames))
            {
                resources = null;
                return false;
            }

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var query =
                        context.Resources.Where(r => domainNames.Contains(r.DomainName))
                            .Select(r => new
                            {
                                resource = r,
                                goals = r.GoalTrackings.Where(g => !g.VerifiedByManager && g.Progress == (int)ProgressEnumeration.Completed),
                                progress = r.GoalTrackings.Select(g => g.ProgressEnum),
                                objective = r.GoalTrackings.Where(g => !g.VerifiedByManager && g.Progress == (int)ProgressEnumeration.Completed).Select(g=>g.Objective),
                                employee = r.Employee
                            }).ToList();

                    resources = query.Where(q => q.goals.Any()).Select(q => q.resource).ToList();
                }

                return true;
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                resources = new List<Resource>();
                return false;
            }
        }

        public static bool TryGetResourcesFromManager(string managerDomainName, out List<Resource> resources)
        {
            List<string> domainNames;
            if (!TryGetManagerDomainNameList(managerDomainName.StripDomain(), out domainNames))
            {
                resources = null;
                return false;
            }

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var query =
                        context.Resources.Where(r => domainNames.Contains(r.DomainName))
                            .Select(r => new
                            {
                                resource = r,
                                objective = r.Objective,
                                goals = r.GoalTrackings.OrderBy(goal => goal.FinishDate.HasValue ? goal.FinishDate.Value : DateTime.MaxValue),
                                progress = r.GoalTrackings.Select(g => g.ProgressEnum),
                                employee = r.Employee
                            }).ToList();

                    resources = query.Where(q => q.goals.Any()).Select(q => q.resource).ToList();
                }

                return true;
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                resources = new List<Resource>();
                return false;
            }
        }

        public static bool TryGetProgresEnums(out IEnumerable<ProgressEnum> progressEnums)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    progressEnums = context.ProgressEnums.ToList();
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                progressEnums = null;
                return false;
            }
        }

        public static bool TryGetLocations(out IEnumerable<Location> locationEnums) 
        {
            try 
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    locationEnums = context.Locations.ToList();
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                locationEnums = null;
                return false;
            }
        }

        public static bool TryGetGeneralTrainings(out IEnumerable<GeneralTrainingProgram> generalTrainingEnums)
        {
            try
            {
                 using (CDPTrackEntities context = new CDPTrackEntities())
                 {
                     generalTrainingEnums = context.GeneralTrainingProgram.Where(x=> x.Enabled==true).ToList();
                     return true;
                 }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                generalTrainingEnums = null;
                return false;
            }
        }

        public static bool TryGetCategories(out IEnumerable<Category> categoryEnums)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    categoryEnums = context.Category.ToList();
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                categoryEnums = null;
                return false;
            }
        }

        public static bool TryGetObjectives(int ResourceId, out IEnumerable<Objective> objectiveEnums)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    objectiveEnums = context.Objectives.Where(m => m.ResourceId == ResourceId).ToList();
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                objectiveEnums = null;
                return false;
            }
        }

        public static bool TryGetAreas(out IEnumerable<Area> areaEnums)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    areaEnums = context.Area.OrderBy(x => x.Name).ToList();
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                areaEnums = null;
                return false;
            }
        }

        public static bool TryGetPositions(out IEnumerable<Position> positionEnums)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    positionEnums = context.Position.OrderBy(x => x.PositionName).ToList();
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                positionEnums = null;
                return false;
            }
        }

        public static bool TryGetTechnologies(out IEnumerable<Technologies> technologyEnums)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    technologyEnums = context.Technologies.ToList();
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                technologyEnums = null;
                return false;
            }
        }

        public static bool TryGetLevels(out IEnumerable<Level> levelEnums)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    levelEnums = context.Level.ToList();
                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                levelEnums = null;
                return false;
            }
        }

        public static bool IsTalentManagementResource(string userName)
        {
            const string memberOf = "memberOf";
            const string talentManagementGroupValue = "CN=tdg-moss-talent management,OU=Sharepoint Groups,DC=tiempodev,DC=local";

            try
            {
                return ActiveDirectory.GetUserContainsPropertyValue(userName, memberOf, talentManagementGroupValue);
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                return false;
            }
        }

        public static bool TryGetDefaultResourceFromActiveDirectoryData(string loginName, out Resource resource)
        {
            const string mailProperty = "mail";
            const string domainNameProperty = "sAMAccountName";
            const string userNameProperty = "cn";

            DirectoryEntry directoryEntry = ActiveDirectory.GetEmployeeByLogonName(loginName.StripDomain());
            if(directoryEntry==null)
            {
                resource = null;
                return false;
            }

            string userName = directoryEntry.Properties[userNameProperty].Value.ToString();
            string domainName = directoryEntry.Properties[domainNameProperty].Value.ToString();
            string email = directoryEntry.Properties[mailProperty].Value.ToString();
            string position = GetCurrentPositionFromAD(loginName.StripDomain());
            int positionID = GetPositionID(position);
            int positionFromAD = GetIdFromAD(loginName.StripDomain());
            
            resource = new Resource
                           {
                               Name = userName,
                               DomainName = domainName,
                               Email = email,
                               LastLogin = DateTime.Now,
                               ActiveDirectoryId = positionFromAD                   
                           };
            
            Employee employee = new Employee { CurrentPosition = position};


            if (positionID != 0)
                employee.CurrentPositionID = positionID;

            resource.Employee = employee;
            
            return true;
        }

        public static string GetManagerName(string logonName)
        {
            const string managerProperty = "manager";
            
            string managerName = ActiveDirectory.GetUserPropertyValue(logonName, managerProperty);
            if (managerName == null) return string.Empty;
            return managerName.GetNameFromCommonNameMatch();
        }

        public static string GetCurrentPositionFromAD(string logonName)
        {
            const string currentPositionProperty = "description";
            
            string currentPosition = ActiveDirectory.GetUserPropertyValue(logonName, currentPositionProperty);
            if (currentPosition == null) return string.Empty;
            return currentPosition;
        }

        public static List<EmployeeActiveDirectoryManager.UserData> GetUserData()
        {
            return ActiveDirectory.GetUserList();
        }

        public static int GetPositionID(string position) {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var currentPosition = context.Position.Where(a => a.PositionName == position).FirstOrDefault();
                    int idNewPosition = currentPosition.PositionId;

                    return idNewPosition;
                }
            }
            catch (NullReferenceException a) {
                ErrorLogHelper.LogException(a, "CDPTracker");
            }

            return 0;
        }

        public static int GetIdFromAD(string loginName) {
            string idProperty = "initials";
            DirectoryEntry directoryEntry = ActiveDirectory.GetEmployeeByLogonName(loginName.StripDomain());
            try
            {
                string id = directoryEntry.Properties[idProperty].Value.ToString();
                return Convert.ToInt32(id);
            }
            catch (Exception e) {
                ErrorLogHelper.LogException(e, "CDPTracker");
                return 0;
            }

        }

    }
}