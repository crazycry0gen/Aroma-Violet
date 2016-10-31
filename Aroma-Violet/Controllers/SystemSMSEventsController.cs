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
    public class SystemSMSEventsController : Controller
    {
        private AromaContext db = new AromaContext();
        private Guid _adminUserId = Guid.Parse("94e870fc-134d-4a7d-8191-3e7acb20c723");

        // GET: SystemSMSEvents
        public async Task<ActionResult> Index()
        {
            var systemSMSEvents = db.SystemSMSEvents.Include(s => s.SystemSMSTemplate);
            return View(await systemSMSEvents.ToListAsync());
        }

       

        // GET: SystemSMSEvents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMSEvent systemSMSEvent = await db.SystemSMSEvents.FindAsync(id);
            if (systemSMSEvent == null)
            {
                return HttpNotFound();
            }
            return View(systemSMSEvent);
        }

        // GET: SystemSMSEvents/Create
        public ActionResult Create()
        {
            ViewBag.SystemSMSTemplateId = new SelectList(db.SystemSMSTemplates, "SystemSMSTemplateId", "Description");
            return View();
        }

        // POST: SystemSMSEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemSMSEventId,SystemSMSEventName,SystemSMSTemplateId,Active")] SystemSMSEvent systemSMSEvent)
        {
            if (ModelState.IsValid)
            {
                db.SystemSMSEvents.Add(systemSMSEvent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.SystemSMSTemplateId = new SelectList(db.SystemSMSTemplates, "SystemSMSTemplateId", "Description", systemSMSEvent.SystemSMSTemplateId);
            return View(systemSMSEvent);
        }

        // GET: SystemSMSEvents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMSEvent systemSMSEvent = await db.SystemSMSEvents.FindAsync(id);
            if (systemSMSEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.SystemSMSTemplateId = new SelectList(db.SystemSMSTemplates, "SystemSMSTemplateId", "Description", systemSMSEvent.SystemSMSTemplateId);
            return View(systemSMSEvent);
        }

        // POST: SystemSMSEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemSMSEventId,SystemSMSEventName,SystemSMSTemplateId,Active")] SystemSMSEvent systemSMSEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemSMSEvent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SystemSMSTemplateId = new SelectList(db.SystemSMSTemplates, "SystemSMSTemplateId", "Description", systemSMSEvent.SystemSMSTemplateId);
            return View(systemSMSEvent);
        }

        // GET: SystemSMSEvents/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemSMSEvent systemSMSEvent = await db.SystemSMSEvents.FindAsync(id);
            if (systemSMSEvent == null)
            {
                return HttpNotFound();
            }
            return View(systemSMSEvent);
        }

        // POST: SystemSMSEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SystemSMSEvent systemSMSEvent = await db.SystemSMSEvents.FindAsync(id);
            db.SystemSMSEvents.Remove(systemSMSEvent);
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
