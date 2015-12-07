using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aroma_Violet.Models;

namespace Aroma_Violet.Controllers
{
    public class SystemProceduresController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SystemProcedures
        public async Task<ActionResult> Index()
        {
            var systemProcedures = db.SystemProcedures.Include(s => s.IntervalSpecifier);
            return View(await systemProcedures.ToListAsync());
        }

        // GET: SystemProcedures/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemProcedure systemProcedure = await db.SystemProcedures.FindAsync(id);
            if (systemProcedure == null)
            {
                return HttpNotFound();
            }
            return View(systemProcedure);
        }

        // GET: SystemProcedures/Create
        public ActionResult Create()
        {
            ViewBag.IntervalSpecifierId = new SelectList(db.SystemIntervalSpecifiers, "IntervalSpecifierId", "IntervalSpecifierName");
            return View(new SystemProcedure() { LastRun=DateTime.Now});
        }

        // POST: SystemProcedures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemProcedureId,ProcedureName,Proceduredescription,IntervalSpecifierId,Interval,Active,LastRun")] SystemProcedure systemProcedure)
        {
            if (ModelState.IsValid)
            {
                systemProcedure.SystemProcedureId = Guid.NewGuid();
                db.SystemProcedures.Add(systemProcedure);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IntervalSpecifierId = new SelectList(db.SystemIntervalSpecifiers, "IntervalSpecifierId", "IntervalSpecifierName", systemProcedure.IntervalSpecifierId);
            return View(systemProcedure);
        }

        // GET: SystemProcedures/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemProcedure systemProcedure = await db.SystemProcedures.FindAsync(id);
            if (systemProcedure == null)
            {
                return HttpNotFound();
            }
            ViewBag.IntervalSpecifierId = new SelectList(db.SystemIntervalSpecifiers, "IntervalSpecifierId", "IntervalSpecifierName", systemProcedure.IntervalSpecifierId);
            return View(systemProcedure);
        }

        // POST: SystemProcedures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemProcedureId,ProcedureName,Proceduredescription,IntervalSpecifierId,Interval,Active,LastRun")] SystemProcedure systemProcedure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemProcedure).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IntervalSpecifierId = new SelectList(db.SystemIntervalSpecifiers, "IntervalSpecifierId", "IntervalSpecifierName", systemProcedure.IntervalSpecifierId);
            return View(systemProcedure);
        }

        // GET: SystemProcedures/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemProcedure systemProcedure = await db.SystemProcedures.FindAsync(id);
            if (systemProcedure == null)
            {
                return HttpNotFound();
            }
            return View(systemProcedure);
        }

        // POST: SystemProcedures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            SystemProcedure systemProcedure = await db.SystemProcedures.FindAsync(id);
            db.SystemProcedures.Remove(systemProcedure);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
