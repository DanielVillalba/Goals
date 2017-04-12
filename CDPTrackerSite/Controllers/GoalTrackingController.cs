using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using CDPTrackerSite.DataAccessor;
// ReSharper disable RedundantUsingDirective ==> Directive is used when application is in Release mode 
using CDPTrackerSite.RoleManagement;
using CDPTrackerSite.Models;
using CDPTrackerSite.Models.GoalTracking;
// ReSharper restore RedundantUsingDirective
using DataSource;
using Utils;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using System.Configuration;
using System.DirectoryServices;
using System.Collections;
using System.IO;
using System.Data.Entity.Validation;
using System.Net.Sockets;
using CsvHelper;
using CsvHelper.Configuration;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Antlr.Runtime;
using Newtonsoft.Json.Linq;
using CDPTrackerSite.ProfileManager;
using SelectPdf;
using Ionic.Zip;
using CDPTrackerSite.Helpers;
using System.Net.Mime;
using System.Data.SqlClient;
namespace CDPTrackerSite.Controllers
{


    public enum ProgressEnumeration
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2,
    }

    public class GoalTrackingController : Controller
    {
        private const string SessionUserManagerName = "ManagerName";
        private const string SessionCurrentPosition = "CurrentPosition";
        private static EmployeeActiveDirectoryManager activeDirectoryInstance;
        private const string PHOTOS_URL = "https://clientes2.intelexion.com/V5TiempoDev/Fotos/";

        private const int STATUS_NOT_STARTED = 0;
        private const int STATUS_STARTED = 1;
        private const int STATUS_COMPLETED = 2;
        private const int TEAM_MEMBERS_INPUT = 1;
        private const int MANAGERS_CHECK = 2;

        CDPTrackEntities context = new CDPTrackEntities();

        /***********************************************************  Region for Helpers Methods*********************************************/
        #region Helpers

        public static IEnumerable<string> ExclusionList = new List<string>
        {
            "Jeff Jumpe",
            "Cliff Schertz",
            "itsupport",
            "Maria Pena"
        };


        public ActionResult ClickLink(int ResourceId, int id, string link, string trainingType)
        {
            try
            {
                SetupCommonViewBagValues();
                DateTime date = DateTime.Now;
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    if (trainingType == "General")
                    {
                        GeneralTrainingProgramVisits visit = new GeneralTrainingProgramVisits { IdGeneralTrainingProgram = id, ResourceId = ResourceId, VisitDate = date };
                        context.GeneralTrainingProgramVisits.Add(visit);
                    }
                    else if (trainingType == "Training Program")
                    {
                        TrainingProgramVisits visit = new TrainingProgramVisits { IdTrainingProgram = id, ResourceId = ResourceId, VisitDate = date };
                        context.TrainingProgramVisits.Add(visit);
                    }
                    else if (trainingType == "On Demand")
                    {
                        TrainingProgramOnDemandVisits visit = new TrainingProgramOnDemandVisits { IdTrainingProgramOnDemand = id, ResourceId = ResourceId, VisitDate = date };
                        context.TrainingProgramOnDemandVisits.Add(visit);
                    }
                    context.SaveChanges();
                }
                return Content("<script language='javascript' type='text/javascript'>location.href='" + link + "'</script>");
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                return Content("<script language='javascript' type='text/javascript'>location.href='" + link + "'</script>");
            }
        }

        public static EmployeeActiveDirectoryManager getActiveDirectoryInstance()
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

        public int? GetActiveDirectoryId()
        {
            Resource resource;
            ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);

            if (resource.ActiveDirectoryId == null)
            {
                return 0;
            }
            else
            {
                return resource.ActiveDirectoryId;
            }
        }

        private string GetCurrentUserManagerName()
        {
            string currentUserManagerName = Session[SessionUserManagerName] as string;
            if (currentUserManagerName != null)
            {
                return currentUserManagerName;
            }

            string managerName = ResourceDataAccessor.GetManagerName(User.Identity.Name.StripDomain());
            Session[SessionUserManagerName] = managerName;

            return managerName;
        }

        public string GetCurrentPositionFromAD()
        {
            string currentPosition = Session[SessionCurrentPosition] as string;
            if (currentPosition != null)
            {
                return currentPosition;
            }

            currentPosition = ResourceDataAccessor.GetCurrentPositionFromAD(User.Identity.Name.StripDomain());
            Session[SessionCurrentPosition] = currentPosition;

            return currentPosition;
        }

        public void BaseViewBagSetup()
        {
            ViewBag.ManagerName = GetCurrentUserManagerName();
            ViewBag.CurrentPositionFromAD = GetCurrentPositionFromAD();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());
            ViewBag.CurrentCycle = GetCurrentCycle();

#if !DEBUG
            ViewBag.IsManager = RoleManagementHelper.UserIsInRole(User, Role.Manager);
            ViewBag.IsTalentManagementResource = RoleManagementHelper.UserIsInRole(User, Role.TalentManagement);
#else
            ViewBag.IsManager = true;
            ViewBag.IsTalentManagementResource = true;
#endif
        }

        public void SetupCommonViewBagValues()
        {
            BaseViewBagSetup();
            if (ViewBag.IsManager != true) return;

            ViewBag.PendingGoalsCount = GetManagerPendingGoalsCount();
        }

        private void SetupCommonViewBagValues(int pendingGoalsCount)
        {
            BaseViewBagSetup();
            if (ViewBag.IsManager != true) return;

            ViewBag.PendingGoalsCount = pendingGoalsCount;
        }

        private void SetProfileData(Resource resource)
        {
            int slashIndex = User.Identity.Name.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
            ViewBag.userName = slashIndex >= 0 ? User.Identity.Name.Substring(slashIndex + 1).ToUpper() : User.Identity.Name.ToUpper();
            ViewBag.PROFILE_PICTURE_URL = PHOTOS_URL + resource.ActiveDirectoryId + ".jpg";
            ViewBag.resourceName = resource.Name;
            ViewBag.resourceId = resource.ResourceId;
            ViewBag.resourceAspiringPosition = resource.Employee.AspiringPosition;
        }

        private ErrorModel ErrorHandler(int errorCode)
        {
            ErrorModel err = new ErrorModel();
            err.errcode = errorCode;
            err.message = context.AppError.Where(e => e.errcode == errorCode).Select(e => e.message).FirstOrDefault();
            if (err.message == null)
            {
                err.errcode = 400;
                err.message = "Bad request";
            }
            Response.StatusCode = err.errcode;
            return err;
        }

        protected string GetManager(string domainName, IEnumerable<EmployeeActiveDirectoryManager.UserData> userDataList)
        {
            string managerName = userDataList.Where(u => string.Compare(u.DomainName, domainName, StringComparison.OrdinalIgnoreCase) == 0).Select(u => u.Manager).FirstOrDefault();
            if (managerName == null) return string.Empty;

            return managerName.GetNameFromCommonNameMatch();
        }

        private List<string> GetManagersNames()
        {
            var managers = (from mngr in context.Resources
                            join empl in context.Employee on mngr.ActiveDirectoryId equals empl.ManagerId
                            //group mngr by mngr.Name into mngrName
                            select mngr.Name).Distinct();
            //EmployeeActiveDirectoryManager employeeActiveDirectoryManager = GoalTrackingController.getActiveDirectoryInstance();
            //IEnumerable<EmployeeActiveDirectoryManager.UserData> userDataList = employeeActiveDirectoryManager.GetUserList();
            //List<string> managerNames = userDataList.GroupBy(m => m.Manager).Select(grp => grp.First().Manager.GetNameFromCommonNameMatch()).OrderBy(m => m).Where(m => !string.IsNullOrWhiteSpace(m)).ToList();
            //return managerNames;
            return managers.ToList();
        }

        private int GetManagerPendingGoalsCount()
        {
            // ReSharper disable RedundantAssignment
            string managerName = User.Identity.Name.StripDomain();

            // ReSharper restore RedundantAssignment

#if DEBUG

            //managerName = "cdptracker manager";
            //managerName = "francisco ponce";
#endif
            Resource manager;
            if (!ResourceDataAccessor.TryGetResourceByUserName(managerName, out manager))
            {
                return 0;
            }

            List<Resource> managerResources;
            if (!ResourceDataAccessor.TryGetVerifiableResourcesFromManager(managerName, out managerResources))
            {
                return 0;
            }

            int goalCount = GetGoalsCountFromManagerResources(managerResources);
            return goalCount;
        }

        private IEnumerable<ProgressEnum> GetProgresEnums()
        {
            const string sessionProgressEnums = "ProgressEnums";
            IEnumerable<ProgressEnum> progressEnums = Session[sessionProgressEnums] as IEnumerable<ProgressEnum>;
            if (progressEnums != null) return progressEnums;

            if (!ResourceDataAccessor.TryGetProgresEnums(out progressEnums))
            {
                return new List<ProgressEnum>();
            }

            Session[sessionProgressEnums] = progressEnums;
            return progressEnums;
        }

        private IEnumerable<Location> GetLocation()
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

        private IEnumerable<GeneralTrainingProgram> GetGeneralTraining()
        {
            //const string sessionGeneralTrainingEnums = "GeneralTrainingEnums";
            //IEnumerable<GeneralTrainingProgram> generalTrainingEnums = Session[sessionGeneralTrainingEnums] as IEnumerable<GeneralTrainingProgram>;
            //if (generalTrainingEnums != null) return generalTrainingEnums;
            IEnumerable<GeneralTrainingProgram> generalTrainingPrograms = new List<GeneralTrainingProgram>();
            if (!ResourceDataAccessor.TryGetGeneralTrainings(out generalTrainingPrograms))
            {
                return new List<GeneralTrainingProgram>();
            }

            return generalTrainingPrograms;
        }

        //private List<Category> GetCategories()
        private List<CategoriesModel> GetCategories()
        {
            List<CategoriesModel> categoryList = new List<CategoriesModel>();

            var qry = context.Category.Where(x => x.Visibility == true).ToList();

            foreach (var item in qry)
            {
                CategoriesModel category = new CategoriesModel();
                category.categoryId = item.CategoryId;
                category.categoryDescription = item.Category1;
                category.categoryVisibility = (bool)item.Visibility;
                categoryList.Add(category);
            }

            return categoryList;
        }

        private List<TrainingCategory> GetTrainingCategories()
        {
            //using (CDPTrackEntities context = new CDPTrackEntities())
            //{
            return context.TrainingCategory.ToList();
            //}
        }


        ////private IEnumerable<Category> GetTrainingCategories()
        //private IEnumerable<Category> GetTrainingCategories()
        //{
        //    //using (CDPTrackEntities context = new CDPTrackEntities())
        //    //{

        //    return context.Category.Where(x => x.Visibility == true).ToList();
        //    //}
        //}

        private List<Sources> GetSources()
        {
            //using (CDPTrackEntities context = new CDPTrackEntities())
            //{
            var sourcesList = context.Sources.OrderBy(x => x.Name).ToList();
            sourcesList.Add(new Sources { SourceId = 0, Name = "Other" });
            return sourcesList;
            //}
        }

        private List<KeyThrusts> GetSourcesKT(int id)
        {
            var sourceList = context.KeyThrusts.OrderBy(x => x.OnePagePlanId == id).ToList();
            return sourceList;
        }

        private IEnumerable<Area> GetArea()
        {
            const string sessionArea = "areaEnums";
            IEnumerable<Area> areaEnums = Session[sessionArea] as IEnumerable<Area>;
            if (areaEnums != null) return areaEnums;

            if (!ResourceDataAccessor.TryGetAreas(out areaEnums))
            {
                return areaEnums;
            }

            return areaEnums.Where(x => x.AreaId > 0);
        }

        private IEnumerable<Objective> GetObjective(int ResourceId)
        {
            //using (CDPTrackEntities context = new CDPTrackEntities())
            //{
            int quarter = GetActualQuarter();
            int year = DateTime.Now.Year;
            return context.Objectives.Where(x => x.ResourceId == ResourceId && x.Quarter == quarter && x.Year == year).ToList();
            //}
            //const string sessionObjective = "objectiveEnums";
            //IEnumerable<Objective> objectiveEnums = Session[sessionObjective] as IEnumerable<Objective>;
            //if (objectiveEnums != null) return objectiveEnums;

            //if (!ResourceDataAccessor.TryGetObjectives(ResourceId, out objectiveEnums))
            //{
            //    return objectiveEnums;
            //}

            //return objectiveEnums;
        }


        private IEnumerable<Position> GetPosition()
        {
            const string sessionPosition = "positionEnums";
            IEnumerable<Position> positionEnums = Session[sessionPosition] as IEnumerable<Position>;
            if (positionEnums != null) return positionEnums;

            if (!ResourceDataAccessor.TryGetPositions(out positionEnums))
            {
                return positionEnums;
            }

            return positionEnums;
        }

        private IEnumerable<Technologies> GetTechnology()
        {
            const string sessionTechnology = "technologyEnums";
            IEnumerable<Technologies> technologyEnums = Session[sessionTechnology] as IEnumerable<Technologies>;
            if (technologyEnums != null) return technologyEnums;

            if (!ResourceDataAccessor.TryGetTechnologies(out technologyEnums))
            {
                return technologyEnums;
            }

            return technologyEnums;
        }

        private IEnumerable<Level> GetLevel()
        {
            const string sessionLevel = "levelEnums";
            IEnumerable<Level> levelEnums = Session[sessionLevel] as IEnumerable<Level>;
            if (levelEnums != null) return levelEnums;

            if (!ResourceDataAccessor.TryGetLevels(out levelEnums))
            {
                return levelEnums;
            }

            return levelEnums;
        }

        public static int GetPositionID(string position)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var currentPosition = context.Position.Where(a => a.PositionName == position).FirstOrDefault();
                    int idNewPosition = currentPosition.PositionId;

                    return idNewPosition;
                }
            }
            catch (NullReferenceException ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
            }

            return 0;
        }

        public Resource GetResource(int resourceId)
        {
            //using (CDPTrackEntities context = new CDPTrackEntities())
            //{
            var resource = (from g in context.Resources
                            where g.ResourceId == resourceId
                            select g).ToList();
            if (resource != null) return resource.FirstOrDefault();
            else return null;
            //}
        }

        public Resource GetResource(string resourceName)
        {
            //using (CDPTrackEntities context = new CDPTrackEntities())
            //{
            var resource = (from g in context.Resources
                            where g.Name.ToLower().Contains(resourceName.ToLower())
                            select g).ToList();
            if (resource != null) return resource.FirstOrDefault();
            else return null;
            //}
        }

        private int GetGoalsCountFromManagerResources(IEnumerable<Resource> managerResources)
        {
            int goalCount = managerResources.Where(r => r.Employee != null).Sum(r => r.Employee.Resource.GoalTrackings.Count());
            return goalCount;
        }

        public static ExpandoObject ToExpando(object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }

        public static int getPercentage(int resourceId)
        {
            const int COMPLETED_GOAL_ENUM = 2;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                try
                {
                    int year = DateTime.Now.Year;
                    //int year = 2014;
                    var allGoals = context.GoalTrackings.Include("Objective").Where(x => x.ResourceId == resourceId && x.Objective.Year == year);
                    int allGoalsCount = allGoals.Count();

                    var goalsCompleted = allGoals.Where(x => x.Progress == COMPLETED_GOAL_ENUM);
                    int goalsCompletedCount = goalsCompleted.Count();

                    decimal allGoalsCountDecimal = Convert.ToDecimal(allGoalsCount);
                    decimal goalsCompletedCountDecimal = Convert.ToDecimal(goalsCompletedCount);

                    decimal result = Decimal.Divide(goalsCompletedCountDecimal, allGoalsCountDecimal) * 100;

                    return Convert.ToInt32(result);
                }
                catch (DivideByZeroException ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                    return 0;
                }
            }
        }

        public static int getGoalsCompletedCount(int ResourceId)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                int year = DateTime.Now.Year;
                var allGoals = context.GoalTrackings.Include("Objective").Where(x => x.ResourceId == ResourceId && x.Objective.Year == year);
                int allGoalsCount = allGoals.Count();
                var goalsCompleted = allGoals.Where(x => x.Progress == 2);
                return goalsCompleted.Count();
            }
        }

        public void getTeammates(int ResourceId)
        {
            if (Session["HallOfFame"] == null)
            {
                List<Employee> teammates = new List<Employee>();
                List<dynamic> hallOfFame = new List<dynamic>();


                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    int? projectId = context.Employee.Where(x => x.ResourceId == ResourceId).Select(x => x.ProjectId).FirstOrDefault();
                    if (projectId != null)
                    {
                        teammates = context.Employee.Where((x => x.ProjectId == projectId)).ToList();
                        teammates.RemoveAll(x => !GetIfIsActiveEmployeeFromAD(x.Resource.DomainName));
                    }
                    foreach (Employee employee in teammates)
                    {
                        hallOfFame.Add(new
                        {
                            Name = employee.Resource.Name,
                            ActiveDirectoryId = employee.Resource.ActiveDirectoryId,
                            Percent = getPercentage(employee.ResourceId),
                            GoalCompletedCount = getGoalsCompletedCount(employee.ResourceId)
                        });
                    }

                    var result = (from n in hallOfFame orderby n.Percent descending, n.GoalCompletedCount descending select n).Take(3);
                    var finalResult = new List<ExpandoObject>();
                    foreach (var teammate in result.ToList())
                    {
                        finalResult.Add(ToExpando(teammate));
                    }
                    Session["HallOfFame"] = finalResult;
                }
            }
        }

        public static int getPercentageVerified(int resourceId)
        {
            const int COMPLETED_GOAL_ENUM = 2;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                try
                {
                    int year = DateTime.Now.Year;

                    var allGoals = context.GoalTrackings.Include("Objective").Where(x => x.ResourceId == resourceId && x.Objective.Year == year);
                    int allGoalsCount = allGoals.Count();

                    var goalsCompleted = allGoals.Where(x => x.Progress == COMPLETED_GOAL_ENUM && x.VerifiedByManager == true);
                    int goalsCompletedCount = goalsCompleted.Count();

                    decimal allGoalsCountDecimal = Convert.ToDecimal(allGoalsCount);
                    decimal goalsCompletedCountDecimal = Convert.ToDecimal(goalsCompletedCount);

                    decimal result = Decimal.Divide(goalsCompletedCountDecimal, allGoalsCountDecimal) * 100;

                    return Convert.ToInt32(result);
                }
                catch (DivideByZeroException ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                    return 0;
                }
            }
        }

        public static Employee GetEmployee(int resourceId)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var employee = (from g in context.Employee
                                where g.ResourceId == resourceId
                                select g).ToList();
                if (employee != null) return employee.FirstOrDefault();
                else return null;
            }
        }

        public static int GetTDUPointsToRedeemablePoints(int points)
        {
            if (points >= 60) return 60;
            else if (points >= 45 && points < 60) return 45;
            else if (points >= 30 && points < 45) return 30;
            else if (points >= 15 && points < 30) return 15;
            else return 0;

        }

        public static int GetTDUPointsToDollars(int points)
        {
            if (points >= 60) return 300;
            else if (points >= 45 && points < 60) return 225;
            else if (points >= 30 && points < 45) return 150;
            else if (points >= 15 && points < 30) return 75;
            else return 0;
        }

        public static bool GetIfIsActiveEmployeeFromAD(string logonName)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            IList<String> OUFromEmployee;

            try
            {
                //if employee is part of an OU different from Ex-User Or Customers then is an ActiveEmployee.
                OUFromEmployee = ActiveDirectory.GetEmployeeOrganizationalUnitList(logonName).ToList();

                foreach (var OU in OUFromEmployee)
                {
                    if (OU.Equals("Ex-User") || OU.Equals("Customers") || OU.Equals("Interns"))
                        return false;
                }
            }
            catch (ArgumentException ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                return false;
            }

            return (OUFromEmployee.Count > 0);
        }

        private bool SendEmail(string emailAddress, string emailBody, string subject)
        {
            bool isEmailSent;

            MailAddress to = new MailAddress(emailAddress);
            MailAddress from = new MailAddress("tm@tiempodevelopment.com");
            MailMessage mail = new MailMessage(from, to)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = emailBody
            };

            string emailTarget = emailAddress;
            try
            {
                using (SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential("tiempotm@gmail.com", "Ti3mpoTalent")
                })
                {
                    smtp.Send(mail);
                }
                Console.WriteLine("Mail Sent to: " + emailTarget);
                isEmailSent = true;
            }
            catch (SmtpException ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                Console.WriteLine("Failure to send email to " + emailTarget);
                isEmailSent = false;
            }
            //write the result of the email send attempt to a log file
            string fileName = DateTime.Today.Year.ToString(CultureInfo.InvariantCulture) + "-" + DateTime.Today.Month.ToString(CultureInfo.InvariantCulture) + ".log";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
            {
                if (isEmailSent)
                {
                    file.WriteLine(DateTime.Now + " Succesfully email sent to: " + emailTarget);
                }
                else
                {
                    file.WriteLine(DateTime.Now + " Failure sending email to: " + emailTarget);
                }
            }
            return isEmailSent;
        }

        private bool SendEmail(string emailAddress, string emailBody, string subject, string[] CC)
        {
            bool isEmailSent;

            MailAddress to = new MailAddress(emailAddress);
            MailAddress from = new MailAddress("tm@tiempodevelopment.com");
            MailMessage mail = new MailMessage(from, to)
            {
                Subject = subject,
                IsBodyHtml = true,
                Body = emailBody
            };
            if (CC.Any())
            {
                foreach (var email in CC)
                {
                    mail.CC.Add(email);
                }
            }

            string emailTarget = emailAddress;
            try
            {
                using (SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential("tiempotm@gmail.com", "Ti3mpoTalent")
                })
                {
                    smtp.Send(mail);
                }
                Console.WriteLine("Mail Sent to: " + emailTarget);
                isEmailSent = true;
            }
            catch (SmtpException ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                Console.WriteLine("Failure to send email to " + emailTarget);
                isEmailSent = false;
            }
            catch (Exception i)
            {
                ErrorLogHelper.LogException(i, "CDPTracker");
                isEmailSent = false;
            }
            //write the result of the email send attempt to a log file
            string fileName = DateTime.Today.Year.ToString(CultureInfo.InvariantCulture) + "-" + DateTime.Today.Month.ToString(CultureInfo.InvariantCulture) + ".log";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName, true))
            {
                if (isEmailSent)
                {
                    file.WriteLine(DateTime.Now + " Succesfully email sent to: " + emailTarget);
                }
                else
                {
                    file.WriteLine(DateTime.Now + " Failure sending email to: " + emailTarget);
                }
            }
            return isEmailSent;
        }

        public string GetCurrentCycle()
        {
            string currentCycle = "";
            string currentQuarter = GetQuarter(DateTime.Now.Month).ToString();

            foreach (var item in ViewBag.ListOfQuarters)
            {
                if (currentQuarter == item.Value)
                {
                    currentCycle = item.Text;
                    break;
                }
            }

            return currentCycle + " " + DateTime.Now.Year;
        }

        public string GetCycleText(int? quarter, int? year)
        {
            string cycle = "";
            string quarterStr = quarter.ToString();

            foreach (var item in ViewBag.ListOfQuarters)
            {
                if (quarterStr == item.Value)
                {
                    cycle = item.Text;
                    break;
                }
            }

            return cycle + " " + year;
        }

        public int? GetTduInQuarter(int resourceId)
        {
            int? totalTdus = GetValidTDUQuarter(resourceId);

            if (totalTdus == null)
            {
                totalTdus = 0;
            }

            return totalTdus;
        }

        [HttpPost]
#if !DEBUG
        [ComplexRoleAuthorization(Role.Manager, Role.TalentManagement)]
