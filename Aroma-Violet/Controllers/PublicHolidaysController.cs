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
    public class PublicHolidaysController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: PublicHolidays
        public async Task<ActionResult> Index()
        {
            return View(await db.PublicHolidays.ToListAsync());
        }

        // GET: PublicHolidays/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PublicHolidays publicHolidays = await db.PublicHolidays.FindAsync(id);
            if (publicHolidays == null)
            {
                return HttpNotFound();
            }
            return View(publicHolidays);
        }

        // GET: PublicHolidays/Create
        public ActionResult Create()
        {
            var ph = new PublicHolidays() {HolidayDate=DateTime.Now, HolidayDescription=string.Empty };
            return View(ph);
        }

        // POST: PublicHolidays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,HolidayDate,HolidayDescription")] PublicHolidays publicHolidays)
        {
            if (ModelState.IsValid)
            {
                db.PublicHolidays.Add(publicHolidays);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(publicHolidays);
        }

        // GET: PublicHolidays/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PublicHolidays publicHolidays = await db.PublicHolidays.FindAsync(id);
            if (publicHolidays == null)
            {
                return HttpNotFound();
            }
            return View(publicHolidays);
        }

        // POST: PublicHolidays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,HolidayDate,HolidayDescription")] PublicHolidays publicHolidays)
        {
            if (ModelState.IsValid)
            {
                db.Entry(publicHolidays).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(publicHolidays);
        }

        // GET: PublicHolidays/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PublicHolidays publicHolidays = await db.PublicHolidays.FindAsync(id);
            if (publicHolidays == null)
            {
                return HttpNotFound();
            }
            return View(publicHolidays);
        }

        // POST: PublicHolidays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PublicHolidays publicHolidays = await db.PublicHolidays.FindAsync(id);
            db.PublicHolidays.Remove(publicHolidays);
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
