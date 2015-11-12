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
    public class SystemTicketTemplatesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SystemTicketTemplates
        public async Task<ActionResult> Index()
        {
            return View(await db.SystemTicketTemplates.ToListAsync());
        }

        // GET: SystemTicketTemplates/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemTicketTemplate systemTicketTemplate = await db.SystemTicketTemplates.FindAsync(id);
            if (systemTicketTemplate == null)
            {
                return HttpNotFound();
            }
            return View(systemTicketTemplate);
        }

        // GET: SystemTicketTemplates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemTicketTemplates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SystemQueryTemplateId,Description,Text,Active")] SystemTicketTemplate systemTicketTemplate)
        {
            if (ModelState.IsValid)
            {
                db.SystemTicketTemplates.Add(systemTicketTemplate);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(systemTicketTemplate);
        }

        // GET: SystemTicketTemplates/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemTicketTemplate systemTicketTemplate = await db.SystemTicketTemplates.FindAsync(id);
            if (systemTicketTemplate == null)
            {
                return HttpNotFound();
            }
            return View(systemTicketTemplate);
        }

        // POST: SystemTicketTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SystemQueryTemplateId,Description,Text,Active")] SystemTicketTemplate systemTicketTemplate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemTicketTemplate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(systemTicketTemplate);
        }

        // GET: SystemTicketTemplates/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemTicketTemplate systemTicketTemplate = await db.SystemTicketTemplates.FindAsync(id);
            if (systemTicketTemplate == null)
            {
                return HttpNotFound();
            }
            return View(systemTicketTemplate);
        }

        // POST: SystemTicketTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SystemTicketTemplate systemTicketTemplate = await db.SystemTicketTemplates.FindAsync(id);
            db.SystemTicketTemplates.Remove(systemTicketTemplate);
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