#endif
        public ActionResult SendMailToEmployeesWithNoPriorities(Nullable<int> _year, Nullable<int> _quarter)
        {
            List<NoPrioritiesReportModel> employeesWithoutPriorities = Reporting.NoPrioritiesReport.getEmployeesWithoutPriorities(_year, _quarter);

            string subject = "CDPTracker - Priorities have not been created";

            string body = "Hello {0},<br> We have noticed that you still haven't created priorities in this period. " +
                          "Please go ahead make this list so you can track your progress and keep a record of your achievements.<br>" +
                          "Sincerely,<br>Talent Management Team<br>Collaborating with you to achieve your professional aspirations.";

            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            foreach (var resource in employeesWithoutPriorities)
            {
                string email = ActiveDirectory.GetUserPropertyValue(resource.DomainName, "mail");
#if DEBUG
                email = "abojorquez@tiempodevelopment.com";
#endif
                SendEmail(email, string.Format(body, resource.Name), subject);
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
#if !DEBUG
        [ComplexRoleAuthorization(Role.Manager, Role.TalentManagement)]
#endif
        public ActionResult SendMailToEmployeesWithoutTeamsMembers(string _manager, string _completed, Nullable<int> _year, Nullable<int> _quarter)
        {
            List<TeamMemberInputReportModel> employeesWithoutTeamsMembersInputs = Reporting.TeamMembersInputReport.getEmployeesTeamMemberInputStatus(_manager, _completed, _year, _quarter);

            string subject = "CDPTracker - Team Member's Input survey not answered";

            string body = "Hello {0},<br> We have noticed that you still haven't answered the Team Member's Input survey in this period. " +
                          "We invited you to answer this quick survey.<br>" +
                          "Sincerely,<br>Talent Management Team<br>Collaborating with you to achieve your professional aspirations.";

            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            foreach (var resource in employeesWithoutTeamsMembersInputs)
            {
                string email = ActiveDirectory.GetUserPropertyValue(resource.DomainName, "mail");
#if DEBUG
                email = "abojorquez@tiempodevelopment.com";
#endif
                SendEmail(email, string.Format(body, resource.Name), subject);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
#if !DEBUG
        [ComplexRoleAuthorization(Role.Manager, Role.TalentManagement)]
#endif
        public ActionResult SendMailToManagersWithoutManagersCheck(string _manager, string _completed, Nullable<int> _year, Nullable<int> _quarter)
        {
            List<ManagerCheckReportModel> managersWithoutManagersCheck = Reporting.ManagersCheckReport.GetManagerCheckReportStatus(_manager, _completed, _year, _quarter);

            string subject = "CDPTracker - Manager's check survey not answered";

            string body = "Hello {0},<br> We have noticed that you still haven't answered all the Manager's Check surveys in this period. " +
                          "We invited you to answer these quick surveys.<br>" +
                          "<br>" +
                          "Pending Employees to Evaluate: <br>";


            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            foreach (var manager in managersWithoutManagersCheck.GroupBy(x => new { x.Manager, x.DomainName }).Select(x => x.First()))
            {
                string email = ActiveDirectory.GetUserPropertyValue(manager.DomainName, "mail");
#if DEBUG
                email = "abojorquez@tiempodevelopment.com";
#endif
                foreach (var employee in managersWithoutManagersCheck.Where(x => x.Manager == manager.Manager))
                {

                    body += employee.EvaluatedEmployee + "<br>";

                }
                body += "<br>Sincerely,<br>Talent Management Team<br>Collaborating with you to achieve your professional aspirations.";
                SendEmail(email, string.Format(body, manager.Manager), subject);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
#if !DEBUG
        [ComplexRoleAuthorization(Role.Manager, Role.TalentManagement)]
#endif
        public ActionResult SendMailToEmployeesWithUnassignObjectives()
        {
            List<dynamic> employeesWithUnassignObjectives = Reporting.UnassignObjectiveReport.getEmployeesWithUnassignObjectives();

            string subject = "CDPTracker - goals have not been linked to your objectives";

            string body = "Hello, <br> We have noticed that you still haven't linked goals to objectives in this period. " +
                          "Please go ahead and fill these lists so you can track your progress and keep a record of your achievements.<br>" +
                          "Sincerely,<br>Talent Management Team<br>Collaborating with you to achieve your professional aspirations.";

            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            foreach (dynamic resource in employeesWithUnassignObjectives)
            {
                string email = ActiveDirectory.GetUserPropertyValue(resource.DomainName, "mail");
#if DEBUG
                email = "claredo@tiempodevelopment.com";
#endif
                SendEmail(email, body, subject);
            }


            return RedirectToAction("/UnassignObjectiveReport");
        }

        public bool SendEmailTDURedeem(TDUReward reward, Resource resource)
        {
            try
            {
                var employee = GetEmployee(resource.ResourceId);
                string employeeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(resource.Name);
                int points = GetTDUPointsToRedeemablePoints(reward.TotalTDUReward);
                int dollars = GetTDUPointsToDollars(points);
                string subject = string.Format("CDPTracker - {0} would like to redeem {1} points in exchange for ${2} USD.", employeeName, points, dollars);
                string body = string.Format("Hello, <br> {0} would like to redeem {1} points in exchange for ${2} USD. <br> " +
                "The points pertain to Quarter {3}, {4} to the end of Quarter {5}, {6}.<br>" +
                "Sincerely,<br>Talent Management Team<br>Collaborating with you to achieve your professional aspirations."
                , employeeName, points, dollars, reward.StartingQuarter, reward.StartingYear, reward.EndingQuarter, reward.EndingYear);

                EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

                string tmEmail = "tm@tiempodevelopment.com";
                string[] email = new string[] { ActiveDirectory.GetUserPropertyValue(resource.DomainName, "mail") };
#if DEBUG
                tmEmail = "claredo@tiempodevelopment.com";
                email = new string[] { "claredo@tiempodevelopment.com" };
#endif
                return SendEmail(tmEmail, body, subject, email);
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                return false;
            }

        }

        public static int GetCurrentYear()
        {
            DateTime? date = DateTime.Now;
            return date.Value.Year;
        }

        public void ListOfYears()
        {
            try
            {
                List<SelectListItem> dropdownItems = new List<SelectListItem>();

                for (int i = 0; i <= 20; i++)
                {
                    int Year = (GetCurrentYear() + 2) - i;

                    dropdownItems.AddRange(new[] { new SelectListItem() { Text = Year.ToString(), Value = Year.ToString() } });
                }

                ViewBag.ListOfYears = dropdownItems;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
            }
        }

        public List<SelectListItem> ListOfYearsFuture()
        {
            try
            {
                List<SelectListItem> dropdownItems = new List<SelectListItem>();

                for (int i = 0; i <= 4; i++)
                {
                    int Year = (GetCurrentYear() + i);

                    dropdownItems.AddRange(new[] { new SelectListItem() { Text = Year.ToString(), Value = Year.ToString() } });
                }

                return dropdownItems;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                return null;
            }
        }

        public List<SelectListItem> GetQuarter()
        {
            List<SelectListItem> lstQuarter = new List<SelectListItem>();
            lstQuarter.Add(new SelectListItem { Text = "Jan - Mar", Value = "1" });
            lstQuarter.Add(new SelectListItem { Text = "Apr - June", Value = "2" });
            lstQuarter.Add(new SelectListItem { Text = "July - Sept", Value = "3" });
            lstQuarter.Add(new SelectListItem { Text = "Oct - Dec", Value = "4" });

            return lstQuarter;
        }

        /// <summary>
        /// Gets the Highest Date from a Quarter and Year
        /// </summary>
        /// <param name="quarter"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public DateTime GetMaxDatePeriodQuarter(int quarter, int year)
        {
            var endingDate = new DateTime();
            if (quarter == 1)
            {
                endingDate = new DateTime(year, 4, 1);
            }
            else if (quarter == 2)
            {
                endingDate = new DateTime(year, 7, 1);
            }
            else if (quarter == 3)
            {
                endingDate = new DateTime(year, 10, 1);
            }
            else
            {
                endingDate = new DateTime(year + 1, 1, 1);
            }
            return endingDate;

        }

        /// <summary>
        /// Gets the Lowest Date in a Quarter and Year
        /// </summary>
        /// <param name="quarter"></param>
        /// <param name="year"></param>
        public DateTime GetMinDatePeriodQuarter(int quarter, int year)
        {
            var startingDate = new DateTime();
            if (quarter == 1)
            {
                startingDate = new DateTime(year, 1, 1);
            }
            else if (quarter == 2)
            {
                startingDate = new DateTime(year, 4, 1);
            }
            else if (quarter == 3)
            {
                startingDate = new DateTime(year, 7, 1);
            }
            else
            {
                startingDate = new DateTime(year, 10, 1);
            }
            return startingDate;
        }

        public int GetQuarter(int month)
        {
            if (month <= 3)
            {
                return 1;
            }
            else if (month > 3 && month <= 6)
            {
                return 2;
            }
            else if (month > 6 && month <= 9)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        public bool isCurrentQuarter(int? year, int? quarter)
        {
            if (year == DateTime.Now.Year && quarter == GetActualQuarter())
            {
                return true;
            }
            return false;
        }

        public static Int32 GetActualQuarter()
        {
            int month = DateTime.Now.Month;

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

        /// <summary>
        /// Get the TimeDiference In Months Between a set Quarter and Year and the Current Quarter and Year
        /// </summary>
        /// <param name="Quarter"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static int TimeDiference(int quarter, int year)
        {
            int currentQuarter = GetActualQuarter();
            int currentYear = GetCurrentYear();
            int quartersPerYear = 4;
            var yearDiference = currentYear - year;
            var diferenceQuarters = (yearDiference * quartersPerYear) + (currentQuarter - quarter);
            return Math.Abs(diferenceQuarters);
        }
        /// <summary>
        /// Outputs the minimum and maximum dates of a Quarter
        /// </summary>
        /// <param name="quarter"></param>
        /// <param name="year"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        private void GetMinMaxMonths(string quarter, string year, out DateTime minDate, out DateTime maxDate)
        {
            minDate = DateTime.Now;
            int uQuarter;
            int uYear;
            if (!int.TryParse(quarter, out uQuarter)) uQuarter = GetQuarter(DateTime.Now.Month);
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

        public int GetTDUSumQuarter(int quarter, int year, int resourceId)
        {
            DateTime minDate = DateTime.Now;
            DateTime maxDate = DateTime.Now;
            GetMinMaxMonths(Convert.ToString(quarter), Convert.ToString(year), out minDate, out maxDate);
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var TDUSum = (from g in context.GoalTrackings
                              where g.ResourceId == resourceId &&
                                    g.FinishDate <= maxDate &&
                                     g.FinishDate >= minDate &&
                                     g.Progress == 2 &&
                                     g.VerifiedByManager == true
                              select g.TDU).Sum();

                if (TDUSum != null) return TDUSum.Value;
                else return 0;
            }
        }

        public bool FindTDUReedeemDuplicate(int quarter, int quarterYear, int resourceId)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var TDURedeem = from r in context.TDURedeem
                                where (r.Quarter == quarter && r.QuarterYear == quarterYear && r.resourceId == resourceId)
                                select r;
                if (TDURedeem != null && TDURedeem.Count() > 0)
                {
                    return true;
                }
                else return false;
            }
        }

        public bool InsertTDURedeem(List<GoalTracking> Goals)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                bool updated = false;
                foreach (GoalTracking goal in Goals)
                {
                    int YearGoal = goal.FinishDate.Value.Year;
                    int QuarterGoal = GetQuarter(goal.FinishDate.Value.Month);
                    DateTime minMonth = DateTime.Now;
                    DateTime maxMonth = DateTime.Now;
                    GetMinMaxMonths(Convert.ToString(QuarterGoal), Convert.ToString(YearGoal), out minMonth, out maxMonth);
                    int TotalTDU = GetTDUSumQuarter(QuarterGoal, YearGoal, goal.ResourceId);
                    bool duplicate = FindTDUReedeemDuplicate(QuarterGoal, YearGoal, goal.ResourceId);
                    if (TotalTDU > 15) TotalTDU = 15;
                    if (!duplicate && TotalTDU >= 15)
                    {
                        TDURedeem TDURedeemed = new TDURedeem
                        {
                            resourceId = goal.ResourceId,
                            QuarterYear = YearGoal,
                            Quarter = QuarterGoal,
                            TDU = TotalTDU,
                            DateReached = DateTime.Now
                        };
                        context.TDURedeem.Add(TDURedeemed);
                        context.SaveChanges();
                        updated = true;
                        var validRewards = new List<TDURedeem>();
                        CalculateReward(GetResource(goal.ResourceId), out validRewards);
                    }
                }
                if (updated)
                {
                    context.SaveChanges();

                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// Calculate if there is a TDUReward that correspond to any of the validTDUs Period
        /// </summary>
        /// <param name="ValidTDUs"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public List<TDUReward> DuplicateRewards(List<TDURedeem> ValidTDUs, Resource resource)
        {
            int quarterdif = TimeDiference(ValidTDUs.FirstOrDefault().Quarter, ValidTDUs.FirstOrDefault().QuarterYear);
            ValidTDUs = ValidTDUs.OrderBy(x => x.QuarterYear).ThenBy(x => x.Quarter).ToList();
            var overlapTDUReward = new List<TDUReward>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var TDURewards = (from r in context.TDUReward
                                  where r.resourceId == resource.ResourceId
                                  select r).ToList();

                if (!TDURewards.Any())
                {
                    return null;
                }
                else
                {
                    var startingYear = ValidTDUs.FirstOrDefault().QuarterYear;
                    var startingQuarter = ValidTDUs.FirstOrDefault().Quarter;
                    var endingYear = ValidTDUs.LastOrDefault().QuarterYear;
                    var endingQuarter = ValidTDUs.LastOrDefault().Quarter;
                    var startingDate = GetMinDatePeriodQuarter(startingQuarter, startingYear);
                    var endingDate = GetMaxDatePeriodQuarter(endingQuarter, endingYear);
                    bool overlap = false;
                    foreach (var TDUReward in TDURewards)
                    {
                        var startingDateTDUReward = GetMinDatePeriodQuarter(TDUReward.StartingQuarter, TDUReward.StartingYear);
                        var endingDateTDUReward = GetMaxDatePeriodQuarter(TDUReward.EndingQuarter, TDUReward.EndingYear);
                        //it is true if there is a date overlap
                        if ((startingDate < endingDateTDUReward) && (endingDate > startingDateTDUReward))
                        {
                            overlap = true;
                            overlapTDUReward.Add(TDUReward);
                        }
                    }
                    if (overlap)
                    {
                        return overlapTDUReward.OrderByDescending(x => x.EndingYear).ThenByDescending(x => x.EndingQuarter).ToList();
                    }
                    else return null;
                }
            }
        }

        ///<summary>
        ///Method to obtain all the related TDURedeem fields of a Resource
        /// </summary>
        public List<TDURedeem> GetTDURedeemedResource(Resource Resource)
        {
            var userTDURedeem = new List<TDURedeem>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                userTDURedeem = (from t in context.TDURedeem
                                 where t.resourceId == Resource.ResourceId
                                 orderby t.QuarterYear descending, t.QuarterYear descending
                                 select t).ToList();
            }
            return userTDURedeem;
        }

        /// <summary>
        /// Method to return all related TDURewards of a Resource
        /// </summary>
        /// <param name="Resource"></param>
        /// <returns></returns>
        public List<TDUReward> GetTDURewardResource(Resource Resource)
        {
            var userTDURewards = new List<TDUReward>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                userTDURewards = (from t in context.TDUReward
                                  where t.resourceId == Resource.ResourceId
                                  orderby t.EndingQuarter descending, t.EndingYear descending
                                  select t).ToList();

            }
            if (userTDURewards.Any()) return userTDURewards;
            else return null;

        }

        /// <summary>
        /// Takes a list of Valid TDU's for a Resource and verifies which TDUReward they belong to and returns it.
        /// </summary>
        /// <param name="Resource"></param>
        /// <param name="ValidTDUs"></param>
        /// <returns></returns>
        public TDUReward CalculateReward(Resource Resource, out List<TDURedeem> ValidTDUs)
        {
            int quarter = 3;
            bool valid = false;
            bool Paid, Redeemed;
            ValidTDUs = new List<TDURedeem>();
            var userTDURedeem = GetTDURedeemedResource(Resource).OrderByDescending(x => x.QuarterYear).ThenByDescending(x => x.Quarter).Where(x => !(x.Redeemed == true) && !(x.Paid == true)).ToList();
            var TDURewards = GetTDURewardResource(Resource);


            if (userTDURedeem == null || userTDURedeem.Count == 0) return null;
            if (userTDURedeem.Any())
            {
                if (userTDURedeem.First().TDUReward != null)
                {
                    var firstTDU = TDURewards.Where(x => x.TDURewardId == userTDURedeem.First().TDUReward);
                    if (firstTDU.Any())
                    {
                        if (firstTDU.First().TotalTDUReward == 60) return null;
                    }
                }
                //add the first item to the TDUValidList
                ValidTDUs.Add(userTDURedeem.FirstOrDefault());
                userTDURedeem.Remove(ValidTDUs.FirstOrDefault());
                //add the consecutive TDU list from past quarters
                if (userTDURedeem.Count >= 1) ValidTDUs = (ValidTDUList(ValidTDUs.FirstOrDefault().QuarterYear, ValidTDUs.FirstOrDefault().Quarter, userTDURedeem, ValidTDUs, TDURewards));
            }
            int monthDiference = TimeDiference(ValidTDUs.Last().Quarter, ValidTDUs.Last().QuarterYear);

            //TDU Reward Parameter Placeholders
            var reward = new TDUReward();
            int validForQuarters = 0;
            int totalTDUReward = 0;
            var DatetoEndValidity = new DateTime();
            int StartingQuarter, StartingYear, EndingQuarter, EndingYear;
            int resouceId = Resource.ResourceId;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var LatestTDUReward = ValidTDUs.FirstOrDefault();
                ValidTDUs.Reverse();
                if (ValidTDUs.Count >= 4)
                {
                    if (monthDiference <= 5 * quarter)
                    {
                        ValidTDUs = ValidTDUs.Take(4).ToList();
                        validForQuarters = 4;
                        DatetoEndValidity = GetMaxDatePeriodQuarter(ValidTDUs.Last().Quarter - 1, ValidTDUs.Last().QuarterYear + 1);
                    }
                    else return null;
                }
                else if (ValidTDUs.Count < 4 && ValidTDUs.Count > 0)
                {
                    if (monthDiference <= 1 * quarter)
                    {
                        validForQuarters = 1;
                        if (LatestTDUReward.Quarter == 4) DatetoEndValidity = GetMaxDatePeriodQuarter(1, ValidTDUs.Last().QuarterYear + 1);
                        else DatetoEndValidity = GetMaxDatePeriodQuarter(ValidTDUs.Last().Quarter + 1, ValidTDUs.Last().QuarterYear);
                    }
                    else return null;
                }
                else
                {
                    return null;
                }
                //Save the TDUReward Object that overlaps over any period on the ValidTDUList
                var overlapTDURewards = DuplicateRewards(ValidTDUs, Resource);

                valid = true;
                totalTDUReward = ValidTDUs.Sum(x => x.TDU);

                //If there was an overlap over valid quarters
                if (overlapTDURewards != null)
                {
                    if (overlapTDURewards.Count > 1)
                    {
                        StartingQuarter = ValidTDUs.First().Quarter;
                        StartingYear = ValidTDUs.First().QuarterYear;
                        EndingQuarter = ValidTDUs.Last().Quarter;
                        EndingYear = ValidTDUs.Last().QuarterYear;
                        Paid = false;
                        Redeemed = false;

                        foreach (var overlapTDUReward in overlapTDURewards)
                        {
                            //remove the overlapRewards from dB
                            if (!(overlapTDUReward.Paid == true) && !(overlapTDUReward.Redeemed == true))
                            {
                                reward = new TDUReward
                                {

                                    resourceId = ValidTDUs.First().resourceId,
                                    StartingQuarter = ValidTDUs.First().Quarter,
                                    StartingYear = ValidTDUs.First().QuarterYear,
                                    EndingQuarter = ValidTDUs.Last().Quarter,
                                    EndingYear = ValidTDUs.Last().QuarterYear,
                                    DatetoLoseValidity = DatetoEndValidity,
                                    ValidForQuarters = validForQuarters,
                                    Redeemed = false,
                                    Paid = false,
                                    TotalTDUReward = totalTDUReward
                                };
                                context.TDUReward.Add(reward);
                                foreach (var TDURedeemQuarter in ValidTDUs)
                                {
                                    if (TDURedeemQuarter.TDUReward == overlapTDUReward.TDURewardId && TDURedeemQuarter.TDUReward != null)
                                    {
                                        TDURedeemQuarter.TDUReward = reward.TDURewardId;
                                        TDURedeemQuarter.Paid = false;
                                        context.Entry(TDURedeemQuarter).State = EntityState.Modified;
                                        context.SaveChanges();
                                    }
                                }
                                context.Entry(overlapTDUReward).State = EntityState.Deleted;
                                context.SaveChanges();
                            }
                        }
                    }
                    else if (overlapTDURewards.Count == 1)
                    {
                        var overlapTDUReward = overlapTDURewards.FirstOrDefault();
                        if (!(overlapTDUReward.Paid == true) && !(overlapTDUReward.Redeemed == true))
                        {
                            overlapTDUReward.DatetoLoseValidity = DatetoEndValidity;
                            overlapTDUReward.ValidForQuarters = validForQuarters;
                            overlapTDUReward.TotalTDUReward = totalTDUReward;
                            overlapTDUReward.StartingQuarter = ValidTDUs.First().Quarter;
                            overlapTDUReward.StartingYear = ValidTDUs.First().QuarterYear;
                            overlapTDUReward.EndingQuarter = ValidTDUs.Last().Quarter;
                            overlapTDUReward.EndingYear = ValidTDUs.Last().QuarterYear;
                            overlapTDUReward.Paid = false;
                            overlapTDUReward.Redeemed = false;
                            context.Entry(overlapTDUReward).State = EntityState.Modified;
                            context.SaveChanges();
                            foreach (var TDURedeemQuarter in ValidTDUs)
                            {
                                TDURedeemQuarter.TDUReward = overlapTDUReward.TDURewardId;
                                TDURedeemQuarter.Paid = false;
                                context.Entry(TDURedeemQuarter).State = EntityState.Modified;
                            }
                            context.SaveChanges();
                            return overlapTDUReward;
                        }
                        else
                        {
                            return overlapTDUReward;
                        }
                    }
                }
                //if there is no overlapped reward
                else
                {
                    //If there wasn't any similar reward found then create a new one, insert it and return it.
                    reward = new TDUReward
                    {
                        resourceId = ValidTDUs.FirstOrDefault().resourceId,
                        StartingQuarter = ValidTDUs.First().Quarter,
                        StartingYear = ValidTDUs.First().QuarterYear,
                        EndingQuarter = ValidTDUs.Last().Quarter,
                        EndingYear = ValidTDUs.Last().QuarterYear,
                        DatetoLoseValidity = DatetoEndValidity,
                        ValidForQuarters = validForQuarters,
                        Redeemed = false,
                        Paid = false,
                        TotalTDUReward = totalTDUReward
                    };
                    context.TDUReward.Add(reward);
                    context.SaveChanges();
                    valid = true;
                    foreach (var TDURedeemQuarter in ValidTDUs)
                    {
                        TDURedeemQuarter.TDUReward = reward.TDURewardId;
                        TDURedeemQuarter.Paid = false;
                        context.Entry(TDURedeemQuarter).State = EntityState.Modified;

                    }
                    context.SaveChanges();
                }

                if (valid)
                {
                    context.SaveChanges();
                    return reward;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Method to get the TDUReward asociated with the Resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="existingReward"></param>
        /// <returns>TDUReward that is for this Quarter
        /// If there is non it will return null
        /// </returns>
        public TDUReward RedeemReward(Resource resource, out bool existingReward)
        {
            existingReward = false;
            List<TDUReward> validRewards = GetTDURewardResource(resource);
            List<TDURedeem> validTDURedeem = GetTDURedeemedResource(resource);
            if (validRewards != null && validRewards.Count > 0)
            {
                validRewards = validRewards.FindAll(x => !(x.Paid == true));
                var redeemedReward = validRewards.FindAll(x => x.Redeemed);
                if (redeemedReward.Count > 0)
                {
                    foreach (var reward in redeemedReward)
                    {
                        var newestRewardRedeemed = redeemedReward.First();
                        if (DateTime.Now <= newestRewardRedeemed.DatetoLoseValidity)
                        {
                            existingReward = true;
                            return newestRewardRedeemed;
                        }
                    }
                }
                var newestReward = validRewards.First();
                if (DateTime.Now <= newestReward.DatetoLoseValidity) return newestReward;
                else return CalculateReward(resource, out validTDURedeem);
            }
            //if there is no valid reward already then try to calculate if one can be made.
            else
            {
                return CalculateReward(resource, out validTDURedeem);
            }
        }

        public int? GetValidTDUQuarter(int ResourceId)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var sumTDUs = (from g in context.GoalTrackings
                               where g.VerifiedByManager == true && g.Progress == 2
                               && g.ResourceId == ResourceId
                               select g.TDU).Sum();
                if (sumTDUs.HasValue) return (int)sumTDUs;
                else return null;
            }
        }

        public int? GetToBeCompletedQuarterTDUs(int ResourceId)
        {
            int currentQuarter = GetActualQuarter();
            DateTime MaxDate, MinDate;
            GetMinMaxMonths(currentQuarter.ToString(), DateTime.Now.Year.ToString(), out MinDate, out MaxDate);
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var sumTDUs = (from g in context.GoalTrackings
                               where g.VerifiedByManager == false
                               && g.ResourceId == ResourceId
                               && g.FinishDate <= MaxDate && g.FinishDate >= MinDate
                               select g.TDU).Sum();
                if (sumTDUs.HasValue) return (int)sumTDUs;
                else return 0;
            }
        }

        /// <summary>
        /// Recursive Method for Generating the Final List of Valid TDU
        /// Redeems for a Resource
        /// </summary>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <param name="TotalList">List of all the TDURedeems for the Selected Resource.</param>
        /// <param name="ValidTDU"></param>
        /// <returns></returns>
        public List<TDURedeem> ValidTDUList(int oldYear, int oldQuarter, List<TDURedeem> TotalList, List<TDURedeem> ValidTDU, List<TDUReward> TDURewards)
        {
            var OrderedTotalList = TotalList.OrderBy(x => x.QuarterYear).ThenBy(x => x.Quarter).ToList();
            var TotalStack = new Stack<TDURedeem>(OrderedTotalList);
            if (TotalList.Count == 0) return ValidTDU;

            if (TotalList.First().TDUReward != null)
            {
                var firstTDU = TDURewards.Where(x => x.TDURewardId == TotalList.First().TDUReward);
                if (firstTDU.Any())
                {
                    if (firstTDU.First().TotalTDUReward == 60) return ValidTDU;
                }
            }
            if (oldQuarter == 1)
            {
                if (TotalList.FirstOrDefault().Quarter == 4 && TotalList.FirstOrDefault().QuarterYear == oldYear - 1 && !(TotalList.FirstOrDefault().Paid == true) && !(TotalList.FirstOrDefault().Redeemed == true))
                {
                    ValidTDU.Add(TotalStack.Pop());
                    return ValidTDUList(ValidTDU.Last().QuarterYear, ValidTDU.Last().Quarter, TotalStack.ToList<TDURedeem>(), ValidTDU, TDURewards);
                }
                else
                {
                    return ValidTDU;
                }
            }
            else
            {
                if (TotalList.FirstOrDefault().Quarter == oldQuarter - 1 && !(TotalList.FirstOrDefault().Paid == true) && !(TotalList.FirstOrDefault().Redeemed == true))
                {
                    ValidTDU.Add(TotalStack.Pop());
                    return ValidTDUList(ValidTDU.Last().QuarterYear, ValidTDU.Last().Quarter, TotalStack.ToList<TDURedeem>(), ValidTDU, TDURewards);
                }
                else
                {
                    return ValidTDU;
                }
            }
        }

        private int ptRestrictions(int trainingCategoryId, int resourceId, int TDUs, int TDUsedit, bool changeCategory = false)
        {
            int quarter = GetQuarter(DateTime.Now.Month);
            int maxTDUCategory, pointsCategory, pointsQuarter, remainingCategory;
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var mTDUCategory = from trainingCategory in context.TrainingCategory
                                   where trainingCategory.TrainingCategoryId == trainingCategoryId
                                   select trainingCategory.MaxTDU;
                maxTDUCategory = 0;
                foreach (var p in mTDUCategory)
                {
                    maxTDUCategory = Convert.ToInt16(p);
                }
                var ptsCategory = from goal in context.GoalTrackings
                                  join trainingCategory in context.TrainingCategory on goal.TrainingCategoryId equals trainingCategory.TrainingCategoryId
                                  join obj in context.Objectives on goal.ObjectiveId equals obj.ObjectiveId
                                  where obj.Quarter == quarter && obj.Year == DateTime.Now.Year && goal.TrainingCategoryId == trainingCategoryId && goal.ResourceId == resourceId
                                  select goal.TDU;
                //Total Points Used in the Current Goal Category
                pointsCategory = 0;
                if (!changeCategory)
                {
                    pointsCategory = 0 - TDUsedit;
                }

                foreach (var p in ptsCategory)
                {
                    pointsCategory += Convert.ToInt16(p);
                }
                //Total points already used up for this Quarter by Other Goals
                var ptsQuarter = from goal in context.GoalTrackings
                                 join trainingCategory in context.TrainingCategory on goal.TrainingCategoryId equals trainingCategory.TrainingCategoryId
                                 join obj in context.Objectives on goal.ObjectiveId equals obj.ObjectiveId
                                 where obj.Quarter == quarter && obj.Year == DateTime.Now.Year && goal.ResourceId == resourceId
                                 select goal.TDU;
                pointsQuarter = 0;
                //if (!changeCategory)
                //{
                pointsQuarter = 0 - TDUsedit;
                //}
                foreach (var p in ptsQuarter)
                {
                    pointsQuarter += Convert.ToInt16(p);
                }
                //Points that remain available for the Goal Category
                remainingCategory = maxTDUCategory - pointsCategory;
                var remainingQuarter = 15 - pointsQuarter;
                //If the amount of TDU's selected is greater than what remains in the category then asign it that remainder
                if (TDUs >= remainingCategory || TDUs > remainingQuarter)
                {
                    TDUs = remainingCategory;
                    //If the remainder of the category is greater than the points remaining in the quarter then asign these points.
                    if (TDUs > remainingQuarter)
                    {
                        TDUs = remainingQuarter;
                    }
                }
                //If TDU ends up being less than 0 because the Remainder in the Quarter or the Remainder in the category is negative set
                //TDU to 0
                if (TDUs < 0) TDUs = 0;
                return TDUs;
            }
        }

        /// <summary>
        /// Calculate the "quarter of the year" data required to handle the different kinds of reports
        /// </summary>
        /// <param name="numberOfPeriods">Number of periods we need to calculate from the current quarter and backwards</param>
        /// <returns>
        ///         A Tuple array where Item1 == Quarter / Item2 == Year
        ///         The 0 index contains the current Quarter - Year data, 
        ///         by increase the index of the array you'll get the previous quarter values
        /// </returns>
        private Tuple<int, int>[] quarterCalculation(int numberOfPeriods)
        {
            Tuple<int, int>[] data = new Tuple<int, int>[numberOfPeriods];
            int quarter = GetActualQuarter();
            int year = GetCurrentYear();

            for (int i = 0; i < numberOfPeriods; i++)
            {
                data[i] = new Tuple<int, int>(quarter, year);
                if (quarter == 1)
                {
                    quarter = 4;
                    year--;
                }
                else
                    quarter--;
            }
            return data;
        }

        /// <summary>
        /// Calculate a set of future "quarter of the year" data required to handle the different kinds of reports
        /// </summary>
        /// <param name="numberOfPeriods">Number of periods we need to calculate from the current quarter and FOWARD</param>
        /// <param name="lastQuarter">Quarter that we want to start the calculation from, this won't be included in the result</param>
        /// <param name="lastYear">Year that we want to start the calculation from, this won't be included in the result</param>
        /// <returns>
        ///         A Tuple array where Item1 == Quarter / Item2 == Year
        ///         The 0 index contains the first Quarter - Year data calculated after the lastQuarter and lastYear params, 
        ///         by increase the index of the array you'll get the next quarter values
        /// </returns>
        private Tuple<int, int>[] fowardQuarterCalculation(int numberOfPeriods, int lastQuarter, int lastYear)
        {
            Tuple<int, int>[] data = new Tuple<int, int>[numberOfPeriods];
            int quarter = lastQuarter;
            int year = lastYear;

            for (int i = 0; i < numberOfPeriods; i++)
            {
                if (quarter == 4)
                {
                    quarter = 1;
                    year++;
                }
                else
                    quarter++;
                data[i] = new Tuple<int, int>(quarter, year);
            }
            return data;
        }

        private Tuple<int, int>[] quarterCalculationFromSelected(int numberOfPeriods, int _quarter, int _year)
        {
            Tuple<int, int>[] data = new Tuple<int, int>[numberOfPeriods];
            int quarter = _quarter;
            int year = _year;

            for (int i = 0; i < numberOfPeriods; i++)
            {
                data[i] = new Tuple<int, int>(quarter, year);
                if (quarter == 1)
                {
                    quarter = 4;
                    year--;
                }
                else
                    quarter--;
            }
            return data;
        }

        void InitialSettingForView(int? quarter, int? year, Resource resource)
        {
            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;
        }

        #endregion

        /***********************************************************  Region for Index*********************************************/
        #region General



        public ActionResult Index()
        {
            Resource resource;

            ViewBag.Year = GetCurrentYear();
            ListOfYears();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            // Resource object
            ViewBag.ActiveDirectoryId = resource.ActiveDirectoryId;
            ViewBag.Position = resource.Employee.CurrentPosition;
            ViewBag.AspiringPosition = resource.Employee.AspiringPosition;
            ViewBag.Name = resource.Name;
            ViewBag.ResourceId = resource.ResourceId;
            ViewBag.CurrentCycle = GetCurrentCycle();
            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.TeamMembersInputSurveyAvailability = GetSurveyAvailability(GetActualQuarter(), GetCurrentYear(), TEAM_MEMBERS_INPUT, resource.ResourceId);

            ViewBag.QuartelyGoalsTime = "Quartely Goals " + GetCurrentCycle();

            // we chack 
            //List<Resource> managerResources;
            //    if (ResourceDataAccessor.TryGetResourcesFromManager(User.Identity.Name, out managerResources))
            //{
            //    if (managerResources.Count > 0)
            //        ViewBag.canCheck = 1;
            //}

            // AP | Check user have any resources
            if (context.Resources.Where(r => r.Employee.ManagerId == resource.ActiveDirectoryId).ToList<Resource>().Count > 0)
                ViewBag.canCheck = 1;

            /*
             * Code commented since there is an error when loading the Index action on test enviroment.
             * 
            RolePermissionObjectViewBag s = new RolePermissionObjectViewBag();
            s.Permissions = ProfileManager.ProfileManager.UserSeccionAccessAllActive(resource.ResourceId).AsQueryable();
            ViewBag.QueryPermissions = s.Permissions;
            */
            return View(resource);
        }

        #endregion

        /********************************************  Region for Team Members Input and Managers Check Methods ******************************************/
        #region Team Members Input and Managers Check

        // select PDF implementation
        [HttpPost]
        [ValidateInput(false)]  // TODO: How to overcome this ValildateInput
        public ActionResult ConvertToPDF(string data)
        {
            var m = JsonConvert.DeserializeObject<Html2PdfModel>(data);

            // read parameters from the webpage
            string baseUrl = this.Request.PhysicalPath;

            string pdf_page_size = "Letter";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(
                typeof(PdfPageOrientation), pdf_orientation, true);

            int webPageWidth = 1024;

            int webPageHeight = 0;

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            converter.Options.ViewerPreferences.PageLayout = PdfViewerPageLayout.SinglePage;

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(m.html2PDF, baseUrl);

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();

            // this is just a test and should not go to production
            System.IO.File.WriteAllBytes("c:\\test\\" + m.Name + ".pdf", pdf);

            // return resulted pdf document
            FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            fileResult.FileDownloadName = "Document.pdf";
            return fileResult;
        }

        //  
        public ActionResult TeamMembersInputAreadyFilled()
        {

            return View();
        }

        // ActionResult for Team Members Input
        public ActionResult TeamMembersInput(int? id)
        {
            Resource resource;
#if DEBUG
            //  username = "cdptracker manager";
#endif
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            // It is time for fill the form?

            //Aready forms filled?

            int iQuarter = GetActualQuarter();
            int iYear = GetCurrentYear();

            var oAreadyFormsFilled = context.SurveyResource.Where(
                x => x.ResourceId.Equals(resource.ResourceId) &&
                x.Survey.Quarter == iQuarter && x.ResourceEvaluatedId == null &&
                x.Survey.Year == iYear).FirstOrDefault();

            if (oAreadyFormsFilled != null)
                return RedirectToAction("TeamMembersInputAreadyFilled", "GoalTracking");





            ViewBag.Year = iYear;
            ListOfYears();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", iQuarter);
            ViewBag.CurrentCycle = GetCurrentCycle();
            SetupCommonViewBagValues();
            SetProfileData(resource);

            var surveyData = GetSurveyQuestionsResponses(TEAM_MEMBERS_INPUT, iQuarter, iYear);

            var questions = ((ViewResult)surveyData).Model;
            ViewBag.evaluatedResourceId = id;



            return View(questions);
        }


        // POST: ActionResult for Saving Team Members Input Survey
        [HttpPost]
        public ActionResult SaveTeamMembersInputSurvey(String data)
        {
            SaveSurvey(data, TEAM_MEMBERS_INPUT);
            return RedirectToAction("PrioritiesManagement", "GoalTracking");
        }

        // POST: ActionResult for Saving Managers Check Survey
        [HttpPost]
        public ActionResult SaveManagersCheckSurvey(String data)
        {
            SaveSurvey(data, MANAGERS_CHECK);
            return RedirectToAction("PrioritiesManagement", "GoalTracking");
        }

        // Saving Survey Data from Team Members Input or Managers Check
        public void SaveSurvey(String data, int surveyType)
        {
            var surveyData = JsonConvert.DeserializeObject<SurveyResourceModel>(data);

            if (surveyData != null)
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);
                SurveyResource survey = new SurveyResource();

                survey.ResourceId = resource.ResourceId;
                survey.SurveyId = surveyData.surveyId;
                survey.DateAnswered = DateTime.Now;
                survey.SurveyType = surveyType;
                survey.ResourceEvaluatedId = surveyData.resourceEvaluatedId;
                context.SurveyResource.Add(survey);

                foreach (var item in surveyData.answers)
                {
                    SurveyResponse response = new SurveyResponse();
                    response.QuestionId = item.questionId;
                    response.ResourceId = resource.ResourceId;
                    response.ResponseId = item.responseId;
                    response.ResponseText = item.responseText;
                    response.SurveyResourceId = survey.SurveyResourceId;

                    context.SurveyResponse.Add(response);

                }
                context.SaveChanges();
            }


        }

        // ActionResult for Manager's Check
        public ActionResult ManagersCheckList(int? quarter, int? year)
        {
            string managerName = User.Identity.Name;
            Resource manager;

            // Fill manager object
            if (!ResourceDataAccessor.TryGetResourceByUserName(managerName, out manager))
            {
                return View();
            }

            SetupCommonViewBagValues();
            SetProfileData(manager);
            ListOfYears();

            if (quarter == null || year == null)
            {
                quarter = GetActualQuarter();
                year = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", quarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", year);
            ViewBag.CurrentCycle = GetCurrentCycle();
            ViewBag.changeQuarter = true;


            // AP | Need them for sql function and retrieve information 
            DateTime dtStarDateQuarter = GetMinDatePeriodQuarter((int)quarter, (int)year);
            DateTime dtEndDateQuarter = GetMaxDatePeriodQuarter((int)quarter, (int)year);

            //AP | Calculate the last Quarter

            int iLastQuarter = 0;
            int iLastYear = 0;

            if (quarter == 1)
            {
                iLastQuarter = 4;
                iLastYear = (int)(year - 1);
            }
            else
            {
                iLastQuarter = (int)(quarter - 1);
                iLastYear = (int)year;
            }

            //AP | Calculate the next Quarter 
            int iNextQuarter = 0;
            int iNextYear = 0;

            if (quarter == 4)
            {
                iNextQuarter = 1;
                iNextYear = (int)(year + 1);
            }
            else
            {
                iNextQuarter = (int)(quarter + 1);
                iNextYear = (int)year;
            }

            ViewBag.NextQuarter = GetCycleText(iNextQuarter, iNextYear);
            ViewBag.current = GetCycleText(quarter, year);

            //AP | Get min and max date from the last quarter
            DateTime dtStarLastDateQuarter = GetMinDatePeriodQuarter(iLastQuarter, iLastYear);
            DateTime dtEndLastDateQuarter = GetMaxDatePeriodQuarter(iLastQuarter, iLastYear);

            // AP | Get information from DataBase, we get a list of manager's team menber 
            List<ManagerResource> listManagerResource = context.udf_ManagersCheck_members_list(
                manager.ActiveDirectoryId,
                manager.ResourceId,
                dtStarDateQuarter,
                dtEndDateQuarter,
                dtStarLastDateQuarter,
                dtEndLastDateQuarter
                ).Select(m => new ManagerResource
                {
                    iManagerActiveDirectoryId = (int)m.employeManagerId,
                    bResourceEvaluated = (bool)m.employeEvaluated,
                    bResourceGoals = (bool)m.employeGoals,
                    bResourceTeamMembersInput = (bool)m.employeTeamMembersImput,
                    sResourceName = (string)m.employeName,
                    iResourceActiveDirectoryId = (int)m.employeId

                }).ToList<ManagerResource>();


            ViewBag.lstManagerResource = listManagerResource;
            ViewBag.SurveyAvailable = true; //GetSurveyAvailability((int)quarter,(int)year, MANAGERS_CHECK, item.Resource.ResourceId);
            return View();
        }

        // ActionResult for Manager's Check        
        public ActionResult ManagersCheck(int? id, int? quarter, int? year)
        {
            if (id == null)
            {
                return RedirectToAction("ManagersCheckList", "GoalTracking");
            }

            string managerName = User.Identity.Name;

            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(managerName, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            //#if DEBUG
            //            managerName = "cdptracker manager";
            //#endif
            //List<Resource> managerResources;
            //if (!ResourceDataAccessor.TryGetResourcesFromManager(managerName, out managerResources))
            //{
            //    return RedirectToAction("ManagersCheckList", "GoalTracking");
            //}
            //var employeeFound = managerResources.Where(r => r.Employee != null).Select(r => r.Employee).Where(r => r.ResourceId == id).FirstOrDefault(); //.OrderBy(r => r.Resource.Name).ToList();

            //if (employeeFound == null)
            //{
            //    return RedirectToAction("ManagersCheckList", "GoalTracking");
            //}

            //AP | 2016

            // List --------------------
            //var listResources = context.Resources.Where(r => r.Employee.ManagerId == resource.ResourceId).ToList<Resource>();


            ViewBag.Year = GetCurrentYear();
            ListOfYears();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());
            ViewBag.CurrentCycle = GetCurrentCycle();
            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);

            SetupCommonViewBagValues();

            SetProfileData(resource);

            if (quarter == null || year == null)
            {
                quarter = GetActualQuarter();
                year = GetCurrentYear();
            }

            ViewBag.CurrentCycle = GetCycleText(quarter, year);

            var surveyData = GetSurveyQuestionsResponses(MANAGERS_CHECK, (int)quarter, (int)year);
            var questions = ((ViewResult)surveyData).Model;
            ViewBag.evaluatedResourceId = id;

            var surveyAnswered = GetSurveyAnswers(MANAGERS_CHECK, (int)id, (int)quarter, (int)year);
            var answers = ((ViewResult)surveyAnswered).Model;

            bool canConsultSurvey = false;

            if (isCurrentQuarter(year, quarter))
            {
                canConsultSurvey = true;
            }
            else if (((SurveyAnsweredModel)answers).surveyResponses != null)
            {
                canConsultSurvey = true;
            }

            var data = new Tuple<Object, Object, Boolean>(questions, answers, canConsultSurvey);

            return View(data);
        }


        // ActionResult for Getting Questions and Responses
        public ActionResult GetSurveyQuestionsResponses(int SurveyType, int Quarter, int Year)
        {
            List<Survey> survey = context.Survey.OrderByDescending(s => s.SurveyId).Where(s => s.Quarter == Quarter && s.Year == Year && s.SurveyType == SurveyType).ToList();
            var surveyId = survey.FirstOrDefault().SurveyId;

            List<Question> questions = context.Question.OrderBy(q => q.Sequence).Where(q => q.SurveyId == surveyId).ToList();
            List<QuestionModel> questionList = new List<QuestionModel>();
            foreach (var question in questions)
            {
                QuestionModel questionModel = new QuestionModel();

                questionModel.questionId = question.QuestionId;
                questionModel.surveyId = question.SurveyId;
                questionModel.text = question.Text;
                questionModel.sequence = question.Sequence;
                questionModel.questionType = question.QuestionType;
                questionModel.questionChild = question.QuestionChild;
                questionModel.required = question.Required;
                questionModel.displayWhenValue = question.DisplayWhenValue;

                var responses = context.QuestionResponse.
                    Join(context.Response,
                    qr => qr.ResponseId, r => r.ResponseId,
                    (qr, r) => new
                    {
                        responseId = qr.ResponseId,
                        answer = r.Answer,
                        questionId = qr.QuestionId
                    }).Where(qr => qr.questionId == question.QuestionId).ToList();

                List<ResponseModel> responseList = new List<ResponseModel>();
                foreach (var response in responses)
                {
                    ResponseModel responseModel = new ResponseModel();
                    responseModel.questionId = response.questionId;
                    responseModel.responseId = response.responseId;
                    responseModel.answer = response.answer;
                    responseList.Add(responseModel);
                }
                questionModel.responses = responseList;

                questionList.Add(questionModel);
            }

            return View(questionList);
        }

        // ActionResult for reviewing if there is a survey available and has not been answered
        public int GetSurveyAvailability(int Quarter, int Year, int SurveyType, int ResourceId)
        {
            var survey = context.Survey.Where(s => s.Quarter == Quarter && s.Year == Year && s.SurveyType == SurveyType).FirstOrDefault();
            int surveyAvailavility = 0;

            // with the new logic to create a Survey we consider the CreatedTimeStamp field as enabled date, if it hasn't been enabled today then the survey is not available.
            if (survey == null || DateTime.Compare((DateTime)survey.CreatedTimeStamp, DateTime.Today) > 0)
            {
                surveyAvailavility = -1;
                return surveyAvailavility;
            }

            if (survey.SurveyId != 0)
            {
                if (SurveyType == TEAM_MEMBERS_INPUT)
                {
                    var querySurveyAvailavility = context.SurveyResource.Where(sr => sr.SurveyId == survey.SurveyId && sr.ResourceId == ResourceId).FirstOrDefault();
                    if (querySurveyAvailavility != null)
                        surveyAvailavility = querySurveyAvailavility.SurveyId;
                }
                else if (SurveyType == MANAGERS_CHECK)
                {
                    var querySurveyAvailavility = context.SurveyResource.Where(sr => sr.SurveyId == survey.SurveyId && sr.ResourceEvaluatedId == ResourceId).FirstOrDefault();
                    if (querySurveyAvailavility != null)
                        surveyAvailavility = querySurveyAvailavility.SurveyId;
                }
            }
            return surveyAvailavility;
        }

        public ActionResult GetSurveyAnswers(int SurveyType, int resourceId, int quarter, int year)
        {
            SurveyAnsweredModel survey = new SurveyAnsweredModel();
            var quarterSurvey = context.Survey.Where(s => s.Quarter == quarter && s.Year == year && s.SurveyType == SurveyType).FirstOrDefault();
            var surveyInfo = context.SurveyResource.Where(x => (SurveyType == MANAGERS_CHECK ? x.ResourceEvaluatedId : x.ResourceId) == resourceId &&
x.SurveyType == SurveyType &&
                x.SurveyId == quarterSurvey.SurveyId).FirstOrDefault();

            if (surveyInfo != null)
            {
                survey.managerName = context.Resources.Where(x => x.ResourceId == surveyInfo.ResourceId).FirstOrDefault().Name;
                survey.surveyQuarterId = surveyInfo.SurveyId;
                survey.resourceId = (SurveyType == MANAGERS_CHECK) ? surveyInfo.ResourceEvaluatedId : surveyInfo.ResourceId;
                List<SurveyResponseModel> surveyResponses = new List<SurveyResponseModel>();
                foreach (var item in surveyInfo.SurveyResponse)
                {
                    SurveyResponseModel surveyResponse = new SurveyResponseModel();
                    surveyResponse.questionId = item.QuestionId;
                    surveyResponse.responseId = item.ResponseId;
                    surveyResponse.responseText = item.ResponseText;
                    surveyResponses.Add(surveyResponse);
                }
                survey.surveyResponses = surveyResponses;
            }
            return View(survey);
        }

        // ActionResult for Team Members Input Survey Status
        public ActionResult TeamMembersInputFollowUp()
        {
            Resource resource;
#if DEBUG
            //  username = "cdptracker manager";
#endif
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            SetupProgressReportData();

            ListOfYears();
            ViewBag.Year = GetCurrentYear();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());

            return View();

            //using (CDPTrackEntities context = new CDPTrackEntities())
            //{
            //    var list = (from r in context.Resources
            //                join surveyResource in context.SurveyResource on r.ResourceId equals surveyResource.ResourceId into resourceInner
            //                from subSurvey in resourceInner.Where(x => x.SurveyType == 1).DefaultIfEmpty()
            //                select new { r.ResourceId, r.Name, completed = (subSurvey == null ? false : true) });

            //    return View(list);
            //}

        }

        #endregion

        /***********************************************************  Region for Priorities and Tasks Methods*********************************************/
        #region Priorities and Tasks

        // ActionResult for Priorities Management
        public ActionResult PrioritiesManagement(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);
            ViewBag.changeQuarter = true;

            List<Objective> objectives = resource.Objective.OrderByDescending(r => r.ObjectiveId).Where(r => r.Quarter == quarter && r.Year == year).ToList();

            List<CyclePrioritiesModel> priorityList = new List<CyclePrioritiesModel>();
            foreach (var objective in objectives)
            {
                CyclePrioritiesModel priority = new CyclePrioritiesModel();
                priority.priorityId = objective.ObjectiveId;
                priority.priorityDescription = objective.Objective1;
                priority.priorityProgress = objective.Progress;

                switch (objective.Progress)
                {
                    case STATUS_NOT_STARTED:
                        priority.statusTitle = "Not Started";
                        break;
                    case STATUS_STARTED:
                        priority.statusTitle = "Started";
                        break;
                    case STATUS_COMPLETED:
                        priority.statusTitle = "Completed";
                        break;
                    default:
                        priority.statusTitle = "Undefined";
                        break;
                }

                List<TasksModel> taskList = new List<TasksModel>();
                foreach (var goal in objective.GoalTracking)
                {
                    TasksModel task = new TasksModel();
                    task.taskId = goal.GoalId;
                    task.taskDescription = goal.Goal;
                    task.taskFinishDate = goal.FinishDate;
                    task.taskProgress = goal.Progress;
                    task.taskVerified = goal.VerifiedByManager;
                    task.taskTdu = goal.TDU;
                    task.trainingCategoryId = goal.TrainingCategoryId;

                    if (goal.VerifiedByManager)
                    {
                        task.imgProgress = "~/Content/images/verifiedManager.png";
                        task.statusTitle = "Verified by manager";
                    }
                    else
                    {
                        switch ((int)task.taskProgress)
                        {
                            case STATUS_NOT_STARTED:
                                task.imgProgress = "~/Content/images/notStarted.png";
                                task.statusTitle = "Not started";
                                break;
                            case STATUS_STARTED:
                                task.imgProgress = "~/Content/images/inProgress.png";
                                task.statusTitle = "Started";
                                break;
                            case STATUS_COMPLETED:
                                task.imgProgress = "~/Content/images/done.png";
                                task.statusTitle = "Completed";
                                break;
                            default:
                                task.imgProgress = "~/Content/images/DeleteGoal-icon.png";
                                task.statusTitle = "Undefined";
                                break;
                        }
                    }

                    taskList.Add(task);
                }
                priority.tasks = taskList;

                priorityList.Add(priority);
            }

            return View(priorityList);
        }

        // GET: ActionResult for Adding a Priority
        public ActionResult Priority(int? id)
        {
            Resource resource;

            string userName = User.Identity.Name;

#if DEBUG
            userName = "cdptracker manager";
#endif
            if (!ResourceDataAccessor.TryGetResourceByUserName(userName, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.PriorityVerified = false;
            CyclePrioritiesModel priority = new CyclePrioritiesModel();
            Objective query = new Objective();
            if (id != null)
            {
                query = context.Objectives.Where(r => r.ObjectiveId == id).FirstOrDefault();

                if (query == null || query.ResourceId != resource.ResourceId)
                {
                    return RedirectToAction("PrioritiesManagement", "GoalTracking");
                }

                priority.priorityId = query.ObjectiveId;
                priority.priorityDescription = query.Objective1;
                priority.priorityProgress = query.Progress;
                priority.priorityQuarter = query.Quarter;
                priority.priorityYear = query.Year;
                priority.resourceId = query.ResourceId;
                priority.categoryId = query.CategoryId;
                List<TasksModel> taskList = new List<TasksModel>();
                foreach (var item in query.GoalTracking)
                {
                    TasksModel task = new TasksModel();
                    task.taskId = item.GoalId;
                    task.taskDescription = item.Goal;
                    task.taskProgress = item.Progress;
                    task.taskTdu = item.TDU;
                    task.taskFinishDate = item.FinishDate;
                    task.sourceId = item.SourceId == null ? 0 : item.SourceId;
                    task.trainingCategoryId = item.TrainingCategoryId;
                    task.taskVerified = item.VerifiedByManager;
                    taskList.Add(task);
                }
                priority.tasks = taskList;

                foreach (var item in taskList)
                {
                    if (item.taskVerified) { ViewBag.PriorityVerified = item.taskVerified; break; }
                }

                ViewBag.ActualQuarter = priority.priorityQuarter;
            }
            else
            {
                ViewBag.ActualQuarter = GetActualQuarter();
            }

            ViewBag.ListOfCategories = GetCategories();
            ViewBag.ListOfYears = ListOfYearsFuture();
            ViewBag.ListOfQuarters = GetQuarter();
            ViewBag.ListOfSources = GetSources();
            ViewBag.ListOfTCategories = GetTrainingCategories();
            ViewBag.CurrentCycle = GetCurrentCycle();
            return View(priority);
        }

        // POST: ActionResult for Adding a Priority and tasks
        [HttpPost]
        public ActionResult AddPriority(String data)
        {
            var priorityData = JsonConvert.DeserializeObject<CyclePrioritiesModel>(data);

            if (priorityData != null)
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);
                //Save Object
                Objective priority = new Objective();

                priority.Objective1 = priorityData.priorityDescription;
                priority.Progress = STATUS_NOT_STARTED;
                priority.Duplicated = false;
                priority.CategoryId = priorityData.categoryId;
                priority.Quarter = priorityData.priorityQuarter;
                priority.Year = priorityData.priorityYear;
                priority.ResourceId = resource.ResourceId;

                foreach (var item in priorityData.tasks)
                {
                    GoalTracking task = new GoalTracking();
                    task.Goal = item.taskDescription;
                    task.FinishDate = item.taskFinishDate;
                    task.SourceId = item.sourceId;
                    task.Progress = STATUS_NOT_STARTED;
                    task.TrainingCategoryId = item.trainingCategoryId;
                    task.ResourceId = resource.ResourceId;
                    task.VerifiedByManager = false;
                    task.TDU = item.taskTdu;
                    task.LastUpdate = DateTime.Now;

                    priority.GoalTracking.Add(task);
                }
                context.Objectives.Add(priority);
                context.SaveChanges();
            }

            return RedirectToAction("PrioritiesManagement", "GoalTracking");
        }

        // POST: ActionResult for Update a Priority and tasks
        [HttpPost]
        public ActionResult UpdatePriority(String data)
        {
            var priorityData = JsonConvert.DeserializeObject<CyclePrioritiesModel>(data);

            if (priorityData != null)
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);

                Objective dbPriority = context.Objectives.Where(r => r.ObjectiveId == priorityData.priorityId).FirstOrDefault();

                dbPriority.Objective1 = priorityData.priorityDescription;
                dbPriority.CategoryId = priorityData.categoryId;
                dbPriority.Quarter = priorityData.priorityQuarter;
                dbPriority.Year = priorityData.priorityYear;

                //DELETE TASKS
                List<int> oldTasks = dbPriority.GoalTracking.Select(t => t.GoalId).ToList();
                List<int> newTasks = priorityData.tasks.Select(t => t.taskId).ToList();
                List<int> deleteTasks = oldTasks.Except(newTasks).ToList();
                foreach (var item in deleteTasks)
                {
                    GoalTracking task = context.GoalTrackings.Where(r => r.GoalId == item).FirstOrDefault();
                    context.GoalTrackings.Remove(task);
                }

                //UPDATES
                foreach (var item in priorityData.tasks)
                {
                    GoalTracking task = (from t in context.GoalTrackings
                                         where t.GoalId == item.taskId
                                         select t).FirstOrDefault();

                    //INSERT NEW TASK
                    if (task == null)
                    {
                        GoalTracking dbTask = new GoalTracking();
                        dbTask.Goal = item.taskDescription;
                        dbTask.FinishDate = item.taskFinishDate;
                        dbTask.TDU = item.taskTdu;
                        dbTask.TrainingCategoryId = item.trainingCategoryId;
                        dbTask.SourceId = item.sourceId;
                        dbTask.LastUpdate = DateTime.Now;
                        dbTask.ResourceId = resource.ResourceId;
                        dbTask.ObjectiveId = dbPriority.ObjectiveId;
                        dbTask.Progress = 0;
                        dbTask.VerifiedByManager = false;

                        dbPriority.GoalTracking.Add(dbTask);
                    }
                    else
                    {
                        //UPDATE EXISTING TASK
                        if (!item.taskVerified)
                        {
                            task.Goal = item.taskDescription;
                            task.FinishDate = item.taskFinishDate;
                            task.TDU = item.taskTdu;
                            task.TrainingCategoryId = item.trainingCategoryId;
                            task.SourceId = item.sourceId;
                            task.LastUpdate = DateTime.Now;
                            task.Progress = item.taskProgress;
                            task.VerifiedByManager = item.taskVerified;
                        }
                    }
                }

                context.SaveChanges();
            }

            return RedirectToAction("PrioritiesManagement", "GoalTracking");
        }

        // POST: ActionResult for Delete a Priority and tasks
        [HttpPost]
        public ActionResult DeletePriority(int priorityId)
        {
            if (priorityId != 0)
            {
                Objective dbPriority = context.Objectives.Where(r => r.ObjectiveId == priorityId).FirstOrDefault();
                bool denyDelete = false;

                foreach (var item in dbPriority.GoalTracking)
                {
                    if (item.VerifiedByManager) { denyDelete = true; break; }
                }

                if (!denyDelete)
                {
                    List<int> deleteTasks = dbPriority.GoalTracking.Select(t => t.GoalId).ToList();
                    foreach (var item in deleteTasks)
                    {
                        GoalTracking task = context.GoalTrackings.Where(r => r.GoalId == item).FirstOrDefault();
                        context.GoalTrackings.Remove(task);
                    }

                    context.Objectives.Remove(dbPriority);
                    context.SaveChanges();
                }
                else
                {
                    return Json(new { error = ErrorHandler(403) }, JsonRequestBehavior.AllowGet);
                }
            }

            return RedirectToAction("PrioritiesManagement", "GoalTracking");
        }

        // POST: ActionResult for Change the status of task and priority
        [HttpPost]
        public ActionResult ChangeTaskStatus(int taskId)
        {
            if (taskId != 0)
            {
                GoalTracking dbTask = context.GoalTrackings.Where(t => t.GoalId == taskId).FirstOrDefault();

                dbTask.Progress = (dbTask.Progress == STATUS_COMPLETED) ? STATUS_NOT_STARTED : dbTask.Progress + 1;
                context.SaveChanges();

                List<GoalTracking> listTasks = context.GoalTrackings.OrderByDescending(x => x.Progress).Where(t => t.ObjectiveId == dbTask.ObjectiveId).ToList();
                var groups = listTasks.GroupBy(o => o.Progress).Select(group =>
                         new
                         {
                             progress = group.Key,
                             count = group.Count()
                         }).ToList();

                int progress = STATUS_STARTED;
                foreach (var pair in groups)
                {
                    if (pair.progress == 0)
                    {
                        if (pair.count == listTasks.Count)
                        {
                            progress = STATUS_NOT_STARTED;
                        }
                    }
                    if (pair.progress == 2)
                    {
                        if (pair.count == listTasks.Count)
                        {
                            progress = STATUS_COMPLETED;
                        }
                    }
                }

                Objective priority = context.Objectives.Where(t => t.ObjectiveId == dbTask.ObjectiveId).FirstOrDefault();
                priority.Progress = progress;

                context.SaveChanges();
            }
            return RedirectToAction("PrioritiesManagement", "GoalTracking");
        }

        // ActionResult for Quarterly Priorities
        public ActionResult QuarterlyPriorities(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                //verify if exist one page plan
                var onePagePlanConsult = (from se in context.OnePagePlan where se.year == year && se.Quarter == quarter && se.ResourceId == resource.ResourceId select se).ToList();


                if (onePagePlanConsult.Count > 0) // if one page plan is active
                {

                    // Critical Numbers
                    ViewBag.CriticalNumberSG = onePagePlanConsult[0].SG;
                    ViewBag.CriticalNumberG = onePagePlanConsult[0].G;
                    ViewBag.CriticalNumberR = onePagePlanConsult[0].R;

                    //CoreValues labels
                    var val1 = context.CoreValues.Find(onePagePlanConsult[0].month1_CoreValueId);
                    var val2 = context.CoreValues.Find(onePagePlanConsult[0].month2_CoreValueId);
                    var val3 = context.CoreValues.Find(onePagePlanConsult[0].month3_CoreValueId);

                    ViewBag.Value1 = val1.coreValue;
                    ViewBag.Value2 = val2.coreValue;
                    ViewBag.Value3 = val3.coreValue;


                    ViewBag.onePagePlanId = onePagePlanConsult[0].OnePagePlanId;
                    int idOPP = onePagePlanConsult[0].OnePagePlanId;

                    var quartelyData = (from qry in context.QuarterlyPriorities where qry.Quarter == quarter && qry.Year == year && qry.ResourceId == resource.ResourceId && qry.OnePagePlanId == idOPP select qry).ToList();

                    var KthurstList = (from ktl in context.KeyThrusts where ktl.OnePagePlanId == idOPP select ktl).ToList();
                    ViewBag.KthurstList = KthurstList;

                    if (quartelyData.Count != 0)
                    {
                        int id = (int)quartelyData[0].QuarterlyPrioritiesId;

                        ViewBag.quarterlyPriorityId = id;

                        var kpiData = (from kpd in context.KPI where kpd.QuaterlyPrioritiesID == id select kpd).ToList();
                        var qaData = (from qad in context.QuarterlyActions where qad.QuarterlyPrioritiesId == id select qad).ToList();
                        var valIData = (from valI in context.ValuesInfusion where valI.QuarterlyPrioritiesId == id select valI).ToList();
                        var pdData = (from pdD in context.PersonalDevelopment where pdD.QuarterlyPrioritiesId == id select pdD).ToList();


                        ViewBag.kpiData = kpiData;
                        ViewBag.qaData = qaData;
                        ViewBag.valIData = valIData;
                        ViewBag.pdData = pdData;

                    }
                    else
                    {

                        ViewBag.quarterlyPriorityId = 0;
                        ViewBag.kpiData = null;
                        ViewBag.qaData = null;
                        ViewBag.valIData = null;
                        ViewBag.pdData = null;

                    }
                }
                else // other wise all point to null
                {
                    ViewBag.quarterlyPriorityId = 0;
                    ViewBag.CriticalNumberSG = "0";
                    ViewBag.CriticalNumberG = "0";
                    ViewBag.CriticalNumberR = "0";
                    ViewBag.Value1 = "value 1";
                    ViewBag.Value2 = "value 2";
                    ViewBag.Value3 = "value 3";
                    ViewBag.KthurstList = "";
                    ViewBag.quarterlyPriorityId = 0;
                    ViewBag.kpiData = null;
                    ViewBag.qaData = null;
                    ViewBag.valIData = null;
                    ViewBag.pdData = null;
                }


            }



            return View();
        }

        [HttpPost]
        public ActionResult AddQuarterlyPriorities(string data)
        {
            var m = JsonConvert.DeserializeObject<CDPTrackerSite.Models.QuartelyPriorities>(data);

            if (m != null)
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);

                int quarter = GetQuarter(DateTime.Now.Month);
                int year = DateTime.Now.Year;
                int idQuartelyPriorities = 0;
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    DataSource.QuarterlyPriorities qp = new QuarterlyPriorities();
                    qp.ResourceId = resource.ResourceId;
                    qp.Quarter = quarter;
                    qp.Year = year;
                    qp.CreationDate = (DateTime)DateTime.Now;
                    qp.OnePagePlanId = m.onePagePlanId;
                    qp.KeyIssue = "";
                    context.QuarterlyPriorities.Add(qp);
                    context.SaveChanges();
                    idQuartelyPriorities = qp.QuarterlyPrioritiesId;

                    foreach (var item in m.Kpi)
                    {
                        KPI kp = new KPI();
                        kp.Name = item.name;
                        //kp.Goal = item.goal;
                        kp.QuaterlyPrioritiesID = idQuartelyPriorities;
                        context.KPI.Add(kp);
                    }

                    foreach (var item in m.quarterly_actions)
                    {
                        QuarterlyActions qa = new QuarterlyActions();
                        qa.Action = item.action;
                        qa.DueDate = DateTime.Parse(item.due_date);
                        qa.Outcome = item.outcome;
                        qa.QuarterlyPrioritiesId = idQuartelyPriorities;
                        qa.KeyIniciative = Int32.Parse(item.key_initiative); //cambiar por la key initiative
                        context.QuarterlyActions.Add(qa);
                    }

                    foreach (var item in m.values_infusion)
                    {
                        DataSource.ValuesInfusion vi = new DataSource.ValuesInfusion();
                        vi.Value = item.action;
                        vi.IsDone = true;
                        vi.QuarterlyPrioritiesId = idQuartelyPriorities;
                        context.ValuesInfusion.Add(vi);
                    }

                    foreach (var item in m.personal_development)
                    {
                        DataSource.PersonalDevelopment pd = new DataSource.PersonalDevelopment();
                        pd.Skill = item.skill;
                        pd.DefinitionOfSuccess = item.definition_of_success;
                        pd.Outcome = item.outcome;
                        pd.QuarterlyPrioritiesId = idQuartelyPriorities;
                        context.PersonalDevelopment.Add(pd);
                    }


                    context.SaveChanges();
                }
            }


            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateQuarterlyPrioritiesEndOfQuarter(string data)
        {
            var m = JsonConvert.DeserializeObject<CDPTrackerSite.Models.QuartelyPriorities>(data);

            if (m != null)
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);

                int quarter = GetQuarter(DateTime.Now.Month);
                int year = DateTime.Now.Year;
                int idQuartelyPriorities = m.quarterlyPriorityId;
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    DataSource.QuarterlyPriorities qp = new QuarterlyPriorities();
                    qp.ResourceId = resource.ResourceId;
                    qp.Quarter = quarter;
                    qp.Year = year;
                    qp.QuarterlyPrioritiesId = m.quarterlyPriorityId;
                    qp.OnePagePlanId = m.onePagePlanId;
                    qp.KeyIssue = m.cycle_comments;
                    qp.QuarterlyPrioritiesId = m.quarterlyPriorityId;

                    context.Entry(qp).State = System.Data.EntityState.Modified;
                    context.SaveChanges();

                    //Delete elemets from connector tables

                    var ktdel = from ktd in context.KPI where ktd.QuaterlyPrioritiesID == m.quarterlyPriorityId select ktd;
                    foreach (var item in ktdel)
                    {
                        context.KPI.Remove(item);
                    }
                    if (ktdel.Count() > 0)
                        context.SaveChanges();

                    var ktAct = from ktd in context.QuarterlyActions where ktd.QuarterlyPrioritiesId == m.quarterlyPriorityId select ktd;
                    foreach (var item in ktAct)
                    {
                        context.QuarterlyActions.Remove(item);
                    }
                    if (ktAct.Count() > 0)
                        context.SaveChanges();

                    var ktValI = from ktd in context.ValuesInfusion where ktd.QuarterlyPrioritiesId == m.quarterlyPriorityId select ktd;
                    foreach (var item in ktValI)
                    {
                        context.ValuesInfusion.Remove(item);
                    }
                    if (ktValI.Count() > 0)
                        context.SaveChanges();

                    var ktPdev = from ktd in context.PersonalDevelopment where ktd.QuarterlyPrioritiesId == m.quarterlyPriorityId select ktd;
                    foreach (var item in ktPdev)
                    {
                        context.PersonalDevelopment.Remove(item);
                    }
                    if (ktPdev.Count() > 0)
                        context.SaveChanges();

                    //Insert elements 
                    foreach (var item in m.Kpi)
                    {
                        KPI kp = new KPI();
                        kp.Name = item.name;
                        //kp.Goal = item.goal;
                        kp.QuaterlyPrioritiesID = idQuartelyPriorities;
                        kp.Result = item.results;
                        context.KPI.Add(kp);
                    }

                    foreach (var item in m.quarterly_actions)
                    {
                        QuarterlyActions qa = new QuarterlyActions();
                        qa.Action = item.action;
                        qa.DueDate = DateTime.Parse(item.due_date);
                        qa.Outcome = item.outcome;
                        qa.QuarterlyPrioritiesId = idQuartelyPriorities;
                        qa.KeyIniciative = Int32.Parse(item.key_initiative);
                        context.QuarterlyActions.Add(qa);
                    }

                    foreach (var item in m.values_infusion)
                    {
                        DataSource.ValuesInfusion vi = new DataSource.ValuesInfusion();
                        vi.Value = item.action;
                        vi.IsDone = item.value_checked != 0;
                        vi.QuarterlyPrioritiesId = idQuartelyPriorities;
                        context.ValuesInfusion.Add(vi);
                    }

                    foreach (var item in m.personal_development)
                    {
                        DataSource.PersonalDevelopment pd = new DataSource.PersonalDevelopment();
                        pd.Skill = item.skill;
                        pd.DefinitionOfSuccess = item.definition_of_success;
                        pd.Outcome = item.outcome;
                        pd.QuarterlyPrioritiesId = idQuartelyPriorities;
                        context.PersonalDevelopment.Add(pd);
                    }

                    context.SaveChanges();


                }
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateQuarterlyPriorities(string data)
        {
            var m = JsonConvert.DeserializeObject<CDPTrackerSite.Models.QuartelyPriorities>(data);

            if (m != null)
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);

                int quarter = GetQuarter(DateTime.Now.Month);
                int year = DateTime.Now.Year;
                int idQuartelyPriorities = m.quarterlyPriorityId;
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    DataSource.QuarterlyPriorities qp = new QuarterlyPriorities();
                    qp.ResourceId = resource.ResourceId;
                    qp.Quarter = quarter;
                    qp.Year = year;
                    qp.QuarterlyPrioritiesId = m.quarterlyPriorityId;
                    qp.OnePagePlanId = m.onePagePlanId;
                    qp.KeyIssue = "";

                    context.Entry(qp).State = System.Data.EntityState.Modified;
                    context.SaveChanges();

                    //Delete elemets from connector tables

                    var ktdel = from ktd in context.KPI where ktd.QuaterlyPrioritiesID == m.quarterlyPriorityId select ktd;
                    foreach (var item in ktdel)
                    {
                        context.KPI.Remove(item);
                    }
                    if (ktdel.Count() > 0)
                        context.SaveChanges();

                    var ktAct = from ktd in context.QuarterlyActions where ktd.QuarterlyPrioritiesId == m.quarterlyPriorityId select ktd;
                    foreach (var item in ktAct)
                    {
                        context.QuarterlyActions.Remove(item);
                    }
                    if (ktAct.Count() > 0)
                        context.SaveChanges();

                    var ktValI = from ktd in context.ValuesInfusion where ktd.QuarterlyPrioritiesId == m.quarterlyPriorityId select ktd;
                    foreach (var item in ktValI)
                    {
                        context.ValuesInfusion.Remove(item);
                    }
                    if (ktValI.Count() > 0)
                        context.SaveChanges();

                    var ktPdev = from ktd in context.PersonalDevelopment where ktd.QuarterlyPrioritiesId == m.quarterlyPriorityId select ktd;
                    foreach (var item in ktPdev)
                    {
                        context.PersonalDevelopment.Remove(item);
                    }
                    if (ktPdev.Count() > 0)
                        context.SaveChanges();

                    //Insert elements 
                    foreach (var item in m.Kpi)
                    {
                        KPI kp = new KPI();
                        kp.Name = item.name;
                        //kp.Goal = item.goal;
                        kp.QuaterlyPrioritiesID = idQuartelyPriorities;
                        kp.Result = item.results;
                        context.KPI.Add(kp);
                    }

                    foreach (var item in m.quarterly_actions)
                    {
                        QuarterlyActions qa = new QuarterlyActions();
                        qa.Action = item.action;
                        qa.DueDate = DateTime.Parse(item.due_date);
                        qa.Outcome = item.outcome;
                        qa.QuarterlyPrioritiesId = idQuartelyPriorities;
                        qa.KeyIniciative = Int32.Parse(item.key_initiative);
                        context.QuarterlyActions.Add(qa);
                    }

                    foreach (var item in m.values_infusion)
                    {
                        DataSource.ValuesInfusion vi = new DataSource.ValuesInfusion();
                        vi.Value = item.action;
                        vi.IsDone = true;
                        vi.QuarterlyPrioritiesId = idQuartelyPriorities;
                        context.ValuesInfusion.Add(vi);
                    }

                    foreach (var item in m.personal_development)
                    {
                        DataSource.PersonalDevelopment pd = new DataSource.PersonalDevelopment();
                        pd.Skill = item.skill;
                        pd.DefinitionOfSuccess = item.definition_of_success;
                        pd.Outcome = item.outcome;
                        pd.QuarterlyPrioritiesId = idQuartelyPriorities;
                        context.PersonalDevelopment.Add(pd);
                    }

                    context.SaveChanges();


                }
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        // ActionResult for Quarterly Priorities End of Quarter
        public ActionResult QuarterlyPrioritiesEndOfQuarter(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();


            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                //verify if exist one page plan
                var onePagePlanConsult = (from se in context.OnePagePlan where se.year == year && se.Quarter == quarter && se.ResourceId == resource.ResourceId select se).ToList();


                if (onePagePlanConsult.Count > 0) // if one page plan is active
                {

                    // Critical Numbers
                    ViewBag.CriticalNumberSG = onePagePlanConsult[0].SG;
                    ViewBag.CriticalNumberG = onePagePlanConsult[0].G;
                    ViewBag.CriticalNumberR = onePagePlanConsult[0].R;

                    //CoreValues labels
                    var val1 = context.CoreValues.Find(onePagePlanConsult[0].month1_CoreValueId);
                    var val2 = context.CoreValues.Find(onePagePlanConsult[0].month2_CoreValueId);
                    var val3 = context.CoreValues.Find(onePagePlanConsult[0].month3_CoreValueId);

                    ViewBag.Value1 = val1.coreValue;
                    ViewBag.Value2 = val2.coreValue;
                    ViewBag.Value3 = val3.coreValue;


                    ViewBag.onePagePlanId = onePagePlanConsult[0].OnePagePlanId;
                    int idOPP = onePagePlanConsult[0].OnePagePlanId;


                    var quartelyData = (from qry in context.QuarterlyPriorities where qry.Quarter == quarter && qry.Year == year && qry.ResourceId == resource.ResourceId && qry.OnePagePlanId == idOPP select qry).ToList();


                    var KthurstList = (from ktl in context.KeyThrusts where ktl.OnePagePlanId == idOPP select ktl).ToList();
                    ViewBag.KthurstList = KthurstList;


                    if (quartelyData.Count != 0)
                    {
                        int id = (int)quartelyData[0].QuarterlyPrioritiesId;

                        ViewBag.quarterlyPriorityId = id;

                        var kpiData = (from kpd in context.KPI where kpd.QuaterlyPrioritiesID == id select kpd).ToList();
                        var qaData = (from qad in context.QuarterlyActions where qad.QuarterlyPrioritiesId == id select qad).ToList();
                        var valIData = (from valI in context.ValuesInfusion where valI.QuarterlyPrioritiesId == id select valI).ToList();
                        var pdData = (from pdD in context.PersonalDevelopment where pdD.QuarterlyPrioritiesId == id select pdD).ToList();

                        ViewBag.kpiData = kpiData;
                        ViewBag.qaData = qaData;
                        ViewBag.valIData = valIData;
                        ViewBag.pdData = pdData;
                        ViewBag.KeyIssues = quartelyData[0].KeyIssue;

                    }
                    else
                    {

                        ViewBag.quarterlyPriorityId = 0;
                        ViewBag.kpiData = null;
                        ViewBag.qaData = null;
                        ViewBag.valIData = null;
                        ViewBag.pdData = null;
                        ViewBag.KeyIssues = "";

                    }
                }
                else // other wise all point to null
                {
                    ViewBag.quarterlyPriorityId = 0;
                    ViewBag.CriticalNumberSG = "0";
                    ViewBag.CriticalNumberG = "0";
                    ViewBag.CriticalNumberR = "0";
                    ViewBag.Value1 = "value 1";
                    ViewBag.Value2 = "value 2";
                    ViewBag.Value3 = "value 3";
                    ViewBag.KeyIssues = "";
                    ViewBag.KthurstList = "";
                    ViewBag.quarterlyPriorityId = 0;
                    ViewBag.kpiData = null;
                    ViewBag.qaData = null;
                    ViewBag.valIData = null;
                    ViewBag.pdData = null;
                }


            }




            return View();
        }
        public ActionResult TeamMembersInputExecutives2v(int? quarter, int? year)
        {

            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            var sitecombo = from cv in context.Locations orderby cv.Name ascending select cv;

            return View(sitecombo);

        }

        public ActionResult TeamMembersInputExecutive(int? quarter, int? year)
        {

            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            var sitecombo = from cv in context.Locations orderby cv.Name ascending select cv;

            return View(sitecombo);

        }

        public ActionResult TeamMembersInputByProject(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            var aux = (from emp in context.Employee where emp.ResourceId == resource.ResourceId select emp.ManagerId).Take(1);


            //var projectcombo = from cv in context.Project orderby cv.Project1 ascending select cv;
            var projectcombo = context.Database.SqlQuery<Project>(@"SELECT DISTINCT p.ProjectId,p.Project as Project1
  FROM [CDPTrack].[dbo].[Employee] e, Project p
  WHERE p.ProjectId = e.ProjectId AND e.ManagerId = " + Convert.ToInt32(aux.FirstOrDefault().Value) + " ORDER BY p.Project ASC").ToList();

            //using (CDPTrackEntities context = new CDPTrackEntities())
            // {


            //ViewBag.projects = project;

            /* var sr = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT s.SurveyId, s.Quarter, q.Text,  sr.ResponseId, sr.ResourceId FROM Survey s
                                                     INNER JOIN Question q
                                                     on q.SurveyId =  s.SurveyId
                                                     INNER JOIN SurveyResponse sr 
                                                     on sr.QuestionId = q.QuestionId
                                                     WHERE q.QuestionType =1 AND SurveyType = 1 AND sr.ResourceId = 627 
                                                     ").ToList();

             ViewBag.resourceId = 627;
             if (sr.Count > 0)
             {
                 ViewBag.sr = sr;
             }*/

            return View(projectcombo);


        }

        public ActionResult TeamMembersInputByProjectAverage(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            //retrieving all the available projects from the DB, the next step is to filter based on the USER requesting this report
            var projectcombo = context.Database.SqlQuery<Project>(@"SELECT DISTINCT p.ProjectId,p.Project as Project1
                                                                    FROM Project p
                                                                    ORDER BY p.Project ASC").ToList();
            return View(projectcombo);
        }

        public ActionResult TeamMembersInputBySite(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            //retrieving all the available sites from the DB, the next step is to filter based on the USER requesting this report
            var sitecombo = from cv in context.Locations orderby cv.Name ascending select cv;

            return View(sitecombo);
        }

        public ActionResult TeamMembersInputByAllSites(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            //just provide values for the last four quarters ids, those values will be used as entry to identify the  
            Tuple<int, int>[] quarterValues = quarterCalculation(4);

            return View(quarterValues);
        }

        public ActionResult DashboardReport(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            // setting initial Viewbag data for the view
            InitialSettingForView(quarter, year, resource);

            Tuple<int, int>[] quarterValues = quarterCalculation(4);
            List<Location> sitecombo;

            // identify if we are a VIP(all sites) or a regular user(only designated site)
            // here we need to apply the logic to identify the level of access
            bool VIP = true;

            if (VIP)
            {
                sitecombo = context.Locations.Where(x => x != null).ToList();   // return all locations here since is a VIP user
                ViewBag.HideLocations = false;
            }
            else
            {
                sitecombo = context.Locations.Where(x => x.ID == 4).ToList();  //supposing the NO VIP user is the director of GDL site
                ViewBag.HideLocations = true;
            }

            
            

            List<object> model = new List<object>();
            model.Add(quarterValues);
            model.Add(sitecombo);


            return View(model);
        }

        [HttpPost]
        public JsonResult GetSurveyCompletePercentage(string data)
        {
            var m = JsonConvert.DeserializeObject<QuarterSiteAndEmployeeTypeModel>(data);
            List<IEnumerable> completePercentages = new List<IEnumerable>();

            // get the 3 letter code of the location or default
            SqlParameter locationParam, locationParam2;
            if (m.siteAndQuarter.selectedSite.LocationId == -1)
            {
                locationParam = new SqlParameter("Location", System.Data.SqlTypes.SqlBoolean.Null);
                locationParam2 = new SqlParameter("Location", System.Data.SqlTypes.SqlBoolean.Null);
            }
            else
            {
                Location location = context.Locations.Where(x => x.ID == m.siteAndQuarter.selectedSite.LocationId).FirstOrDefault();
                locationParam = new SqlParameter("Location", location.abbreviation);
                locationParam2 = new SqlParameter("Location", location.abbreviation);
            }

            // get the employee type parameter
            SqlParameter employeeTypeParam, employeeTypeParam2;
            if (m.employeeType.Equals("All"))
            {
                employeeTypeParam = new SqlParameter("EmployeeType", System.Data.SqlTypes.SqlBoolean.Null);
                employeeTypeParam2 = new SqlParameter("EmployeeType", System.Data.SqlTypes.SqlBoolean.Null);
            }
            else
            {
                employeeTypeParam = new SqlParameter("EmployeeType", m.employeeType);
                employeeTypeParam2 = new SqlParameter("EmployeeType", m.employeeType);
            }

            if (m != null)
            {
                //calculate the percentage of completion for both kind of surveys
                IEnumerable TeamMembersInputData = context.Database.SqlQuery<SurveyCompletedPercentageModel>(@"SELECT * FROM [dbo].[udf_CalculateCompletionPercentage] (@Quarter, @Year, @SurveyType, @Location, @EmployeeType)",
                                                new SqlParameter("Quarter", m.siteAndQuarter.selectedQuarter.quarterOfTheYear),
                                                new SqlParameter("Year", m.siteAndQuarter.selectedQuarter.selectedYear),
                                                new SqlParameter("SurveyType", 1),
                                                locationParam,
                                                employeeTypeParam);

                IEnumerable ManagersCheckData = context.Database.SqlQuery<SurveyCompletedPercentageModel>(@"SELECT * FROM [dbo].[udf_CalculateCompletionPercentage] (@Quarter, @Year, @SurveyType, @Location, @EmployeeType)",
                                                new SqlParameter("Quarter", m.siteAndQuarter.selectedQuarter.quarterOfTheYear),
                                                new SqlParameter("Year", m.siteAndQuarter.selectedQuarter.selectedYear),
                                                new SqlParameter("SurveyType", 2),
                                                locationParam2,
                                                employeeTypeParam2);

                completePercentages.Add(TeamMembersInputData);
                completePercentages.Add(ManagersCheckData);

            }
            return Json(completePercentages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetManagersCheckSurveyDataBasedOnQuestionSequence(string data)
        {
            var m = JsonConvert.DeserializeObject<SiteQuarterAndSequenceQuestionModel>(data);
            List<ManagersCheckSurveyPercentagesModel> questionPercentages = new List<ManagersCheckSurveyPercentagesModel>();

            // get the 3 letter code of the location or default
            SqlParameter locationParam, locationParam2;
            if (m.LocationQuarterAndType.siteAndQuarter.selectedSite.LocationId == -1)
            {
                locationParam = new SqlParameter("Location", System.Data.SqlTypes.SqlBoolean.Null);
                locationParam2 = new SqlParameter("Location", System.Data.SqlTypes.SqlBoolean.Null);
            }
            else
            {
                Location location = context.Locations.Where(x => x.ID == m.LocationQuarterAndType.siteAndQuarter.selectedSite.LocationId).FirstOrDefault();
                locationParam = new SqlParameter("Location", location.abbreviation);
                locationParam2 = new SqlParameter("Location", location.abbreviation);
            }

            // get the employee type parameter
            SqlParameter employeeTypeParam, employeeTypeParam2;
            if (m.LocationQuarterAndType.employeeType.Equals("All"))
            {
                employeeTypeParam = new SqlParameter("EmployeeType", System.Data.SqlTypes.SqlBoolean.Null);
                employeeTypeParam2 = new SqlParameter("EmployeeType", System.Data.SqlTypes.SqlBoolean.Null);
            }
            else
            {
                employeeTypeParam = new SqlParameter("EmployeeType", m.LocationQuarterAndType.employeeType);
                employeeTypeParam2 = new SqlParameter("EmployeeType", m.LocationQuarterAndType.employeeType);
            }


            if (m != null)
            {
                if (m.QuestionSequence == 6) // when the sequence is 6 there is a special SQL function used to retrieve the proper data
                {
                    questionPercentages = context.Database.SqlQuery<ManagersCheckSurveyPercentagesModel>(@"SELECT * FROM [dbo].[udf_Calculate_Promotion_Percentage] (@Quarter, @Year, @Location, @EmployeeType)",
                                                    new SqlParameter("Quarter", m.LocationQuarterAndType.siteAndQuarter.selectedQuarter.quarterOfTheYear),
                                                    new SqlParameter("Year", m.LocationQuarterAndType.siteAndQuarter.selectedQuarter.selectedYear),
                                                    locationParam,
                                                    employeeTypeParam).ToList();
                }
                else
                {
                    questionPercentages = context.Database.SqlQuery<ManagersCheckSurveyPercentagesModel>(@"SELECT * FROM [dbo].[udf_Calculate_MangCheck_Comendation] (@Quarter, @Year, @Sequence, @Location, @EmployeeType)",
                                                    new SqlParameter("Quarter", m.LocationQuarterAndType.siteAndQuarter.selectedQuarter.quarterOfTheYear),
                                                    new SqlParameter("Year", m.LocationQuarterAndType.siteAndQuarter.selectedQuarter.selectedYear),
                                                    new SqlParameter("Sequence", m.QuestionSequence),
                                                    locationParam,
                                                    employeeTypeParam).ToList();
                }
            }
            return Json(questionPercentages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMeWeQuestionsBasedOnSequence(string data)
        {
            var m = JsonConvert.DeserializeObject<SiteQuarterAndSequenceQuestionModel>(data);
            List<TeamMembersInputSurveyPercentagesModel> questionPercentages = new List<TeamMembersInputSurveyPercentagesModel>();

            // get the 3 letter code of the location or default
            SqlParameter locationParam, locationParam2;
            if (m.LocationQuarterAndType.siteAndQuarter.selectedSite.LocationId == -1)
            {
                locationParam = new SqlParameter("Location", System.Data.SqlTypes.SqlBoolean.Null);
                locationParam2 = new SqlParameter("Location", System.Data.SqlTypes.SqlBoolean.Null);
            }
            else
            {
                Location location = context.Locations.Where(x => x.ID == m.LocationQuarterAndType.siteAndQuarter.selectedSite.LocationId).FirstOrDefault();
                locationParam = new SqlParameter("Location", location.abbreviation);
                locationParam2 = new SqlParameter("Location", location.abbreviation);
            }

            // get the employee type parameter
            SqlParameter employeeTypeParam, employeeTypeParam2;
            if (m.LocationQuarterAndType.employeeType.Equals("All"))
            {
                employeeTypeParam = new SqlParameter("EmployeeType", System.Data.SqlTypes.SqlBoolean.Null);
                employeeTypeParam2 = new SqlParameter("EmployeeType", System.Data.SqlTypes.SqlBoolean.Null);
            }
            else
            {
                employeeTypeParam = new SqlParameter("EmployeeType", m.LocationQuarterAndType.employeeType);
                employeeTypeParam2 = new SqlParameter("EmployeeType", m.LocationQuarterAndType.employeeType);
            }

            if (m != null)
            {
                questionPercentages = context.Database.SqlQuery<TeamMembersInputSurveyPercentagesModel>(@"SELECT * FROM [dbo].[udf_Calculate_team_agreement] (@Quarter, @Year, @Sequence, @Location, @EmployeeType)",
                                                new SqlParameter("Quarter", m.LocationQuarterAndType.siteAndQuarter.selectedQuarter.quarterOfTheYear),
                                                new SqlParameter("Year", m.LocationQuarterAndType.siteAndQuarter.selectedQuarter.selectedYear),
                                                new SqlParameter("Sequence", m.QuestionSequence),
                                                locationParam,
                                                employeeTypeParam).ToList();
            }

            // need to turn the 'null' values into 0.00
            foreach (TeamMembersInputSurveyPercentagesModel u in questionPercentages.Where(u => u != null))
            {
                u.Agree = u.Agree.HasValue ? u.Agree.Value : 0;
                u.Disagree = u.Disagree.HasValue ? u.Disagree.Value : 0;
                u.Neutral = u.Neutral.HasValue ? u.Neutral.Value : 0;
                u.Strongly_Agree = u.Strongly_Agree.HasValue ? u.Strongly_Agree.Value : 0;
                u.Strongly_Disagree = u.Strongly_Disagree.HasValue ? u.Strongly_Disagree.Value : 0;
            }

            return Json(questionPercentages, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TeamMembersInputMaster(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            // setting initial Viewbag data for the view
            InitialSettingForView(quarter, year, resource);

            //we need to send the available sites data and the quarters back to the view so the user can properly filter the information
            var sitecombo = from cv in context.Locations orderby cv.Name ascending select cv;
            Tuple<int, int>[] quarterValues = quarterCalculation(4);

            List<object> model = new List<object>();
            model.Add(sitecombo);
            model.Add(quarterValues);

            return View(model);
        }


        public ActionResult TeamMembersInputByManagersPerSites(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            //we need to send the available sites data and the quarters back to the view so the user can properly filter the information
            var sitecombo = from cv in context.Locations orderby cv.Name ascending select cv;
            Tuple<int, int>[] quarterValues = quarterCalculation(4);

            List<object> model = new List<object>();
            model.Add(sitecombo);
            model.Add(quarterValues);

            return View(model);
        }

        public ActionResult TeamMembersInputByManagersAllSites(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            //we need to send the available sites data and the quarters back to the view so the user can properly filter the information
            Tuple<int, int>[] quarterValues = quarterCalculation(4);

            return View(quarterValues);
        }

        public ActionResult TeamMembersInputByProjectsAllSites(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            //we need to send the available sites data and the quarters back to the view so the user can properly filter the information
            Tuple<int, int>[] quarterValues = quarterCalculation(4);

            return View(quarterValues);
        }

        public ActionResult TeamMembersInputByProjectVsAllProjects(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            //we need to send the available projects data and the quarters back to the view so the user can properly filter the information
            var projectcombo = context.Database.SqlQuery<Project>(@"SELECT DISTINCT p.ProjectId,p.Project as Project1
                                                                    FROM Project p
                                                                    ORDER BY p.Project ASC").ToList();
            Tuple<int, int>[] quarterValues = quarterCalculation(4);

            List<object> model = new List<object>();
            model.Add(projectcombo);
            model.Add(quarterValues);

            return View(model);
        }

        // Action Result for Team members intput  
        public ActionResult TeamMembersInputReportLEAA(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;
            ViewBag.ResourceId = resource.ResourceId;
            return View();

        }
        public ActionResult TeamMembersInputTalent(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            var sitecombo = from cv in context.Locations orderby cv.Name ascending select cv;

            return View(sitecombo);

        }

        public ActionResult EmployeeTrendsReports(int idUser)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);


            return RedirectToAction("EmployeeTrendsReportsLEAA", "GoalTracking");
        }

        public ActionResult TrendsReportsDirectorView()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            var sitecombo = from cv in context.Locations orderby cv.Name ascending select cv;


            return View(sitecombo);
        }

        public ActionResult EmployeeTrendsReportsLEAA(int? ID)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }


            SetupCommonViewBagValues();
            SetProfileData(resource);
            int? resourceId = ID;
            if (resourceId == null)
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var resourceselected = context.Resources.Find(resourceId);
                ViewBag.idUser = resourceselected.ResourceId;
                ViewBag.nameUser = resourceselected.Name;
                ViewBag.currentPosition = resourceselected.Employee.CurrentPosition;
                var aux = (from emp in context.Resources where emp.ActiveDirectoryId == resourceselected.Employee.ManagerId select emp.Name).ToList();
                ViewBag.managerID = aux[0];
            }




            return View();
        }

        // Action Result for Team members intput  
        public ActionResult TeamMembersInputManagerView(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;
            var aux = (from emp in context.Employee where emp.ResourceId == resource.ResourceId select emp.ManagerId).Take(1);

            //var projectcombo = from cv in context.Project orderby cv.Project1 ascending select cv;
            var projectcombo = context.Database.SqlQuery<Project>(@"SELECT DISTINCT p.ProjectId,p.Project as Project1
  FROM [CDPTrack].[dbo].[Employee] e, Project p
  WHERE p.ProjectId = e.ProjectId AND e.ManagerId = " + Convert.ToInt32(aux.FirstOrDefault().Value) + " ORDER BY p.Project ASC").ToList();


            return View(projectcombo);
        }

        public ActionResult TeamMembersInputDirectorView(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();
            ViewBag.Name = resource.Name;

            // need to retrieve all the managers that will be offered in the views filter
            List<EmployeeProject> managers = new List<EmployeeProject>();
            managers = context.Database.SqlQuery<EmployeeProject>(@"SELECT e.ProjectId, r.ResourceId, r.Name 
                                                                            FROM Resource r, Employee e 
                                                                            WHERE ActiveDirectoryId in 
                                                                                (select distinct managerid from Employee where ManagerId is not NULL) 
                                                                            AND e.ResourceId = r.ResourceId").OrderBy(o => o.Name).ToList();


            return View(managers);
        }


        [HttpPost]
        public JsonResult GetDataResponsesTeamMemberInput(string data)
        {
            var m = JsonConvert.DeserializeObject<SiteLocation>(data);
            var site = m.LocationId;
            List<TMIResponses> sr = new List<TMIResponses>();
            if (m != null)
            {
                sr = context.Database.SqlQuery<TMIResponses>(@"SELECT q.Text, [dbo].[udf_TeamMembers_Question_NResponses] (s.Quarter,s.year," + site + ",q.QuestionId,5) as  Strongly_Agree, [dbo].[udf_TeamMembers_Question_NResponses] (s.Quarter,s.year," + site + ",q.QuestionId,4) as  Agree, [dbo].[udf_TeamMembers_Question_NResponses] (s.Quarter,s.year," + site + ",q.QuestionId,3) as  Neutral, [dbo].[udf_TeamMembers_Question_NResponses] (s.Quarter,s.year," + site + ",q.QuestionId,2) as  Disagree, [dbo].[udf_TeamMembers_Question_NResponses] (s.Quarter,s.year," + site + ",q.QuestionId,1) as  Strongly_Disagree FROM survey s , Question q WHERE q.SurveyId = s.SurveyId AND s.SurveyType=1 AND s.Quarter = 4 AND s.year= 2016").ToList();
            }

            return Json(sr, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetDataResponsesTeamMemberInputBySiteAverage(string data)
        {
            var m = JsonConvert.DeserializeObject<SiteLocation>(data);
            var site = m.LocationId;
            List<SurveyResponseReport> sr = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr1 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr2 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr3 = new List<SurveyResponseReport>();
            if (m != null)
            {
                Tuple<int, int>[] quarterValues = quarterCalculation(4);
                sr = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                       FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Resource resourceData
                                                       Where resourceData.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[0].Item1 + " AND s.Year = " + quarterValues[0].Item2 + " AND " + (site == 0 ? "resourceData.LocationId > 0 " : "resourceData.LocationId = " + site) + @"
                                                       GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                       ORDER By SurveyId").ToList();

                sr1 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                       FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Resource resourceData
                                                       Where resourceData.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[1].Item1 + " AND s.Year = " + quarterValues[1].Item2 + " AND " + (site == 0 ? "resourceData.LocationId > 0 " : "resourceData.LocationId = " + site) + @"
                                                       GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                       ORDER By SurveyId").ToList();

                sr2 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                       FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Resource resourceData
                                                       Where resourceData.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[2].Item1 + " AND s.Year = " + quarterValues[2].Item2 + " AND " + (site == 0 ? "resourceData.LocationId > 0 " : "resourceData.LocationId = " + site) + @"
                                                       GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                       ORDER By SurveyId").ToList();

                sr3 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                       FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Resource resourceData
                                                       Where resourceData.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[3].Item1 + " AND s.Year = " + quarterValues[3].Item2 + " AND " + (site == 0 ? "resourceData.LocationId > 0 " : "resourceData.LocationId = " + site) + @"
                                                       GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                       ORDER By SurveyId").ToList();
            }

            MultipleSurveyData msd = new MultipleSurveyData();
            if (sr.Count == 0)
                msd.Q1 = null;
            else
                msd.Q1 = sr;

            if (sr1.Count == 0)
                msd.Q2 = null;
            else
                msd.Q2 = sr1;

            if (sr2.Count == 0)
                msd.Q3 = null;
            else
                msd.Q3 = sr2;

            if (sr3.Count == 0)
                msd.Q4 = null;
            else
                msd.Q4 = sr3;

            return Json(msd, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataResponsesTeamMemberInputBySiteAverageForSpecificQuarter(string data)
        {
            var m = JsonConvert.DeserializeObject<SelectedQuarter>(data);
            List<SurveyResponseAndIdentifierModel> responsesBySite = new List<SurveyResponseAndIdentifierModel>();

            if (m != null)
            {
                // retrieving the existing sites from DB
                var sitecombo = from cv in context.Locations orderby cv.Name ascending select cv;
                IEnumerable<Location> sites = sitecombo.AsEnumerable().Cast<Location>().ToList();

                // foreach site found retrieve the survey response average
                foreach (Location site in sites)
                {
                    SurveyResponseAndIdentifierModel response = new SurveyResponseAndIdentifierModel();
                    response.Identifier = site.Name;
                    response.SurveyResponse = new List<SurveyResponseReport>();

                    response.SurveyResponse = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                                           FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Resource resourceData
                                                                           Where resourceData.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + m.quarterOfTheYear + " AND s.Year = " + m.selectedYear + " AND resourceData.LocationId = " + site.ID + @"
                                                                           GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                                           ORDER By SurveyId").ToList();
                    responsesBySite.Add(response);
                }

            }

            return Json(responsesBySite, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataResponsesTeamMemberInputBySiteAndManagerAverageForSpecificQuarter(string data)
        {
            var m = JsonConvert.DeserializeObject<SiteAndQuarter>(data);
            List<SurveyResponseAndManagerModel> responsesBySitePerManager = new List<SurveyResponseAndManagerModel>();

            if (m != null)
            {
                // need to retrieve all the managers from selected location
                List<EmployeeProject> managers = new List<EmployeeProject>();
                if (m != null)
                {
                    managers = context.Database.SqlQuery<EmployeeProject>(@"SELECT e.ProjectId, r.ResourceId, r.Name 
                                                                            FROM Resource r, Employee e 
                                                                            WHERE ActiveDirectoryId in 
                                                                                (select distinct managerid from Employee where ManagerId is not NULL) 
                                                                            AND e.ResourceId = r.ResourceId AND " + (m.selectedSite.LocationId == 0 ? "r.LocationId > 0 " : "r.LocationId = " + m.selectedSite.LocationId)).OrderBy(o => o.Name).ToList();
                }


                foreach (EmployeeProject manager in managers)
                {
                    SurveyResponseAndManagerModel response = new SurveyResponseAndManagerModel();
                    response.Manager = manager.Name;
                    response.SurveyResponse = new List<SurveyResponseReport>();

                    response.SurveyResponse = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                        FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                        Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId and  s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + m.selectedQuarter.quarterOfTheYear + " AND s.Year = " + m.selectedQuarter.selectedYear + " AND emp.ProjectId = " + manager.ProjectId + " Group by sr.QuestionId, q.Text ,s.Quarter, s.Year ORDER By SurveyId").ToList();
                    responsesBySitePerManager.Add(response);
                }

            }

            return Json(responsesBySitePerManager, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataResponsesTeamMemberInputHistorical(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProjectWithQuarter>(data);
            List<SurveyResponseAndIdentifierModel> responsesHistorical = new List<SurveyResponseAndIdentifierModel>();

            if (m != null)
            {
                Tuple<int, int>[] quarterValues = quarterCalculationFromSelected(4, m.Quarter, m.Year);

                foreach (Tuple<int, int> date in quarterValues)
                {
                    SurveyResponseAndIdentifierModel response = new SurveyResponseAndIdentifierModel();
                    response.Identifier = "Q" + date.Item1.ToString() + " " + date.Item2.ToString();
                    response.SurveyResponse = new List<SurveyResponseReport>();

                    response.SurveyResponse = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT s.SurveyId, 'Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter, q.Text,  sr.ResponseId, sr.ResourceId FROM Survey s
                                                    INNER JOIN Question q
                                                    on q.SurveyId =  s.SurveyId
                                                    INNER JOIN SurveyResponse sr 
                                                    on sr.QuestionId = q.QuestionId
                                                    WHERE s.Quarter = " + date.Item1 + " AND s.Year = " + date.Item2 + " AND q.QuestionType =1 AND SurveyType = 1 AND sr.ResourceId = " + m.ResourceId).ToList();
                    if (response.SurveyResponse.Count > 0)
                        responsesHistorical.Add(response);
                }

            }

            return Json(responsesHistorical, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataResponsesTeamMemberInputBenchmark(string data)
        {
            BenchmarkReportDataModel m = JsonConvert.DeserializeObject<BenchmarkReportDataModel>(data);
            List<SurveyResponseAndIdentifierModel> responsesBenchmark = new List<SurveyResponseAndIdentifierModel>();

            if (m != null)
            {
                // getting Individual responses
                SurveyResponseAndIdentifierModel individualResponse = new SurveyResponseAndIdentifierModel();
                individualResponse.Identifier = m.employeeProjectData.Name;
                individualResponse.SurveyResponse = new List<SurveyResponseReport>();

                individualResponse.SurveyResponse = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT s.SurveyId, 'Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter, q.Text,  sr.ResponseId, sr.ResourceId FROM Survey s
                                                INNER JOIN Question q
                                                on q.SurveyId =  s.SurveyId
                                                INNER JOIN SurveyResponse sr 
                                                on sr.QuestionId = q.QuestionId
                                                WHERE s.Quarter = " + m.quarter + " AND s.Year = " + m.year + " AND q.QuestionType =1 AND SurveyType = 1 AND sr.ResourceId = " + m.employeeProjectData.ResourceId).ToList();
                if (individualResponse.SurveyResponse.Count > 0)
                    responsesBenchmark.Add(individualResponse);

                // getting the benchmark group responses
                SurveyResponseAndIdentifierModel benchmarkResponse = new SurveyResponseAndIdentifierModel();
                benchmarkResponse.Identifier = "Benchmark Data";
                benchmarkResponse.SurveyResponse = new List<SurveyResponseReport>();

                StringBuilder QueryBenchmark = new StringBuilder();


                QueryBenchmark.Append(@"SELECT sr.QuestionId as SurveyId , 'Benchmark Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId");
                QueryBenchmark.Append(@" from SurveyResource ");
                QueryBenchmark.Append(@" sre inner join SurveyResponse sr ");
                QueryBenchmark.Append(@" on sre.SurveyResourceId = sr.SurveyResourceId ");
                QueryBenchmark.Append(@" inner join Question q on q.QuestionId = sr.QuestionId ");
                QueryBenchmark.Append(@" inner join Employee e on e.ResourceId = sr.ResourceId ");
                QueryBenchmark.Append(@" inner join Resource r on e.ResourceId = r.ResourceId ");
                QueryBenchmark.Append(@" inner join Survey S on s.SurveyId = sre.SurveyId ");
                QueryBenchmark.Append(@" WHERE s.SurveyType = 1 AND q.QuestionType = 1 ");
                QueryBenchmark.AppendFormat(@" AND s.Quarter = {0} AND  ", m.quarter); // Quarter filter 
                QueryBenchmark.AppendFormat(@" s.Year = {0} AND ", m.year);
                QueryBenchmark.AppendFormat(@" (e.ProjectId = {0} or {0} = -1 ) and ", m.ProjectId);
                QueryBenchmark.AppendFormat(@" (r.LocationId = {0} or {0} = -1) ", m.LocationId);
                QueryBenchmark.AppendFormat(@" and ( sre.ResourceId = {0} or {0} = -1) ", m.ResourceId);
                QueryBenchmark.Append(@" GROUP BY sr.QuestionId, q.Text, s.Quarter, s.Year ");
                QueryBenchmark.Append(@" ORDER BY SurveyId");
                                                                                                     

                benchmarkResponse.SurveyResponse = context.Database.SqlQuery<SurveyResponseReport>(QueryBenchmark.ToString()).ToList();
                if (benchmarkResponse.SurveyResponse.Count > 0)
                    responsesBenchmark.Add(benchmarkResponse);
            }

            return Json(responsesBenchmark, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataResponsesTeamMemberInputByAllProjectsForSpecificQuarter(string data)
        {
            var m = JsonConvert.DeserializeObject<SiteAndQuarter>(data);
            List<SurveyResponseAndProjectModel> responsesByAllProjects = new List<SurveyResponseAndProjectModel>();

            if (m != null)
            {
                //retrieving all the available projects from the DB, the next step is to filter based on the USER requesting this report
                List<Project> projects = new List<Project>();
                if (m != null)
                {
                    projects = context.Database.SqlQuery<Project>(@"SELECT DISTINCT p.ProjectId,p.Project as Project1
                                                                    FROM Project p
                                                                    ORDER BY p.Project ASC").ToList();
                }


                foreach (Project project in projects)
                {
                    SurveyResponseAndProjectModel response = new SurveyResponseAndProjectModel();
                    response.Project = project.Project1;
                    response.SurveyResponse = new List<SurveyResponseReport>();

                    response.SurveyResponse = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                        FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                        Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId and  s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + m.selectedQuarter.quarterOfTheYear + " AND s.Year = " + m.selectedQuarter.selectedYear + " AND emp.ProjectId = " + project.ProjectId + " Group by sr.QuestionId, q.Text ,s.Quarter, s.Year ORDER By SurveyId").ToList();
                    responsesByAllProjects.Add(response);
                }

            }

            return Json(responsesByAllProjects, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetDataResponsesTeamMemberInputOneProjectVsAllProjects(string data)
        {
            var m = JsonConvert.DeserializeObject<ProjectAndQuarter>(data);

            List<SurveyResponseProjectVsAllProjectsModel> responsesByProjectVsAllProjects = new List<SurveyResponseProjectVsAllProjectsModel>();

            if (m != null)
            {
                SurveyResponseProjectVsAllProjectsModel responseSelectedProject = new SurveyResponseProjectVsAllProjectsModel();
                responseSelectedProject.Project = m.selectedProject.Project1;
                responseSelectedProject.SurveyResponse = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , '" + m.selectedProject.Project1 + @" Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                                                                          FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                                                                          WHERE emp.ResourceId = sr.ResourceId 
                                                                                                            AND sr.SurveyResourceId = sre.SurveyResourceId 
                                                                                                            AND s.SurveyId = sre.SurveyId 
                                                                                                            AND q.QuestionId = sr.QuestionId 
                                                                                                            AND q.QuestionType=1 
                                                                                                            AND  s.SurveyType = 1 
                                                                                                            AND s.Quarter= " + m.selectedQuarter.quarterOfTheYear + @" 
                                                                                                            AND s.Year = " + m.selectedQuarter.selectedYear + @" 
                                                                                                            AND emp.ProjectId = " + m.selectedProject.ProjectId + @"
                                                                                                          GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                                                                          ORDER By SurveyId").ToList();


                SurveyResponseProjectVsAllProjectsModel responseAllProjects = new SurveyResponseProjectVsAllProjectsModel();
                responseAllProjects.Project = "All projects AVG";
                responseAllProjects.SurveyResponse = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All Projects Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                                                                      WHERE emp.ResourceId = sr.ResourceId 
                                                                                                        AND sr.SurveyResourceId = sre.SurveyResourceId 
                                                                                                        AND s.SurveyId = sre.SurveyId 
                                                                                                        AND q.QuestionId = sr.QuestionId 
                                                                                                        AND q.QuestionType=1 
                                                                                                        AND  s.SurveyType = 1 
                                                                                                        AND s.Quarter= " + m.selectedQuarter.quarterOfTheYear + @" 
                                                                                                        AND s.Year = " + m.selectedQuarter.selectedYear + @" 
                                                                                                        AND emp.ProjectId > 0 
                                                                                                      GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                                                                      ORDER By SurveyId").ToList();

                responsesByProjectVsAllProjects.Add(responseSelectedProject);
                responsesByProjectVsAllProjects.Add(responseAllProjects);


            }
            return Json(responsesByProjectVsAllProjects, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmployeeFromProject(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<EmployeeProject> sr = new List<EmployeeProject>();
            if (m != null)
            {
                sr = context.Database.SqlQuery<EmployeeProject>("SELECT r.ResourceId, r.Name FROM Employee e, Resource r, Project p WHERE p.ProjectId = e.ProjectId AND e.ResourceId = r.ResourceId AND p.ProjectId= " + m.ResourceId + "").ToList();
            }
            return Json(sr, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetEmployeeFromProjectAndSite(string data)
        {
            var m = JsonConvert.DeserializeObject<ProjectAndLocationModel>(data);
            List<EmployeeProject> sr = new List<EmployeeProject>();
            if (m != null)
            {
                sr = context.Database.SqlQuery<EmployeeProject>(@"SELECT r.ResourceId, r.Name 
                                                                  FROM Employee e, Resource r, Project p 
                                                                  WHERE p.ProjectId = e.ProjectId AND 
                                                                        e.ResourceId = r.ResourceId AND
                                                                        p.ProjectId= " + m.ProjectId + @" AND 
                                                                        (r.LocationId = " + m.LocationId + " OR " + m.LocationId + "= -1)").ToList();
            }
            return Json(sr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSiteFromProject(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<SiteLocation> sr = new List<SiteLocation>();
            if (m != null)
            {
                sr = context.Database.SqlQuery<SiteLocation>(@"SELECT DISTINCT l.ID as LocationId, l.Name as nameLocation
                                                                  FROM Employee e, Resource r, Project p, Location l  
                                                                  WHERE p.ProjectId = e.ProjectId AND 
	                                                              e.ResourceId = r.ResourceId AND 
	                                                              r.LocationId = l.ID AND
	                                                              p.ProjectId =" + m.ProjectId).ToList();
            }
            return Json(sr, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetManagerFromLocation(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<EmployeeProject> sr = new List<EmployeeProject>();
            if (m != null)
            {
                sr = context.Database.SqlQuery<EmployeeProject>("select ResourceId, Name from Resource where ActiveDirectoryId in (select distinct managerid from Employee where ManagerId is not NULL) and LocationId = " + m.ResourceId).ToList();
            }
            return Json(sr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetManagerComment(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<ManagerComment> sr = new List<ManagerComment>();

            if (m != null)
            {
                //last quarter answered
                sr = context.Database.SqlQuery<ManagerComment>(@"SELECT top 1 ResponseText, ResourceId, ResourceEvaluatedId FROM (
                                                                SELECT sres.DateAnswered, sr.ResponseText, sres.ResourceId, sres.ResourceEvaluatedId 
                                                                FROM Question q
                                                                INNER JOIN Survey s ON q.SurveyId = s.SurveyId 
                                                                INNER JOIN  SurveyResponse sr ON sr.QuestionId = q.QuestionId 
                                                                INNER JOIN SurveyResource sres ON sres.SurveyResourceId = sr.SurveyResourceId AND sres.ResourceEvaluatedId = " + m.ResourceId + " WHERE Q.[Sequence] = 10 ) as A ORDER BY A.DateAnswered DESC").ToList();
            }

            if (sr.Count == 0)
            {
                sr.Add(new ManagerComment { ResourceEvaluatedId = m.ResourceId, ResourceId = 0, ResponseText = "" });
            }

            return Json(sr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetManagerCommentIndv(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProjectWithQuarter>(data);
            List<ManagerComment> sr = new List<ManagerComment>();

            if (m != null)
            {
                //last quarter answered
                sr = context.Database.SqlQuery<ManagerComment>(@"SELECT top 1 ResponseText, ResourceId, ResourceEvaluatedId FROM (
                                                                SELECT sres.DateAnswered, sr.ResponseText, sres.ResourceId, sres.ResourceEvaluatedId 
                                                                FROM Question q
                                                                INNER JOIN Survey s ON q.SurveyId = s.SurveyId AND s.Quarter = " + m.Quarter + " and s.Year = " + m.Year + "    INNER JOIN  SurveyResponse sr ON sr.QuestionId = q.QuestionId  INNER JOIN SurveyResource sres ON sres.SurveyResourceId = sr.SurveyResourceId AND sres.ResourceEvaluatedId = " + m.ResourceId + " WHERE Q.[Sequence] = 10 ) as A ORDER BY A.DateAnswered DESC").ToList();
            }

            if (sr.Count == 0)
            {
                sr.Add(new ManagerComment { ResourceEvaluatedId = m.ResourceId, ResourceId = 0, ResponseText = "" });
            }

            return Json(sr, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetEmployeeComment(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProjectWithQuarter>(data);
            List<EmployeeComment> sr = new List<EmployeeComment>();

            if (m != null)
            {
                //last quarter answered
                sr = context.Database.SqlQuery<EmployeeComment>(@"SELECT top 1 ResponseText, ResourceId FROM (
                                                                SELECT sres.DateAnswered, sr.ResponseText, sres.ResourceId, sres.ResourceEvaluatedId 
                                                                FROM Question q
                                                                INNER JOIN Survey s ON q.SurveyId = s.SurveyId AND s.Quarter = " + m.Quarter + " and s.Year = " + m.Year +"    INNER JOIN  SurveyResponse sr ON sr.QuestionId = q.QuestionId  INNER JOIN SurveyResource sres ON sres.SurveyResourceId = sr.SurveyResourceId AND sres.ResourceId = " + m.ResourceId + " WHERE Q.[Sequence] = 10 ) as A ORDER BY A.DateAnswered DESC").ToList();
            }

            if (sr.Count == 0)
            {
                sr.Add(new EmployeeComment { ResourceId = 0, ResponseText = "" });
            }

            return Json(sr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEmployeeLastComment(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProjectWithQuarter>(data);
            List<EmployeeComment> sr = new List<EmployeeComment>();

            if (m != null)
            {
                //last quarter answered
                sr = context.Database.SqlQuery<EmployeeComment>(@"SELECT top 1 ResponseText, ResourceId FROM (
                                                                SELECT sres.DateAnswered, sr.ResponseText, sres.ResourceId, sres.ResourceEvaluatedId 
                                                                FROM Question q
                                                                INNER JOIN Survey s ON q.SurveyId = s.SurveyId 
                                                                INNER JOIN  SurveyResponse sr ON sr.QuestionId = q.QuestionId  
                                                                INNER JOIN SurveyResource sres ON sres.SurveyResourceId = sr.SurveyResourceId 
                                                                AND sres.ResourceId = " + m.ResourceId +
                                                                " WHERE Q.[Sequence] = 10 ) as A ORDER BY A.DateAnswered DESC").ToList();
            }

            if (sr.Count == 0)
            {
                sr.Add(new EmployeeComment { ResourceId = 0, ResponseText = "" });
            }

            return Json(sr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDataManagerCheck(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<ManagerCheck> sr = new List<ManagerCheck>();
            List<ManagerCheck> sr1 = new List<ManagerCheck>();
            List<ManagerCheck> sr2 = new List<ManagerCheck>();
            List<ManagerCheck> sr3 = new List<ManagerCheck>();

            if (m != null)
            {
                Tuple<int, int>[] quarterValues = quarterCalculation(4);

                 

                sr = context.Database.SqlQuery<ManagerCheck>(@"SELECT 'Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter, q.Text , sr.ResourceId, ISNULL(sr.ResponseId, 0 ) as ResponseId,  sr.ResponseText, sres.ResourceEvaluatedId FROM Question q, Survey s, SurveyResponse sr , SurveyResource sres
                    WHERE sres.SurveyResourceId = sr.SurveyResourceId and sr.QuestionId = q.QuestionId and q.SurveyId = s.SurveyId 
                    and s.Quarter = " + quarterValues[0].Item1 + " and s.Year = " + quarterValues[0].Item2 + " and s.SurveyType = 2 and sres.ResourceEvaluatedId = " + m.ResourceId).ToList();

                sr1 = context.Database.SqlQuery<ManagerCheck>(@"SELECT 'Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter, q.Text , sr.ResourceId, ISNULL(sr.ResponseId, 0 ) as ResponseId,  sr.ResponseText, sres.ResourceEvaluatedId FROM Question q, Survey s, SurveyResponse sr , SurveyResource sres
                    WHERE sres.SurveyResourceId = sr.SurveyResourceId and sr.QuestionId = q.QuestionId and q.SurveyId = s.SurveyId 
                    and s.Quarter = " + quarterValues[1].Item1 + " and s.Year = " + quarterValues[1].Item2 + " and s.SurveyType = 2 and sres.ResourceEvaluatedId = " + m.ResourceId).ToList();

                sr2 = context.Database.SqlQuery<ManagerCheck>(@"SELECT 'Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter, q.Text , sr.ResourceId, ISNULL(sr.ResponseId, 0 ) as ResponseId,  sr.ResponseText, sres.ResourceEvaluatedId FROM Question q, Survey s, SurveyResponse sr , SurveyResource sres
                    WHERE sres.SurveyResourceId = sr.SurveyResourceId and sr.QuestionId = q.QuestionId and q.SurveyId = s.SurveyId 
                    and s.Quarter = " + quarterValues[2].Item1 + " and s.Year = " + quarterValues[2].Item2 + " and s.SurveyType = 2 and sres.ResourceEvaluatedId = " + m.ResourceId).ToList();

                sr3 = context.Database.SqlQuery<ManagerCheck>(@"SELECT 'Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter, q.Text , sr.ResourceId, ISNULL(sr.ResponseId, 0 ) as ResponseId,  sr.ResponseText, sres.ResourceEvaluatedId FROM Question q, Survey s, SurveyResponse sr , SurveyResource sres
                    WHERE sres.SurveyResourceId = sr.SurveyResourceId and sr.QuestionId = q.QuestionId and q.SurveyId = s.SurveyId 
                    and s.Quarter = " + quarterValues[3].Item1 + " and s.Year = " + quarterValues[3].Item2 + " and s.SurveyType = 2 and sres.ResourceEvaluatedId = " + m.ResourceId).ToList();
            }

            MultipleManagerCheckData msd = new MultipleManagerCheckData();
            if (sr.Count == 0)
                msd.Q1 = null;
            else
                msd.Q1 = sr;

            if (sr1.Count == 0)
                msd.Q2 = null;
            else
                msd.Q2 = sr1;

            if (sr2.Count == 0)
                msd.Q3 = null;
            else
                msd.Q3 = sr2;

            if (sr3.Count == 0)
                msd.Q4 = null;
            else
                msd.Q4 = sr3;




            return Json(msd, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetDataSurveysByProjectOnlyAverage(string data)
        {
            var m = JsonConvert.DeserializeObject<Projects>(data);
            List<SurveyResponseReport> sr = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr1 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr2 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr3 = new List<SurveyResponseReport>();

            if (m != null)
            {
                Tuple<int, int>[] quarterValues = quarterCalculation(4);

                sr = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[0].Item1 + " AND s.Year = " + quarterValues[0].Item2 + " AND emp.ProjectId = " + m.ProjectId + @" 
                                                      GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                      ORDER By SurveyId").ToList();

                sr1 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[1].Item1 + " AND s.Year = " + quarterValues[1].Item2 + " AND emp.ProjectId = " + m.ProjectId + @" 
                                                      GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                      ORDER By SurveyId").ToList();

                sr2 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[2].Item1 + " AND s.Year = " + quarterValues[2].Item2 + " AND emp.ProjectId = " + m.ProjectId + @" 
                                                      GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                      ORDER By SurveyId").ToList();

                sr3 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId AND s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[3].Item1 + " AND s.Year = " + quarterValues[3].Item2 + " AND emp.ProjectId = " + m.ProjectId + @" 
                                                      GROUP BY sr.QuestionId, q.Text ,s.Quarter, s.Year 
                                                      ORDER By SurveyId").ToList();
            }
            MultipleSurveyData msd = new MultipleSurveyData();
            if (sr.Count == 0)
                msd.Q1 = null;
            else
                msd.Q1 = sr;

            if (sr1.Count == 0)
                msd.Q2 = null;
            else
                msd.Q2 = sr1;

            if (sr2.Count == 0)
                msd.Q3 = null;
            else
                msd.Q3 = sr2;

            if (sr3.Count == 0)
                msd.Q4 = null;
            else
                msd.Q4 = sr3;

            return Json(msd, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetDataSurveysByProjectAverage(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<SurveyResponseReport> sr = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr1 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr2 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr3 = new List<SurveyResponseReport>();

            if (m != null)
            {
                int actualQuarter = GetActualQuarter();
                int currentYear = GetCurrentYear();

                sr = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT s.SurveyId, 'Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter, q.Text,  sr.ResponseId, sr.ResourceId FROM Survey s
                                                    INNER JOIN Question q
                                                    on q.SurveyId =  s.SurveyId
                                                    INNER JOIN SurveyResponse sr 
                                                    on sr.QuestionId = q.QuestionId
                                                    WHERE q.QuestionType =1 AND SurveyType = 1 AND sr.ResourceId = " + m.ResourceId).ToList();


                //Change next 3 querys with respective quarter and year
                sr1 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId and  s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= 4 aND s.Year = 2016 AND emp.ProjectId = " + m.ProjectId + " Group by sr.QuestionId, q.Text ,s.Quarter, s.Year ORDER By SurveyId").ToList();


            }
            MultipleSurveyData msd = new MultipleSurveyData();
            if (sr.Count == 0)
                msd.Q1 = null;
            else
                msd.Q1 = sr;

            if (sr1.Count == 0)
                msd.Q2 = null;
            else
                msd.Q2 = sr1;

            if (sr2.Count == 0)
                msd.Q3 = null;
            else
                msd.Q3 = sr2;

            if (sr3.Count == 0)
                msd.Q4 = null;
            else
                msd.Q4 = sr3;

            return Json(msd, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        public JsonResult GetDataSurveysByProject(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<SurveyResponseReport> sr = new List<SurveyResponseReport>();
            if (m != null)
            {
                sr = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT s.SurveyId, s.Quarter, q.Text,  sr.ResponseId, sr.ResourceId FROM Survey s
                                                    INNER JOIN Question q
                                                    on q.SurveyId =  s.SurveyId
                                                    INNER JOIN SurveyResponse sr 
                                                    on sr.QuestionId = q.QuestionId
                                                    WHERE q.QuestionType =1 AND SurveyType = 1 AND sr.ResourceId = " + m.ResourceId).ToList();

            }

            return Json(sr, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetProjectFromManager(string data)
        {


            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<Projects> sr = new List<Projects>();
            if (m != null)
            {
                sr = context.Database.SqlQuery<Projects>(@"SELECT p.ProjectId, p.Project
                FROM Employee e, Project p where p.ProjectId = e.ProjectId AND e.ResourceId = " + m.ResourceId).ToList();

            }

            return Json(sr, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public JsonResult GetDataSurveysAllMembers(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<SurveyResponseReport> sr = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr1 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr2 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr3 = new List<SurveyResponseReport>();

            if (m != null)
            {
                Tuple<int, int>[] quarterValues = quarterCalculation(4);


                sr = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId and  s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[0].Item1 + " AND s.Year = " + quarterValues[0].Item2 + " AND emp.ProjectId = " + m.ProjectId +
                                                      " Group by sr.QuestionId, q.Text ,s.Quarter, s.Year ORDER By SurveyId").ToList();

                sr1 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId and  s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[1].Item1 + " AND s.Year = " + quarterValues[1].Item2 + " AND emp.ProjectId = " + m.ProjectId +
                                                      " Group by sr.QuestionId, q.Text ,s.Quarter, s.Year ORDER By SurveyId").ToList();

                sr2 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId and  s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[2].Item1 + " AND s.Year = " + quarterValues[2].Item2 + " AND emp.ProjectId = " + m.ProjectId +
                                                      " Group by sr.QuestionId, q.Text ,s.Quarter, s.Year ORDER By SurveyId").ToList();

                sr3 = context.Database.SqlQuery<SurveyResponseReport>(@"SELECT sr.QuestionId as SurveyId , 'All members Q' + STR(s.Quarter, 1) + STR(s.Year, 6) as Quarter , q.Text, AVG(sr.ResponseId) as ResponseId, 0 as ResourceId
                                                      FROM SurveyResource sre , SurveyResponse sr, Survey s, Question q, Employee emp
                                                      Where emp.ResourceId = sr.ResourceId AND sr.SurveyResourceId = sre.SurveyResourceId and  s.SurveyId = sre.SurveyId AND q.QuestionId = sr.QuestionId AND q.QuestionType=1 AND  s.SurveyType = 1 AND s.Quarter= " + quarterValues[3].Item1 + " AND s.Year = " + quarterValues[3].Item2 + " AND emp.ProjectId = " + m.ProjectId +
                                                      " Group by sr.QuestionId, q.Text ,s.Quarter, s.Year ORDER By SurveyId").ToList();


            }
            MultipleSurveyData msd = new MultipleSurveyData();
            if (sr.Count == 0)
                msd.Q1 = null;
            else
                msd.Q1 = sr;

            if (sr1.Count == 0)
                msd.Q2 = null;
            else
                msd.Q2 = sr1;

            if (sr2.Count == 0)
                msd.Q3 = null;
            else
                msd.Q3 = sr2;

            if (sr3.Count == 0)
                msd.Q4 = null;
            else
                msd.Q4 = sr3;


            return Json(msd, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public JsonResult GetDataSurveys(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProject>(data);
            List<SurveyResponseReport> sr = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr1 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr2 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr3 = new List<SurveyResponseReport>();

            if (m != null)
            {
                Tuple<int, int>[] quarterValues = quarterCalculation(4);

                var _query = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", quarterValues[0].Item1, quarterValues[0].Item2, m.ResourceId);

                sr = context.Database.SqlQuery<SurveyResponseReport>(_query).ToList();

                var _query1 = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", quarterValues[1].Item1, quarterValues[1].Item2, m.ResourceId);

                sr1 = context.Database.SqlQuery<SurveyResponseReport>(_query1).ToList();

                var _query2 = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", quarterValues[2].Item1, quarterValues[2].Item2, m.ResourceId);

                sr2 = context.Database.SqlQuery<SurveyResponseReport>(_query2).ToList();


                var _query3 = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", quarterValues[3].Item1, quarterValues[3].Item2, m.ResourceId);

                sr3 = context.Database.SqlQuery<SurveyResponseReport>(_query3).ToList();


                


            }
            MultipleSurveyData msd = new MultipleSurveyData();
            if (sr.Count == 0)
                msd.Q1 = null;
            else
                msd.Q1 = sr;

            if (sr1.Count == 0)
                msd.Q2 = null;
            else
                msd.Q2 = sr1;

            if (sr2.Count == 0)
                msd.Q3 = null;
            else
                msd.Q3 = sr2;

            if (sr3.Count == 0)
                msd.Q4 = null;
            else
                msd.Q4 = sr3;


            return Json(msd, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetDataSurveysFromSelected(string data)
        {
            var m = JsonConvert.DeserializeObject<EmployeeProjectWithQuarter>(data);
            List<SurveyResponseReport> sr = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr1 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr2 = new List<SurveyResponseReport>();
            List<SurveyResponseReport> sr3 = new List<SurveyResponseReport>();

            if (m != null)
            {
                Tuple<int, int>[] quarterValues = quarterCalculationFromSelected(4, m.Quarter, m.Year);

                var _query = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", quarterValues[0].Item1, quarterValues[0].Item2, m.ResourceId);

                sr = context.Database.SqlQuery<SurveyResponseReport>(_query).ToList();

                var _query1 = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", quarterValues[1].Item1, quarterValues[1].Item2, m.ResourceId);

                sr1 = context.Database.SqlQuery<SurveyResponseReport>(_query1).ToList();

                var _query2 = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", quarterValues[2].Item1, quarterValues[2].Item2, m.ResourceId);

                sr2 = context.Database.SqlQuery<SurveyResponseReport>(_query2).ToList();

                var _query3 = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", quarterValues[3].Item1, quarterValues[3].Item2, m.ResourceId);

                sr3 = context.Database.SqlQuery<SurveyResponseReport>(_query3).ToList();
            }
            MultipleSurveyData msd = new MultipleSurveyData();
            if (sr.Count == 0)
                msd.Q1 = null;
            else
                msd.Q1 = sr;

            if (sr1.Count == 0)
                msd.Q2 = null;
            else
                msd.Q2 = sr1;

            if (sr2.Count == 0)
                msd.Q3 = null;
            else
                msd.Q3 = sr2;

            if (sr3.Count == 0)
                msd.Q4 = null;
            else
                msd.Q4 = sr3;


            return Json(msd, JsonRequestBehavior.AllowGet);

        }

        public int lastQuarter(int actualQuarter, int index, int year)
        {
            int quarter = 1;

            return quarter;
        }

        public int lastYear(int actualQuarter, int index, int currentyear)
        {
            int year = 1;
            if (actualQuarter == 4)
                year = currentyear;

            return year;
        }

        // ActionResult for One Page Plan
        public ActionResult OnePagePlan(int? quarter, int? year)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ListOfYears();

            int? actualQuarter = quarter;
            int? currentYear = year;
            if (quarter == null || year == null)
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
            }

            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);
            ViewBag.changeQuarter = true;
            ViewBag.ListOfSources = GetSources();



            var coreValuesCombo = from cv in context.CoreValues select cv;
            try
            {
                var onePagePlanConsult = (from se in context.OnePagePlan where se.year == year && se.Quarter == quarter && se.ResourceId == resource.ResourceId select se).ToList();
                if (onePagePlanConsult.Count != 0)
                {
                    var onePagePlanID = (from se in context.OnePagePlan where se.year == year && se.Quarter == quarter && se.ResourceId == resource.ResourceId select se.OnePagePlanId).ToList();
                    int onePagePlanIDval = onePagePlanID.ElementAt(0);
                    ViewBag.onePagePlanId = onePagePlanIDval;
                    if (onePagePlanIDval != 0)
                    {
                        var keythrustVal = (from kt in context.KeyThrusts where kt.OnePagePlanId == onePagePlanIDval select kt).ToList();
                        var annualPrioritiesVal = (from ap in context.AnnualPriorities where ap.OnePagePlanId == onePagePlanIDval select ap).ToList();

                        ViewBag.keythrust = keythrustVal;
                        ViewBag.anualPriorities = annualPrioritiesVal;
                        ViewBag.onePagePlanValues = onePagePlanConsult;
                        ViewBag.coreValue1 = onePagePlanConsult[0].month1_CoreValueId.ToString();
                        ViewBag.coreValue2 = onePagePlanConsult[0].month2_CoreValueId.ToString();
                        ViewBag.coreValue3 = onePagePlanConsult[0].month3_CoreValueId.ToString();

                    }
                    else
                    {
                        ViewBag.onePagePlanId = 0;
                        ViewBag.keythrust = null;
                        ViewBag.anualPriorities = null;
                        ViewBag.onePagePlanValues = onePagePlanConsult;
                        ViewBag.coreValue1 = "1";
                        ViewBag.coreValue2 = "2";
                        ViewBag.coreValue3 = "3";
                    }
                }
                else
                {

                    ViewBag.onePagePlanId = 0;
                    ViewBag.keythrust = null;
                    ViewBag.anualPriorities = null;
                    ViewBag.onePagePlanValues = null;
                    ViewBag.coreValue1 = "1";
                    ViewBag.coreValue2 = "2";
                    ViewBag.coreValue3 = "3";

                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                ViewBag.onePagePlanId = 0;
                ViewBag.keythrust = null;
                ViewBag.anualPriorities = null;
                ViewBag.onePagePlanValues = null;
                ViewBag.coreValue1 = "1";
                ViewBag.coreValue2 = "2";
                ViewBag.coreValue3 = "3";

            }


            return View(coreValuesCombo);
        }

        [HttpPost]
        public ActionResult AddOnePagePlan(string data)
        {
            var m = JsonConvert.DeserializeObject<CDPTrackerSite.Models.OnePagePlan>(data);

            if (m != null)
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);

                int quarter = GetQuarter(DateTime.Now.Month);
                int year = DateTime.Now.Year;
                int idOnePage = 0;
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    int corevalue1 = int.Parse(m.core_values[0].core_value);
                    int corevalue2 = int.Parse(m.core_values[1].core_value);
                    int corevalue3 = int.Parse(m.core_values[2].core_value);
                    DataSource.OnePagePlan opp = new DataSource.OnePagePlan();
                    opp.ResourceId = resource.ResourceId;
                    opp.Quarter = quarter;
                    opp.year = year;
                    opp.month1_CoreValueId = corevalue1;
                    opp.month2_CoreValueId = corevalue2;
                    opp.month3_CoreValueId = corevalue3;
                    opp.SG = m.critical_numbers[0].critical_number;
                    opp.G = m.critical_numbers[1].critical_number;
                    opp.R = m.critical_numbers[2].critical_number;
                    context.OnePagePlan.Add(opp);
                    context.SaveChanges();
                    idOnePage = opp.OnePagePlanId;

                    foreach (var item in m.key_thrusts)
                    {
                        KeyThrusts kt = new KeyThrusts();
                        kt.OnePagePlanId = idOnePage;
                        kt.KeyThrust = item.key_thrust;
                        context.KeyThrusts.Add(kt);
                    }

                    foreach (var item in m.annual_priorities)
                    {
                        AnnualPriorities ap = new AnnualPriorities();
                        ap.OnePagePlanId = idOnePage;
                        ap.AnnualPriorities1 = item.annual_priority;
                        context.AnnualPriorities.Add(ap);
                    }
                    context.SaveChanges();
                }


            }



            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateOnePagePlan(string data)
        {
            var m = JsonConvert.DeserializeObject<CDPTrackerSite.Models.OnePagePlan>(data);

            if (m != null)
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);

                int quarter = GetQuarter(DateTime.Now.Month);
                int year = DateTime.Now.Year;

                DataSource.OnePagePlan opp = new DataSource.OnePagePlan();

                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    int corevalue1 = int.Parse(m.core_values[0].core_value);
                    int corevalue2 = int.Parse(m.core_values[1].core_value);
                    int corevalue3 = int.Parse(m.core_values[2].core_value);

                    // Edit OnePagePlan 
                    opp = context.OnePagePlan.Where(oppl => oppl.OnePagePlanId == m.onePagePlanID).FirstOrDefault<DataSource.OnePagePlan>();
                    if (opp != null)
                    {
                        opp.month1_CoreValueId = corevalue1;
                        opp.month2_CoreValueId = corevalue2;
                        opp.month3_CoreValueId = corevalue3;
                        opp.SG = m.critical_numbers[0].critical_number;
                        opp.G = m.critical_numbers[1].critical_number;
                        opp.R = m.critical_numbers[2].critical_number;

                        context.Entry(opp).State = System.Data.EntityState.Modified;
                        context.SaveChanges();
                    }

                    var ktdel = from ktd in context.KeyThrusts where ktd.OnePagePlanId == m.onePagePlanID select ktd;
                    foreach (var item in ktdel)
                    {
                        context.KeyThrusts.Remove(item);
                    }
                    if (ktdel.Count() > 0)
                        context.SaveChanges();

                    foreach (var item in m.key_thrusts)
                    {
                        KeyThrusts kt = new KeyThrusts();
                        kt.OnePagePlanId = m.onePagePlanID;
                        kt.KeyThrust = item.key_thrust;
                        context.KeyThrusts.Add(kt);

                    }
                    context.SaveChanges();


                    var annualPdel = from ktd in context.AnnualPriorities where ktd.OnePagePlanId == m.onePagePlanID select ktd;
                    foreach (var item in annualPdel)
                    {
                        context.AnnualPriorities.Remove(item);
                    }
                    if (annualPdel.Count() > 0)
                        context.SaveChanges();

                    foreach (var item in m.annual_priorities)
                    {
                        AnnualPriorities ap = new AnnualPriorities();
                        ap.OnePagePlanId = m.onePagePlanID;
                        ap.AnnualPriorities1 = item.annual_priority;
                        context.AnnualPriorities.Add(ap);
                    }
                    context.SaveChanges();
                }


            }


            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        //---------------------------------------------------------------- OLD ACTIONRESULTS -------------------------------------------------------------------

        public ActionResult PositionEdit()
        {
            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                return RedirectToAction("Index");
            }

            string currentPosition = GetCurrentPositionFromAD();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {


                Position positionObject = (from position in context.Position where currentPosition.Equals(position.PositionName) select position).FirstOrDefault();

                var optionsForDropDownList = (from positionHierarchy in context.PositionsHierarchy
                                              join position in context.Position
                                              on positionHierarchy.PositionId equals position.PositionId
                                              where positionHierarchy.PositionId == positionObject.PositionId
                                              select new { Text = positionHierarchy.Position1.PositionName, Value = positionHierarchy.Position1.PositionName }).ToList();

                SelectList nextPositionsAvailable = new SelectList(optionsForDropDownList, "Value", "Text");
                ViewBag.NextPositions = nextPositionsAvailable;
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            Employee employee = resource.Employee ?? new Employee { ResourceId = resource.ResourceId };

            employee.CurrentPosition = currentPosition;
            return View(employee);
        }

        [HttpPost]
        public ActionResult PositionEdit(Employee employee)
        {
            employee.AspiringPositionID = GetPositionID(employee.AspiringPosition);

            if (ModelState.IsValid)
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    if (context.Employee.Any(e => e.ResourceId == employee.ResourceId))
                    {
                        context.Entry(employee).State = EntityState.Modified;
                    }
                    else
                    {
                        context.Entry(employee).State = EntityState.Added;
                    }
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorLogHelper.LogException(ex, "CDPTracker");
                    }
                }
                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult BulkEdit(int ObjectiveId, int ResourceId)
        {
            ViewBag.ProgressList = new SelectList(GetProgresEnums(), "Id", "Label");
            ViewBag.ObjectiveList = new SelectList(GetObjective(ResourceId), "ObjectiveId", "Objective1");
            ViewBag.CategoriesList = new SelectList(GetCategories(), "TrainingCategoryId", "Name");
            ViewBag.SourcesList = new SelectList(GetSources(), "SourceId", "Name");

            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                return RedirectToAction("Index");
            }


            if (ObjectiveId != 0)
            {
                resource.GoalTrackings = resource.GoalTrackings.Where(goal => goal.ObjectiveId == ObjectiveId && goal.VerifiedByManager != true).
                                         OrderBy(goal => goal.FinishDate.HasValue ? goal.FinishDate.Value : DateTime.MinValue).ToList();
            }
            else
            {
                resource.GoalTrackings = resource.GoalTrackings.Where(goal => goal.ObjectiveId == null && goal.VerifiedByManager != true).
                                         OrderBy(goal => goal.FinishDate.HasValue ? goal.FinishDate.Value : DateTime.MinValue).ToList();
            }


            if (!resource.GoalTrackings.Any()) return RedirectToAction("Index");

            ViewBag.IsResourceAvailable = true;

            SetupCommonViewBagValues();
            return View(resource);
        }

        [HttpPost]
        public ActionResult BulkEdit(Resource resource)
        {
            bool isUpdated = false;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                string userName = resource.Name;
                //we only care for those Goals that have not yet been verified; all others cannot be edited
                List<GoalTracking> resourceGoalTrackings = context.GoalTrackings.Where(g => g.ResourceId == resource.ResourceId && !g.VerifiedByManager).ToList();
                //List to save all TDU's that are saved diferently from User Specified TDU
                List<GoalTracking> TDUChanged = new List<GoalTracking>();

                foreach (var goalTracking in resource.GoalTrackings.Where(g => !g.VerifiedByManager))
                {
                    GoalTracking original = resourceGoalTrackings.SingleOrDefault(g => g.GoalId == goalTracking.GoalId);
                    //Check if any fields have changed and only move on to update if they have
                    /*
                    if (original == null || (original.Goal == goalTracking.Goal
                                             && original.ObjectiveId == goalTracking.ObjectiveId
                                             && original.Progress == goalTracking.Progress
                                             && original.FinishDate == goalTracking.FinishDate
                                             && original.TDU == goalTracking.TDU
                                             && original.TrainingCategoryId == goalTracking.TrainingCategoryId))
                        continue;*/
                    var query = from goal in context.GoalTrackings
                                where goal.GoalId == goalTracking.GoalId && goal.ResourceId == goalTracking.ResourceId
                                select goal.TDU;
                    int cTDU = 0;
                    if (query != null)
                    {
                        cTDU = (int)query.Sum();
                    }
                    int TDUs = 0;
                    if (goalTracking.TDU != null)
                    {
                        TDUs = (int)goalTracking.TDU;
                    }
                    goalTracking.LastUpdate = DateTime.Now;
                    //Check if the category was changed and if so call the PointRestriction Method with the changedCategory parameter as True
                    if (goalTracking.TrainingCategoryId != original.TrainingCategoryId)
                    {
                        goalTracking.TDU = ptRestrictions(Convert.ToInt16(goalTracking.TrainingCategoryId), goalTracking.ResourceId, TDUs, cTDU, true);
                    }
                    else
                    {
                        goalTracking.TDU = ptRestrictions(Convert.ToInt16(goalTracking.TrainingCategoryId), goalTracking.ResourceId, TDUs, cTDU);
                    }

                    //If the goal tracking is a negative number then we have surpassed the number of points allowed and they 
                    //should be established to 0 for the modified goal.
                    if (goalTracking.TDU < 0 || goalTracking.TDU != TDUs)
                    {
                        if (goalTracking.TDU < 0) goalTracking.TDU = 0;
                        TDUChanged.Add(goalTracking);
                    }
                    context.Entry(original).CurrentValues.SetValues(goalTracking);
                    isUpdated = true;
                    context.SaveChanges();
                }

                if (isUpdated)
                {
                    //Pass a Javascript Alert with those goals who's TDU is diferent from user input so it can display them
                    ViewBag.TDUChanged = TDUChanged;
                    if (TDUChanged.Count != 0)
                    {
                        string script = "<script language='javascript' type='text/javascript'> alert('The maximum amount of TDUs for this category or quarter has been reached.";
                        string endScript = "'); location.href='Index'</script>";
                        foreach (var tdu in TDUChanged)
                        {
                            script += "\\nThe value of TDUs for the goal \"" + tdu.Goal + "\" will be " + tdu.TDU + ".";
                        }
                        script += endScript;
                        return Content(script);
                    }
                }
                else
                    return Content("<script language='javascript' type='text/javascript'>alert('The maximum amount of TDUs for one of the categories or quarter has been reached. The changes cannot be saved.'); location.href='Index'</script>");

                return RedirectToAction("Index");
            }
        }

        #endregion

        /***********************************************************  Region for Action Results of "Personal Evaluation" ********************/
        #region Personal Evaluation



        #region SelfAssessment
#if !DEBUG

#endif
        public ActionResult PrioritiesVisualization()
        {
            SetupCommonViewBagValues();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                Resource resource;
                if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
                {
                    TempData[ResourceController.IsRedirectBycode] = true;
                    return RedirectToAction("SetupResourceData", "Resource");
                }
                SetProfileData(resource);
                int ActualQuarter = GetActualQuarter();
                int ActualYear = DateTime.Now.Year;
                List<Objective> assessmentList = context.Objectives.Where(x => x.ResourceId == resource.ResourceId).ToList();

                // List of shortTerm assesments  (Short is current 0-6 months)
                List<Objective> shortTermList = null;
                if (ActualQuarter != 4)
                {
                    shortTermList = assessmentList.Where(x => (x.Quarter == ActualQuarter && x.Year == ActualYear) || (x.Quarter == ActualQuarter + 1 && x.Year == ActualYear)).ToList();
                }
                else
                {
                    // List of all assessments
                    shortTermList = assessmentList.Where(x => (x.Quarter == ActualQuarter && x.Year == ActualYear) || (x.Quarter == 1 && x.Year == (ActualYear + 1))).ToList();
                }

                // List of long assesments (Long term is 18 months and beyond
                List<Objective> longTermList = assessmentList.Where(x => x.Quarter >= ActualQuarter && x.Year == (ActualYear + 2)).ToList();

                //exclude Elements from long term and short term
                // List of middle assesments  (Middle term is 6-18 months)

                List<Objective> middleTermList = assessmentList.Except(longTermList).Except(shortTermList).ToList();
                List<Objective> submiddleTermList = middleTermList.Where(x => x.Quarter < ActualQuarter && x.Year == ActualYear).ToList();
                middleTermList = middleTermList.Except(submiddleTermList).ToList();

                var models = new Tuple<IEnumerable<Objective>, IEnumerable<Objective>, IEnumerable<Objective>>(shortTermList, middleTermList, longTermList);

                ViewBag.ResourceId = resource.ResourceId;

                return View(models);
            }
        }

#if !DEBUG

#endif
        //public ActionResult DeleteAssessment(int idAssessment)
        //{
        //    SetupCommonViewBagValues();

        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        if (isHisAssessment(idAssessment))
        //        {
        //            Assessment assessmentToDelete = context.Assessment.Include("TermProperty").Where(x => x.AssessmentId == idAssessment).FirstOrDefault();
        //            return View(assessmentToDelete);
        //        }

        //        return RedirectToAction("SelfAssessment");
        //    }
        //}

#if !DEBUG

#endif
        //[HttpPost]
        //public ActionResult DeleteAssessmentConfirmed(int AssessmentId)
        //{
        //    SetupCommonViewBagValues();

        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        Assessment assessment = context.Assessment.Find(AssessmentId);
        //        context.Assessment.Remove(assessment);
        //        context.SaveChanges();

        //        return RedirectToAction("SelfAssessment");
        //    }
        //}

#if !DEBUG

#endif
        //public ActionResult EditAssessment(int IdAssessment)
        //{
        //    SetupCommonViewBagValues();
        //    if (isHisAssessment(IdAssessment))
        //    {
        //        using (CDPTrackEntities context = new CDPTrackEntities())
        //        {
        //            Assessment assessment = context.Assessment.Include("TermProperty").FirstOrDefault(x => x.AssessmentId == IdAssessment);

        //            ViewBag.ListOfTerms = (from term in context.Term select new { Text = term.Name, Value = term.TermId }).ToList();
        //            ViewBag.ListOfAssessmentCategories = (from category in context.AssessmentCategory select new { Text = category.Name, Value = category.AssessmentCategoryId }).ToList();

        //            return View(assessment);
        //        }
        //    }

        //    return RedirectToAction("SelfAssessment");
        //}

#if !DEBUG

#endif
        //[HttpPost]
        //public ActionResult EditAssessmentConfirmed(Assessment assessment)
        //{
        //    SetupCommonViewBagValues();
        //    if (ModelState.IsValid)
        //    {
        //        using (CDPTrackEntities context = new CDPTrackEntities())
        //        {
        //            context.Entry(assessment).State = EntityState.Modified;
        //            context.SaveChanges();
        //        }
        //    }

        //    return RedirectToAction("SelfAssessment");
        //}

#if !DEBUG

#endif
        //public ActionResult CreateAssessment(int activeDirectoryID, int category, int term)
        //{
        //    SetupCommonViewBagValues();
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        Assessment newAssessment = new Assessment();
        //        ViewBag.ListOfTerms = (from termOption in context.Term select new { Text = termOption.Name, Value = termOption.TermId }).ToList();
        //        ViewBag.ListOfAssessmentCategories = (from categoryOption in context.AssessmentCategory select new { Text = categoryOption.Name, Value = categoryOption.AssessmentCategoryId }).ToList();

        //        newAssessment.AssessmentCategory = category;
        //        newAssessment.Term = term;
        //        return View(newAssessment);
        //    }
        //}


#if !DEBUG

#endif
        //[HttpPost]
        //public ActionResult CreateAssessmentConfirmed(Assessment assessment)
        //{
        //    SetupCommonViewBagValues();
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        Resource resource;
        //        if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
        //        {
        //            TempData[ResourceController.IsRedirectBycode] = true;
        //            return RedirectToAction("SetupResourceData", "Resource");
        //        }

        //        if (assessment.ResourceId == resource.ResourceId)
        //        {
        //            assessment.DateCreated = DateTime.Now;
        //            context.Assessment.Add(assessment);
        //            context.SaveChanges();
        //        }
        //        return RedirectToAction("SelfAssessment");
        //    }
        //}
        #endregion

        #region Training Program

        public ActionResult TrainingProgram()
        {
            SetupCommonViewBagValues();

            try
            {
                Resource resource;
                ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource);
                SetProfileData(resource);

                CDPTrackEntities context = new CDPTrackEntities();
                var lstTrainingByEmployee = from a in context.TrainingProgram
                                            join b in context.TrainingProgramDetails on a.IdTrainingProgram equals b.IdTrainingProgram
                                            join c in context.TrainingProgramCategory on a.Category equals c.IdTrainingProgramCategory
                                            join d in context.Resources on b.ResourceId equals d.ResourceId
                                            join e in context.ProgressEnums on b.Status equals e.Id
                                            where b.ResourceId == resource.ResourceId
                                            select new { TrainingProgram = a, TrainingProgramDetail = b, TrainingProgramCategory = c, Resource = d, Progress = e };

                var lstTrainingProgramDetails = (from element in lstTrainingByEmployee
                                                 select new
                                                 {
                                                     IdTrainingProgramDetails = element.TrainingProgramDetail.IdTrainingProgramDetails,
                                                     IdTrainingProgram = element.TrainingProgramDetail.IdTrainingProgram,
                                                     TrainingProgram = element.TrainingProgram,
                                                     Status = element.TrainingProgramDetail.Status,
                                                     Progress = element.Progress,
                                                     ResourceId = element.Resource.ResourceId
                                                 }).ToList().Select(x => new TrainingProgramDetails
                                                 {
                                                     IdTrainingProgramDetails = x.IdTrainingProgramDetails,
                                                     IdTrainingProgram = x.IdTrainingProgram,
                                                     TrainingProgram = x.TrainingProgram,
                                                     Status = x.Status,
                                                     ProgressEnum = x.Progress,
                                                     ResourceId = x.ResourceId
                                                 }).OrderBy(x => x.TrainingProgram.TrainingProgramCategory.Name).ToList();

                var listOfDeleteTrainingsIds = context.TrainingProgram.Where(x => x.Enable == false).Select(x => x.IdTrainingProgram).ToList();

                List<TrainingProgramDetails> tpd = new List<TrainingProgramDetails>();

                foreach (var item in lstTrainingProgramDetails)
                {
                    if (listOfDeleteTrainingsIds.Contains(item.IdTrainingProgram))
                    {
                        if (item.Status != 0)
                        {
                            tpd.Add(item);
                        }
                    }
                    else
                    {
                        tpd.Add(item);
                    }
                }

                /**************************************************************************************/

                var listOfDeleteProgramsIds = context.GeneralTrainingProgram.Where(x => x.Enabled == false).Select(x => x.IdGeneralTrainingProgram).ToList();
                var generalTrainingProgramList = context.GeneralTrainingProgramDetails
                                                        .Where(x => x.ResourceId == resource.ResourceId)
                                                        .Select(x => x)
                                                        .OrderBy(x => x.GeneralTrainingProgram.TrainingProgramCategory.Name).ToList();

                List<GeneralTrainingProgramDetails> finalListGeneralPrograms = new List<GeneralTrainingProgramDetails>();

                foreach (var item in generalTrainingProgramList)
                {
                    if (listOfDeleteProgramsIds.Contains(item.IdGeneralTrainingProgram))
                    {
                        if (item.Status != 0)
                        {
                            finalListGeneralPrograms.Add(item);
                        }
                    }
                    else
                    {
                        finalListGeneralPrograms.Add(item);
                    }
                }

                /**************************************************************************************/

                List<TrainingProgramOnDemandDetails> tpodd = context.TrainingProgramOnDemandDetails.Include("TrainingProgramOnDemand").Where(m => m.ResourceId == resource.ResourceId).ToList();

                /**************************************************************************************/

                var models = new Tuple<IEnumerable<TrainingProgramDetails>, IEnumerable<GeneralTrainingProgramDetails>, IEnumerable<TrainingProgramOnDemandDetails>>
                    (tpd, finalListGeneralPrograms, tpodd);

                return View(models);

            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                return View();
            }
        }

        public ActionResult EditTraining(int Id)
        {
            if (isHisTrainingProgram(Id))
            {
                TrainingProgramDetails tpd = new TrainingProgramDetails();

                try
                {
                    CDPTrackEntities context = new CDPTrackEntities();
                    tpd = context.TrainingProgramDetails.Find(Id);
                    ViewBag.ListOfProgress = new SelectList(GetProgresEnums(), "Id", "Label", tpd.ProgressEnum.Id);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                    ViewBag.ListOfProgress = new SelectList(GetProgresEnums(), "Id", "Label");
                    tpd = null;
                }

                return View(tpd);
            }

            return RedirectToAction("TrainingProgram");
        }

        [HttpPost]
        public ActionResult EditTraining(TrainingProgramDetails tpd)
        {
            if (isHisTrainingProgram(tpd.IdTrainingProgramDetails))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        using (CDPTrackEntities context = new CDPTrackEntities())
                        {
                            context.Entry(tpd).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        return RedirectToAction("TrainingProgram");
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                }

                return View();
            }

            return RedirectToAction("TrainingProgram");
        }

        public ActionResult EditGeneralTraining(int Id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            if (isHisGeneralTrainingProgram(Id))
            {
                GeneralTrainingProgramDetails tpd = new GeneralTrainingProgramDetails();

                try
                {
                    CDPTrackEntities context = new CDPTrackEntities();
                    tpd = context.GeneralTrainingProgramDetails.Find(Id);
                    ViewBag.ListOfProgress = new SelectList(GetProgresEnums(), "Id", "Label", tpd.ProgressEnum.Id);
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                    ViewBag.ListOfProgress = new SelectList(GetProgresEnums(), "Id", "Label");
                    tpd = null;
                }

                return View(tpd);
            }

            return RedirectToAction("TrainingProgram");
        }

        [HttpPost]
        public ActionResult EditGeneralTraining(GeneralTrainingProgramDetails tpd)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            if (isHisGeneralTrainingProgram(tpd.IdGeneralTrainingProgramDetails))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        using (CDPTrackEntities context = new CDPTrackEntities())
                        {
                            context.Entry(tpd).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        return RedirectToAction("TrainingProgram");
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                }
                return View();
            }
            return RedirectToAction("TrainingProgram");
        }

        public ActionResult EditTrainingProgramOnDemandStatus(int id)
        {
            TrainingProgramOnDemandDetails tpodd = new TrainingProgramOnDemandDetails();

            try
            {
                CDPTrackEntities context = new CDPTrackEntities();
                tpodd = context.TrainingProgramOnDemandDetails.Find(id);
                ViewBag.ListOfProgress = new SelectList(GetProgresEnums(), "Id", "Label", tpodd.Status);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                ViewBag.ListOfProgress = new SelectList(GetProgresEnums(), "Id", "Label");
                tpodd = null;
            }

            return View(tpodd);
        }

        [HttpPost]
        public ActionResult EditTrainingProgramOnDemandStatus(TrainingProgramOnDemandDetails tpodd)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (CDPTrackEntities context = new CDPTrackEntities())
                    {
                        context.Entry(tpodd).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    return RedirectToAction("TrainingProgram");
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
            }
            return View();
        }

        private bool isHisGeneralTrainingProgram(int idGeneralTrainingProgram)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
                return false;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<GeneralTrainingProgramDetails> assessmentList = context.GeneralTrainingProgramDetails.Where(x => x.ResourceId == resource.ResourceId && x.IdGeneralTrainingProgramDetails == idGeneralTrainingProgram).ToList();
                return assessmentList.Count > 0;
            }
        }

        private bool isHisTrainingProgram(int idTrainingProgram)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
                return false;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<TrainingProgramDetails> assessmentList = context.TrainingProgramDetails.Where(x => x.ResourceId == resource.ResourceId && x.IdTrainingProgramDetails == idTrainingProgram).ToList();
                return assessmentList.Count > 0;
            }
        }
        #endregion

        #region Training Program On Demand

        public ActionResult TrainingProgramOnDemand()
        {
            SetupCommonViewBagValues();

            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<TrainingProgramOnDemand> lstTrainings = new List<TrainingProgramOnDemand>();

                var duplicates = from TrainingEmployee in context.TrainingProgramOnDemandDetails
                                 from TrainingOnDemand in context.TrainingProgramOnDemand
                                 where (TrainingEmployee.IdTrainingProgramOnDemand == TrainingOnDemand.IdTrainingProgramOnDemand &&
                                        TrainingEmployee.ResourceId == resource.ResourceId)
                                 select TrainingOnDemand;

                lstTrainings = context.TrainingProgramOnDemand.Where(x => x.Enable == true).Except(duplicates).ToList();
                return View(lstTrainings);
            }
        }

        [HttpPost]
        public ActionResult TrainingProgramOnDemand(TrainingProgramOnDemandDetails tpodd, int id)
        {
            SetupCommonViewBagValues();

            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                if (ModelState.IsValid)
                {
                    tpodd.IdTrainingProgramOnDemand = id;
                    tpodd.Status = 0;
                    tpodd.ResourceId = resource.ResourceId;
                    context.TrainingProgramOnDemandDetails.Add(tpodd);
                    context.SaveChanges();

                    return RedirectToAction("TrainingProgramOnDemand");
                }
                else
                {
                    ModelState.AddModelError("Error", "There is something wrong.");
                }
            }

            return View();
        }

        #endregion

        #endregion

        /***********************************************************  Region for Action Results of "CDP Guidelines" ************************/
        #region CDP Guidelines

        #region Career Path
        public ActionResult CareerPath()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.AreaId = 0;

            return View();
        }

        [HttpPost]
        public ActionResult CareerPath(Area area, int IdArea)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            List<Area> areas = new List<Area>();

            CDPTrackEntities context = new CDPTrackEntities();
            areas = context.Area.ToList();

            var areaItem = areas.Find(x => x.AreaId == IdArea);

            ViewBag.AreaId = areaItem.AreaId;
            if (areaItem.ImageCareerPath != null)
            {
                ViewBag.FileName = areaItem.ImageCareerPath;
            }
            else
            {
                ViewBag.FileName = "default.jpg";
            }

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", IdArea);

            //string DirPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)));
            //string FilePath = DirPath + @"\CDPTrackerSite\Content\images\CareerPath\" + areaItem.ImageCareerPath;
            //if (System.IO.File.Exists(FilePath))
            //{                
            //}

            return View(area);
        }
        #endregion

        #region Job Description
        public ActionResult JobDescription()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.AreaId = 0;

            return View();
        }

        [HttpPost]
        public ActionResult JobDescription(List<Position> position, int Area)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                ViewBag.AreaId = Area;
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", Area);

                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var positionsByArea = context.Position.Include("Area")
                                                          .Where(x => x.AreaId == Area && x.PositionName != "UNKNOWN")
                                                          .OrderBy(x => x.PositionName).ToList();

                    ViewBag.Area = positionsByArea.FirstOrDefault().Area.Name.ToString();
                    position = positionsByArea;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                position = null;
            }

            return View(position);
        }
        #endregion

        #region Skill Compass

        public ActionResult SkillCompass()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.AreaId = 0;

            return View();
        }

        [HttpPost]
        public ActionResult SkillCompass(Area area, int IdArea)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            List<Area> areas = new List<Area>();
            List<SkillCompassGlossary> glossary = new List<SkillCompassGlossary>();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                areas = context.Area.ToList();
                glossary = context.SkillCompassGlossary.Where(x => x.AreaId == IdArea).OrderBy(x => x.Name).ToList();
            }

            var areaItem = areas.Find(x => x.AreaId == IdArea);

            ViewBag.AreaId = areaItem.AreaId;
            if (areaItem.ImageSkillCompass != null)
            {
                ViewBag.FileName = areaItem.ImageSkillCompass;
            }
            else
            {
                ViewBag.FileName = "default.jpg";
            }

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", IdArea);
            ViewBag.glossary = glossary;
            return View(area);
        }

        public JsonResult JsonGetGlossary(string id)
        {
            List<SelectListItem> glossary = new List<SelectListItem>();

            int AreaId = 0;
            int.TryParse(id, out AreaId);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                if (AreaId != 0)
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var glossaryByArea = context.SkillCompassGlossary.Where(x => x.AreaId == AreaId).ToList();

                    foreach (SkillCompassGlossary item in glossaryByArea)
                    {
                        glossary.Add(new SelectListItem { Value = item.SkillCompassGlossaryId.ToString(), Text = item.Name });
                    }

                    return Json(new SelectList(glossary, "Value", "Text"));
                }
                else
                {
                    return Json(new SelectList(glossary, "Value", "Text"));
                }
            }
        }

        public JsonResult JsonGetSkillDescription(string SkillCompassId)
        {
            int SkillId = 0;
            int.TryParse(SkillCompassId, out SkillId);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                SkillCompassGlossary skillFound = new SkillCompassGlossary();
                if (SkillId != 0)
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    skillFound = context.SkillCompassGlossary.Where(x => x.SkillCompassGlossaryId == SkillId).FirstOrDefault();
                    return Json(skillFound);
                }
                else
                {
                    return Json(skillFound);
                }
            }
        }


        #endregion

        #region Suggestions
        public ActionResult Suggestions()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.AreaId = 0;
            ViewBag.PositionId = 0;

            if (Session["Position"] != null)
            {
                int AreaId = 0;
                int.TryParse(Session["Area"].ToString(), out AreaId);

                int PositionId = 0;
                int.TryParse(Session["Position"].ToString(), out PositionId);

                var lstSuggestions = ObtainSuggestionTable(PositionId);

                ViewBag.AreaId = AreaId;
                ViewBag.PositionId = PositionId;

                Session["Area"] = null;
                Session["Position"] = null;

                return View(lstSuggestions);
            }

            Session["Area"] = null;
            Session["Position"] = null;

            return View();
        }

        [HttpPost]
        public ActionResult Suggestions(Suggestions suggestions, int Area, int? Position)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var lstSuggestions = context.Suggestions.Include("Position")
                                                            .Include("Technologies")
                                                            .Include("Level")
                                                            .Where(m => m.PositionId == Position)
                                                            .OrderBy(m => m.Technologies.Name)
                                                            .ThenBy(m => m.Level.Name)
                                                            .ToList();
                    if (Position == null || Position == 0)
                    {
                        lstSuggestions = context.Suggestions.Include("Position").Include("Technologies").Include("Level")
                        .Where(m => m.AreaId == Area).OrderBy(m => m.Technologies.Name).ThenBy(m => m.Level.Name)
                        .ToList();
                    }

                    ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                    ViewBag.AreaId = Area;
                    ViewBag.PositionId = Position;

                    return View(lstSuggestions);
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                return View();
            }
        }
        public JsonResult JsonGetDescription(int id)
        {
            List<SelectListItem> description = new List<SelectListItem>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                string desc = context.TrainingCategory.Where(x => x.TrainingCategoryId == id).Select(x => x.Description).ToString();
                string trainingCategoryId = context.TrainingCategory.Where(x => x.TrainingCategoryId == id).Select(x => x.TrainingCategoryId).ToString();
                description.Add(new SelectListItem { Value = trainingCategoryId, Text = desc });
                return Json(new SelectList(description, "Value", "Text"));
            }
        }
        public JsonResult JsonGetPosition(string id)
        {
            List<SelectListItem> position = new List<SelectListItem>();

            int AreaId = 0;
            int.TryParse(id, out AreaId);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                if (AreaId != 0)
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var positionsByArea = context.Position.Include("Area")
                                                          .Where(x => x.AreaId == AreaId && x.PositionName != "UNKNOWN")
                                                          .ToList();

                    foreach (Position item in positionsByArea)
                    {
                        position.Add(new SelectListItem { Value = item.PositionId.ToString(), Text = item.PositionName });
                    }

                    return Json(new SelectList(position, "Value", "Text"));
                }
                else
                {
                    return Json(new SelectList(position, "Value", "Text"));
                }
            }
        }
        #endregion

        #endregion

        /**********************************************************  Region for Action Results of "Employee Goals"**************************/
        #region Employee Goals



        #region Employee Goals Tracking
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement, Role.Manager)]
#endif
        public ActionResult EmployeeGoals(Resource resource, int? quarter, int? year)
        {
            string managerName = User.Identity.Name;

#if DEBUG
            managerName = "cdptracker manager";
#endif
            Resource manager;
            if (!ResourceDataAccessor.TryGetResourceByUserName(managerName, out manager))
            {
                return View();
            }

            List<Resource> managerResources;
            if (!ResourceDataAccessor.TryGetResourcesFromManager(managerName, out managerResources))
            {
                return View();
            }


            ListOfYears();
            int? actualQuarter = quarter;
            int? currentYear = year;
            bool currentQuarter = false;

            if (year == null || quarter == null || isCurrentQuarter(year, quarter))
            {
                quarter = GetQuarter(DateTime.Now.Month);
                year = DateTime.Now.Year;
                actualQuarter = GetActualQuarter();
                currentYear = GetCurrentYear();
                currentQuarter = true;
            }

            manager.Employees =
                managerResources.Where(r => r.Employee != null).Select(r => r.Employee).OrderBy(r => r.Resource.Name).
                    ToList();

            List<ResourceModel> data = new List<ResourceModel>();

            foreach (var item in manager.Employees)
            {
                ResourceModel employee = new ResourceModel();
                employee.resourceId = item.Resource.ResourceId;
                employee.resourceName = item.Resource.Name;
                employee.managerId = manager.ResourceId;

                if (!currentQuarter)
                {
                    List<Objective> objectives = item.Resource.Objective.OrderByDescending(r => r.ObjectiveId).Where(r => r.Quarter == quarter && r.Year == year).ToList();
                    item.Resource.Objective = objectives;
                }

                List<CyclePrioritiesModel> priorityList = new List<CyclePrioritiesModel>();
                foreach (var objective in item.Resource.Objective)
                {
                    CyclePrioritiesModel priority = new CyclePrioritiesModel();
                    priority.priorityId = objective.ObjectiveId;
                    priority.priorityDescription = objective.Objective1;
                    priority.priorityProgress = objective.Progress;

                    List<TasksModel> taskList = new List<TasksModel>();
                    foreach (var goal in objective.GoalTracking)
                    {
                        if (goal.VerifiedByManager && currentQuarter && !isCurrentQuarter(goal.Objective.Year, goal.Objective.Quarter))
                        {
                            continue;
                        }
                        TasksModel task = new TasksModel();
                        task.taskId = goal.GoalId;
                        task.taskDescription = goal.Goal;
                        task.taskFinishDate = goal.FinishDate;
                        task.taskProgress = goal.Progress;
                        task.taskVerified = goal.VerifiedByManager;
                        task.taskTdu = goal.TDU;
                        task.trainingCategoryId = goal.TrainingCategoryId;

                        if (goal.VerifiedByManager)
                        {
                            task.imgProgress = "~/Content/images/verifiedManager.png";
                            task.statusTitle = "Verified by manager";
                        }
                        else
                        {
                            switch ((int)task.taskProgress)
                            {
                                case STATUS_NOT_STARTED:
                                    task.imgProgress = "~/Content/images/notStarted.png";
                                    task.statusTitle = "Not started";
                                    break;
                                case STATUS_STARTED:
                                    task.imgProgress = "~/Content/images/inProgress.png";
                                    task.statusTitle = "Started";
                                    break;
                                case STATUS_COMPLETED:
                                    task.imgProgress = "~/Content/images/needsVerification.png";
                                    task.statusTitle = "Completed and needs verification";
                                    break;
                                default:
                                    task.imgProgress = "~/Content/images/DeleteGoal-icon.png";
                                    task.statusTitle = "Undefined";
                                    break;
                            }
                        }

                        taskList.Add(task);
                    }

                    if (taskList.Any())
                    {
                        priority.tasks = taskList;
                        priorityList.Add(priority);
                    }
                }
                employee.priorities = priorityList;

                data.Add(employee);
            }

            SetupCommonViewBagValues();
            SetProfileData(manager);
            ViewBag.CurrentCycle = GetCurrentCycle();
            ViewBag.changeQuarter = true;
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", actualQuarter);
            ViewBag.listOfYears = new SelectList(ViewBag.listOfYears, "Value", "Text", currentYear);

            return View(data);
        }
        #endregion

        #region Goal Verification
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement, Role.Manager)]
#endif
        public ActionResult ManagerVerification()
        {
            // ReSharper disable RedundantAssignment
            string managerName = User.Identity.Name;
            // ReSharper restore RedundantAssignment

#if DEBUG
            //  managerName = "cdptracker manager";
#endif

            Resource manager;
            if (!ResourceDataAccessor.TryGetResourceByUserName(managerName, out manager))
            {
                return View();
            }

            List<Resource> managerResources;
            if (!ResourceDataAccessor.TryGetVerifiableResourcesFromManager(managerName, out managerResources))
            {
                return View();
            }

            manager.Employees =
                managerResources.Where(r => r.Employee != null).Select(r => r.Employee).OrderBy(r => r.Resource.Name).
                    ToList();

            List<ResourceModel> data = new List<ResourceModel>();
            foreach (var item in manager.Employees)
            {
                ResourceModel employee = new ResourceModel();
                employee.resourceId = item.Resource.ResourceId;
                employee.resourceName = item.Resource.Name;
                employee.managerId = manager.ResourceId;

                List<CyclePrioritiesModel> priorityList = new List<CyclePrioritiesModel>();
                foreach (var objective in item.Resource.Objective)
                {
                    CyclePrioritiesModel priority = new CyclePrioritiesModel();
                    priority.priorityId = objective.ObjectiveId;
                    priority.priorityDescription = objective.Objective1;
                    priority.priorityProgress = objective.Progress;

                    List<TasksModel> taskList = new List<TasksModel>();
                    foreach (var goal in objective.GoalTracking)
                    {
                        TasksModel task = new TasksModel();
                        task.taskId = goal.GoalId;
                        task.taskDescription = goal.Goal;
                        task.taskFinishDate = goal.FinishDate;
                        task.taskProgress = goal.Progress;
                        task.taskVerified = goal.VerifiedByManager;
                        task.taskTdu = goal.TDU;
                        task.trainingCategoryId = goal.TrainingCategoryId;
                        task.trainingCategoryDescription = context.TrainingCategory.Where(x => x.TrainingCategoryId == goal.TrainingCategoryId).Select(x => x.Name).FirstOrDefault();

                        if (goal.VerifiedByManager)
                        {
                            task.imgProgress = "~/Content/images/verifiedManager.png";
                            task.statusTitle = "Verified by manager";
                        }
                        else
                        {
                            switch ((int)task.taskProgress)
                            {
                                case STATUS_NOT_STARTED:
                                    task.imgProgress = "~/Content/images/notStarted.png";
                                    task.statusTitle = "Not started";
                                    break;
                                case STATUS_STARTED:
                                    task.imgProgress = "~/Content/images/inProgress.png";
                                    task.statusTitle = "Started";
                                    break;
                                case STATUS_COMPLETED:
                                    task.imgProgress = "~/Content/images/done.png";
                                    task.statusTitle = "Completed and needs verification";
                                    break;
                                default:
                                    task.imgProgress = "~/Content/images/DeleteGoal-icon.png";
                                    task.statusTitle = "Undefined";
                                    break;
                            }
                        }

                        taskList.Add(task);
                    }
                    priority.tasks = taskList;

                    priorityList.Add(priority);
                }
                employee.priorities = priorityList;

                data.Add(employee);
            }

            int goalCount = GetGoalsCountFromManagerResources(managerResources);

            SetupCommonViewBagValues(goalCount);
            SetProfileData(manager);
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());
            ViewBag.CurrentCycle = GetCurrentCycle();
            return View(data);
        }


#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement, Role.Manager)]
#endif
        // POST: ActionResult for verify the task
        [HttpPost]
        public ActionResult VerifyTask(int taskId)
        {
            if (taskId != 0)
            {
                GoalTracking dbTask = context.GoalTrackings.Where(t => t.GoalId == taskId).FirstOrDefault();

                dbTask.VerifiedByManager = (dbTask.VerifiedByManager) ? false : true;
                context.SaveChanges();

            }
            return RedirectToAction("EmployeeGoals", "GoalTracking");
        }


        //---------------------------------------------------------------- OLD ACTIONRESULTS -------------------------------------------------------------------

        #region Pending Verifications
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement, Role.Manager)]
#endif
        public ActionResult PendingVerifications()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            List<EmployeeActiveDirectoryManager.UserData> userData = ResourceDataAccessor.GetUserData();

            bool isManager = RoleManagementHelper.UserIsInRole(User, Role.Manager);
            bool isTalentManagement = RoleManagementHelper.UserIsInRole(User, Role.TalentManagement);
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var resourcesPendingVerification =
                    context.Resources.Where(r => r.GoalTrackings.Any(g => g.Progress == (int)ProgressEnumeration.Completed && !g.VerifiedByManager))
                    .Select(r => new
                    {
                        r.DomainName,
                        PendingGoalCount = r.GoalTrackings.Count(g => g.Progress == (int)ProgressEnumeration.Completed && !g.VerifiedByManager)
                    }).ToList();

                //If the use is not talent then only show his employees
                /*@if (!isTalentManagement)
                {
                   var manager = context.Resources.Where(r => r.Name == User.Identity.Name).ToList().FirstOrDefault();
                   resourcesPendingVerification.Where(r => !!(GetManager(r.DomainName, userData)).Any());
                }*/


                var resourcesPendingVerificationAddedManager =
                    resourcesPendingVerification
                        .Select(r => new
                        {
                            r.DomainName,
                            r.PendingGoalCount,
                            ManagerName = GetManager(r.DomainName, userData)
                        }).ToList();

                //remove employees without a manager.
                resourcesPendingVerificationAddedManager = resourcesPendingVerificationAddedManager.Where(x => x.ManagerName != string.Empty).ToList();

                var managersPendingGoalVerification = from a in resourcesPendingVerificationAddedManager
                                                      group a by a.ManagerName into g
                                                      select new { ManagerName = g.Key, GoalsPendingVerificationCount = g.Sum(m => m.PendingGoalCount), DirectReport = g.ToList() };

                ViewBag.Name = User.Identity.Name.StripDomain();
                return View(managersPendingGoalVerification);
            }
        }


        #endregion


        [HttpPost]
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement, Role.Manager)]
#endif
        public ActionResult ManagerVerification(Resource resource)
        {
            List<Resource> originalResources;
            // ReSharper disable RedundantAssignment
            string managerName = User.Identity.Name;
            // ReSharper restore RedundantAssignment

#if DEBUG
            //  managerName = "cdptracker manager";
#endif
            if (ResourceDataAccessor.TryGetVerifiableResourcesFromManager(managerName, out originalResources))
            {
                var UpdatedGoals = new List<GoalTracking>();
                //using (CDPTrackEntities context = new CDPTrackEntities())
                //{
                bool isUpdated = false;
                foreach (Employee updatedEmployee in resource.Employees)
                {
                    var updatedResource = updatedEmployee.Resource;
                    if (updatedResource == null) continue;

                    var resourceId = updatedResource.ResourceId;
                    var originalGoals = context.GoalTrackings.Where(g => g.ResourceId == resourceId);
                    if (!originalGoals.Any()) continue;

                    foreach (var updatedGoal in updatedResource.GoalTrackings)
                    {
                        var goalId = updatedGoal.GoalId;
                        var originalGoal = originalGoals.FirstOrDefault(g => g.GoalId == goalId);
                        if (originalGoal == null) continue;

                        if (updatedGoal.VerifiedByManager != originalGoal.VerifiedByManager)
                        {
                            originalGoal.VerifiedByManager = updatedGoal.VerifiedByManager;
                            originalGoal.LastUpdate = DateTime.Now;
                            context.Entry(originalGoal).State = EntityState.Modified;
                            isUpdated = true;
                            UpdatedGoals.Add(originalGoal);
                        }
                    }
                }
                if (isUpdated)
                {
                    context.SaveChanges();
                    //If more than one Goal was verified run the TDURedeem method
                    if (UpdatedGoals.Count() > 0) InsertTDURedeem(UpdatedGoals);
                }
                //}
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement, Role.Manager)]
#endif
        public ActionResult ForceGoalVerification(bool IsManager, bool isTalentManagement, string name)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                try
                {
                    var goalsList = new List<GoalTracking>();
                    if (isTalentManagement)
                    {
                        var goalsPendingVerification =
                            context.GoalTrackings.Where(g => g.Progress == (int)ProgressEnumeration.Completed && !g.VerifiedByManager).ToList();

                        foreach (var goalTracking in goalsPendingVerification)
                        {
                            goalTracking.VerifiedByManager = true;
                            goalTracking.FinishDate = goalTracking.FinishDate ?? DateTime.Now;
                            goalsList.Add(goalTracking);
                        }
                        context.SaveChanges();
                        if (goalsList.Count > 0) InsertTDURedeem(goalsList);
                    }
                    else
                    {
                        EmployeeActiveDirectoryManager Ad = getActiveDirectoryInstance();

                        Resource employee = context.Resources.Where(x => x.DomainName == name).FirstOrDefault();
                        List<string> resources = Ad.GetManagerResourcesDomainNames(employee.DomainName);

                        foreach (var resource in resources)
                        {
                            Resource employeeObject = context.Resources.Where(x => x.DomainName == resource).FirstOrDefault();
                            var goalsPendingVerification =
                                context.GoalTrackings.Where(g => g.Progress == (int)ProgressEnumeration.Completed && !g.VerifiedByManager && g.ResourceId == employeeObject.ResourceId).ToList();

                            foreach (var goalTracking in goalsPendingVerification)
                            {
                                goalTracking.VerifiedByManager = true;
                                goalTracking.FinishDate = goalTracking.FinishDate ?? DateTime.Now;
                                goalsList.Add(goalTracking);
                            }
                        }
                        context.SaveChanges();
                        if (goalsList.Count > 0) InsertTDURedeem(goalsList);
                    }
                }

                catch (DbEntityValidationException ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        #endregion

        /*********************************************************** Region for Action Results of "Reports" Area ***************************/
        #region Reports

        #region ProgressReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement, Role.Manager)]
#endif
        public ActionResult ProgressReport()
        {
            SetupProgressReportData();

            return View();
        }
        #endregion

        #region MonthlyReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement, Role.Manager)]
#endif
        public ActionResult MonthlyReport()
        {
            SetupProgressReportData();

            return View();
        }
        #endregion

        #region ProgressReportWithArea

#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult ProgressReportWithArea()
        {
            SetupProgressReportData();

            return View();
        }
        #endregion

        #region NoPrioritiesReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult NoPrioritiesReport()
        {
            SetupProgressReportData();
            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                return View();
            }
            SetProfileData(resource);
            ListOfYears();
            ViewBag.Year = GetCurrentYear();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());

            return View();
        }
        #endregion

        #region TeamMembersInputReport



#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult TeamMembersInputReport()
        {
            SetupProgressReportData();
            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                return View();
            }
            SetProfileData(resource);
            ListOfYears();
            List<string> managers = GetManagersNames();
            if (managers != null)
            {
                List<SelectListItem> managersSelectItems = managers.Select(x => new SelectListItem()
                {
                    Text = x.ToString()
                    ,
                    Value = x.ToString()
                }).ToList();
                managersSelectItems.Insert(0, new SelectListItem() { Text = "All", Value = "All", Selected = true });
                ViewBag.Managers = managersSelectItems;
            }
            ViewBag.Year = GetCurrentYear();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());

            return View();
        }
        #endregion

        #region ManagersCheckReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult ManagersCheckReport()
        {
            SetupProgressReportData();
            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                return View();
            }
            SetProfileData(resource);
            ListOfYears();
            List<string> managers = GetManagersNames();
            if (managers != null)
            {
                List<SelectListItem> managersSelectItems = managers.Select(x => new SelectListItem()
                {
                    Text = x.ToString()
                    ,
                    Value = x.ToString()
                }).ToList();
                managersSelectItems.Insert(0, new SelectListItem() { Text = "All", Value = "All", Selected = true });
                ViewBag.Managers = managersSelectItems;
            }
            ViewBag.Year = GetCurrentYear();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());

            return View();
        }
        #endregion

        #region TDUDetailsReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult TDUDetailsReport()
        {
            SetupProgressReportData();

            ListOfYears();
            ViewBag.Year = GetCurrentYear();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());

            return View();
        }
        #endregion

        #region UnassignObjectiveReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult UnassignObjectiveReport()
        {
            SetupProgressReportData();

            return View();
        }
        #endregion

        #region ProgressByProjectReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult ProgressByProjectReport()
        {
            SetupProgressReportData();

            Reporting.ProgressByProjectReport reportObject = new Reporting.ProgressByProjectReport();
            reportObject.getEmployeesGoalsByProject();

            List<IEnumerable> combosData = new List<IEnumerable>();
            combosData.Add(Reporting.ProgressByProjectReport.areas);
            combosData.Add(Reporting.ProgressByProjectReport.projects);

            return View(combosData);
        }
        #endregion

        #region SelfAssessmentReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult SelfAssessmentReport()
        {
            SetupCommonViewBagValues();
            return View();
        }
        #endregion

        #region commonReportMethods
        private void SetupProgressReportData()
        {
            SetupCommonViewBagValues();

            int minYear = -1;
            int maxYear = -1;
            int minMonth = -1;
            int maxMonth = -1;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                DateTime? maxDate = context.GoalTrackings.Where(g => g.VerifiedByManager).Max(g => g.LastUpdate);
                DateTime? minDate = context.GoalTrackings.Where(g => g.VerifiedByManager).Min(g => g.LastUpdate);
                if (maxDate.HasValue)
                {
                    maxYear = maxDate.Value.Year;
                    maxMonth = maxDate.Value.Month;
                }
                if (minDate.HasValue)
                {
                    minYear = minDate.Value.Year;
                    minMonth = minDate.Value.Month;
                }
            }
            ViewBag.MinYear = minYear;
            ViewBag.MinMonth = minMonth;
            ViewBag.MaxYear = maxYear;
            ViewBag.MaxMonth = maxMonth;
        }
        #endregion

        #region TrainingProgramReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult TrainingProgramReport()
        {
            SetupCommonViewBagValues();
            return View();
        }
        #endregion

        #region KPIReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult KPIReport()
        {
            return View();
        }
        #endregion

        #region TDUReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif

        public ActionResult TDUReport()
        {
            SetupProgressReportData();

            ListOfYears();
            ViewBag.Year = GetCurrentYear();
            ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());

            return View();
        }
        #endregion

        #region TrendByLevelReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult TrendByLevelReport()
        {
            SetupProgressReportData();
            return View();
        }
        #endregion

        #region TrainingProgramVisitsReport
