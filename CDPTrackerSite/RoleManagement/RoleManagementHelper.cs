using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using CDPTrackerSite.DataAccessor;
using Utils;
using DataSource;

namespace CDPTrackerSite.RoleManagement
{
    public enum Role
    {
        Executive,
        TalentManagement,
        Manager,
        Employee,
    }

    public class RoleManagementHelper
    {
        private static readonly Dictionary<Role, string[]> RolePermissions = new Dictionary<Role, string[]>
                                                                         {
                                                                             {Role.Executive, new []{"tdg-moss-executives"}},
                                                                             {Role.TalentManagement, new []{"tdg-moss-talent management", "tdg-moss-administrators"}},
                                                                             {Role.Manager, new []{"tdg-moss-pm", "Manager Level Access"}},
                                                                             {Role.Employee, new []{"tdg-moss-td"}},
                                                                         };

        public static bool UserIsInRole(IPrincipal user, Role role)
        {
            string[] groups;
            if (!RolePermissions.TryGetValue(role, out groups))
            {
                return false;
            }
            if (role == Role.Manager)
            {
                //direct validation for user as manager to handle the Tripwire Srs case
                return groups.Any(user.IsInRole) || UserIsManager(user);
            }

            return groups.Any(user.IsInRole);
        }
        private static bool UserIsManager(IPrincipal user)
        {
            return ResourceDataAccessor.IsManager(user.Identity.Name.StripDomain());
        }

        public static List<RolePermissionObject> UserSeccionAccessAllActive(int resourceId)
        {

            List<RolePermissionObject> retList = new List<RolePermissionObject>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var groupforUser = ProfileManager.ProfileManager.GetGroupsByResourseId(resourceId);
                List<int> groupIds = new List<int>();
                foreach (var s in groupforUser)
                {
                    var groupsforID = context.Groups.Where(x => x.GroupName == s.GroupId).FirstOrDefault();
                    if (groupsforID != null)
                        groupIds.Add(groupsforID.GroupId);
                }
                foreach (var tempGroup in groupIds)
                    retList.AddRange(context.Group_SectionAccess.Where(x => x.GroupId == tempGroup && x.Allow == true).Select(s => new RolePermissionObject { GroupId = s.GroupId, Section = s.Section, Allow = s.Allow }).ToList());
            }
            return retList;
        }

        public static List<RolePermissionObject> UserSeccionAccessByseccion(int resourceId, string section)
        {

            List<RolePermissionObject> retList = new List<RolePermissionObject>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var groupforUser = ProfileManager.ProfileManager.GetGroupsByResourseId(resourceId);
                List<int> groupIds = new List<int>();
                foreach (var s in groupforUser)
                {
                    var groupsforID = context.Groups.Where(x => x.GroupName == s.GroupId).FirstOrDefault();
                    if (groupsforID != null)
                        groupIds.Add(groupsforID.GroupId);
                }
                foreach (var tempGroup in groupIds)
                    retList.AddRange(context.Group_SectionAccess.Where(x => x.GroupId == tempGroup && x.Section.Contains(section) && x.Allow == true).Select(s => new RolePermissionObject { GroupId = s.GroupId, Section = s.Section, Allow = s.Allow }).ToList());
            }
            return retList;
        }

        
    }
}