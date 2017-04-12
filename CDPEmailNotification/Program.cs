using DataSource;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Utils;




#if !DEBUG
using System.Threading;
#endif


namespace CDPEmailNotification
{
    class EmailNotifications
    {
        private static readonly TimeSpan WaitTimeBetweenEmail = TimeSpan.FromSeconds(3);
        private static List<string> peopleWithoutPosition = new List<string>();
        private static List<string> peopleWithoutManager = new List<string>();
        private static List<string> peopleWithoutId = new List<string>();
        private static List<string> peopleWithoutEmail = new List<string>();
        private static List<string> peopleWithoutType = new List<string>();
        private static List<string> peopleWithoutArea = new List<string>();
        private static List<string> peopleWithoutProject = new List<string>();
        private static EmployeeActiveDirectoryManager activeDirectoryInstance;
        readonly static IEnumerable<string> ExclusionList = new List<string>
                {
                    "Jeff Jumpe",
                    "Cliff Schertz",
                    "Maria Pena",
                    "ITSUPPORT"
                };

        #region Method Main
        static void Main(string[] args)
        {

            try
            {
                //get the goals that will be used for  methods below.
                // ReSharper disable PossibleMultipleEnumeration

            
                /***************************/
                /*         FUNCTIONS        /
                /***************************/

                //Arguments Available
                string updateArgument = "update";
                string updateNArgument = "updatenotification";
                string emailArgument = "emails";
                string reportArgument = "reports";
                bool validArgument  =false;
                if (args.Any(x => x.Equals(updateArgument)) || args.Count() == 0)
                {
                    Console.WriteLine("Update");
                   
                    DeleteOldGoalsAndObjectives();
                    AddNewEmployeesFromAD();
                    UpdateEmployeeData();

                    addNewGroups();
                    UpdateEmployeeGroups();
                    validArgument = true;
                }
                //regresar
                var allGoals = GetGoals();
                if (allGoals == null) return;


                if(args.Any(x => x.Equals(updateNArgument)) || args.Count() == 0)
                {
                    Console.WriteLine("Update Notification");
                    DeleteOldGoalsAndObjectives();
                    AddNewEmployeesFromAD();
                    UpdateEmployeeData();
                    SendMailWithPeopleWithoutPositionInAD(peopleWithoutPosition.OrderBy(x => x).Distinct().ToList());
                    SendMailWithPeopleWithoutManagerInAD(peopleWithoutManager.OrderBy(x => x).Distinct().ToList());
                    SendMailWithPeopleWithoutIdInAD(peopleWithoutId.OrderBy(x => x).Distinct().ToList());
                    SendMailWithPeopleWithoutEmailInAD(peopleWithoutEmail.OrderBy(x => x).Distinct().ToList());
                    SendMailWithPeopleWithoutTypeInAD(peopleWithoutType.OrderBy(x => x).Distinct().ToList());
                    SendMailWithPeopleWithoutAreaInAD(peopleWithoutArea.OrderBy(x => x).Distinct().ToList());
                    validArgument = true;
                }
                if (args.Any(x => x.Equals(emailArgument)) || args.Count() == 0)
                {
                    Console.WriteLine("Emails");
                    SendEmployeesProgressNotifications(allGoals);
                    //SendManagersNotifications(allGoals);
                    //SendManagersCompletedNotifications(allGoals);
                    validArgument = true;
                }
                if (args.Any(x => x.Equals(reportArgument)) || args.Count() == 0)
                {
                    Console.WriteLine("Reports");
                    composeEmailForKPI();
                    validArgument = true;
                }
                if (!validArgument)
                {
                    Console.WriteLine("**********************");
                    Console.WriteLine("Arguments available:");
                    Console.WriteLine("update --->Update positions from AD, update Locations from AD and Delete old goals and objectives");
                    Console.WriteLine("updatenotification --->Update database from active directory information and send email to systems department");
                    Console.WriteLine("emails --->Send all the emails about notifications");
                    Console.WriteLine("reports --->Generate and send all the reports");
                    Console.WriteLine();
                }
                else {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write(":::::::......JOB COMPLETED......::::::");
                    Console.ResetColor();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error ocurred: {0}", e);
            }
        }
        #endregion

        #region Methods For Sending Emails or Notifications



        private static void SendManagersNotifications(IEnumerable<ResourceGoals> allGoals)
        {
            var goalsGroupedByManager = from goal in allGoals
                                        group goal by new { goal.ManagerName, goal.ManagerEmail }
                                            into grouped
                                            select grouped;
            foreach (var managerTeamGoals in goalsGroupedByManager)
            {
                int expectedActionsCount, completedActionsCount;

                //if the manager name is in the exclusion list then do not send it an email
                if (ExclusionList.Any(e => e.IndexOf(managerTeamGoals.Key.ManagerName, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    continue;

#if DEBUG
                const string managerEmail = "claredo@tiempodevelopment.com";
#else
                string managerEmail = managerTeamGoals.Key.ManagerEmail;
#endif
                if (string.IsNullOrWhiteSpace(managerEmail))
                {
                    Console.WriteLine("ERROR: Manager {0} does not have an email specified in AD.", managerTeamGoals.Key.ManagerName);
                }

                //get percentage of manager
                GetPreviousPercentageForManager(managerTeamGoals, out expectedActionsCount, out completedActionsCount);

                //get the pending actions results of the employees of the manager
                var pendingResultsCurrentMonth = GetPendingActionsOfMonth(managerTeamGoals);

                //get the pending actions results of the employees of the manager
                var pendingResultsNextMonth = GetPendingActionsOfNextMonth(managerTeamGoals);

                //send email
                int attemptsCounter = 0;
                string emailBody = ComposeEmailForManagers(managerTeamGoals.Key.ManagerName, expectedActionsCount, completedActionsCount,
                                                       pendingResultsCurrentMonth, pendingResultsNextMonth);

                while (true)
                {
                    bool emailSent = SendEmail(managerEmail, emailBody, string.Empty);
                    if (emailSent) break;

#if !DEBUG
                    Thread.Sleep(WaitTimeBetweenEmail);
#endif
                    attemptsCounter++;
                    if (attemptsCounter >= 3) //3 Failed Attempts to Send the Email, 1 hour break
                    {
                        Console.WriteLine("3 Failed Attempts to Send the Email to " + managerTeamGoals.Key);
                        Console.WriteLine("Taking a 1 hour break for the next try...");
#if !DEBUG
                        Thread.Sleep(TimeSpan.FromHours(1));
#endif
                    }
                }
#if !DEBUG
                Thread.Sleep(WaitTimeBetweenEmail);
#endif
            }
        }

        private static void SendManagersCompletedNotifications(IEnumerable<ResourceGoals> allGoals)
        {
            StringBuilder employeesPendingToApprove = new StringBuilder();
            int goalsPendingToApprove;
            var goalsGroupedByManager = from goal in allGoals
                                        group goal by new { goal.ManagerName, goal.ManagerEmail }
                                            into grouped
                                            select grouped;



            foreach (var managerTeamGoals in goalsGroupedByManager)
            {
                employeesPendingToApprove.Clear();
                //if the manager name is in the exclusion list then do not send it an email
                if (ExclusionList.Any(e => e.IndexOf(managerTeamGoals.Key.ManagerName, StringComparison.InvariantCultureIgnoreCase) >= 0))
                    continue;

#if DEBUG
                //const string managerEmail = "jcuamea@tiempodevelopment.com";
                //const string managerEmail = "fwirichaga@tiempodevelopment.com";
                const string managerEmail = "claredo@tiempodevelopment.com";
#else
                string managerEmail = managerTeamGoals.Key.ManagerEmail;
#endif
                if (string.IsNullOrWhiteSpace(managerEmail))
                {
                    Console.WriteLine("ERROR: Manager {0} does not have an email specified in AD.", managerTeamGoals.Key.ManagerName);
                    continue;
                }

                foreach (var employeeTeamGoals in managerTeamGoals)
                {
                    goalsPendingToApprove = 0;
                    if (employeeTeamGoals.Goals.Count > 0)
                    {
                        foreach (var employeeGoals in employeeTeamGoals.Goals)
                        {
                            if (employeeGoals.Progress == 2)
                            {
                                goalsPendingToApprove++;
                            }
                        }
                        if (goalsPendingToApprove > 0)
                        {
                            employeesPendingToApprove.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-" + employeeTeamGoals.Name + "<BR><BR>");
                        }
                    }
                }
                if (employeesPendingToApprove.Length > 0)
                {
                    //send email
                    int attemptsCounter = 0;
                    //string employee = "Jesus Cuamea";
                    string emailBody = ComposeEmailCompletedNotificationForManagers(managerTeamGoals.Key.ManagerName, employeesPendingToApprove);

                    while (true)
                    {
                        bool emailSent = SendEmail(managerEmail, emailBody, string.Empty);
                        if (emailSent) break;

#if !DEBUG
                        Thread.Sleep(WaitTimeBetweenEmail);
#endif
                        attemptsCounter++;
                        if (attemptsCounter >= 3) //3 Failed Attempts to Send the Email, 1 hour break
                        {
                            Console.WriteLine("3 Failed Attempts to Send the Email to " + managerTeamGoals.Key);
                            Console.WriteLine("Taking a 1 hour break for the next try...");
#if !DEBUG
                            Thread.Sleep(TimeSpan.FromHours(1));
#endif
                        }
                    }
                }
            }
#if !DEBUG
            Thread.Sleep(WaitTimeBetweenEmail);
#endif
        }

        private static void SendEmployeesProgressNotifications(IEnumerable<ResourceGoals> allGoals)
        {
            foreach (ResourceGoals personalGoals in allGoals)
            {
                //if (personalGoals.DomainName == "")
                //{
                    if (personalGoals.Goals == null || !personalGoals.IsRegisteredUser)
                        continue;

                    if (ExclusionList.Any(x => x == personalGoals.DomainName))
                        continue;

#if DEBUG
                    //const string managerEmail = "jcuamea@tiempodevelopment.com";
                    const string email = "claredo@tiempodevelopment.com";
#else
                string email = personalGoals.Email;
#endif
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        Console.WriteLine("ERROR: {0} does not have an email specified in AD.", personalGoals.Name);
                        continue;
                    }


                    int expectedActionsCount, completedActionsCount;
                    List<GoalTracking> goalsCompleted;
                    GetPreviousPercentage(personalGoals, out expectedActionsCount, out completedActionsCount, out goalsCompleted);

                    //get the goals of the month
                    var currentMonthGoals = GetCurrentMonthGoals(personalGoals);

                    //get the goals of the next month
                    var nextMonthGoals = GetNextMonthGoals(personalGoals);

                    var obj = GetObjectives(personalGoals);

                    //send email
                    int attemptsCounter = 0;

                    string emailBody = ComposeEmail(personalGoals.Name, expectedActionsCount, completedActionsCount,
                                                    currentMonthGoals, obj, nextMonthGoals, goalsCompleted, personalGoals.DomainName);

                    while (true)
                    {
                        bool emailSent = SendEmail(email, emailBody, string.Empty);
                        if (emailSent) break;

#if !DEBUG
                    Thread.Sleep(WaitTimeBetweenEmail);
#endif

                        attemptsCounter++;
                        if (attemptsCounter >= 3) //3 Failed Attempts to Send the Email, 1 hour break
                        {
                            Console.WriteLine("3 Failed Attempts to Send the Email to " + personalGoals.Name);
                            Console.WriteLine("Taking a 1 hour break for the next try...");
#if !DEBUG
                        Thread.Sleep(TimeSpan.FromHours(1));
#endif
                        }
                    }
                //}
                
#if !DEBUG
                Thread.Sleep(WaitTimeBetweenEmail);
#endif

            }
        } 

        private static bool SendEmail(string emailAddress, string emailBody, string subject)
        {
            bool isEmailSent;

            MailAddress to = new MailAddress(emailAddress);
            MailAddress from = new MailAddress("tm@tiempodevelopment.com");
            MailMessage mail = new MailMessage(from, to)
            {
                Subject = "Career Development Plan progress by " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Today.Month) +
                " " + DateTime.Today.Year,
                IsBodyHtml = true,
                Body = emailBody
            };

            mail.Subject = (subject != string.Empty) ? subject : mail.Subject;
            //string emailTarget = " \"" + emailBody.Substring(3, emailBody.IndexOf("<", StringComparison.Ordinal) - 3) + "\" " +
            //emailAddress;
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
            catch (Exception e)
            {
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

        public static void SendMailWithPeopleWithoutPositionInAD(List<string> peopleWithoutPosition)
        {
            if (peopleWithoutPosition.Count > 0)
            {
                string ToMail = ConfigurationManager.AppSettings["EmailPositionNotification"];

                StringBuilder people = new StringBuilder();
                people.AppendLine("The purpose of this email is to inform you about the employee list " +
                                  "without a position assigned on Active Directory, please send the list to helpdesk@tiempodevelopment.com " +
                                  "with the proper position for each employee so IT Department can update their positions on Active Directory.<br>");

                foreach (var person in peopleWithoutPosition)
                {
                    people.AppendLine("<br>");
                    people.AppendLine(person);
                }

                SendEmail(ToMail, people.ToString(), "CDPTracker - Unassigned Employee's Position");
            }
        }

        public static void SendMailWithPeopleWithoutManagerInAD(List<string> peopleWithoutManager)
        {
            foreach (string manager in ExclusionList)
            {
                peopleWithoutManager.Remove(manager);
            }
            if (peopleWithoutManager.Count > 0)
            {
                string ToMail = ConfigurationManager.AppSettings["EmailPositionNotification"];

                StringBuilder people = new StringBuilder();
                people.AppendLine("The purpose of this email is to inform you about the employee list " +
                                  "without a Manager assigned on Active Directory, please send the list to helpdesk@tiempodevelopment.com " +
                                  "with the proper Manager for each employee so IT Department can update their managers on Active Directory.<br>");

                foreach (var person in peopleWithoutManager)
                {
                    people.AppendLine("<br>");
                    people.AppendLine(person);
                }

                SendEmail(ToMail, people.ToString(), "CDPTracker - Unassigned Employee's Manager");
            }
        }

        public static void SendMailWithPeopleWithoutIdInAD(List<string> peopleWithoutId)
        {
            //exclude ITSUPPORT name from the list
            peopleWithoutId.Remove("ITSUPPORT");

            if (peopleWithoutId.Count > 0)
            {
                string ToMail = ConfigurationManager.AppSettings["EmailPositionNotification"];

                StringBuilder people = new StringBuilder();
                people.AppendLine("The purpose of this email is to inform you about the employee list " +
                                  "without Id assigned on Active Directory, please send the list to helpdesk@tiempodevelopment.com " +
                                  "so IT Department can register their Id on Active Directory.<br>");

                foreach (var person in peopleWithoutId)
                {
                    people.AppendLine("<br>");
                    people.AppendLine(person);
                }

                SendEmail(ToMail, people.ToString(), "CDPTracker - Unassigned Employee's Id");
            }
        }

        public static void SendMailWithPeopleWithoutEmailInAD(List<string> peopleWithoutEmail)
        {
            foreach (string manager in ExclusionList)
            {
                peopleWithoutEmail.Remove(manager);
            }

            if (peopleWithoutEmail.Count > 0)
            {
                string ToMail = ConfigurationManager.AppSettings["EmailPositionNotification"];

                StringBuilder people = new StringBuilder();
                people.AppendLine("The purpose of this email is to inform you about the employee list " +
                                  "without Email assigned on Active Directory, please send the list to helpdesk@tiempodevelopment.com " +
                                 "with the proper Email for each employee so IT Department can update their Mail on Active Directory.<br>");

                foreach (var person in peopleWithoutEmail)
                {
                    people.AppendLine("<br>");
                    people.AppendLine(person);
                }

                SendEmail(ToMail, people.ToString(), "CDPTracker - Unassigned Employee's Email");
            }
        }

        public static void SendMailWithPeopleWithoutTypeInAD(List<string> peopleWithoutType)
        {
            foreach (string manager in ExclusionList)
            {
                peopleWithoutType.Remove(manager);
            }

            if (peopleWithoutType.Count > 0)
            {
                string ToMail = ConfigurationManager.AppSettings["EmailPositionNotification"];

                StringBuilder people = new StringBuilder();
                people.AppendLine("The purpose of this email is to inform you about the employee list " +
                                  "without Type assigned on Active Directory, please send the list to helpdesk@tiempodevelopment.com " +
                                  "so IT Department can register their Type on Active Directory.<br>");

                foreach (var person in peopleWithoutType)
                {
                    people.AppendLine("<br>");
                    people.AppendLine(person);
                }

                SendEmail(ToMail, people.ToString(), "CDPTracker - Unassigned Employee's Type");
            }
        }

        public static void SendMailWithPeopleWithoutAreaInAD(List<string> peopleWithoutArea)
        {
            //exclude ITSUPPORT name from the list
            peopleWithoutId.Remove("ITSUPPORT");

            if (peopleWithoutArea.Count > 0)
            {
                string ToMail = ConfigurationManager.AppSettings["EmailPositionNotification"];

                StringBuilder people = new StringBuilder();
                people.AppendLine("The purpose of this email is to inform you about the employee list " +
                                  "which area doesn't match with the CDP Area catalog, please send the list to helpdesk@tiempodevelopment.com " +
                                  "so IT Department can change their Area on Active Directory.<br>");

                foreach (var person in peopleWithoutArea)
                {
                    people.AppendLine("<br>");
                    people.AppendLine(person);
                }

                SendEmail(ToMail, people.ToString(), "CDPTracker - Unmatch Employee's Area");
            }
        }
        #endregion

        #region Helpers Methods

        private static IEnumerable<PendingActions> GetPendingActionsOfNextMonth(IEnumerable<ResourceGoals> fromManagerGoals)
        {
            int currentYear = DateTime.Today.Year, currentMonth = DateTime.Today.Month;
            DateTime firstOfNextMonth = new DateTime(currentYear, currentMonth, 1).AddMonths(1);
            DateTime firstOf2NextMonth = new DateTime(currentYear, currentMonth, 1).AddMonths(2);

            List<PendingActions> teamPendingactions = (from fromManagerGoal in fromManagerGoals
                                                       let actionPendingCount = fromManagerGoal.Goals.Count(goal => goal.FinishDate.HasValue && !goal.VerifiedByManager && goal.FinishDate >= firstOfNextMonth && goal.FinishDate < firstOf2NextMonth)
                                                       select new PendingActions
                                                       {
                                                           EmployeName = fromManagerGoal.Name,
                                                           PendingActionCount = actionPendingCount
                                                       }).ToList();
            return teamPendingactions;
        }

        private static IEnumerable<PendingActions> GetPendingActionsOfMonth(IEnumerable<ResourceGoals> fromManagerGoals)
        {
            DateTime firstOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime maxDate = firstOfMonth.AddMonths(1);
            List<PendingActions> teamPendingactions = (from fromManagerGoal in fromManagerGoals
                                                       let actionPendingCount = fromManagerGoal.Goals.Count(goal => goal.FinishDate.HasValue && !goal.VerifiedByManager && goal.FinishDate < maxDate)
                                                       select new PendingActions
                                                       {
                                                           EmployeName = fromManagerGoal.Name,
                                                           PendingActionCount = actionPendingCount
                                                       }).ToList();
            return teamPendingactions;
        }

        private static string ComposeEmailForManagers(string managerName, int expectedActionsCount, int completedActionsCount, IEnumerable<PendingActions> currentMonthPendingResults, IEnumerable<PendingActions> nextMonthPendingresults)
        {
            string previousMonthPercentageResult = GetPercentage(expectedActionsCount, completedActionsCount);

            const string table = "<BR><table border=\"1\"><tr><th>Employee</th><th>Pending Actions</th></tr>{0}</table><BR>";
            const string rowFormat = "<tr><td>{0}</td><td>{1}</td></tr>";

            const string messageBody =
                "Hi {0}" +
                "<BR><BR>The purpose of this email is to inform you about your team member results on the completed CDP Action Items on {1}" +
                "<BR>Your overall completion rate was of: <I><B>{2}</I></B> - <I><B> {3} Activities Completed</I></B> from <I><B>{4} Expected Activities to Complete</I></B>" +
                "<BR><BR>At the same time we would like to invite you to continue working with your team on their next action items of their Career Development Plan." +
                "<BR><BR><BR>Please see below for this month’s pending action items:{5}" +
                "<BR>And just as a heads up, your team has the following actions items for next month:{6}" +
                "<BR>Remember that when someone of your team members has marked an action item to completed, you are required to certify it by marking the action item as verified. To start updating your Action Items’ status and verifying your team members completed action items please  <A HREF=" + "\"" +
                "http://cdptracker.tiempodevelopment.com/" + "\"" + ">Click Here</A>" +
                "<BR><BR>" +
                "Remember that if for any reason any of your team members will not be able to complete any of their action items defined review the case with him/her and redefine the actions to be taken or redefine the due date if the situation requires it." +
                "<BR><BR><BR>Keep in mind that:" +
                "<BR><BR><I>\"Your ability to discipline yourself to set clear goals, and then to work toward them every day, will do more to guarantee your success that any other single factor\"</I>" +
                "<BR><BR><p style=\"text-align: right;\">-Brian Tracy, Author and motivational speaker.</p>" +
                "If you have any other questions or concerns, please do not hesitate to let us know by replying to this email or visiting the Talent Management Department." +
                "<BR><BR><BR><BR>Sincerely," +
                "<BR><BR><BR>The Talent Management Team" +
                "<BR><BR>\"Collaborating with you to reach your professional aspirations.\"";

            //building current month table
            StringBuilder currentMonthRows = new StringBuilder();
            foreach (PendingActions pendingActions in currentMonthPendingResults)
            {
                currentMonthRows.Append(string.Format(rowFormat, pendingActions.EmployeName,
                                                 pendingActions.PendingActionCount));
            }
            string currentMonthTable = String.Format(table, currentMonthRows);

            //building next month table
            StringBuilder nextMonthRows = new StringBuilder();
            foreach (PendingActions pendingActions in nextMonthPendingresults)
            {
                nextMonthRows.Append(string.Format(rowFormat, pendingActions.EmployeName,
                                                 pendingActions.PendingActionCount));
            }
            string nextMonthTable = String.Format(table, nextMonthRows);

            string finalBody = string.Format(messageBody,
                managerName, GetPreviousMonth(), previousMonthPercentageResult, completedActionsCount, expectedActionsCount, currentMonthTable, nextMonthTable);

            return finalBody;

        } //end of ManagerComposeEmail

        private static string ComposeEmailCompletedNotificationForManagers(string managerName, StringBuilder employeeNames)
        {
            const string messageBody =
                "Hi {0}:<BR><BR>" +
                "The purpose of this email is to inform you that there are tasks pending for your verification from:<BR><BR>" +
                "{1}" +
                "Therefore, we invite you to verify them at the <A HREF=" + "\"" +
                "http://cdptracker.tiempodevelopment.com/" + "\"" + ">CDP Tracker</A>.<BR><BR>" +

                "Remember that when your team members mark an action item as completed, you are required to certify it. <BR><BR>" +

                "At the same time we would like to invite you to continue developing your team.<BR><BR>" +

                "If you have any other questions or concerns, please do not hesitate to let us know by replying to this email or visiting the Talent Management Department.<BR><BR>" +

                "Have a great day.<BR>" +
                "Sincerely, " +
                "<BR><BR>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;The Talent Management Team" +
                "<BR>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;\"Collaborating with you to reach your professional aspirations.\"<BR><BR>" +

                "<I>\"Your ability to discipline yourself to set clear goals, and then to work toward them every day, will do more to guarantee your success that any other single factor\"</I>" +
                "<p style=\"text-align: right;\">-Brian Tracy, Author and motivational speaker.</p>";


            string finalBody = string.Format(messageBody,
                managerName, employeeNames);
            return finalBody;

        }

        private static void GetPreviousPercentageForManager(IEnumerable<ResourceGoals> resourcesGoals, out int expectedActionsCount, out int completedActionsCount)
        {
            int currentYear = DateTime.Today.Year, currentMonth = DateTime.Today.Month;
            var startOfMonth = new DateTime(currentYear, currentMonth, 1);
            var startOfPreviousMonth = startOfMonth.AddMonths(-1);
            int previousMonth = startOfPreviousMonth.Month, previousYear = startOfPreviousMonth.Year;
            var gracePeriod = new DateTime(currentYear, currentMonth, 6);
            var previousGracePeriod = new DateTime(previousYear, previousMonth, 6);
            expectedActionsCount = 0; completedActionsCount = 0;
            foreach (ResourceGoals resourceGoal in resourcesGoals)
            {
                foreach (GoalTracking goal in resourceGoal.Goals.Where(goal => goal.FinishDate.HasValue))
                {
                    if (goal.FinishDate < startOfMonth && (!goal.VerifiedByManager ||
                        (goal.LastUpdate >= previousGracePeriod && goal.LastUpdate < gracePeriod)))
                    {
                        expectedActionsCount++;
                    }
                    if (goal.VerifiedByManager && goal.LastUpdate >= previousGracePeriod &&
                        goal.LastUpdate < gracePeriod)
                    {
                        completedActionsCount++;
                    }
                }
            }
        }

        private static string ComposeEmail(string name, int expectedToComplete, int completed, List<Goals> monthGoals, List<Objectives> obj, List<Goals> nextMonthGoals, List<GoalTracking> goalsCompleted, string domainName)
        {
            string previousMonthPercentageResult = GetPercentage(expectedToComplete, completed);
            const string tableBody =
                "<BR><table border=\"1\"><tr><th>Name of Activitiy</th><th>Due Date</th><th>Status</th></tr>{0}</table><BR>";
            const string tableBodyObj =
                "<BR><table border=\"1\"><tr><th>Name of Objective</th><th>Due Date</th><th>Status</th></tr>{0}</table><BR>";
            const string rowBody = "<tr><td>{0}</td><td>{1} {2} {3}</td><td>{4}</td></tr>";
            const string rowBodyObj = "<tr><td><strong>{0}</strong></td><td>{1}</td><td>{2}</td></tr>";
            const string noActivitiesMessage = "<BR>No Activities Assigned on the Month" + "<BR><BR>";


            //Get goals completed in this month
            StringBuilder goalsCompletedTable = new StringBuilder();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var completedGoalsInThisMonth = context.GoalTrackings.Where(x => x.FinishDate.Value.Month == DateTime.Now.Month && x.FinishDate.Value.Year == DateTime.Now.Year && x.Progress == 2 && x.Resource.DomainName == domainName).ToList();

                if (completedGoalsInThisMonth.Any())
                {
                    goalsCompletedTable.Append("<table border=1><tr><th>Names of Activities Finished On the Month</th>");
                    foreach (var goal in completedGoalsInThisMonth)
                    {
                        goalsCompletedTable.Append("<tr>" + goal.Goal + "</tr>");
                    }
                    goalsCompletedTable.Append("</table>");
                }
                else
                {
                    goalsCompletedTable.Append("No Activities Finished on the Month");
                }
            }
            string messageBody =
            "Hello {0}" +
            "<BR><BR>This is a summary of your results from the {1} goals." +
            "<BR><BR>" + goalsCompletedTable +
            "<BR><BR><BR>We would like to encourage you to continue working on the next goals of your Career Development Plan:" +
            "<BR>{5}" +
            "<BR>Remember that the next month you have the following activities:" +
            "<BR>{6}" +
            "<BR>" +
            "To update your goal's status please <A HREF=" + "\"" +
            "http://cdptracker.tiempodevelopment.com/" + "\"" + ">Click Here</A>." +
            "<BR><BR>Do not forget to ask your manager to review your goals once you change the status to 'Complete'." +
            "If for any reason you are not going to be able to complete any of your goals in the specified timeframe, " +
            "discuss it with your manager and redefine its due date if needed." +
            "<BR>" +
            "<BR>If you have any question or concerns, please do not hesitate to let us know by replying to this email " +
            "or visiting the Talent Management Department. " +
            "<BR><BR>" +
            "Sincerely," +
            "<BR>The Talent Management Team";

            //Current month body (saved on currentMonthBody)
            

            //Current objectives
            string objectives, currentMonthBody;
            if (obj.Any())
            {
                StringBuilder allObj = new StringBuilder();
                foreach (Objectives objective in obj)
                {
                    if (objective.Quarter >= getQuarter(DateTime.Now))
                    {
                        //string eachObjective = string.Format(rowBodyObj, objective.Name, objective.Quarter, objective.Status);
                        string eachObjective = string.Format(rowBodyObj, objective.Name, "", objective.Status);
                        allObj.Append(eachObjective);
                    }
                    if (monthGoals.Any())
                    {
                        StringBuilder allGoalsOnMonth = new StringBuilder();
                        foreach (Goals goal in monthGoals)
                        {
                            if (!goal.FinishDate.HasValue) continue;
                            if (getQuarter(goal.FinishDate.Value) >= getQuarter(DateTime.Now) && (goal.ObjectiveId == objective.ObjectiveId))
                            {
                                var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(goal.FinishDate.Value.Month);
                                string eachGoalOnMonth = string.Format(rowBody, goal.Name, monthName, goal.FinishDate.Value.Day, goal.FinishDate.Value.Year, GetStatus(goal.Progress));
                                allObj.Append(eachGoalOnMonth);
                            }
                        }
                        string emptyRow = string.Format(rowBody, "", "", "", "", "");
                        allObj.Append(emptyRow);
                        currentMonthBody = string.Format(tableBodyObj, allObj);
                    }
                    else
                    {
                        currentMonthBody = noActivitiesMessage;
                    }
                }
                objectives = string.Format(tableBodyObj, allObj);
            }
            else
            {
                objectives = noActivitiesMessage;
            }

            //Next Month Body (saved on nextMonthBody)
            string nextMonthBody;
            if (nextMonthGoals.Any())
            {
                StringBuilder allGoalsOnNextMonth = new StringBuilder();
                foreach (Goals goal in nextMonthGoals)
                {
                    if (!goal.FinishDate.HasValue) continue;
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(goal.FinishDate.Value.Month);
                    string eachGoalOnMonth = string.Format(
                        rowBody,
                        goal.Name, monthName, goal.FinishDate.Value.Day, goal.FinishDate.Value.Year, GetStatus(goal.Progress));
                    allGoalsOnNextMonth.Append(eachGoalOnMonth);
                }
                nextMonthBody = string.Format(tableBody, allGoalsOnNextMonth);
            }
            else
            {
                nextMonthBody = noActivitiesMessage;
            }


            string finalBody = string.Format(messageBody,
                name, GetPreviousMonth(), previousMonthPercentageResult, completed, expectedToComplete, objectives, nextMonthBody);

            return finalBody;
        }

        private static IEnumerable<ResourceGoals> GetGoals()
        {
            List<ResourceGoals> resourcesGoals = new List<ResourceGoals>();
            EmployeeActiveDirectoryManager employeeActiveDirectoryManager = new EmployeeActiveDirectoryManager(
                ConfigurationManager.AppSettings["ADPath"],
                ConfigurationManager.AppSettings["ADUsr"],
                ConfigurationManager.AppSettings["ADPass"]);

            var firstDayOfCurrentMonth = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            var firstOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);
            var previousGracePeriod = firstOfPreviousMonth.AddDays(5);
            var maxDate = firstDayOfCurrentMonth.AddMonths(2);
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var dbGoals = (from goals in context.GoalTrackings
                               where
                               !(goals.VerifiedByManager && goals.LastUpdate < previousGracePeriod)
                               &&
                               !(goals.FinishDate >= maxDate && goals.VerifiedByManager)
                               select goals).ToList();

                var dbObj = (from objectives in context.Objectives
                             select objectives).ToList();

                var userData = employeeActiveDirectoryManager.GetUserList();

                IEnumerable<string> registeredDomainNames = context.Resources.Select(r => r.DomainName.ToLower()).ToList();

                //NO EX USERS
                userData = userData.Where(user => user.OperationalUnits.First() != "Ex-User").ToList();

                //NO PEOPLE WITH DEFINED CONTRACT OR WITHOUT HIRE DATE
                try
                {
                    DateTime actualDate = DateTime.Now;
                    var employeesWithHireDate = userData.Where(x => employeeActiveDirectoryManager.GetUserPropertyValue(x.DomainName, "homephone") != null).ToList();
                    userData = employeesWithHireDate.Where(user => actualDate.Subtract(Convert.ToDateTime(employeeActiveDirectoryManager.GetUserPropertyValue(user.DomainName, "Homephone"))).TotalDays > 90).ToList();
                }
                catch (Exception e)
                {
                    //Not all the employees have hire date so far.
                }
                //NO INTERNS
                var interns = userData.Where(user => user.OperationalUnits.Any(OU => OU == "Interns")).ToList();
                userData = userData.Except(interns).ToList();


                resourcesGoals.AddRange(from resource in userData
                                        let goalList = (from goals in dbGoals
                                                        where (string.Compare(goals.Resource.DomainName, resource.DomainName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                                        select goals).ToList()
                                        let oList = (from objectives in dbObj
                                                     where (string.Compare(objectives.Resource.DomainName, resource.DomainName, StringComparison.InvariantCultureIgnoreCase) == 0) && (objectives.Quarter >= getQuarter(DateTime.Now) && (objectives.Year == DateTime.Now.Year))
                                                     select objectives).ToList()
                                        select new ResourceGoals
                                        {
                                            IsRegisteredUser = registeredDomainNames.Any(d => d.IndexOf(resource.DomainName, StringComparison.InvariantCultureIgnoreCase) >= 0),
                                            Name = resource.Name,
                                            DomainName = resource.DomainName,
                                            Email = resource.Email,
                                            Goals = goalList,
                                            Objectives = oList,
                                            ManagerName = !string.IsNullOrWhiteSpace(resource.Manager) ? resource.Manager.Substring(3, resource.Manager.IndexOf(",", StringComparison.Ordinal) - 3) : string.Empty
                                        });

                //add email to resourceGoals
                foreach (var resource in resourcesGoals)
                {
                    ResourceGoals resourceGoal = resource;
                    var user =
                        userData.FirstOrDefault(
                            u =>
                            string.Compare(resourceGoal.ManagerName, u.Name, StringComparison.InvariantCultureIgnoreCase) ==
                            0);
                    if (user == null) continue;
                    resourceGoal.ManagerEmail = user.Email;
                }
                return resourcesGoals;
            }

        }

        private static List<Objectives> GetObjectives(ResourceGoals personalGoals)
        {
            List<Objectives> objectivesList = new List<Objectives>();
            objectivesList.AddRange(from objective in personalGoals.Objectives
                                    select new Objectives { Name = objective.Objective1, Quarter = Convert.ToInt16(objective.Quarter), Status = GetStatus(Convert.ToInt16(objective.Progress)), ObjectiveId = objective.ObjectiveId});
            return objectivesList;
        }

        static List<Goals> GetCurrentMonthGoals(ResourceGoals personalGoals)
        {
            var firstOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var maxDate = firstOfMonth.AddMonths(1);
            List<Goals> goalList = new List<Goals>();
            goalList.AddRange(from goal in personalGoals.Goals
                              where goal.FinishDate.HasValue && !goal.VerifiedByManager &&
                              goal.FinishDate < maxDate
                              select new Goals { Name = goal.Goal, FinishDate = goal.FinishDate, Progress = goal.Progress, ObjectiveId = Convert.ToInt16(goal.ObjectiveId) });
            return goalList;
        }

        static List<Goals> GetNextMonthGoals(ResourceGoals personalGoals)
        {
            int currentYear = DateTime.Today.Year, currentMonth = DateTime.Today.Month;
            var firstOfNextMonth = new DateTime(currentYear, currentMonth, 1).AddMonths(1);
            var firstOf2NextMonth = new DateTime(currentYear, currentMonth, 1).AddMonths(2);
            return (from goal in personalGoals.Goals
                    where goal.FinishDate.HasValue && !goal.VerifiedByManager &&
                    goal.FinishDate >= firstOfNextMonth &&
                    goal.FinishDate < firstOf2NextMonth
                    select new Goals { Name = goal.Goal, FinishDate = goal.FinishDate, Progress = goal.Progress, ObjectiveId = Convert.ToInt16(goal.ObjectiveId) }).ToList();
        }

        static void GetPreviousPercentage(ResourceGoals personalGoals, out int expectedActionsCount, out int completedActionsCount, out List<GoalTracking> goalsCompleted)
        {
            int currentYear = DateTime.Today.Year, currentMonth = DateTime.Today.Month;
            var startOfMonth = new DateTime(currentYear, currentMonth, 1);
            var startOfPreviousMonth = startOfMonth.AddMonths(-1);
            int previousMonth = startOfPreviousMonth.Month, previousYear = startOfPreviousMonth.Year;
            var gracePeriod = new DateTime(currentYear, currentMonth, 6);
            var previousGracePeriod = new DateTime(previousYear, previousMonth, 6);
            expectedActionsCount = 0; completedActionsCount = 0;
            goalsCompleted = new List<GoalTracking>();
            foreach (GoalTracking goal in personalGoals.Goals.Where(goal => goal.FinishDate.HasValue))
            {
                if (goal.FinishDate < startOfMonth && (!goal.VerifiedByManager ||
                    (goal.LastUpdate >= previousGracePeriod && goal.LastUpdate < gracePeriod)))
                {
                    expectedActionsCount++;
                }
                if (goal.VerifiedByManager && goal.LastUpdate >= previousGracePeriod &&
                    goal.LastUpdate < gracePeriod)
                {
                    completedActionsCount++;
                }

            }
        }

        private static string GetPreviousMonth()
        {
            DateTime previousMonth = DateTime.Today.AddMonths(-1);
            string previousMonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(previousMonth.Month);
            string previousYearString = previousMonth.Year.ToString(CultureInfo.InvariantCulture);
            return previousMonthName + " " + previousYearString;
        }

        private static string GetStatus(int status)
        {
            const string notStarted = "Not Started";
            const string inProgress = "In Progress";
            const string needsVerification = "Needs Manager Verification";
            const string unknown = "Unknown";

            switch (status)
            {
                case 0:
                    return notStarted;
                case 1:
                    return inProgress;
                case 2:
                    return needsVerification;
                default:
                    return unknown;
            }
        }

        private static string GetPercentage(int expected, int completed)
        {
            string previousMonthPercentageResult;
            if (completed > expected)
            {
                previousMonthPercentageResult = "AHEAD";
            }
            else
            {
                if (completed == 0 && expected == 0)
                {
                    previousMonthPercentageResult = "No action expected or completed in the previous month";
                }
                else
                {
                    double percentage = completed * 100.0 / expected;
                    previousMonthPercentageResult = percentage.ToString("0.00");
                    previousMonthPercentageResult += "%";
                }
            }
            return previousMonthPercentageResult;
        }

        public static void AddNewEmployeesFromAD()
        {
            EmployeeActiveDirectoryManager employeeActiveDirectoryManager = getActiveDirectoryInstance();
            //Get user from Active Directory
            var userData = employeeActiveDirectoryManager.GetUserList();
            var activeUserFromAD = userData.Where(data => GetIfIsActiveEmployeeFromAD(data.DomainName));
            // Get Employees from database 


            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                IEnumerable<Resource> userFromDB = context.Resources.ToList();

                foreach (var user in activeUserFromAD)
                {
                    //foreach (var resource in userFromDB)
                    //{
                    bool re = userFromDB.Any(item => item.Email == user.Email);

                    if (re == false)
                    {
                        if (user.Email != "")
                        {
                            // add new employee to Data Base
                            try
                            {
                                Resource resorce = new Resource();
                                resorce.ResourceId = context.Resources.Max(r => r.ResourceId) + 1;
                                resorce.Name = user.Name;
                                resorce.Email = user.Email;
                                resorce.RolId = 0;
                                resorce.DomainName = user.DomainName;
                                resorce.LastLogin = DateTime.Now;
                                resorce.LocationId = 1;
                                resorce.ActiveDirectoryId = 0;
                                resorce.IsEnable = 1;

                                context.Resources.Add(resorce);
                                context.SaveChanges();

                                context.Employee.Add(new Employee
                                {
                                    ResourceId = resorce.ResourceId,
                                    ManagerId = null,
                                    CurrentPosition = "UNKNOWN",
                                    AspiringPosition = "UNKNOWN",
                                    CurrentPositionID = 1,
                                    AspiringPositionID = null,
                                    ProjectId = null
                                });
                                context.SaveChanges();


                            }
                            catch (Exception exs)
                            {
                            }
                        }

                    }
                }
            }

        }

        public static string GetCurrentPositionFromAD(string logonName)
        {
            const string currentPositionProperty = "description";

            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            string currentPosition = ActiveDirectory.GetUserPropertyValue(logonName, currentPositionProperty);
            if (currentPosition == null)
            {
                return string.Empty;
            }


            return currentPosition;
        }

        public static string GetCurrentLocationFromAD(string logonName)
        {
            const string locationPositionProperty = "physicalDeliveryOfficeName";

            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            string currentPosition = ActiveDirectory.GetUserPropertyValue(logonName, locationPositionProperty);
            if (currentPosition == null)
            {
                return string.Empty;
            }


            return currentPosition;
        }

        /// <summary>
        /// Method to acces the groups created by Luis Alfaro
        /// </summary>
        /// <param name="name">Name of the employee as a key value</param>
        /// <returns></returns>
        public static List<string> GetEmployeeGroups(string name)
        {
            List<string> groupAssigned = new List<string>();
               
            try
            {
                 EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
                 DirectoryEntry user = ActiveDirectory.GetEmployeeByLogonName(name);
                 foreach (string group in user.Properties["memberof"])
                 {
                     if(isWordGroup(group))
                        groupAssigned.Add(getGroupByString(group));
                 }
            }
            catch(ArgumentException e)
            {
                Console.Write("{0}", e.ToString());
            }

            return groupAssigned;
        }

        /// <summary>
        /// Test method to ensure that is only tdg-moss roles
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool isWordGroup(String text)
        {
            bool isCorrect = false;            
            string[] textSeparator = text.Split(',');
            string aux = textSeparator[0];
            
            string aux2 = aux.Substring(3,aux.Length-4);
            if (aux2.StartsWith("tdg-moss-"))
                isCorrect = true;
            return isCorrect;
        }
       
        public static string getGroupByString(string text)
        {
            string groupName = "";
            string[] textSeparator = text.Split(',');
            string aux = textSeparator[0];

            string aux2 = aux.Substring(3, aux.Length - 3);
            if (aux2.StartsWith("tdg-moss-"))
                groupName = aux2;
            return groupName;

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

        public static bool GetIfIsActiveEmployeeFromADById(int id)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            IList<String> OUFromEmployee;
            Resource employee;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                employee = context.Resources.Where(x => x.ResourceId == id).FirstOrDefault();
            }

            try
            {
                //if employee is part of an OU different from Ex-User then is an ActiveEmployee.
                OUFromEmployee = ActiveDirectory.GetEmployeeOrganizationalUnitList(employee.DomainName).ToList();

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

        private static bool positionExists(string currentPosition)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var matchPositions = context.Position.Where(a => a.PositionName == currentPosition).ToList();

                if (matchPositions.Count > 0)
                    return true;
            }

            return false;
        }

        private static bool insertNewPosition(string newPosition)
        {
            try
            {
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    Position position = new Position();
                    position.PositionName = newPosition;
                    position.AreaId = 0; //unknown position Area

                    context.Position.Add(position);
                    context.SaveChanges();
                    return true;

                }
            }
            catch (DbUpdateException error)
            {
                Console.WriteLine(error.Message);
                return false;
            }
        }