#if !DEBUG
        [RoleAuthorization(Role.Executive, Role.TalentManagement)]
#endif
        public ActionResult TrainingProgramVisitsReport()
        {
            SetupProgressReportData();
            return View();
        }
        #endregion

        #endregion

        /************************************************************ Region for Action Results of "Admin Catalogs" Area ********************/
        #region Admin Catalogs

        #region Job Description Admin
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult JobDescriptionAdmin()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            //string id = Request.QueryString["id"] ?? "0";
            int id = 0;
            int.TryParse(Request.QueryString["id"], out id);

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.AreaId = id;

            if (id != 0)
            {
                ViewBag.AreaId = id;
                return View(ObtainPositionTable(id));
            }

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult JobDescriptionAdmin(List<Position> position, int Area)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", Area);
                position = ObtainPositionTable(Area);

                ViewBag.AreaId = Area;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                position = null;
            }

            return View(position);
        }

        public List<Position> ObtainPositionTable(int AreaId)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var positionsByArea = context.Position.Include("Area").Where(x => x.AreaId == AreaId && x.PositionName != "UNKNOWN")
                    .OrderBy(x => x.PositionName).ToList();
                ViewBag.Area = positionsByArea.FirstOrDefault().Area.Name.ToString();
                return positionsByArea;
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult EditLink(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            Position position;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var queryPosition = context.Position.Include("Area").Where(x => x.PositionId == id);

                position = new Position();
                position.Area = new Area();

                position.AreaId = queryPosition.FirstOrDefault().AreaId;
                position.Area.Name = queryPosition.FirstOrDefault().Area.Name;
                position.PositionId = queryPosition.FirstOrDefault().PositionId;
                position.PositionName = queryPosition.FirstOrDefault().PositionName;
                position.Description = queryPosition.FirstOrDefault().Description;

                return View(position);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditLink(Position position)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();
            ViewBag.error = "";

            try
            {
                if (ModelState.IsValid)
                {
                    bool verifyUrl = UrlIsValid(position.Description);
                    if (verifyUrl)
                    {
                        using (CDPTrackEntities context = new CDPTrackEntities())
                        {
                            context.Entry(position).State = EntityState.Modified;
                            context.SaveChanges();
                        }

                        return RedirectToAction("JobDescriptionAdmin", new { id = position.AreaId });
                    }
                    else
                    {
                        ViewBag.error = "The URL is invalid.";
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                ViewBag.error = "Value cannot be null.";
            }

            return View(position);
        }

        public static bool UrlIsValid(string url)
        {
            bool br = false;
            try
            {
                Uri myUri = new Uri(url);
                string host = myUri.Host;

                IPHostEntry ipHost = Dns.GetHostEntry(host);
                br = true;
            }
            catch (SocketException se)
            {
                ErrorLogHelper.LogException(se, "CDPTracker");
                if (se.ErrorCode == 11004)
                { br = true; }
                else
                { br = false; }
            }
            return br;
        }
        #endregion

        #region Technology
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult Technology()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<Technologies> lstTechnologies = context.Technologies.OrderBy(m => m.Name).ToList();

                return View(lstTechnologies);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult CreateTechnology()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            ViewBag.error = "";

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult CreateTechnology(Technologies technologies)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            //var errors = ModelState.Values.SelectMany(v => v.Errors);

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Technologies.Add(technologies);
                    context.SaveChanges();

                    return RedirectToAction("Technology");
                }
            }
            catch (Exception)
            {
                ViewBag.error = "The Technology field is required";
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult EditTechnology(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();
            Technologies technology = new Technologies();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var query = context.Technologies.Where(x => x.TechnologyId == id);

                    technology.TechnologyId = query.FirstOrDefault().TechnologyId;
                    technology.Name = query.FirstOrDefault().Name;

                    ViewBag.error = "";
                    return View(technology);
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.error = "Error Message: " + e.Message;
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditTechnology(Technologies technologies)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            try
            {
                if (ModelState.IsValid)
                {
                    using (CDPTrackEntities context = new CDPTrackEntities())
                    {
                        context.Entry(technologies).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    return RedirectToAction("Technology");
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    ViewBag.error = message.ToString();
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.error = "The Technology field is required";
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult DeleteTechnology(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();
            Technologies technologies = new Technologies();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var technology = context.Technologies.Where(m => m.TechnologyId == id);
                    technologies.TechnologyId = technology.FirstOrDefault().TechnologyId;
                    technologies.Name = technology.FirstOrDefault().Name;

                    ViewBag.error = "";
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                technologies = null;
                ViewBag.error = "Error Message: " + e.Message;
            }

            return View(technologies);
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult DeleteTechnology(Technologies technologies)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var technology = context.Technologies.Find(technologies.TechnologyId);
                    technologies.Name = technology.Name;
                    context.Technologies.Remove(technology);
                    context.SaveChanges();

                    return RedirectToAction("Technology");
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.error = "Error Message: " + e.Message;
                return View(technologies);
            }
        }
        #endregion

        #region Suggestion Admin
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult SuggestionsAdmin()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.AreaId = 0;
            ViewBag.PositionId = 0;

            if (Session["Position"] != null)
            {
                int AreaId = 0;
                int.TryParse(Session["Area"].ToString(), out AreaId);

                int PositionId = 0;
                int.TryParse(Session["Position"].ToString(), out PositionId);

                var lstSuggestions = ObtainSuggestionTable(PositionId);

                ViewBag.AreaId = AreaId;
                ViewBag.PositionId = PositionId;

                Session["Area"] = null;
                Session["Position"] = null;
                Session["Technology"] = null;
                Session["Level"] = null;

                return View(lstSuggestions);
            }

            Session["Area"] = null;
            Session["Position"] = null;
            Session["Technology"] = null;
            Session["Level"] = null;

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult SuggestionsAdmin(Suggestions suggestions, int Area, int? Position)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();
            if (Position == null)
                Position = 0;
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    var lstSuggestions = context.Suggestions.Include("Position").Include("Technologies").Include("Level")
                        .Where(m => m.PositionId == Position).OrderBy(m => m.Technologies.Name).ThenBy(m => m.Level.Name)
                        .ToList();
                    if (Position == 0)
                    {
                        lstSuggestions = context.Suggestions.Include("Position").Include("Technologies").Include("Level")
                        .Where(m => m.AreaId == Area).OrderBy(m => m.Technologies.Name).ThenBy(m => m.Level.Name)
                        .ToList();
                    }

                    ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                    ViewBag.AreaId = Area;
                    ViewBag.PositionId = Position;

                    return View(lstSuggestions);
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult CreateSuggestion()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var other = from position in context.Position
                            where position.PositionName == "Other"
                            select position.PositionId;
                int otherId = 0;
                foreach (var id in other)
                {
                    otherId = Convert.ToInt16(id);
                }
                ViewBag.Other = Convert.ToInt16(otherId);
                try
                {
                    string ida = Request.QueryString["ida"];
                    string idp = Request.QueryString["idp"];

                    ViewBag.AreaId = ida;
                    ViewBag.PositionId = idp;
                    ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                    ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name");
                    ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name");
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogException(ex, "CDPTracker");
                    ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                    ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name");
                    ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name");
                }

                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult CreateSuggestion(Suggestions suggestion, int Area, int Position, int Technology, int Level)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            try
            {
                suggestion.PositionId = Position;
                suggestion.TechnologyId = Technology;
                suggestion.LevelId = Level;
                suggestion.AreaId = Area;

                ViewBag.AreaId = Area;
                ViewBag.PositionId = Position;
                ViewBag.error = "";

                bool verifyUrl = UrlIsValid(suggestion.Source);
                if (verifyUrl)
                {
                    ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                    ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name");
                    ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name");

                    using (CDPTrackEntities context = new CDPTrackEntities())
                    {
                        context.Suggestions.Add(suggestion);
                        context.SaveChanges();
                    }

                    return RedirectToAction("SuggestionsAdmin");
                }
                else
                {
                    ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", Area);
                    ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name", suggestion.TechnologyId);
                    ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name", suggestion.LevelId);
                    ViewBag.
                    ViewBag.error = "The URL is invalid";
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", Area);
                ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name", suggestion.TechnologyId);
                ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name", suggestion.LevelId);
                ViewBag.error = e.Message;
            }

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult EditSuggestion(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();
            Suggestions suggestion = new Suggestions();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var querySuggestion = context.Suggestions.Where(x => x.SuggestionId == id);

                    suggestion.SuggestionId = querySuggestion.FirstOrDefault().SuggestionId;
                    suggestion.PositionId = querySuggestion.FirstOrDefault().PositionId;
                    suggestion.TechnologyId = querySuggestion.FirstOrDefault().TechnologyId;
                    suggestion.LevelId = querySuggestion.FirstOrDefault().LevelId;
                    suggestion.Topic = querySuggestion.FirstOrDefault().Topic;
                    suggestion.Source = querySuggestion.FirstOrDefault().Source;

                    var queryArea = context.Position.Where(x => x.PositionId == querySuggestion.FirstOrDefault().PositionId);

                    ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", queryArea.FirstOrDefault().AreaId);
                    ViewBag.ListOfPositions = new SelectList(GetPosition(), "PositionId", "PositionName", querySuggestion.FirstOrDefault().PositionId);
                    ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name", querySuggestion.FirstOrDefault().TechnologyId);
                    ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name", querySuggestion.FirstOrDefault().LevelId);

                    Session["Area"] = queryArea.FirstOrDefault().AreaId;
                    Session["Position"] = querySuggestion.FirstOrDefault().PositionId;
                    Session["Technology"] = querySuggestion.FirstOrDefault().TechnologyId;
                    Session["Level"] = querySuggestion.FirstOrDefault().LevelId;

                    ViewBag.AreaId = 0;
                    ViewBag.PositionId = 0;
                    ViewBag.error = "";

                    return View(suggestion);
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditSuggestion(Suggestions suggestion, int Area, int Position, int Technology, int Level)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();
            ViewBag.error = "";

            try
            {
                bool verifyUrl = UrlIsValid(suggestion.Source);
                if (verifyUrl)
                {
                    using (CDPTrackEntities context = new CDPTrackEntities())
                    {
                        suggestion.AreaId = Area;
                        suggestion.PositionId = Position;
                        suggestion.TechnologyId = Technology;
                        suggestion.LevelId = Level;
                        context.Entry(suggestion).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    return RedirectToAction("SuggestionsAdmin");
                }
                else
                {
                    ViewBag.error = "The URL is invalid";
                }

                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                ViewBag.AreaId = Area;
                ViewBag.PositionId = Position;

                ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name", Technology);
                ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name", Level);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                ViewBag.AreaId = int.Parse(Session["Area"].ToString());
                ViewBag.PositionId = int.Parse(Session["Position"].ToString());

                ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name", int.Parse(Session["Technology"].ToString()));
                ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name", int.Parse(Session["Level"].ToString()));
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                ViewBag.AreaId = int.Parse(Session["Area"].ToString());
                ViewBag.PositionId = int.Parse(Session["Position"].ToString());

                ViewBag.ListOfTechnologies = new SelectList(GetTechnology(), "TechnologyId", "Name", int.Parse(Session["Technology"].ToString()));
                ViewBag.ListOfLevels = new SelectList(GetLevel(), "LevelId", "Name", int.Parse(Session["Level"].ToString()));
                ViewBag.error = e.Message;
            }

            return View(suggestion);
        }

        public List<Suggestions> ObtainSuggestionTable(int PositionId)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var lstSuggestions = context.Suggestions.Include("Position").Include("Technologies").Include("Level")
                    .Where(m => m.PositionId == PositionId).OrderBy(m => m.Technologies.Name).ThenBy(m => m.Level.Name)
                    .ToList();

                return lstSuggestions;
            }
        }


#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult DeleteSuggestion(int id)
        {
            SetupCommonViewBagValues();
            Suggestions suggestion = new Suggestions();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var queryTopic = context.Suggestions.Include("Position").Include("Technologies").Include("Level").Where(m => m.SuggestionId == id);

                    suggestion.Position = new Position();
                    suggestion.Position.Area = new Area();
                    suggestion.Technologies = new Technologies();
                    suggestion.Level = new Level();

                    suggestion.SuggestionId = queryTopic.FirstOrDefault().SuggestionId;
                    suggestion.Position.Area.Name = queryTopic.FirstOrDefault().Position.Area.Name;
                    suggestion.Position.PositionName = queryTopic.FirstOrDefault().Position.PositionName;
                    suggestion.Technologies.Name = queryTopic.FirstOrDefault().Technologies.Name;
                    suggestion.Level.Name = queryTopic.FirstOrDefault().Level.Name;
                    suggestion.Topic = queryTopic.FirstOrDefault().Topic;
                    suggestion.Source = queryTopic.FirstOrDefault().Source;

                    Session["Area"] = queryTopic.FirstOrDefault().Position.AreaId;
                    Session["Position"] = queryTopic.FirstOrDefault().PositionId;
                    ViewBag.error = "";
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                suggestion = null;
                ViewBag.error = "Error Message: " + e.Message;
            }

            return View(suggestion);
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult DeleteSuggestion(Suggestions suggestion)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var querySuggestion = context.Suggestions.Find(suggestion.SuggestionId);
                    context.Suggestions.Remove(querySuggestion);
                    context.SaveChanges();

                    return RedirectToAction("SuggestionsAdmin");
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.error = "Error Message: " + e.Message;
                return View();
            }
        }
        #endregion

        #region Level
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult Level()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<Level> lstLevel = context.Level.OrderBy(m => m.Name).ToList();

                return View(lstLevel);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult CreateLevel()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.error = "";

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult CreateLevel(Level level)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Level.Add(level);
                    context.SaveChanges();

                    return RedirectToAction("Level");
                }
            }
            catch (Exception)
            {
                ViewBag.error = "The Level field is required";
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult EditLevel(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            Level level = new Level();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var query = context.Level.Where(x => x.LevelId == id);

                    level.LevelId = query.FirstOrDefault().LevelId;
                    level.Name = query.FirstOrDefault().Name;
                }

                ViewBag.error = "";
                return View(level);
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.error = "Error Message: " + e.Message;
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditLevel(Level level)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                if (ModelState.IsValid)
                {
                    using (CDPTrackEntities context = new CDPTrackEntities())
                    {
                        context.Entry(level).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    return RedirectToAction("Level");
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    ViewBag.error = message.ToString();
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.error = "The Level field is required";
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult DeleteLevel(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            Level level = new Level();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var queryLevel = context.Level.Where(m => m.LevelId == id);
                    level.LevelId = queryLevel.FirstOrDefault().LevelId;
                    level.Name = queryLevel.FirstOrDefault().Name;

                    ViewBag.error = "";
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                level = null;
                ViewBag.error = "Error Message: " + e.Message;
            }

            return View(level);
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult DeleteLevel(Level level)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var queryLevel = context.Level.Find(level.LevelId);
                    level.Name = queryLevel.Name;
                    context.Level.Remove(queryLevel);
                    context.SaveChanges();

                    return RedirectToAction("Level");
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.error = "Error Message: " + e.Message;
                return View(level);
            }
        }
        #endregion

        #region Training Program Category
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult TrainingProgramCategory()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<TrainingProgramCategory> lstTraining = context.TrainingProgramCategory.OrderBy(m => m.Name).ToList();

                return View(lstTraining);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult CreateTrainingProgramCategory()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.error = "";

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult CreateTrainingProgramCategory(TrainingProgramCategory training)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.TrainingProgramCategory.Add(training);
                    context.SaveChanges();

                    return RedirectToAction("TrainingProgramCategory");
                }
            }
            catch (Exception)
            {
                ViewBag.error = "The Training Program field is required";
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult EditTrainingProgramCategory(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            TrainingProgramCategory training = new TrainingProgramCategory();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var query = context.TrainingProgramCategory.Where(x => x.IdTrainingProgramCategory == id);

                    training.IdTrainingProgramCategory = query.FirstOrDefault().IdTrainingProgramCategory;
                    training.Name = query.FirstOrDefault().Name;

                    ViewBag.error = "";
                    return View(training);
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.error = "Error Message: " + e.Message;
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditTrainingProgramCategory(TrainingProgramCategory training)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                if (ModelState.IsValid)
                {
                    using (CDPTrackEntities context = new CDPTrackEntities())
                    {
                        context.Entry(training).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    return RedirectToAction("TrainingProgramCategory");
                }
                else
                {
                    var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    ViewBag.error = message.ToString();
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.error = "The Training Program field is required";
                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult DeleteTrainingProgramCategory(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            TrainingProgramCategory training = new TrainingProgramCategory();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var queryTraining = context.TrainingProgramCategory.Where(m => m.IdTrainingProgramCategory == id);
                    training.IdTrainingProgramCategory = queryTraining.FirstOrDefault().IdTrainingProgramCategory;
                    training.Name = queryTraining.FirstOrDefault().Name;

                    ViewBag.error = "";
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                training = null;
                ViewBag.error = "Error Message: " + e.Message;
            }

            return View(training);
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult DeleteTrainingProgramCategory(TrainingProgramCategory training)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var queryTraining = context.TrainingProgramCategory.Find(training.IdTrainingProgramCategory);
                    training.Name = queryTraining.Name;
                    context.TrainingProgramCategory.Remove(queryTraining);
                    context.SaveChanges();

                    return RedirectToAction("TrainingProgramCategory");
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.error = "Error Message: " + e.Message;
                return View(training);
            }
        }
        #endregion

        #region Training Program Admin

        public List<TrainingProgram> ObtainTrainingTable(int PositionId)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                ViewBag.PositionSelected = context.Position.Find(PositionId).PositionName;
                List<TrainingProgram> listTrainingPrograms = context.TrainingProgram.Include("TrainingProgramCategory")
                                                                                    .Where(x => x.Position == PositionId && x.Enable == true)
                                                                                    .OrderBy(m => m.TrainingProgramCategory.Name).ToList();

                return listTrainingPrograms;
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult TrainingProgramAdmin()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.PositionId = 0;
            ViewBag.AreaId = 0;

            if (Session["PositionId"] != null)
            {
                int AreaId = 0;
                int.TryParse(Session["AreaId"].ToString(), out AreaId);

                int PositionId = 0;
                int.TryParse(Session["PositionId"].ToString(), out PositionId);

                var lstTraining = ObtainTrainingTable(PositionId);

                ViewBag.AreaId = AreaId;
                ViewBag.PositionId = PositionId;

                Session["AreaId"] = null;
                Session["PositionId"] = null;

                return View(lstTraining);
            }

            Session["AreaId"] = null;
            Session["PositionId"] = null;

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult TrainingProgramAdmin(TrainingProgram program, int Area)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.PositionId = program.Position;
            ViewBag.AreaId = Area;

            Session["AreaId"] = Area;
            Session["PositionId"] = program.Position;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                int idPosition = program.Position;
                ViewBag.PositionSelected = context.Position.Find(idPosition).PositionName;
                List<TrainingProgram> listTrainingPrograms = context.TrainingProgram.Include("TrainingProgramCategory")
                                                                    .Where(x => x.Position == idPosition && x.Enable == true)
                                                                    .OrderBy(m => m.TrainingProgramCategory.Name).ToList();

                return View(listTrainingPrograms);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult CreateTrainingProgram()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            try
            {
                string ida = Request.QueryString["ida"];
                string idp = Request.QueryString["idp"];

                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                ViewBag.PositionId = idp;
                ViewBag.AreaId = ida;

                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    List<TrainingProgramCategory> listCategories = context.TrainingProgramCategory.OrderBy(m => m.Name).ToList();
                    ViewBag.ListOfCategories = from category in listCategories
                                               select new { Text = category.Name, Value = category.IdTrainingProgramCategory };
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                ViewBag.PositionId = 0;
                ViewBag.AreaId = 0;
            }

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult CreateTrainingProgram(TrainingProgram program, int Area, int Position)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);

            SetupCommonViewBagValues();
            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");

            Session["AreaId"] = Area;
            Session["PositionId"] = Position;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                program.Enable = true;
                context.TrainingProgram.Add(program);
                context.SaveChanges();

                int lastId = (from record in context.TrainingProgram orderby record.IdTrainingProgram descending select record.IdTrainingProgram).First();

                var EmployeeByPosition = context.Employee.Where(x => x.CurrentPositionID == Position).ToList();

                foreach (Employee item in EmployeeByPosition)
                {
                    TrainingProgramDetails tpd = new TrainingProgramDetails();

                    tpd.IdTrainingProgramDetails = 0;
                    tpd.IdTrainingProgram = lastId;
                    tpd.Status = 0;
                    tpd.ResourceId = item.ResourceId;

                    context.TrainingProgramDetails.Add(tpd);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("TrainingProgramAdmin");
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult EditTrainingProgram(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                SetupCommonViewBagValues();
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");

                TrainingProgram program = context.TrainingProgram
                    .Include("Position1")
                    .Include("TrainingProgramCategory")
                    .Where(x => x.IdTrainingProgram == id)
                    .FirstOrDefault();

                if (program.StartDate == null)
                {
                    program.StartDate = DateTime.MinValue;
                }

                if (program.FinishDate == null)
                {
                    program.FinishDate = DateTime.MinValue;
                }

                List<TrainingProgramCategory> listCategories = context.TrainingProgramCategory.OrderBy(m => m.Name).ToList();
                ViewBag.ListOfCategories = from category in listCategories
                                           select new { Text = category.Name, Value = category.IdTrainingProgramCategory };

                ViewBag.PositionId = program.Position;
                return View(program);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditTrainingProgram(TrainingProgram program)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();
            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");

            Session["AreaId"] = program.Position1.AreaId;
            Session["PositionId"] = program.Position;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                TrainingProgram instanceProgram = context.TrainingProgram.Find(program.IdTrainingProgram);
                instanceProgram.Link = program.Link;
                instanceProgram.Name = program.Name;
                //instanceProgram.Points = program.Points;
                instanceProgram.Category = program.Category;
                instanceProgram.Position = program.Position;
                instanceProgram.StartDate = program.StartDate;
                instanceProgram.FinishDate = program.FinishDate;
                context.SaveChanges();

                var lstIdTrainingProgramDetails = context.TrainingProgramDetails.Where(x => x.IdTrainingProgram == program.IdTrainingProgram).ToList();

                foreach (var item in lstIdTrainingProgramDetails)
                {
                    context.Entry(item).State = EntityState.Modified;
                    context.SaveChanges();
                }

                return RedirectToAction("TrainingProgramAdmin");
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult DeleteTrainingProgram(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                TrainingProgram program = context.TrainingProgram
                    .Include("Position1")
                    .Include("TrainingProgramCategory")
                    .Where(x => x.IdTrainingProgram == id)
                    .FirstOrDefault();

                Session["AreaId"] = program.Position1.AreaId;
                Session["PositionId"] = program.Position;

                return View(program);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult DeleteTrainingProgram(TrainingProgram program)
        {
            SetupCommonViewBagValues();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                ViewBag.PositionId = 0;
                ViewBag.AreaId = 0;

                TrainingProgram programToDelete = context.TrainingProgram.Find(program.IdTrainingProgram);
                programToDelete.Enable = false;
                context.Entry(programToDelete).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("TrainingProgramAdmin");
            }
        }
        #endregion

        #region Training Program On Demand Admin

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult TrainingProgramOnDemandAdmin()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<TrainingProgramOnDemand> lstTrainings = context.TrainingProgramOnDemand.Where(x => x.Enable == true)
                                                                                            .OrderBy(m => m.StartDate).ToList();

                return View(lstTrainings);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult TrainingProgramOnDemandAdmin(TrainingProgram program)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetProfileData(resource);
            SetupCommonViewBagValues();

            return RedirectToAction("CreateTrainingProgramOnDemand");
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult CreateTrainingProgramOnDemand()
        {
            SetupCommonViewBagValues();

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult CreateTrainingProgramOnDemand(TrainingProgramOnDemand tpod)
        {
            SetupCommonViewBagValues();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                if (ModelState.IsValid)
                {
                    tpod.Enable = true;
                    context.TrainingProgramOnDemand.Add(tpod);
                    context.SaveChanges();

                    return RedirectToAction("TrainingProgramOnDemandAdmin");
                }
                else
                {
                    ModelState.AddModelError("Error", "There is something wrong.");
                }
            }

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult EditTrainingProgramOnDemand(int id)
        {
            SetupCommonViewBagValues();

            try
            {
                CDPTrackEntities context = new CDPTrackEntities();
                TrainingProgramOnDemand tpod = context.TrainingProgramOnDemand.Find(id);

                return View(tpod);
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ModelState.AddModelError("Error", "There is something wrong.");

                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditTrainingProgramOnDemand(TrainingProgramOnDemand tpod)
        {
            SetupCommonViewBagValues();

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Entry(tpod).State = EntityState.Modified;
                    context.SaveChanges();

                    return RedirectToAction("TrainingProgramOnDemandAdmin");
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ModelState.AddModelError("Error", "There is something wrong.");
            }

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult DeleteTrainingProgramOnDemand(int id)
        {
            SetupCommonViewBagValues();

            try
            {
                CDPTrackEntities context = new CDPTrackEntities();
                TrainingProgramOnDemand tpod = context.TrainingProgramOnDemand.Find(id);

                return View(tpod);
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                ModelState.AddModelError("Error", "There is something wrong.");
            }

            return View();
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult DeleteTrainingProgramOnDemand(TrainingProgramOnDemand tpod)
        {
            SetupCommonViewBagValues();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                TrainingProgramOnDemand programToDelete = context.TrainingProgramOnDemand.Find(tpod.IdTrainingProgramOnDemand);
                programToDelete.Enable = false;
                context.Entry(programToDelete).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("TrainingProgramOnDemandAdmin");
            }
        }

        #endregion

        #region General Training Program Admin

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult GeneralTrainingProgramAdmin()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<GeneralTrainingProgram> allGeneralProgramsList = context.GeneralTrainingProgram.Include("TrainingProgramCategory")
                                                                             .Where(x => x.Enabled)
                                                                             .OrderBy(m => m.TrainingProgramCategory.Name).ToList();
                return View(allGeneralProgramsList);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult CreateGeneralTrainingProgram()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<TrainingProgramCategory> listCategories = context.TrainingProgramCategory.OrderBy(m => m.Name).ToList();
                ViewBag.ListOfCategories = from category in listCategories
                                           select new { Text = category.Name, Value = category.IdTrainingProgramCategory };

                return View();
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult CreateGeneralTrainingProgram(GeneralTrainingProgram generalProgram)
        {
            Resource profileData;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out profileData))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(profileData);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                generalProgram.Enabled = true;
                context.GeneralTrainingProgram.Add(generalProgram);
                context.SaveChanges();

                int idGeneralProgram = generalProgram.IdGeneralTrainingProgram;
                foreach (var resource in context.Resources)
                {

                    GeneralTrainingProgramDetails TrainingProgramDetail = new GeneralTrainingProgramDetails();
                    TrainingProgramDetail.IdGeneralTrainingProgram = idGeneralProgram;
                    TrainingProgramDetail.ResourceId = resource.ResourceId;
                    TrainingProgramDetail.Status = 0;

                    context.GeneralTrainingProgramDetails.Add(TrainingProgramDetail);
                }

                context.SaveChanges();
            }

            return RedirectToAction("GeneralTrainingProgramAdmin");
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult DeleteGeneralTrainingProgram(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                GeneralTrainingProgram generalTrainingProgram = context.GeneralTrainingProgram.Include("TrainingProgramCategory").Where(x => x.IdGeneralTrainingProgram == id).FirstOrDefault();
                return View(generalTrainingProgram);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult DeleteGeneralTrainingProgram(GeneralTrainingProgram program)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                GeneralTrainingProgram generalTP = context.GeneralTrainingProgram.Find(program.IdGeneralTrainingProgram);
                generalTP.Enabled = false;

                context.SaveChanges();

                return RedirectToAction("GeneralTrainingProgramAdmin");
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult EditGeneralTrainingProgram(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                GeneralTrainingProgram generalTrainingProgram = context.GeneralTrainingProgram.Include("TrainingProgramCategory").Where(x => x.IdGeneralTrainingProgram == id).FirstOrDefault();

                if (generalTrainingProgram.StartDate == null)
                {
                    generalTrainingProgram.StartDate = DateTime.MinValue;
                }

                if (generalTrainingProgram.FinishDate == null)
                {
                    generalTrainingProgram.FinishDate = DateTime.MinValue;
                }

                List<TrainingProgramCategory> listCategories = context.TrainingProgramCategory.OrderBy(m => m.Name).ToList();
                ViewBag.ListOfCategories = from category in listCategories
                                           select new { Text = category.Name, Value = category.IdTrainingProgramCategory };

                return View(generalTrainingProgram);
            }
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditGeneralTrainingProgram(GeneralTrainingProgram program)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                GeneralTrainingProgram generalTrainingProgram = context.GeneralTrainingProgram.Where(x => x.IdGeneralTrainingProgram == program.IdGeneralTrainingProgram).FirstOrDefault();
                generalTrainingProgram.Link = program.Link;
                generalTrainingProgram.Name = program.Name;
                //generalTrainingProgram.Points = program.Points;
                generalTrainingProgram.Category = program.Category;
                generalTrainingProgram.StartDate = program.StartDate;
                generalTrainingProgram.FinishDate = program.FinishDate;
                context.SaveChanges();

                return RedirectToAction("GeneralTrainingProgramAdmin");
            }
        }

        #endregion

        #region Skill Compass Glossary

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        //Skill compass Glossary Home for Admin
        public ActionResult SkillCompassGlossary()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.AreaId = 0;
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var fullGlossary = context.SkillCompassGlossary.Include("Area").OrderBy(x => x.Name).ToList();
                return View(fullGlossary);
            }
        }

        //when selecting an Area
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult SkillCompassGlossary(List<SkillCompassGlossary> position, int Area)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            try
            {
                ViewBag.AreaId = Area;
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", Area);

                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    // glossary by Area
                    var glossarybyArea = context.SkillCompassGlossary.Include("Area").Where(x => x.AreaId == Area).OrderBy(x => x.Name).ToList();
                    ViewBag.Area = glossarybyArea.FirstOrDefault().Area.Name.ToString();
                    position = glossarybyArea;
                }
            }
            catch (Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                position = null;
            }
            return View(position);
        }
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        // ActionResult for create new Skill on SCG (Skill Compass Glossary)
        public ActionResult CreateSkillCompassGlossary()
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
            ViewBag.error = "";
            return View();
        }
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        //ActionResult for Create new Skill on SCG with POST request
        [HttpPost]
        public ActionResult CreateSkillCompassGlossary(int Area, string Name, string Description)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            SkillCompassGlossary skillcompassglossary = new SkillCompassGlossary();
            skillcompassglossary.AreaId = Area;
            skillcompassglossary.Name = Name;
            skillcompassglossary.Description = Description;
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                if (ModelState.IsValid)
                {
                    context.SkillCompassGlossary.Add(skillcompassglossary);
                    context.SaveChanges();
                    return RedirectToAction("SkillCompassGlossary");
                }
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name");
                return View();
            }

        }
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        // Action Result for DELETE a Skill on SCG
        public ActionResult DeleteSkillCompassGlossary(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                SkillCompassGlossary skill = context.SkillCompassGlossary.Include("Area")
                                                .Where(x => x.SkillCompassGlossaryId == id).FirstOrDefault();
                return View(skill);
            }
        }
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        // Action Result Confirmed Post request
        [HttpPost, ActionName("DeleteSkillCompassGlossary")]
        public ActionResult DeleteSkillCompassGlossarConfirmed(int id)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                SkillCompassGlossary SCG = context.SkillCompassGlossary.Find(id);
                context.SkillCompassGlossary.Remove(SCG);
                context.SaveChanges();
            }
            return RedirectToAction("SkillCompassGlossary");
        }
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        // Action Result to Edit a Skill 
        public ActionResult EditSkillCompassGlossary(int id)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);

            SkillCompassGlossary SCG = new SkillCompassGlossary();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                SCG = context.SkillCompassGlossary.Find(id);
                ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", SCG.AreaId);
            }
            return View(SCG);
        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult EditSkillCompassGlossary(SkillCompassGlossary SCG)
        {
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            if (ModelState.IsValid)
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    context.Entry(SCG).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("SkillCompassGlossary");
            }
            ViewBag.ListOfAreas = new SelectList(GetArea(), "AreaId", "Name", SCG.AreaId);
            return View(SCG);
        }

        #endregion

        #region TDUReward Verification
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult RewardsVerification()
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var model = new RewardsVerificationModel();
                var groupedTDURewards = (from t in context.TDUReward
                                         where t.Redeemed == true && !(t.Paid == true) && t.DatetoLoseValidity >= DateTime.Now
                                         select t
                                             ).ToList();
                var resources = (from r in context.Resources
                                 join reward in context.TDUReward
                                 on r.ResourceId equals reward.resourceId
                                 select r).ToList();
                var allRewards = (from a in context.TDUReward
                                  select a).ToList();

                if (groupedTDURewards.Any() && groupedTDURewards != null)
                {
                    model.ValidRewards = groupedTDURewards.OrderBy(x => x.resourceId).ToList();
                    model.AllRewards = allRewards;
                    model.ValidResources = resources;
                    ViewBag.TotalTDUs = groupedTDURewards;
                    BaseViewBagSetup();
                    return View(model);
                }
                return Content("<script language='javascript' type='text/javascript'>alert('There are no rewards to be verified.'); location.href='Index'</script>");
            }
        }
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult RewardsVerification(RewardsVerificationModel model)
        {

            if (ModelState.IsValid)
            {
                bool isUpdated = false;
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var originalRewards = (from r in context.TDUReward
                                           select r).ToList();

                    var matchingRewards = new List<TDUReward>();
                    foreach (var reward in model.ValidRewards)
                    {
                        var match = originalRewards.Where(x => x.TDURewardId == reward.TDURewardId).ToList();
                        if (match.Any())
                        {
                            var matchBefore = match.FirstOrDefault();
                            match.FirstOrDefault().Paid = reward.Paid;
                            if (reward.DatePaid != null)
                            {
                                match.FirstOrDefault().DatePaid = reward.DatePaid;
                            }
                            matchingRewards.Add(match.FirstOrDefault());
                            //if the original reward has the same rewards as the modified one then allow the update
                            context.Entry(match.FirstOrDefault()).State = EntityState.Modified;
                            isUpdated = true;

                        }
                    }
                    if (isUpdated)
                    {
                        context.SaveChanges();
                        return Content("<script language='javascript' type='text/javascript'>alert('The changes have been saved successfully!'); location.href='Index'</script>");
                    }
                    else return Content("<script language='javascript' type='text/javascript'>alert('No changes to save.'); location.href='Index'</script>");
                }
            }
            else
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var groupedTDURewards = (from t in context.TDUReward
                                             where t.Redeemed == true && !(t.Paid == true) && t.DatetoLoseValidity >= DateTime.Now
                                             select t
                                             ).ToList();
                    var resources = (from r in context.Resources
                                     join reward in context.TDUReward
                                     on r.ResourceId equals reward.resourceId
                                     select r).ToList();
                    var allRewards = (from a in context.TDUReward
                                      select a).ToList();

                    if (groupedTDURewards.Any() && groupedTDURewards != null)
                    {
                        foreach (var reward in model.ValidRewards)
                        {
                            groupedTDURewards.Where(x => x.TDURewardId == reward.TDURewardId).ToList().FirstOrDefault().Paid = reward.Paid;
                        }

                        model.ValidRewards = groupedTDURewards.OrderBy(x => x.resourceId).ToList();
                        model.AllRewards = allRewards;
                        model.ValidResources = resources;
                        ViewBag.TotalTDUs = groupedTDURewards;
                        ViewBag.DatePaidNotValid = true;
                        BaseViewBagSetup();
                    }
                }
                return View(model);
            }


        }

        #endregion

        #region Tiempo Connect Sync
