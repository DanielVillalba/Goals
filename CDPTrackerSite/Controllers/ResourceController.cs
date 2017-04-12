using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDPTrackerSite.DataAccessor;
using DataSource;
using Utils;

namespace CDPTrackerSite.Controllers
{ 
    public class ResourceController : Controller
    {
        public const string IsRedirectBycode = "IsRedirectBycode";
        
        public ActionResult SetupResourceData()
        {
            if (TempData[IsRedirectBycode] == null)
            {
                throw new HttpException(404, "NotFound");
            }
            string userName = User.Identity.Name.StripDomain();
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                if (context.Resources.Any(r => userName.Contains(r.DomainName)))
                {
#if !DEBUG
                    return RedirectToAction("Index", "GoalTracking");
#endif
                }
            }

            ViewBag.IsResourceSetup = true;

            Resource resource;
            if(!ResourceDataAccessor.TryGetDefaultResourceFromActiveDirectoryData(userName, out resource))
            {
                return View("ResourceSetupError");
            }
            ViewBag.ActiveDirectoryId = resource.ActiveDirectoryId;

            return View(resource);
        }

        [HttpPost]
        public ActionResult SetupResourceData(Resource resource)
        {
            bool isSaved = false;
            for (int i = 0; i < 3 && !isSaved; i++)
            {
                try
                {
                    using (CDPTrackEntities context = new CDPTrackEntities())
                    {
                        //if the domain name is already in the resource table, then break the save loop and go to default
                        //since the domain name will be found and usable
                        if(context.Resources.Any(r=>r.DomainName == resource.DomainName))
                        {
                            return RedirectToAction("Index", "GoalTracking");
                        }
                        resource.ResourceId = context.Resources.Max(r => r.ResourceId) + 1;
                        resource.LastLogin = DateTime.Now;

                        string currentPosition = resource.Employee.CurrentPosition;
                        resource.Employee = new Employee { ResourceId = resource.ResourceId, Resource = resource, CurrentPosition = currentPosition, AspiringPosition = "UNKNOWN",CurrentPositionID = resource.Employee.CurrentPositionID};

                        if (!positionExists(currentPosition)) {
                            insertNewPosition(currentPosition);
                        }

                        context.Resources.Add(resource);
                        context.SaveChanges();
                        isSaved = true;
                    }
                }
                catch (DbUpdateException a)
                {
                }
                catch (Exception e)
                {
                    ErrorLogHelper.LogException(e, "CDPTracker");
                    break;
                }
            }

            if(!isSaved)
            {
                return View("ResourceSetupError");
            }

            return RedirectToAction("Index", "GoalTracking");
        }


        public ActionResult FailedSetup()
        {
            return View("ResourceSetupError");
        }

        private bool positionExists(string currentPosition)
        {
            using (CDPTrackEntities context = new CDPTrackEntities())
            {
                var matchPositions = context.Position.Where(a => a.PositionName == currentPosition).ToList();

                if (matchPositions.Count > 0)
                    return true;
            }

            return false;
        }

        private void insertNewPosition(string newPosition)
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


                }
            }
            catch(DbUpdateException error){
                ErrorLogHelper.LogException(error, "CDPTracker");
            }
        }
    }
}