        private static void DeleteOldGoalsAndObjectives()
        {
            const int UNASSIGN_CATEGORY_ID = 4;

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<int> IdExEmployees = new List<int>();

                List<GoalTracking> goalsUnverified = context.GoalTrackings.Where(x => x.VerifiedByManager == false).ToList();

                //Delete goals unverified from Ex-Employees
                foreach (GoalTracking goal in goalsUnverified)
                {
                    int resourceId = goal.ResourceId;

                    if (IdExEmployees.Contains(resourceId))
                        context.GoalTrackings.Remove(goal);

                    else if (!GetIfIsActiveEmployeeFromADById(goal.ResourceId))
                    {
                        context.GoalTrackings.Remove(goal);
                        IdExEmployees.Add(resourceId);

                    }
                }

                //Delete objectives not completed from Ex-Employees
                foreach (int id in IdExEmployees)
                {
                    List<Objective> objectivesList = context.Objectives.Where(x => x.ResourceId == id && x.CategoryId != UNASSIGN_CATEGORY_ID).ToList();

                    foreach (Objective objective in objectivesList)
                    {
                        List<GoalTracking> goalsFromObjective = context.GoalTrackings.Where(x => x.ObjectiveId == objective.ObjectiveId).ToList();

                        if (goalsFromObjective.Count == 0)
                            context.Objectives.Remove(objective);
                    }
                }

                context.SaveChanges();
            }
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