#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        public ActionResult TiempoConnectSync()
        {
            BaseViewBagSetup();

            var model = new TiempoConnectSyncModel();
            return View(model);

        }

#if !DEBUG
        [RoleAuthorization(Role.TalentManagement)]
#endif
        [HttpPost]
        public ActionResult TiempoConnectSync(TiempoConnectSyncModel model)
        {
            BaseViewBagSetup();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {

                System.IO.Directory.CreateDirectory(@"C:\inetpub\wwwroot\CDPTracker\TiempoConnectSync\");
                var file = model.File;
                if (ExistingFileConnectSync(file.FileName))
                {
                    ViewBag.FileExists = true;
                    return View(model);
                }
                var extension = Path.GetExtension(file.FileName);
                BufferedStream stream = new BufferedStream(file.InputStream);
                StreamReader sr = new StreamReader(stream);

                var csv = new CsvHelper.CsvReader(sr);
                var invalidRows = new List<string[]>();
                var validObjectives = new List<Objective>();

                var fileName = Path.GetFileName(file.FileName);
                var mime = file.ContentType;
                //var path = Path.Combine(Server.MapPath("~/App_Data/uploads/CSVTiempoConnect"), fileName);
                var pathName = Path.Combine(@"C:\inetpub\wwwroot\CDPTracker\TiempoConnectSync", fileName);
                file.SaveAs(pathName);

                ProcessCSVObjectives(csv, out validObjectives, out invalidRows);
                if (validObjectives.Any()) model.InsertedObjectives = validObjectives;
                if (invalidRows.Any()) model.RejectedObjectives = invalidRows;

                if (model.InsertedObjectives == null || model.InsertedObjectives.Count == 0)
                {
                    try
                    {
                        //if (System.IO.File.Exists(Server.MapPath("~/App_Data/uploads/CSVTiempoConnect/")+file.FileName))
                        if (System.IO.File.Exists(@"C:\inetpub\wwwroot\CDPTracker\TiempoConnectSync\" + file.FileName))
                        {
                            ViewBag.InvalidFile = true;
                            //System.IO.File.Delete(Server.MapPath("~/App_Data/uploads/CSVTiempoConnect/"+file.FileName));
                            System.IO.File.Delete(@"C:\inetpub\wwwroot\CDPTracker\TiempoConnectSync\" + file.FileName);

                        }
                    }
                    catch (Exception e)
                    {
                        ErrorLogHelper.LogException(e, "CDPTracker");
                        throw;
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                return RedirectToAction("Index");
            }
        }

        public bool ExistingFileConnectSync(string fileName)
        {
            //string[] csvFileNames = Directory.GetFiles(Server.MapPath("~/App_Data/uploads/CSVTiempoConnect"),"*.csv")
            //                                .Select(path => Path.GetFileName(path))
            //                                    .ToArray();
            System.IO.Directory.CreateDirectory(@"C:\inetpub\wwwroot\CDPTracker\TiempoConnectSync\");
            string[] csvFileNames = Directory.GetFiles(@"C:\inetpub\wwwroot\CDPTracker\TiempoConnectSync\", "*.csv")
                                            .Select(path => Path.GetFileName(path))
                                                .ToArray();
            if (csvFileNames.Contains(fileName)) return true;
            else return false;
        }

        public void ProcessCSVObjectives(CsvReader csv, out List<Objective> validObjectives, out List<String[]> invalidRows)
        {
            var validobjectives = new List<Objective>();
            invalidRows = new List<string[]>();
            var records = new List<TiempoConnectSyncRow>();
            validObjectives = new List<Objective>();

            Dictionary<string, int> categories = new Dictionary<string, int>()
            {
                    {"Areas, skills, and strenghts to develop",1},
                    {"Key activities to perform during this quarter",2},
                    {"Business needs",4},
                    {"How can my manager help?", 4}
            };
            using (csv)
            {
                var invalid = new List<string[]>();
                csv.Configuration.IgnoreReadingExceptions = true;
                csv.Configuration.TrimFields = true;
                csv.Configuration.TrimHeaders = true;
                csv.Configuration.ReadingExceptionCallback = (ex, row) =>
                {
                    var error = ex;
                    invalid.Add(row.CurrentRecord);
                };
                //Add the Class Mapping Details
                csv.Configuration.RegisterClassMap<CSVClassMap>();
                //Iterate through the csv file and add to list
                records = csv.GetRecords<TiempoConnectSyncRow>().ToList();
                invalidRows = invalid;
            };

            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    var resources = context.Resources.ToList();
                    foreach (var record in records)
                    {
                        var objective = new Objective();
                        var resource = resources.Where(x => x.ActiveDirectoryId == record.activeDirectoryID).FirstOrDefault();
                        if (resource == null)
                        {
                            invalidRows.Add(new string[]{Convert.ToString(record.activeDirectoryID),
                            record.employeeName,Convert.ToString(record.year),Convert.ToString(record.quarter),
                            record.typeObjective,record.objective});
                        }
                        else
                        {
                            objective.ResourceId = resource.ResourceId;
                            objective.Year = record.year;
                            objective.Quarter = record.quarter;
                            objective.Progress = 0;
                            objective.Duplicated = false;
                            if (categories.ContainsKey(record.typeObjective))
                            {
                                objective.CategoryId = categories[record.typeObjective];
                            }
                            else objective.CategoryId = 4;

                            objective.Objective1 = record.objective;
                            if (objective.CategoryId == 1)
                                validobjectives.Add(objective);
                            else
                            {
                                invalidRows.Add(new string[]{Convert.ToString(record.activeDirectoryID),
                            record.employeeName,Convert.ToString(record.year),Convert.ToString(record.quarter),
                            record.typeObjective,record.objective});
                            }
                        }

                    }
                    foreach (var validobjective in validobjectives)
                    {
                        validObjectives.Add(validobjective);
                        context.Objectives.Add(validobjective);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogException(ex, "CDPTracker");
                throw;
            }

        }
        #endregion

        #endregion

        /************************************************************Region for Action Results "Help Area"  **********************************/
        #region Help

        public ActionResult Help()
        {
            SetupCommonViewBagValues();
            return View();
        }

        public ActionResult About()
        {
            SetupCommonViewBagValues();
            return View();
        }

        #endregion

        /************************************************************Region for Action Results "COnfiguration Area"  **********************************/
        #region Configuration Module

        public ActionResult ConfigurationOptions(int? quarter, int? year)
        {
            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            // setting initial Viewbag data for the view
            InitialSettingForView(quarter, year, resource);

            return View();
        }

        public ActionResult ConfigurationAddSurvey(int? quarter, int? year, int? surveyType)
        {
            Resource resource;
            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            // setting initial Viewbag data for the view
            InitialSettingForView(quarter, year, resource);

            // in case the surveyType param is null just return data for team members input
            int surveyTypeCorrect = surveyType ?? TEAM_MEMBERS_INPUT;

            // return the all the available team members input surveys from the DB to be displayed
            var surveysFromDB = context.Survey.Where(x => x.SurveyType == surveyTypeCorrect).OrderBy(x => x.Year).ThenBy(x => x.Quarter);
            var lastSurveyFromDB = surveysFromDB.ToList().Last();

            // get available cycle options for new survey
            Tuple<int, int>[] quarters = fowardQuarterCalculation(4, lastSurveyFromDB.Quarter ?? GetActualQuarter(), lastSurveyFromDB.Year ?? GetCurrentYear());
            List<SelectListItem> listOfCycles = new List<SelectListItem>();
            foreach (Tuple<int, int> date in quarters)
            {
                listOfCycles.Add(new SelectListItem { Value = "Q" + date.Item1 + " " + date.Item2, Text = "Q" + date.Item1 + " " + date.Item2 });
            }

            // putting the model togheter
            List<object> model = new List<object>();
            model.Add(surveysFromDB);
            model.Add(listOfCycles);

            ViewBag.SurveyType = surveyTypeCorrect;
            ViewBag.SurveyText = surveyTypeCorrect == TEAM_MEMBERS_INPUT ? "Team Members Input" : "Manager Check";

            return View(model);
        }

        [HttpPost]
        public JsonResult AddNewSurvey(string data)
        {
            var newSurveyData = JsonConvert.DeserializeObject<NewSurveyModel>(data);

            // retrieve the new quarter
            int newQuarter;
            if (!int.TryParse(newSurveyData.Cycle.Substring(1, 1), out newQuarter))
            {
                return Json(new { success = false, message = "The new quarter value detected is not a valid int, the DB can't be updated with new Survey data" }, JsonRequestBehavior.AllowGet);
            }

            // retrieve the new year
            int newYear;
            if (!int.TryParse(newSurveyData.Cycle.Substring(newSurveyData.Cycle.Length - 4), out newYear))
            {
                return Json(new { success = false, message = "The new year value detected is not a valid int, the DB can't be updated with new Survey data" }, JsonRequestBehavior.AllowGet);
            }

            // retrieve last survey data available
            var lastSurveysFromDB = context.Survey.Where(x => x.SurveyType == newSurveyData.Type).OrderBy(x => x.Year).ThenBy(x => x.Quarter).ToList().Last();

            context.Database.ExecuteSqlCommand(@"EXECUTE [dbo].[usp_AddSurvey] 
                                                   @OldQuarter
                                                  ,@OldYear
                                                  ,@NewQuarter
                                                  ,@NewYear
                                                  ,@SurveyType
                                                  ,@EnabledDate",
                                                new SqlParameter("OldQuarter", lastSurveysFromDB.Quarter ?? GetActualQuarter()),
                                                new SqlParameter("OldYear", lastSurveysFromDB.Year ?? GetCurrentYear()),
                                                new SqlParameter("NewQuarter", newQuarter),
                                                new SqlParameter("NewYear", newYear),
                                                new SqlParameter("SurveyType", newSurveyData.Type),
                                                new SqlParameter("EnabledDate", newSurveyData.EnableDate)
            );

            
            return Json(new { success = true, message = "DB updated with new Survey data" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /************************************************************Region for Old Action Results **********************************/

        #region Old

        //
        // GET: /GoalTracking/
        //public ActionResult Index()
        //{
        //    Resource resource;

        //    ViewBag.Year = GetCurrentYear();
        //    ListOfYears();
        //    ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());

        //    if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
        //    {
        //        TempData[ResourceController.IsRedirectBycode] = true;
        //        return RedirectToAction("SetupResourceData", "Resource");
        //    }

        //resource.GoalTrackings = resource.GoalTrackings
        //                                 .OrderBy(goal => goal.FinishDate.HasValue ? goal.FinishDate.Value : DateTime.MinValue).ToList();

        //resource.Objective = resource.Objective.Where(obj => obj.Year == GetCurrentYear() && obj.Quarter == GetActualQuarter() || obj.Year == null || obj.Quarter == null)
        //                             .OrderBy(obj => obj.CategoryId).ToList();

        //ViewBag.PercentageCompleted = getPercentage(resource.ResourceId);
        //ViewBag.PercentageVerified = getPercentageVerified(resource.ResourceId);
        //ViewBag.ActiveDirectoryId = resource.ActiveDirectoryId;
        //ViewBag.Position = resource.Employee.CurrentPosition;
        //ViewBag.AspiringPosition = resource.Employee.AspiringPosition;
        //ViewBag.Name = resource.Name;
        //ViewBag.ResourceId = resource.ResourceId;

        //ViewBag.CurrentCycle = GetCurrentCycle();
        //ViewBag.TDUPointsDisplay = GetTduInQuarter(resource.ResourceId);

        //var ValidTDU = new List<TDURedeem>();
        //var tdureward = CalculateReward(resource, out ValidTDU);
        //if (tdureward != null)
        //{
        //    ViewBag.TDUReward = tdureward;
        //}
        //using (CDPTrackEntities context = new CDPTrackEntities())
        //{
        //    ViewBag.TrainingCategory = context.TrainingCategory.Select(x => x.Name).ToList();
        //}

        //SetupCommonViewBagValues();
        //#if DEBUG
        //            getTeammates(99);
        //#else
        //                    getTeammates(resource.ResourceId);
        //#endif
        //    return View(resource);
        //}

        //[HttpPost]
        //public ActionResult Index(Resource resource)
        //{
        //    int Year = GetCurrentYear();
        //    int.TryParse(Request.Form["Year"].ToString(), out Year);

        //    int Quarter = GetActualQuarter();
        //    int.TryParse(Request.Form["Quarter"].ToString(), out Quarter);

        //    if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
        //    {
        //        TempData[ResourceController.IsRedirectBycode] = true;
        //        return RedirectToAction("SetupResourceData", "Resource");
        //    }

        //    resource.GoalTrackings = resource.GoalTrackings
        //                                        .OrderBy(goal => goal.FinishDate.HasValue ? goal.FinishDate.Value : DateTime.MinValue).ToList();
        //    resource.Objective = resource.Objective.Where(obj => obj.Year == Year && obj.Quarter == Quarter)
        //                                    .OrderBy(obj => obj.CategoryId).ToList();
        //    ListOfYears();
        //    ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text");
        //    ViewBag.Year = Year;
        //    ViewBag.Quarter = Quarter;
        //    ViewBag.PercentageCompleted = getPercentage(resource.ResourceId);
        //    ViewBag.PercentageVerified = getPercentageVerified(resource.ResourceId);
        //    ViewBag.ActiveDirectoryId = resource.ActiveDirectoryId;
        //    ViewBag.Position = resource.Employee.CurrentPosition;
        //    ViewBag.AspiringPosition = resource.Employee.AspiringPosition;
        //    ViewBag.Name = resource.Name;
        //    ViewBag.ResourceId = resource.ResourceId;


        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        ViewBag.TrainingCategory = context.TrainingCategory.Select(x => x.Name).ToList();
        //    }
        //    SetupCommonViewBagValues();

        //    return View(resource);
        //}

        // GET: /GoalTracking/CreateObjective
        //public ActionResult CreateObjective(int id)
        //{
        //    SetupCommonViewBagValues();
        //    ViewBag.ListOfYears = new SelectList(ListOfYearsFuture(), "Value", "Text", GetCurrentYear());
        //    ViewBag.ListOfCategories = new SelectList(GetTrainingCategories(), "CategoryId", "Category1");
        //    ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());
        //    var model = new Objective { ResourceId = id };
        //    return View(model);
        //}

        // POST: /GoalTracking/CreateObjective
        //[HttpPost]
        //public ActionResult CreateObjective(Objective objective)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //Save Object
        //        using (CDPTrackEntities context = new CDPTrackEntities())
        //        {
        //            objective.Progress = 0;
        //            objective.Duplicated = false;
        //            context.Objectives.Add(objective);
        //            context.SaveChanges();
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    return View(objective);
        //}

        //public ActionResult Create(int id, int ObjectiveId)
        //{
        //    SetupCommonViewBagValues();
        //    ViewBag.ListOfSources2 = new SelectList(GetSources(), "SourceId", "Name");
        //    ViewBag.ListOfCategories2 = new SelectList(GetCategories(), "TrainingCategoryId", "Name");
        //    var model = new GoalTracking { ResourceId = id, ObjectiveId = ObjectiveId };
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        ViewBag.MaxTDU = context.TrainingCategory.Select(x => x.MaxTDU).ToList();
        //        ViewBag.TDU = context.TrainingCategory.Select(x => x.TDU).ToList();
        //        ViewBag.Description = context.TrainingCategory.Select(x => x.Description).ToList();
        //    }
        //    return View(model);
        //}

        //// POST: /GoalTracking/Create
        //[HttpPost]
        //public ActionResult Create(GoalTracking goalTracking)
        //{
        //    var category = Convert.ToInt16(Request.Form["trainingCategoryId"]);
        //    var TDUs = Convert.ToInt16(Request.Form["TDUs2"]);
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        goalTracking.TrainingCategoryId = category;
        //        goalTracking.LastUpdate = DateTime.Now;
        //        context.GoalTrackings.Add(goalTracking);
        //        if (goalTracking.SourceId == 0)
        //        {
        //            string newSource = Request.Form["source"].ToString();
        //            Sources sources = new Sources { Name = newSource };
        //            context.Sources.Add(sources);
        //            goalTracking.SourceId = sources.SourceId;
        //            context.SaveChanges();
        //        }
        //        goalTracking.TDU = ptRestrictions(category, goalTracking.ResourceId, TDUs, 0);
        //        if (goalTracking.TDU != TDUs)
        //        {
        //            context.SaveChanges();
        //            return Content("<script language='javascript' type='text/javascript'>alert('The maximum amount of TDUs for this category or quarter has been reached. The value of TDUs for this goal will be " + goalTracking.TDU.ToString() + ".'); location.href='Index'</script>");
        //        }
        //        else
        //        {
        //            context.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    ViewBag.Progress = new SelectList(GetProgresEnums(), "Id", "Label", goalTracking.Progress);
        //    return View(goalTracking);
        //}

        //// GET: /GoalTracking/EditObjective
        //public ActionResult EditObjective(int id)
        //{
        //    SetupCommonViewBagValues();

        //    Objective objective;
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        objective = context.Objectives.Find(id);
        //    }
        //    ViewBag.ListOfCategories = new SelectList(GetTrainingCategories(), "CategoryId", "Category1");
        //    ViewBag.ListOfQuarters = new SelectList(GetQuarter(), "Value", "Text", GetActualQuarter());
        //    ViewBag.Progress = new SelectList(GetProgresEnums(), "Id", "Label", objective.Progress);
        //    ViewBag.ListOfSources = new SelectList(GetSources(), "SourceId", "Name");
        //    ListOfYears();

        //    return View(objective);
        //}

        //// POST: /GoalTracking/EditObjective
        //[HttpPost]
        //public ActionResult EditObjective(Objective objective)
        //{
        //    SetupCommonViewBagValues();
        //    if (ModelState.IsValid)
        //    {
        //        using (CDPTrackEntities context = new CDPTrackEntities())
        //        {
        //            //Edit Object
        //            context.Entry(objective).State = EntityState.Modified;
        //            objective.Year = DateTime.Now.Year;
        //            objective.Quarter = GetQuarter(DateTime.Now.Month);
        //            objective.Duplicated = false;
        //            context.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    return View(objective);
        //}

        ////GET
        //public ActionResult CopyObjective(int id, int resourceid)
        //{
        //    SetupCommonViewBagValues();
        //    Objective objective;
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        context.Configuration.LazyLoadingEnabled = false;
        //        objective = context.Objectives.Include("GoalTracking").Include("Category").FirstOrDefault(r => r.ObjectiveId == id);
        //    }
        //    return View(objective);
        //}

        ////POST
        //[HttpPost, ActionName("CopyObjective")]
        //public ActionResult CopyObjectiveConfirmed(int id, int resourceid)
        //{
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        Objective objective = context.Objectives.Find(id);
        //        if (!Convert.ToBoolean(objective.Duplicated))
        //        {
        //            var goals = objective.GoalTracking.Where(x => x.Progress != 2 && !x.VerifiedByManager);
        //            objective.Duplicated = true;
        //            context.SaveChanges();
        //            Objective copiedObj = context.Objectives.Add(objective);
        //            copiedObj.Year = DateTime.Now.Year;
        //            copiedObj.Quarter = GetQuarter(DateTime.Now.Month);
        //            copiedObj.Progress = 0;
        //            var mTDUCategory = from trainingCategory in context.TrainingCategory
        //                               where trainingCategory.TrainingCategoryId == 6
        //                               select trainingCategory.TDU;
        //            int TDUCategory = 0;
        //            foreach (var p in mTDUCategory)
        //            {
        //                TDUCategory = Convert.ToInt16(p);
        //            }
        //            foreach (var item in goals)
        //            {
        //                item.FinishDate = DateTime.Now;
        //                item.ResourceId = resourceid;
        //                item.LastUpdate = DateTime.Now;
        //                item.ObjectiveId = copiedObj.ObjectiveId;
        //                item.TDU = TDUCategory;
        //                item.TrainingCategoryId = 6;
        //                context.GoalTrackings.Add(item);
        //            }
        //            try
        //            {
        //                context.SaveChanges();
        //            }
        //            catch
        //            {
        //                return RedirectToAction("Index");
        //            }
        //        }
        //        else
        //        {
        //            return Content("<script language='javascript' type='text/javascript'>alert('This objective has already been duplicated.'); location.href='Index'</script>");
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        //// GET: /GoalTracking/Edit/5
        //public ActionResult Edit(int GoalId, int ResourceId, int? TrainingCategoryId)
        //{
        //    SetupCommonViewBagValues();

        //    GoalTracking goalTracking;
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        goalTracking = context.GoalTrackings.Find(GoalId);
        //        ViewBag.ListOfObjectives = new SelectList(GetObjective(ResourceId), "ObjectiveId", "Objective1");
        //        ViewBag.Progress = new SelectList(GetProgresEnums(), "Id", "Label", goalTracking.Progress);
        //        ViewBag.ListOfCategories = new SelectList(GetCategories(), "TrainingCategoryId", "Name", goalTracking.TrainingCategoryId);
        //        ViewBag.ListOfSources = new SelectList(GetSources(), "SourceId", "Name", goalTracking.SourceId);
        //        var trainingCategory = context.TrainingCategory.Where(x => x.TrainingCategoryId == TrainingCategoryId || x.TrainingCategoryId == goalTracking.TrainingCategoryId);
        //        if (goalTracking.TrainingCategoryId != null)
        //        {
        //            ViewBag.MaxTDU = context.TrainingCategory.Select(x => x.MaxTDU).ToList();
        //            ViewBag.TDU = context.TrainingCategory.Select(x => x.TDU).ToList();
        //            ViewBag.Description = context.TrainingCategory.Select(x => x.Description).ToList();
        //            ViewBag.TrainingCategoryId = goalTracking.TrainingCategoryId;
        //        }

        //    }
        //    return View(goalTracking);
        //}

        //// POST: /GoalTracking/Edit/5
        //[HttpPost]
        //public ActionResult Edit(GoalTracking goalTracking)
        //{
        //    var category = Convert.ToInt16(Request.Form["trainingCategory"]);
        //    int TDUs = Convert.ToInt32(Request.Form["TDUs"]);
        //    if (ModelState.IsValid)
        //    {
        //        goalTracking.LastUpdate = DateTime.Now;
        //        using (CDPTrackEntities context = new CDPTrackEntities())
        //        {
        //            context.Entry(goalTracking).State = EntityState.Modified;
        //            if (goalTracking.SourceId == 0)
        //            {
        //                string newSource = Request.Form["source"].ToString();
        //                Sources sources = new Sources { Name = newSource };
        //                context.Sources.Add(sources);
        //                goalTracking.SourceId = sources.SourceId;
        //                context.SaveChanges();
        //            }
        //            var TDU = from goal in context.GoalTrackings
        //                      where goal.GoalId == goalTracking.GoalId && goal.ResourceId == goalTracking.ResourceId
        //                      select goal.TDU;

        //            int cTDU = 0;
        //            foreach (var f in TDU)
        //            {
        //                cTDU += Convert.ToInt16(f);
        //            }
        //            var originalCategory = from goal in context.GoalTrackings
        //                                   where goal.GoalId == goalTracking.GoalId && goal.ResourceId == goalTracking.ResourceId
        //                                   select goal.TrainingCategoryId;
        //            int? orgCategory = originalCategory.Sum();
        //            var trainingCategory = goalTracking.TrainingCategoryId;
        //            if (orgCategory != trainingCategory)
        //            {
        //                goalTracking.TDU = ptRestrictions(Convert.ToInt16(goalTracking.TrainingCategoryId), goalTracking.ResourceId, TDUs, cTDU, true);
        //            }
        //            else
        //            {
        //                goalTracking.TDU = ptRestrictions(Convert.ToInt16(goalTracking.TrainingCategoryId), goalTracking.ResourceId, TDUs, cTDU);
        //            }

        //            if (goalTracking.TDU != TDUs)
        //            {
        //                context.SaveChanges();
        //                return Content("<script language='javascript' type='text/javascript'>alert('The maximum amount of TDUs for this category or quarter has been reached. The value of TDUs for this goal will be " + goalTracking.TDU.ToString() + ".'); location.href='Index'</script>");
        //            }
        //            else
        //            {
        //                context.SaveChanges();
        //                return RedirectToAction("Index");
        //            }
        //        }
        //    }

        //    ViewBag.Progress = new SelectList(GetProgresEnums(), "Id", "Label", goalTracking.Progress);
        //    return View(goalTracking);
        //}

        //public ActionResult RedeemTDUReward(int id)
        //{
        //    TDUReward reward = new TDUReward();
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        bool is45TDU = false;
        //        bool emailSent;
        //        reward = context.TDUReward.Find(id);
        //        if (reward != null)
        //        {
        //            if (reward.Redeemed || reward.Paid)
        //            {
        //                if (reward.Paid)
        //                {
        //                    return Content("<script language='javascript' type='text/javascript'>alert('Reward was already paid!.'); location.href='Index'</script>");

        //                }
        //                else return Content("<script language='javascript' type='text/javascript'>alert('Reward was already Redeemed. Please wait for contact by TM.'); location.href='Index'</script>");
        //            }
        //            //if (reward.TotalTDUReward == 45)
        //            //{
        //            //    is45TDU = true;
        //            //    reward.TotalTDUReward = 30;
        //            //    reward.ValidForQuarters = 1;
        //            //    if (reward.EndingQuarter == 1)
        //            //    {
        //            //        reward.EndingQuarter = 4;
        //            //        reward.EndingYear -= 1;
        //            //    }
        //            //    else
        //            //    {
        //            //        reward.EndingQuarter -= 1;
        //            //    }
        //            //}
        //            //else 
        //            if (reward.TotalTDUReward == 15) return Content("<script language='javascript' type='text/javascript'>alert('15 TDU Reward is not Redeemable!'); location.href='Index'</script>");
        //            reward.DateRedeemed = DateTime.Now;
        //            reward.Redeemed = true;
        //            emailSent = SendEmailTDURedeem(reward, GetResource(reward.resourceId));
        //            if (!emailSent) return Content("<script language='javascript' type='text/javascript'>alert('Could not send Email, please try again later!'); location.href='Index'</script>");

        //            context.Entry(reward).State = EntityState.Modified;
        //            try
        //            {
        //                //Edit all TDURedeem Related to the Reward and set them as Redeemed
        //                var tduRedeem = context.TDURedeem.Where(x => x.TDUReward == reward.TDURewardId).OrderBy(e => e.QuarterYear).ThenBy(e => e.Quarter);
        //                if (tduRedeem.Any())
        //                {
        //                    var queueTDURedeem = new Stack<TDURedeem>(tduRedeem.ToList());
        //                    for (int i = 0; i < tduRedeem.Count(); i++)
        //                    {
        //                        var tduredeem = queueTDURedeem.Pop();
        //                        if (i == 0 && is45TDU)
        //                        {
        //                            tduredeem.TDUReward = null;
        //                            tduredeem.DateRedeemed = null;
        //                            tduredeem.Redeemed = false;
        //                        }
        //                        else
        //                        {
        //                            tduredeem.Redeemed = true;
        //                            tduredeem.DateRedeemed = DateTime.Now;
        //                        }
        //                        context.Entry(tduredeem).State = EntityState.Modified;
        //                    }
        //                }
        //                context.SaveChanges();
        //                return Content("<script language='javascript' type='text/javascript'>alert('Congratulations! Your reward request has been send to TM, please check your email for email Confirmation.'); location.href='Index'</script>");
        //            }
        //            catch (Exception)
        //            {
        //                return Content("<script language='javascript' type='text/javascript'>alert('Could not send Email please try again later!'); location.href='Index'</script>");
        //            }
        //        }
        //        return Content("<script language='javascript' type='text/javascript'>alert('Could not find the Corresponding Reward.'); location.href='Index'</script>");
        //    }
        //}

        //public PartialViewResult TDUPointsDisplay(int resourceId)
        //{
        //    var model = new TDUPointsDisplayModel();
        //    model.resourceId = resourceId;
        //    var resource = GetResource(resourceId);

        //    model.missingPoints = 15;
        //    model.potentialPoints = 0;
        //    model.quarter = GetActualQuarter();
        //    var listRewards = new List<TDURedeem>();
        //    var reward = CalculateReward(resource, out listRewards);
        //    if (reward != null && reward.DatetoLoseValidity > DateTime.Now) model.TDURewards = reward;
        //    else model.TDURewards = null;
        //    var listofTDUs = new List<TDURedeem>();
        //    model.verifiedPoints = GetValidTDUQuarter(resourceId);
        //    model.potentialPoints = GetToBeCompletedQuarterTDUs(resourceId);
        //    if (model.verifiedPoints != null)
        //    {
        //        model.missingPoints = 15 - (int)model.verifiedPoints;
        //    }
        //    else
        //    {
        //        model.verifiedPoints = 0;
        //    }
        //    return PartialView(model);
        //}

        //public PartialViewResult TDUTrophyDisplay(int resourceId)
        //{
        //    var resource = GetResource(resourceId);
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        var tdureward = (from r in context.TDUReward
        //                         where r.Paid != true && r.DatetoLoseValidity > DateTime.Now
        //                         orderby r.EndingYear descending, r.EndingQuarter descending
        //                         select r).ToList();
        //        if (tdureward.Any()) return PartialView(tdureward.First());
        //        else return PartialView(null);
        //    }
        //}

        //public PartialViewResult TDUButtonDisplay(int resourceId)
        //{
        //    var resource = GetResource(resourceId);
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        var tdureward = (from r in context.TDUReward
        //                         where r.Paid != true && r.Redeemed != true && r.DatetoLoseValidity > DateTime.Now
        //                         orderby r.StartingYear ascending, r.StartingQuarter ascending
        //                         select r).ToList();
        //        if (tdureward.Any()) return PartialView(tdureward.First());
        //        else return PartialView(null);
        //    }
        //}

        //// GET: /GoalTracking/DeleteObjective
        //public ActionResult DeleteObjective(int id)
        //{
        //    SetupCommonViewBagValues();

        //    Objective objective;
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        context.Configuration.LazyLoadingEnabled = false;
        //        objective = context.Objectives.Include("GoalTracking").Include("Category").FirstOrDefault(r => r.ObjectiveId == id);
        //    }

        //    return View(objective);
        //}

        //// POST: /GoalTracking/DeleteObjective/
        //[HttpPost, ActionName("DeleteObjective")]
        //public ActionResult DeleteObjectiveConfirmed(int id)
        //{
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        Objective objective = context.Objectives.Find(id);

        //        if (objective.GoalTracking.Count == 0)
        //        {
        //            context.Objectives.Remove(objective);
        //            context.SaveChanges();
        //        }

        //        return RedirectToAction("Index");
        //    }
        //}

        //// GET: /GoalTracking/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    SetupCommonViewBagValues();

        //    GoalTracking goalTracking;
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        goalTracking = context.GoalTrackings.Include("ProgressEnum").FirstOrDefault(g => g.GoalId == id);
        //    }
        //    return View(goalTracking);
        //}

        //// POST: /GoalTracking/Delete/5
        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    using (CDPTrackEntities context = new CDPTrackEntities())
        //    {
        //        GoalTracking goalTracking = context.GoalTrackings.Find(id);
        //        context.GoalTrackings.Remove(goalTracking);
        //        context.SaveChanges();
        //    }
        //    return RedirectToAction("Index");
        //}
        #endregion



        //MANAGER VIEWS

        public ActionResult MenuManager()
        {
            return View("Manager/Menu");
        }

        public ActionResult GridManagerEmployee()
        {
            /*AP - We could improve this process working with filters and session objets 
             ------------------------------------------------------------------------*/
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            ListOfYears();

            //----------------------------------------------------------------------

            return View("Manager/GridEmployee");
        }

        public ActionResult ManagerView()
        {
            /*AP - We could improve this process working with filters and session objets 
             ------------------------------------------------------------------------*/
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            ListOfYears();

            //----------------------------------------------------------------------
            return View("Manager/ManagerView");
        }

        public ActionResult OwnerView()
        {
            /*AP - We could improve this process working with filters and session objets 
             ------------------------------------------------------------------------*/
            Resource resource;

            if (!ResourceDataAccessor.TryGetResourceByUserName(User.Identity.Name, out resource))
            {
                TempData[ResourceController.IsRedirectBycode] = true;
                return RedirectToAction("SetupResourceData", "Resource");
            }

            SetupCommonViewBagValues();
            SetProfileData(resource);
            ListOfYears();

            //----------------------------------------------------------------------
            return View("Manager/OwnerView");
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UploadHTMLCode(string data)
        {
            try
            {
                var m = JsonConvert.DeserializeObject<Html2PdfModel>(data);
                TempData["DashboardHTML"] = m.html2PDF;
                TempData["Dashboard_Filename"] = m.Name;
            }catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "The HTML code is stored in the controller" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadDashboardPDF()
        {
            string html = (string)TempData["DashboardHTML"];
            string fileName = (string)TempData["Dashboard_Filename"];
            Dictionary<string, string> files = new Dictionary<string, string>();    //assuming the filename is the key and is uninque.
            files.Add(fileName, html);
            return new zipCreatorOutOfHtmlCode("Dashboard.zip", files);
        }

        public ActionResult getZip()
        {
            var action = new ZipResultFromByteArray();     
            action.Context = context;
            return action;
        }

        public class ZipResultFromByteArray : ActionResult
        {

            GraphicsServerController graphicController = new GraphicsServerController();

            CDPTrackEntities _context;
            List<string> _quarterLabels;


            public CDPTrackEntities Context
            {
                get { return _context; }
                set { _context = value; }
            } 
            
            public override void ExecuteResult(ControllerContext context)
            {

                 var zip = GetList();

                var response = context.HttpContext.Response;
                response.ContentType = "application/gzip";

                zip.Save(response.OutputStream);
                var cd = GetCd();
                response.Headers.Add("Content-Disposition", cd.ToString());
            }

            private ContentDisposition GetCd()
            {
                return new ContentDisposition
                {
                    FileName = "ZipFile.zip",
                    Inline = false
                };
            }


            public ZipFile GetList()
            {
                
                var listResources = (from g in _context.Resources 
                                select g).ToList();
                using (var zip = new ZipFile())
                {
                    foreach (var item in listResources.Where(x=> x.IsEnable == 1))
                    {
                        GetSurveys(item.ResourceId);
                        GetManagerCheck(item.ResourceId);
                        var html = new StringBuilder();
                        _quarterLabels.Reverse();
                        graphicController.QuarterLabels = _quarterLabels;
                        var mainGraphic = graphicController.FillMainGraphic(item.ResourceId);
                        if (!String.IsNullOrEmpty(mainGraphic))
                        {
                            html.Append(
                                graphicController.CreateHtmlMain("Employee Name " + item.DomainName,
                                    graphicController.CreateHtmlElement(mainGraphic)
                                )
                             );
                             html.Append(graphicController.CreateElementTrends(graphicController.FillTrends(item.ResourceId)));
                               html.Append(graphicController.CreateHtmlElementsComments(GetEmployeeComents(item.ResourceId), true));
                            html.Append(graphicController.CreateHtmlElementsComments( GetManagerComments(item.ResourceId), false));
                            html.Append(graphicController.CreateEndMainTags());
                            zip.AddEntry(String.Format("{0}_{1}.pdf", item.ActiveDirectoryId, item.DomainName), html2pdf.memoryPDF(html.ToString()));
                        }
                    }
                    return zip;
                } 
            }
  
            public void GetSurveys (int resourceId)
            {
                List<SurveyResponseAndIdentifierModel> responseHistorical = new List<SurveyResponseAndIdentifierModel>();
                Tuple<int, int>[] quarterValues = quarterCalculation(4);

                foreach (Tuple<int, int> date in quarterValues)
                {
                    SurveyResponseAndIdentifierModel response = new SurveyResponseAndIdentifierModel();
                    response.Identifier = "Q" + date.Item1.ToString() + " " + date.Item2.ToString();
                    response.SurveyResponse = new List<SurveyResponseReport>();
                    var _query = string.Format(@"Select SurveyId, Quarter, Text, ResponseId, ResourceId from VW_MembersInputReport
                                                    Where QuarterId = {0} and year = {1} and ResourceId = {2}", date.Item1, date.Item2, resourceId);
                    response.SurveyResponse = _context.Database.SqlQuery<SurveyResponseReport>(_query).ToList();
                    if (response.SurveyResponse.Count > 0)
                        responseHistorical.Add(response);
                }
                graphicController.ListResponsesByResource = responseHistorical;
            }

            private void GetManagerCheck (int resourceId)
            {

                  

                Tuple<int, int>[] quarterValues = quarterCalculation(4);
                _quarterLabels = new List<string>();
                graphicController.ListResponsesManagerCheck = new List<List<ManagerCheck>>();
                foreach (Tuple<int, int> date in quarterValues)
                {
                    _quarterLabels.Add("Q" + date.Item1.ToString() + " " + date.Item2.ToString());

                    var _query = string.Format(@"Select Quarter, Text, ResourceId, ResponseId, ResponseText, ResourceEvaluatedId
                            FROM VW_ManagerCheckAnswersReport 
                                WHERE QuarterId = {0} and year = {1} and ResourceEvaluatedId = {2} order by QuestionId ", 
                                date.Item1, date.Item2, resourceId);

                    var sr = _context.Database.SqlQuery<ManagerCheck>(_query).ToList();  
                    if (sr.Count > 0)
                        graphicController.ListResponsesManagerCheck.Add(sr);
                    else
                        graphicController.ListResponsesManagerCheck.Add(new List<ManagerCheck>()); //add empty set for display purposes
                }
            }

            private string GetManagerComments(int resourceId)
            {
                List<ManagerComment> sr = new List<ManagerComment>();
                    //last quarter answered
                    sr = _context.Database.SqlQuery<ManagerComment>(@"SELECT top 1 ResponseText, ResourceId, ResourceEvaluatedId FROM (
                                                                SELECT sres.DateAnswered, sr.ResponseText, sres.ResourceId, sres.ResourceEvaluatedId 
                                                                FROM Question q
                                                                INNER JOIN Survey s ON q.SurveyId = s.SurveyId 
                                                                INNER JOIN  SurveyResponse sr ON sr.QuestionId = q.QuestionId 
                                                                INNER JOIN SurveyResource sres ON sres.SurveyResourceId = sr.SurveyResourceId AND sres.ResourceEvaluatedId = " + resourceId + " WHERE q.QuestionId = 39 ) as A ORDER BY A.DateAnswered DESC").ToList();
                if (sr.FirstOrDefault() != null)
                    return !String.IsNullOrEmpty(sr.FirstOrDefault().ResponseText) ? sr.FirstOrDefault().ResponseText : string.Empty;
                else
                    return string.Empty;
            }

            private string GetEmployeeComents(int resourceId)
            { 
                List<EmployeeComment> sr = new List<EmployeeComment>();

                
                    sr = _context.Database.SqlQuery<EmployeeComment>(@"SELECT top 1 ResponseText, ResourceId FROM (
                                                                SELECT sres.DateAnswered, sr.ResponseText, sres.ResourceId, sres.ResourceEvaluatedId 
                                                                FROM Question q
                                                                INNER JOIN Survey s ON q.SurveyId = s.SurveyId INNER JOIN  SurveyResponse sr ON sr.QuestionId = q.QuestionId  INNER JOIN SurveyResource sres ON sres.SurveyResourceId = sr.SurveyResourceId AND sres.ResourceId = " + resourceId + " WHERE q.QuestionId IN (10, 29) ) as A ORDER BY A.DateAnswered DESC").ToList();
                if (sr.FirstOrDefault() != null)
                    return !String.IsNullOrEmpty(sr.FirstOrDefault().ResponseText) ? sr.FirstOrDefault().ResponseText : string.Empty;
                else
                    return string.Empty;
            }

            private Tuple<int, int>[] quarterCalculation(int numberOfPeriods)
            {
                Tuple<int, int>[] data = new Tuple<int, int>[numberOfPeriods];
                int quarter = GetActualQuarter();
                int year = GetCurrentYear();

                for (int i = 0; i < numberOfPeriods; i++)
                {
                    data[i] = new Tuple<int, int>(quarter, year);
                    if (quarter == 1)
                    {
                        quarter = 4;
                        year--;
                    }
                    else
                        quarter--;
                }
                return data;
            }
        }

    }
}