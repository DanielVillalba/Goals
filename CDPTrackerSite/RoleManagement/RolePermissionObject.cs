using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDPTrackerSite.RoleManagement
{
    public class RolePermissionObjectViewBag
    {
        public IQueryable<RolePermissionObject> Permissions;
    }


    public class RolePermissionObject
    {
       public int GroupId { get; set; }
       public string Section { get; set; }
       public bool Allow { get; set; }
    }
}