using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;


namespace Utils
{
    public class EmployeeActiveDirectoryManager
    {

        #region Private Attributes
        private const string LogonNameField = "sAMAccountName"; //CN
        private const string AccountControl = "userAccountControl";
        private const int EnabledFlag = 0x2;
        private readonly string mPath, mUser, mPassword;
        private readonly DirectoryEntry mDirectoryEntry;
        private readonly DirectorySearcher mDirectorySearcher;
        #endregion

        #region Public Constructors
        public EmployeeActiveDirectoryManager(string pPath, string pUser, string pPassword)
        {
            mPath = pPath;
            mUser = pUser;
            mPassword = pPassword;
            mDirectoryEntry = new DirectoryEntry(mPath);
            mDirectorySearcher = new DirectorySearcher(mDirectoryEntry);
        }
        #endregion

        #region Public Static Methods
        public static bool IsEnabled(DirectoryEntry directoryEntry)
        {
            int accControl = (int)directoryEntry.Properties[AccountControl].Value;
            return (accControl & EnabledFlag) == 0;
        }

        public static bool IsIntern(DirectoryEntry directoryEntry)
        {
            bool booFlag = Convert.ToString(directoryEntry.Properties["physicalDeliveryOfficeName"].Value) == "HMOINT";
            return booFlag;
        }

        #endregion

        #region Public Methods
        public  bool IsEnabled(string pName)
        {
            //if (pName == "julio escoboza")
            //{
            //    Console.Write(true);
            //}
            //mDirectorySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(CN=" + pName + "))";
            //SearchResultCollection results = mDirectorySearcher.FindAll();
            //Console.WriteLine((int)results[0].Properties["useraccountControl"][0]);


            mDirectorySearcher.Filter = "(&(objectCategory=person)(objectClass=user)(!userAccountControl:1.2.840.113556.1.4.803:=2)(sAMAccountName=" + pName + "))";
            SearchResultCollection results = mDirectorySearcher.FindAll();

            DirectoryEntry de = results[0].GetDirectoryEntry();
            //if (de == null)
            //{
            //    //throw new ArgumentException("The user does not exist.");
            //    return false;
            //}
            //int accControl = (int)de.Properties[AccountControl].Value;
            ////return (accControl & EnabledFlag) == 0;
            return true;
        }

        public IEnumerable<string> GetDirectoryEntryOrganizationalUnitList(DirectoryEntry directoryEntry)
        {
            const string part = "OU=";
            string[] commaSplit = directoryEntry.Path.Split(',');
            return (from split in commaSplit where split.Contains(part) select split.Substring(part.Length)).ToList();
        }

        public List<DirectoryEntry> GetDirectoryEntriesInOrganizationalUnit(string organizationalUnit)
        {
            mDirectorySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(OU=" + organizationalUnit + "))";
            SearchResultCollection results = mDirectorySearcher.FindAll();
            List<DirectoryEntry> list = results.OfType<SearchResult>().Select(e => e.GetDirectoryEntry()).ToList();
            return list;
        }

        public IEnumerable<string> GetEmployeeOrganizationalUnitList(string pName)
        {
            DirectoryEntry de = GetEmployeeByLogonName(pName);
            if (de == null)
            {
                throw new ArgumentException("The user does not exist.");
            }
            return GetDirectoryEntryOrganizationalUnitList(de);
        }

