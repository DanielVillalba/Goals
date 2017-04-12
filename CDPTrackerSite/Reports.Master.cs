using System;
using System.Security.Principal;
using System.Web;

namespace CDPTrackerSite
{
    public partial class Reports : System.Web.UI.MasterPage
    {
        public IPrincipal User
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
     
        protected void Page_Load(object sender, EventArgs e)
        {
            int ok = 0;
        }
    }
}