using CDPTrackerSite.RoleManagement;
using DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.ProfileManager
{
    public static class ProfileManager
    {

        public static List<Employee_Groups> GetGroupsByResourseId(int resourceId)
        {
            List<Employee_Groups> retList = new List<Employee_Groups>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                retList = context.Employee_Groups.Where(x => x.ResourceId == resourceId).ToList();
            }
            return retList;
        }

        public static bool GetBelongsToGroupbyResourceid(int resourceId, string groupName)
        {
            List<Employee_Groups> retList = new List<Employee_Groups>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                retList = context.Employee_Groups.Where(x => x.ResourceId == resourceId && x.GroupId.Contains(groupName)).ToList();
            }
            return retList.Any();
        }

        public static List<RolePermissionObject> UserSeccionAccessAllActive(int resourceId)
        {

            List<RolePermissionObject> retList = new List<RolePermissionObject>();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var groupforUser = GetGroupsByResourseId(resourceId);
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
                var groupforUser = GetGroupsByResourseId(resourceId);
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