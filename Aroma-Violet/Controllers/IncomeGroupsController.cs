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
    public class IncomeGroupsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: IncomeGroups
        public async Task<ActionResult> Index()
        {
            return View(await db.IncomeGroups.ToListAsync());
        }

        // GET: IncomeGroups/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IncomeGroup incomeGroup = await db.IncomeGroups.FindAsync(id);
            if (incomeGroup == null)
            {
                return HttpNotFound();
            }
            return View(incomeGroup);
        }

        // GET: IncomeGroups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IncomeGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IncomeGroupId,IncomeGroupName,Active")] IncomeGroup incomeGroup)
        {
            if (ModelState.IsValid)
            {
                db.IncomeGroups.Add(incomeGroup);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(incomeGroup);
        }

        // GET: IncomeGroups/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IncomeGroup incomeGroup = await db.IncomeGroups.FindAsync(id);
            if (incomeGroup == null)
            {
                return HttpNotFound();
            }
            return View(incomeGroup);
        }

        // POST: IncomeGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IncomeGroupId,IncomeGroupName,Active")] IncomeGroup incomeGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(incomeGroup).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(incomeGroup);
        }

        // GET: IncomeGroups/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IncomeGroup incomeGroup = await db.IncomeGroups.FindAsync(id);
            if (incomeGroup == null)
            {
                return HttpNotFound();
            }
            return View(incomeGroup);
        }

        // POST: IncomeGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            IncomeGroup incomeGroup = await db.IncomeGroups.FindAsync(id);
            db.IncomeGroups.Remove(incomeGroup);
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