        private static void insertNewTrainings(int resourceID, string newPosition)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var trainingPrograms = context.TrainingProgram.Where(x => x.Position1.PositionName.Equals(newPosition)).ToList();

                foreach (var tp in trainingPrograms)
                {
                    if (tp.FinishDate >= DateTime.Now)
                    {
                        TrainingProgramDetails newTraining = new TrainingProgramDetails();
                        newTraining.IdTrainingProgram = tp.IdTrainingProgram;
                        newTraining.ResourceId = resourceID;
                        newTraining.Status = 0;

                        context.TrainingProgramDetails.Add(newTraining);
                        context.SaveChanges();
                    }
                }

            }
        }

        private static IEnumerable<string> GetDirectoryEntryOrganizationalUnitList(DirectoryEntry directoryEntry)
        {
            const string part = "OU=";
            string[] commaSplit = directoryEntry.Path.Split(',');
            return (from split in commaSplit where split.Contains(part) select split.Substring(part.Length)).ToList();
        }

        private static int getQuarter(DateTime date)
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

        #endregion

        #region Updating Methods




        private static void addNewGroups()
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                List<string> listOfRoles = new List<string>();
                List<string> listOfGroups = new List<string>();
                var resources = context.Resources.ToList();
                int counter = 0;
                try
                {
                    foreach (var resource in resources)
                    {
                        try
                        {
                            string employeeName = resource.DomainName.StripDomain();
                            int resourceID = resource.ResourceId;

                            if (GetIfIsActiveEmployeeFromAD(employeeName))
                            {
                               
                                try
                                {
                                    listOfRoles = GetEmployeeGroups(employeeName);
                               
                                    foreach (var roles in listOfRoles)
                                    {
                                        listOfGroups.Add(roles);                                       
                                    }

                                    listOfRoles.Clear();
                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine("Error Saving the Data on DB, {0}", exc);
                                }

                            }
                            Console.WriteLine(String.Format("{0} of {1} Resources proccesed", ++counter, resources.Count));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.InnerException);
                        }
                    }

                    try
                    {
                        List<string> result = new List<string>();
                        result.AddRange(listOfGroups.Distinct());

                        List<Groups> groupsOnDB = new List<Groups>();
                        groupsOnDB = context.Groups.Select(o => o).ToList();
                        List<string> groupsStringOnDB = new List<string>();
                        foreach( var groups in groupsOnDB)
                        {
                            groupsStringOnDB.Add(groups.GroupName);
                        }

                        List<string> listOfDiferences = result.Except(groupsStringOnDB).ToList();

                        //Insert Elements
                        if(listOfDiferences.Count>0)
                        {
                            foreach (var group in listOfDiferences)
                            {
                                Groups gr = new Groups();
                                gr.GroupName = group;
                                context.Groups.Add(gr);
                            }
                            context.SaveChanges();
                        }
                        

                        

                    }
                    catch(Exception exp)
                    {
                        Console.WriteLine(exp.InnerException);
                    }


                }
                catch (Exception a)
                {
                    Console.WriteLine(a.InnerException);
                }
            }
        }
        /// <summary>
        /// Method to Update groups provided by the active directory 
        /// </summary>
        private static void UpdateEmployeeGroups()
        {
             using (CDPTrackEntities context = new CDPTrackEntities())
             {
                 List<string> listOfRoles = new List<string>();
                 var resources = context.Resources.ToList();
                 int counter = 0;
                 try
                 {
                     foreach (var resource in resources)
                     {
                         try
                         {
                             string employeeName = resource.DomainName.StripDomain();
                             int resourceID = resource.ResourceId;

                             if (GetIfIsActiveEmployeeFromAD(employeeName))
                             {
                                
                             
                                 try
                                 {
                                     listOfRoles = GetEmployeeGroups(employeeName);
                                     if(listOfRoles.Count>0)
                                     {
                                       
                                        var result = from r in context.Employee_Groups where r.ResourceId == resourceID select r;  
                                        if (result.Count() > 0)
                                        {
                                            foreach (Employee_Groups eg in result)
                                            {
                                                context.Employee_Groups.Remove(eg);
                                            }
                                            context.SaveChanges();
                                        }
                                         foreach (var roles in listOfRoles)
                                         {
                                             Employee_Groups eg = new Employee_Groups();
                                             eg.ResourceId = resourceID;
                                             eg.GroupId = roles;
                                             context.Employee_Groups.Add(eg);
                                             
                                         }
                                         context.SaveChanges();
                                     }
                                     

                                     listOfRoles.Clear();
                                 }
                                 catch(Exception exc)
                                 {
                                     Console.WriteLine("Error Saving the Data on DB, {0}",exc);
                                 }
                                 
                             }
                             Console.WriteLine(String.Format("{0} of {1} Resources proccesed", ++counter, resources.Count));
                         }
                         catch (Exception ex)
                         {
                             Console.WriteLine(ex.InnerException);
                         }
                     }
                 }
                 catch (Exception a)
                 {
                     Console.WriteLine(a.InnerException);
                 }
             }
        }


        private static void UpdateEmployeeData()
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var resources = context.Resources.ToList();
                int counter = 0;
                try
                {
                    foreach (var resource in resources)
                    {
                        try
                        {
                            string employeeName = resource.DomainName.StripDomain();
                            int resourceID = resource.ResourceId;

                            if (GetIfIsActiveEmployeeFromAD(employeeName))
                            {
                                var positionInDatabase = context.Employee.Where(x => x.ResourceId == resourceID).Select(a => a.CurrentPosition).FirstOrDefault();
                                string positionInActiveDirectory = GetCurrentPositionFromAD(employeeName);

                                if (positionInActiveDirectory == String.Empty)
                                {
                                    peopleWithoutPosition.Add(employeeName);
                                }

                                string LocationInActiveDirectory = GetCurrentLocationFromAD(employeeName);
                                if (LocationInActiveDirectory != String.Empty)
                                {
                                    if (LocationInActiveDirectory == "HMO" || LocationInActiveDirectory == "MTY" || LocationInActiveDirectory == "PHX" || LocationInActiveDirectory == "GDL")
                                    {
                                        updateLocation(resourceID, LocationInActiveDirectory);
                                    }

                                }

                                changePosition(resourceID, positionInActiveDirectory);
                                updateProjectFromActiveDirectory(employeeName, resourceID);
                                updateTypeFromAD(employeeName, resourceID);
                                updateAreaFromAD(employeeName, resourceID);

                                if (!updateIdFromActiveDirectory(employeeName))
                                {
                                    peopleWithoutId.Add(employeeName);
                                }
                                if (!updateEmailFromActiveDirectory(employeeName))
                                {
                                    peopleWithoutEmail.Add(employeeName);
                                }
                                if (!updateManagersFromAD(resourceID, employeeName))
                                {
                                    peopleWithoutManager.Add(employeeName);
                                }

                            }
                            else {
                                disableEmployee(resourceID);
                            }
                            Console.WriteLine(String.Format("{0} of {1} Resources proccesed", ++counter, resources.Count));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.InnerException);
                        }
                    }
                }
                catch (Exception a)
                {
                    Console.WriteLine(a.InnerException);
                }
            }
        }

        private static void disableEmployee(int resourceId)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                Resource resource;
                resource = context.Resources.Where(x => x.ResourceId == resourceId).FirstOrDefault();
                resource.IsEnable = 0;
                context.SaveChanges();
            }
        }

        public static bool changePosition(int resourceID, string newPosition)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                try
                {
                    var employee = context.Employee.Where(a => a.ResourceId == resourceID).SingleOrDefault();



                    if (employee != null)
                    {
                        bool differentPosition = false;

                        if (newPosition.Equals(string.Empty))
                            newPosition = "UNKNOWN";

                        if (!positionExists(newPosition))
                            insertNewPosition(newPosition);

                        if (employee.CurrentPosition != newPosition)
                        {
                            insertNewTrainings(resourceID, newPosition);
                            differentPosition = true;
                        }


                        employee.CurrentPosition = newPosition;

                        employee.AspiringPosition = employee.AspiringPosition ?? newPosition;

                        var position = context.Position.Where(a => a.PositionName == newPosition).FirstOrDefault();
                        int idNewPosition = position.PositionId;

                        employee.CurrentPositionID = idNewPosition;
                        context.SaveChanges();
                        if (differentPosition == true)
                        {

                            changeAspiringPosition(resourceID, idNewPosition);
                        }
                        return true;
                    }
                    return false;
                }
                catch (Exception a)
                {
                    Console.WriteLine(a.Message);
                    return false;
                }

            }
        }

        public static void changeAspiringPosition(int resourceID, int idNewPosition)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var aspiringPosition = (from positionHierarchy in context.PositionsHierarchy
                                        join position in context.Position
                                        on positionHierarchy.PositionId equals position.PositionId
                                        where positionHierarchy.PositionId == idNewPosition
                                        select new { ID = positionHierarchy.NextPosition, Value = positionHierarchy.Position1.PositionName }).FirstOrDefault();
                if (aspiringPosition != null)
                {
                    var employee = context.Employee.Where(a => a.ResourceId == resourceID).SingleOrDefault();

                    employee.AspiringPosition = aspiringPosition.Value;
                    employee.AspiringPositionID = aspiringPosition.ID;

                    context.SaveChanges();
                }
                else
                {
                    var employee = context.Employee.Where(a => a.ResourceId == resourceID).SingleOrDefault();

                    employee.AspiringPosition = "UNKNOWN";
                    employee.AspiringPositionID = null;
                    context.SaveChanges();

                }
            }
        }

        private static bool updateManagersFromAD(int resourceID, string employeeName)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            string ManagerString = ActiveDirectory.GetUserPropertyValue(employeeName, "manager");
            DirectoryEntry directoryEntry = ActiveDirectory.GetEmployeeByLogonName(employeeName);
            try
            {
                string ManagerName = ManagerString.Substring(3, ManagerString.IndexOf(',') - 3);
                DirectoryEntry ManagerEntry = ActiveDirectory.GetEmployeeByCommonName(ManagerName);
                string idManagerADString = ManagerEntry.Properties["initials"].Value.ToString();
                int idManagerAD = Int32.Parse(idManagerADString);
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    Employee employee;
                    employee = context.Employee.Where(x => x.ResourceId == resourceID).FirstOrDefault();
                    employee.ManagerId = idManagerAD;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                Console.WriteLine(String.Format("{0} does not have a Manager in ActiveDirectory", employeeName));
                return false;
            }

        }

        private static bool updateEmailFromActiveDirectory(string employeeName)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            DirectoryEntry directoryEntry = ActiveDirectory.GetEmployeeByLogonName(employeeName);
            try
            {
                string emailFromAD = directoryEntry.Properties["mail"].Value.ToString();
                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    Resource resource;
                    resource = context.Resources.Where(x => x.DomainName == employeeName).FirstOrDefault();
                    resource.Email = emailFromAD;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void updateLocation(int resourceId, string newLocation)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                Resource resource;
                resource = context.Resources.Where(x => x.ResourceId == resourceId).FirstOrDefault();
                int locationId = context.Locations.Where(x => x.abbreviation == newLocation).Select(x => x.ID).FirstOrDefault();
                resource.LocationId = locationId;
                context.SaveChanges();

            }
        }

        private static bool updateIdFromActiveDirectory(string employeeName)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();

            string idProperty = "initials";
            DirectoryEntry directoryEntry = ActiveDirectory.GetEmployeeByLogonName(employeeName);
            try
            {
                string id = directoryEntry.Properties[idProperty].Value.ToString();
                int idAD = Convert.ToInt32(id);

                using (CDPTrackEntities context = new CDPTrackEntities())
                {
                    Resource resource;
                    resource = context.Resources.Where(x => x.DomainName == employeeName).FirstOrDefault();

                    resource.ActiveDirectoryId = idAD;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                Console.WriteLine(String.Format("{0} does not have an id in ActiveDirectory", employeeName));
                return false;
            }
        }

        private static bool updateProjectFromActiveDirectory(string employeeName, int resourceID)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            DirectoryEntry directoryEntry = ActiveDirectory.GetEmployeeByLogonName(employeeName);
            string project = null;
            try
            {
                project = directoryEntry.Properties["department"].Value.ToString();
            }
            catch (Exception e)
            {
                project = GetDirectoryEntryOrganizationalUnitList(directoryEntry).FirstOrDefault();
                //THIS IS IN CASE THEY WANT TO SEND EMAIL WITH EMPLOYEES WITHOUT DEPARTMENT/PROJECT
                //peopleWithoutProject.Add(employeeName);
                //Console.WriteLine(String.Format("{0} does not have a Project/Deparment in ActiveDirectory", employeeName));
                //return false;
            }

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                int projectId = context.Project.Where(x => x.Project1 == project).Select(x => x.ProjectId).FirstOrDefault();
                if (projectId == 0)
                {
                    Project p = new Project();
                    p.Project1 = project;
                    context.Project.Add(p);
                    context.SaveChanges();
                    projectId = p.ProjectId;
                }
                Employee resource = context.Employee.Where(x => x.ResourceId == resourceID).FirstOrDefault();
                resource.ProjectId = projectId;
                context.SaveChanges();
                return true;
            }
        }

        private static bool updateTypeFromAD(string employeeName, int resourceID)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            DirectoryEntry directoryEntry = ActiveDirectory.GetEmployeeByLogonName(employeeName);
            string type = null;
            try
            {
                type = directoryEntry.Properties["employeeType"].Value.ToString();
            }
            catch (Exception e)
            {
                peopleWithoutType.Add(employeeName);
                Console.WriteLine(String.Format("{0} does not have an Type in ActiveDirectory", employeeName));
            }

            using (CDPTrackEntities context = new CDPTrackEntities())
            {

                Employee resource = context.Employee.Where(x => x.ResourceId == resourceID).FirstOrDefault();
                resource.Type = type;
                context.SaveChanges();
                return true;
            }
        }

        private static bool updateAreaFromAD(string employeeName, int resourceID)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            DirectoryEntry directoryEntry = ActiveDirectory.GetEmployeeByLogonName(employeeName);
            string area = null;
            try
            {
                area = directoryEntry.Properties["division"].Value.ToString();

            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("{0} does not have an Area in ActiveDirectory", employeeName));
            }

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                int areaId = context.Area.Where(x => x.Name == area).Select(x => x.AreaId).FirstOrDefault();
                if (areaId == 0)
                {
                    peopleWithoutArea.Add(employeeName);
                }
                Employee resource = context.Employee.Where(x => x.ResourceId == resourceID).FirstOrDefault();
                resource.AreaId = areaId;
                context.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Reports methods

        private static void composeEmailForKPI()
        {
            StringBuilder bodyMessage = new StringBuilder();

            string KPIPart1 = getDataForKPIReport();
            string KPIPart2 = getDataForKPIManagers();

            bodyMessage.Append(KPIPart1);
            bodyMessage.Append(KPIPart2);

            //try to send the email
            int attemptsCounter = 0;
            while (true)
            {
                string ToMail = ConfigurationManager.AppSettings["ReportNotification"];
                bool emailSent = SendEmail(ToMail, bodyMessage.ToString(), "CDP KPI Report");
                if (emailSent) break;
#if !DEBUG
                    Thread.Sleep(WaitTimeBetweenEmail);
#endif
                attemptsCounter++;
                if (attemptsCounter >= 3) //3 Failed Attempts to Send the Email, 1 hour break
                {
                    Console.WriteLine("3 Failed Attempts to Send the Email with CDP KPI Report");
                    Console.WriteLine("Taking a 1 hour break for the next try...");
#if !DEBUG
                   Thread.Sleep(TimeSpan.FromHours(1));
#endif
                }
            }
        }

        private static string getDataForKPIReport()
        {
            StringBuilder bodyMessage = new StringBuilder();
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            List<Resource> resourcesActiveInCDP = new List<Resource>();
            DateTime dateForKPI = DateTime.Now.AddMonths(-6);
            int ActualQuarter = getQuarter(dateForKPI);
            int ActualYear = dateForKPI.Year;

            var userData = ActiveDirectory.GetUserList().Where(x => x.OperationalUnits.Any(OU => OU == "Interns")).ToList();

            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var allResources = context.Resources.ToList();
                foreach (var resource in allResources)
                {
                    if (GetIfIsActiveEmployeeFromAD(resource.DomainName) && !userData.Any(x => x.Email == resource.Email))
                    {
                        resourcesActiveInCDP.Add(resource);
                    }
                }
                //this is to get and display the total of employees in each location
                bodyMessage.Append(string.Format("Total number of people in the system: {0} <BR>", resourcesActiveInCDP.Count()));

                //add every location and total resources per location to the messageBody
                string stringforLocationTotals = "<table><tr><th> </th><th> </th><th> </th></tr>{0}</table><BR>";
                StringBuilder bodyStringForTotals = new StringBuilder();
                foreach (var location in context.Locations.ToList())
                {
                    int totalResourcesInLocation;
                    var resourcesInLocation = resourcesActiveInCDP.Where(x => x.Location != null && x.LocationId == location.ID);
                    if (resourcesInLocation != null) totalResourcesInLocation = resourcesInLocation.Count();
                    else totalResourcesInLocation = 0;
                    bodyStringForTotals.Append(string.Format("<tr><td>{0}</td><td>&nbsp;&nbsp;&nbsp;&nbsp;</td><td>{1}</td></tr>", location.Name.ToUpper(), totalResourcesInLocation));
                }
                //add the total number of people by location to the body message
                bodyMessage.Append(string.Format(stringforLocationTotals, bodyStringForTotals));

                //get the list of persons with Active CDP add to the body message
                var personsWithCDP = context.Resources.Where(x => x.GoalTrackings.Any(y => y.Progress >= 0 && y.FinishDate >= dateForKPI)).ToList();
                bodyMessage.Append("Number of people in Tiempo with a CDP: " + personsWithCDP.Count() + "<BR>");

                //for each location get the neccesary data to fill a table 
                foreach (var location in context.Locations.ToList())
                {
                    const string table = "<BR><table border=\"1\" bgcolor=\"#efefef\"><tr bgcolor=\"#c6daf8\">" +
                            "<th>Term</th>" +
                            "<th>Goals</th>" +
                            "<th>To Do</th>" +
                            "<th>To Do (%)</th>" +
                            "<th>In Progress</th>" +
                            "<th>In Progress (%)</th>" +
                            "<th>Completed</th>" +
                            "<th>Completed (%)</th>" +
                            "</tr>{0}</table><BR>";

                    //this stringbuilder will hold the rows for each location
                    StringBuilder stringROWSforLocation = new StringBuilder();
                    //hold all the employees with active CDP in each location
                    var TotalPersonsWithCDPLocation = personsWithCDP.Where( x => x.Location != null && x.Location.Name == location.Name).ToList();
                    //add the total of persons to the body message
                    bodyMessage.Append("<BR>" + location.Name.ToUpper() + "&nbsp;&nbsp;" + TotalPersonsWithCDPLocation.Count() + " People");

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
                    // List of long assesments (Long term is 18 months and beyond
                    List<Objective> longTermObjectives = assessmentListByLocation.Where(x => x.Quarter >= ActualQuarter && x.Year == (ActualYear + 2)).ToList();
                    //exclude Elements from long term and short term
                    // List of middle assesments  (Middle term is 6-18 months)               
                    List<Objective> middleTermObjectives = assessmentListByLocation.Except(longTermObjectives).Except(shortTermObjectives).ToList();
                    List<Objective> submiddleTermList = middleTermObjectives.Where(x => x.Quarter < ActualQuarter && x.Year == ActualYear).ToList();
                    middleTermObjectives = middleTermObjectives.Except(submiddleTermList).ToList();
                    double[] data = new double[7];
                    //send the list of objectives to generate the row for each term for location
                    stringROWSforLocation.Append(generateRowForKPI(shortTermObjectives, "Short Term", ref data));
                    stringROWSforLocation.Append(generateRowForKPI(middleTermObjectives, "Middle Term", ref data));
                    stringROWSforLocation.Append(generateRowForKPI(longTermObjectives, "Long Term", ref data));
                    data[2] = data[0] != 0 ? Math.Round((double)(data[1] * 100) / data[0], 2) : 0;
                    data[4] = data[0] != 0 ? Math.Round((double)(data[3] * 100) / data[0], 2) : 0;
                    data[6] = data[0] != 0 ? Math.Round((double)(data[5] * 100) / data[0], 2) : 0;
                    stringROWSforLocation.Append(string.Format(
                    "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>",
                    "Total", data[0], data[1], data[2] + " %", data[3], data[4] + " %", data[5], data[6] + " %"));
                    bodyMessage.Append(string.Format(table, stringROWSforLocation));
                }
                return bodyMessage.ToString();
            }
        }

        private static string getDataForKPIManagers()
        {
            StringBuilder builder = new StringBuilder();
            List<Resource> managersList = new List<Resource>();

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

                var locationList = context.Locations.ToList<Location>();

                foreach (var location in locationList)
                {
                    builder.Append("<BR>" + location.Name.ToUpper());

                    builder.Append(generateRowForKPIManagers(managersFilteredList.
                        Where(x => x.Location.abbreviation == location.abbreviation).OrderBy(x => x.DomainName).ToList()));

                }
                return builder.ToString();
            }
        }

        private static string generateRowForKPI(List<Objective> Objectives, string term, ref double[] data)
        {
            int counter = 0;
            const string rowFormat = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>";
            if (Objectives.Count != 0)
            {
                //int goals = Objectives.Select(x => x.ResourceId).Distinct().Count();
                int goals = Objectives.Select(x => counter += x.GoalTracking.Count).Last();
                counter = 0;
                int toDo = Objectives.Select(x => counter += x.GoalTracking.Where(y => y.Progress == 0).Count()).Last();
                counter = 0;
                double toDoPercent = Math.Round((double)(toDo * 100) / goals, 2);
                int inProgress = Objectives.Select(x => counter += x.GoalTracking.Where(y => y.Progress == 1).Count()).Last();
                counter = 0;
                double inProgressPercent = Math.Round((double)(inProgress * 100) / goals);
                int completed = Objectives.Select(x => counter += x.GoalTracking.Where(y => y.Progress == 2).Count()).Last();
                double completedPercent = Math.Round((double)(completed * 100) / goals);
                data[0] = data[0] + goals;
                data[1] = data[1] + toDo;
                data[3] = data[3] + inProgress;
                data[5] = data[5] + completed;
                return string.Format(rowFormat, term, goals, toDo, toDoPercent + " %", inProgress, inProgressPercent + " %", completed, completedPercent + " %");
            }
            else
            {
                return string.Format(rowFormat, term, 0, 0, 0, 0, 0, 0, 0);
            }
        }

        private static string generateRowForKPIManagers(List<Resource> managers)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = getActiveDirectoryInstance();
            StringBuilder tableRows = new StringBuilder();
            int quarter = getQuarter(DateTime.Now);
            if (managers.Count != 0)
            {
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
                                    teamMemberswithCDP++;
                                }
                            }
                        }
                        if (teamMembers > 0)
                        {
                            tableRows.Append(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", StringHelper.ToTitleCase(manager.DomainName), teamMembers, teamMemberswithCDP));
                        }
                    }
                    return string.Format("<BR><table border=\"1\" bgcolor=\"#efefef\"><tr bgcolor=\"#c6daf8\"><th>Manager</th><th>Team members</th><th>Team members with CDP</th></tr>{0}</table><BR>", tableRows);
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
