using DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDPTrackerSite.Services;

namespace CDPTrackerSite.Controllers
{
    public class QuarterlyPrioritiesController : Controller
    {
        // GET: QuarterlyPriorities
        public ActionResult ManagerPriorities()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ManagerPrioritiesDirector()
        {
            QuaterlyPrioritiesService service = new QuaterlyPrioritiesService();
            return View(service.GetQuarterlyPrioritiesByDirector(681, 1));
        }
    }
}