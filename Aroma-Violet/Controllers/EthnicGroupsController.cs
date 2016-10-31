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
    public class EthnicGroupsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: EthnicGroups
        public async Task<ActionResult> Index()
        {
            return View(await db.EthnicGroups.OrderBy(m => m.EthnicGroupName).ToListAsync());
        }

        // GET: EthnicGroups/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EthnicGroup ethnicGroup = await db.EthnicGroups.FindAsync(id);
            if (ethnicGroup == null)
            {
                return HttpNotFound();
            }
            return View(ethnicGroup);
        }

        // GET: EthnicGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EthnicGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EthnicGroupId,EthnicGroupName,Active")] EthnicGroup ethnicGroup)
        {
            if (ModelState.IsValid)
            {
                db.EthnicGroups.Add(ethnicGroup);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ethnicGroup);
        }

        // GET: EthnicGroups/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EthnicGroup ethnicGroup = await db.EthnicGroups.FindAsync(id);
            if (ethnicGroup == null)
            {
                return HttpNotFound();
            }
            return View(ethnicGroup);
        }

        // POST: EthnicGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EthnicGroupId,EthnicGroupName,Active")] EthnicGroup ethnicGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ethnicGroup).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ethnicGroup);
        }

        // GET: EthnicGroups/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EthnicGroup ethnicGroup = await db.EthnicGroups.FindAsync(id);
            if (ethnicGroup == null)
            {
                return HttpNotFound();
            }
            return View(ethnicGroup);
        }

        // POST: EthnicGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EthnicGroup ethnicGroup = await db.EthnicGroups.FindAsync(id);
            db.EthnicGroups.Remove(ethnicGroup);
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
