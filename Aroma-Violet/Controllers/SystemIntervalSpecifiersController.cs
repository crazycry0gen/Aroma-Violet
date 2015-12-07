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
    public class SystemIntervalSpecifiersController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SystemIntervalSpecifiers
        public async Task<ActionResult> Index()
        {
            return View(await db.SystemIntervalSpecifiers.ToListAsync());
        }

        // GET: SystemIntervalSpecifiers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemIntervalSpecifier systemIntervalSpecifier = await db.SystemIntervalSpecifiers.FindAsync(id);
            if (systemIntervalSpecifier == null)
            {
                return HttpNotFound();
            }
            return View(systemIntervalSpecifier);
        }

        // GET: SystemIntervalSpecifiers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemIntervalSpecifiers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IntervalSpecifierId,IntervalSpecifierName,MilisecondConverter")] SystemIntervalSpecifier systemIntervalSpecifier)
        {
            if (ModelState.IsValid)
            {
                db.SystemIntervalSpecifiers.Add(systemIntervalSpecifier);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(systemIntervalSpecifier);
        }

        // GET: SystemIntervalSpecifiers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemIntervalSpecifier systemIntervalSpecifier = await db.SystemIntervalSpecifiers.FindAsync(id);
            if (systemIntervalSpecifier == null)
            {
                return HttpNotFound();
            }
            return View(systemIntervalSpecifier);
        }

        // POST: SystemIntervalSpecifiers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IntervalSpecifierId,IntervalSpecifierName,MilisecondConverter")] SystemIntervalSpecifier systemIntervalSpecifier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemIntervalSpecifier).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(systemIntervalSpecifier);
        }

        // GET: SystemIntervalSpecifiers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemIntervalSpecifier systemIntervalSpecifier = await db.SystemIntervalSpecifiers.FindAsync(id);
            if (systemIntervalSpecifier == null)
            {
                return HttpNotFound();
            }
            return View(systemIntervalSpecifier);
        }

        // POST: SystemIntervalSpecifiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SystemIntervalSpecifier systemIntervalSpecifier = await db.SystemIntervalSpecifiers.FindAsync(id);
            db.SystemIntervalSpecifiers.Remove(systemIntervalSpecifier);
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
