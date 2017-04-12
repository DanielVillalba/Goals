using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDPTrackerSite.RoleManagement
{
    public class ComplexRoleAuthorization : AuthorizeAttribute
    {
        private readonly List<Role> mRoles = new List<Role>();

        public ComplexRoleAuthorization(params Role[] roles)
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
            return mRoles.All(role => RoleManagementHelper.UserIsInRole(httpContext.User, role));
        }
    }
}