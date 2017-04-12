using CDPTrackerSite.DataAccessor;
using DataSource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDPTrackerSite.Controllers
{
    public class SurveyController : Controller
    {
        // GET: Survy
        public ActionResult Index()
        {
            return View();
        }

    }
}