        public DirectoryEntry GetEmployeeByCommonName(string pName)
        {
            mDirectorySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(CN=" + pName + "))";
            SearchResultCollection results = mDirectorySearcher.FindAll();
            if (results.Count == 0)
            {
                return null;
            }
            return results[0].GetDirectoryEntry();
        }

        public DirectoryEntry GetEmployeeByLogonName(string pLogonName)
        {
            SearchResultCollection results;
            try
            {
                mDirectorySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(" + LogonNameField + "=" + pLogonName + "))";
                results = mDirectorySearcher.FindAll();
                if (results != null)
                    if (results.Count == 0)
                    {
                        return null;
                    }
            
            }
            catch(Exception e)
            {
                ErrorLogHelper.LogException(e, "CDPTracker");
                return null;
            }
            return results[0].GetDirectoryEntry();
        }

        public SearchResultCollection GetEmployeeByProperty(string property, string value)
        {
            mDirectorySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(" + property + "=" + value + "))";
            SearchResultCollection results = mDirectorySearcher.FindAll();
            return results;
        }

        public List<DirectoryEntry> GetAllTiempoEmployees()
        {
            //mDirectorySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(|(physicalDeliveryOfficeName=HMO)(physicalDeliveryOfficeName=MTY)))";
            mDirectorySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(|(physicalDeliveryOfficeName=HMO)(physicalDeliveryOfficeName=PHX)(physicalDeliveryOfficeName=STO)(physicalDeliveryOfficeName=MTY)(physicalDeliveryOfficeName=GDL)))";
            //mDirectorySearcher.PropertiesToLoad.Add("mail");
            SearchResultCollection results = mDirectorySearcher.FindAll();
            List<DirectoryEntry> list = results.OfType<SearchResult>().Select(e => e.GetDirectoryEntry()).ToList();
            return list;
        }

        public List<DirectoryEntry> GetAllTiempoInternals()
        {
            mDirectorySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(physicalDeliveryOfficeName=HMOINT))";
            SearchResultCollection results = mDirectorySearcher.FindAll();
            List<DirectoryEntry> list = results.OfType<SearchResult>().Select(e => e.GetDirectoryEntry()).ToList();
            return list;
        }

        public string[] GetAllOperationalUnits()
        {
            List<DirectoryEntry> list = GetAllTiempoEmployees();
            string[] operationalUnits = list.SelectMany(GetDirectoryEntryOrganizationalUnitList).Distinct().ToArray();
            return operationalUnits;
        }

        public string[] FindEmployeesInOperationalUnit(string operationalUnit, string pNameToMatch)
        {
            List<DirectoryEntry> list = GetDirectoryEntriesInOrganizationalUnit(operationalUnit);
            string[] emps = list.Where(l => l.Properties[LogonNameField].Value.ToString().Contains(pNameToMatch)).Select(l => l.Properties[LogonNameField].Value.ToString()).ToArray();
            return emps;
        }

        public string[] GetEmployeesInOperationalUnit(string operationalUnit)
        {
            List<DirectoryEntry> list = GetDirectoryEntriesInOrganizationalUnit(operationalUnit);
            string[] emps = list.Select(l => l.Properties[LogonNameField].Value.ToString()).ToArray();
            return emps;
        }

        public int GetEmployeeNumber(string pEmployeeLoggonName)
        {
            int employeeNumber;
            DirectoryEntry de = GetEmployeeByLogonName(pEmployeeLoggonName);
            object value = de.Properties["initials"].Value;
            if (value == null)
                throw new ArgumentException("The user provided has not set an ID in the expected field.");
            if (!int.TryParse(value.ToString(), out employeeNumber))
                throw new ArgumentException("The user provided has not set a numeric ID in the expected field.");
            return employeeNumber;
        }

        public bool GetResourceIsManager(string userLogonName)
        {
            DirectoryEntry manager = GetEmployeeByLogonName(userLogonName);
            PropertyValueCollection directReports = manager.Properties["directReports"];
            object[] assignedResources = directReports.Value as object[];
            return (assignedResources != null && assignedResources.Length > 0);
        }

        public List<String> GetManagerResourcesDomainNames(string managerLogonName)
        {
            DirectoryEntry manager = GetEmployeeByLogonName(managerLogonName);
            string distinguishedName = manager.Properties["distinguishedName"].Value.ToString();

            var employees = GetEmployeeByProperty("manager", distinguishedName);
            List<string> domainNames =
                (from SearchResult employee in employees
                 select employee.GetDirectoryEntry() into entry
                 select new {DomainName = entry.Properties["sAMAccountName"].Value.ToString(),
                 OperationalUnits = GetDirectoryEntryOrganizationalUnitList(entry)})
                 .Where(user=>!user.OperationalUnits.Any(
                    ou =>
                    Constants.ExcludedOperationUnits.Any(eou => string.Compare(ou, eou, StringComparison.OrdinalIgnoreCase) == 0)))
                 .Select(user=>user.DomainName).ToList();

            return domainNames;
        }

        public bool GetUserContainsPropertyValue(string logonName, string propertyName, string value)
        {
            DirectoryEntry user = GetEmployeeByLogonName(logonName);
            if (user == null) return false;

            object[] collectionByProperty = user.Properties[propertyName].Value as object[];
            if (collectionByProperty == null) return false;

            return collectionByProperty.Any(m => (m is string) && (m as string).IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public string GetUserPropertyValue(string logonName, string propertyName)
        {
            DirectoryEntry foundUser = GetEmployeeByLogonName(logonName);
            if (foundUser == null) return string.Empty;

            PropertyValueCollection property = foundUser.Properties[propertyName];
            if (property == null) return string.Empty;

            return property.Value as string;
        }

        public class UserData
        {
            public string Name { get; set; }
            public string DomainName { get; set; }
            public string Location { get; set; }
            public string Manager { get; set; }
            public string Email { get; set; }
            public string Position { get; set; }
            public IEnumerable<string> OperationalUnits { get; set; }
        }

        protected string GetPropertyValue(DirectoryEntry entry, string propertyName)
        {
            if (!entry.Properties.Contains(propertyName)) return string.Empty;
            object value = entry.Properties[propertyName].Value;
           
            return value is string ? value.ToString() : string.Empty;
        }

        public List<UserData> GetUserList()
        {
            List<DirectoryEntry> directoryEntries = GetAllTiempoEmployees();

            List<UserData> result = directoryEntries.Select(d => new UserData
            {
                Name = GetPropertyValue(d, "cn"),
                DomainName = GetPropertyValue(d, "sAMAccountName"),
                Location = GetPropertyValue(d, "physicalDeliveryOfficeName"),
                Manager = GetPropertyValue(d, "manager"),
                Email = GetPropertyValue(d, "mail"),
                Position = GetPropertyValue(d, "description"),
                OperationalUnits = GetDirectoryEntryOrganizationalUnitList(d)
            }).ToList();
            
            return result;
        }

        public static bool GetIfIsActiveEmployeeFromAD(string logonName, EmployeeActiveDirectoryManager ActiveDirectoryInstance)
        {
            EmployeeActiveDirectoryManager ActiveDirectory = ActiveDirectoryInstance;

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
                ErrorLogHelper.LogException(e, "CDPTracker");
                return false;
            }

            return (OUFromEmployee.Count > 0);
        }
        #endregion

    }//end EmployeeADMng

}//end namespace TiemoperationalUnittilities