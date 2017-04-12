using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace CDPTrackerSite.RoleManagement
{
    public class RoleAuthorizationAttribute : AuthorizeAttribute
    {
        private readonly List<Role> mRoles=new List<Role>();
        
        public RoleAuthorizationAttribute(params Role[] roles)
        {
            foreach (var role in roles)
            {
                mRoles.Add(role);
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized) return false;
            return mRoles.Any(role=>RoleManagementHelper.UserIsInRole(httpContext.User, role));
        }

        
    }
}