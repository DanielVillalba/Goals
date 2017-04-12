using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataSource;

namespace CDPTrackerSite.Controllers
{ 
    public class SkillCompassController : Controller
    {
        private CDPTrackEntities db = new CDPTrackEntities();

        //
        // GET: /SkillCompass/

        public ViewResult Index()
        {
            var skillcompassglossary = db.SkillCompassGlossary.Include(s => s.Area);
            return View(skillcompassglossary.ToList());
        }

        //
        // GET: /SkillCompass/Details/5

        public ViewResult Details(int id)
        {
            SkillCompassGlossary skillcompassglossary = db.SkillCompassGlossary.Find(id);
            return View(skillcompassglossary);
        }

        //
        // GET: /SkillCompass/Create

        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.Area, "AreaId", "Name");
            return View();
        } 

        //
        // POST: /SkillCompass/Create

        [HttpPost]
        public ActionResult Create(SkillCompassGlossary skillcompassglossary)
        {
            if (ModelState.IsValid)
            {
                db.SkillCompassGlossary.Add(skillcompassglossary);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.AreaId = new SelectList(db.Area, "AreaId", "Name", skillcompassglossary.AreaId);
            return View(skillcompassglossary);
        }
        
        //
        // GET: /SkillCompass/Edit/5
 
        public ActionResult Edit(int id)
        {
            SkillCompassGlossary skillcompassglossary = db.SkillCompassGlossary.Find(id);
            ViewBag.AreaId = new SelectList(db.Area, "AreaId", "Name", skillcompassglossary.AreaId);
            return View(skillcompassglossary);
        }

        //
        // POST: /SkillCompass/Edit/5

        [HttpPost]
        public ActionResult Edit(SkillCompassGlossary skillcompassglossary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(skillcompassglossary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Area, "AreaId", "Name", skillcompassglossary.AreaId);
            return View(skillcompassglossary);
        }

        //
        // GET: /SkillCompass/Delete/5
 
        public ActionResult Delete(int id)
        {
            SkillCompassGlossary skillcompassglossary = db.SkillCompassGlossary.Find(id);
            return View(skillcompassglossary);
        }

        //
        // POST: /SkillCompass/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            SkillCompassGlossary skillcompassglossary = db.SkillCompassGlossary.Find(id);
            db.SkillCompassGlossary.Remove(skillcompassglossary);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}