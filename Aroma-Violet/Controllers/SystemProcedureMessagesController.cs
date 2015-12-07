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
    public class SystemProcedureMessagesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SystemProcedureMessages
        public async Task<ActionResult> Index(Guid? systemProcedureId)
        {
            var systemProcedureMessages = db.SystemProcedureMessages.Include(s => s.SystemProcedure);
            if (systemProcedureId.HasValue)
            {
                systemProcedureMessages = systemProcedureMessages.Where(m => m.SystemProcedureId.ToString() == systemProcedureId.ToString());
            }
            return View(await systemProcedureMessages.OrderByDescending(m=>m.MessageDate).Take(20).ToListAsync());
        }

        // GET: SystemProcedureMessages/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemProcedureMessage systemProcedureMessage = await db.SystemProcedureMessages.FindAsync(id);
            if (systemProcedureMessage == null)
            {
                return HttpNotFound();
            }
            return View(systemProcedureMessage);
        }

        // GET: SystemProcedureMessages/Create
        public ActionResult Create()
        {
            ViewBag.SystemProcedureId = new SelectList(db.SystemProcedures, "SystemProcedureId", "ProcedureName");
            return View(new SystemProcedureMessage() { MessageDate=DateTime.Now});
        }

        // POST: SystemProcedureMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemProcedureMessageId,SystemProcedureId,MessageDate,Message")] SystemProcedureMessage systemProcedureMessage)
        {
            if (ModelState.IsValid)
            {
                systemProcedureMessage.SystemProcedureMessageId = Guid.NewGuid();
                db.SystemProcedureMessages.Add(systemProcedureMessage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.SystemProcedureId = new SelectList(db.SystemProcedures, "SystemProcedureId", "ProcedureName", systemProcedureMessage.SystemProcedureId);
            return View(systemProcedureMessage);
        }

        // GET: SystemProcedureMessages/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemProcedureMessage systemProcedureMessage = await db.SystemProcedureMessages.FindAsync(id);
            if (systemProcedureMessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.SystemProcedureId = new SelectList(db.SystemProcedures, "SystemProcedureId", "ProcedureName", systemProcedureMessage.SystemProcedureId);
            return View(systemProcedureMessage);
        }

        // POST: SystemProcedureMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemProcedureMessageId,SystemProcedureId,MessageDate,Message")] SystemProcedureMessage systemProcedureMessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemProcedureMessage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SystemProcedureId = new SelectList(db.SystemProcedures, "SystemProcedureId", "ProcedureName", systemProcedureMessage.SystemProcedureId);
            return View(systemProcedureMessage);
        }

        // GET: SystemProcedureMessages/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemProcedureMessage systemProcedureMessage = await db.SystemProcedureMessages.FindAsync(id);
            if (systemProcedureMessage == null)
            {
                return HttpNotFound();
            }
            return View(systemProcedureMessage);
        }

        // POST: SystemProcedureMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            SystemProcedureMessage systemProcedureMessage = await db.SystemProcedureMessages.FindAsync(id);
            db.SystemProcedureMessages.Remove(systemProcedureMessage);
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
