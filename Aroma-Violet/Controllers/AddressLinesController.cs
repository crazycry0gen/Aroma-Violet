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
    public class AddressLinesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: AddressLines
        public async Task<ActionResult> Index()
        {
            var addressLines = db.AddressLines.Include(a => a.Address);
            return View(await addressLines.ToListAsync());
        }

        // GET: AddressLines/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressLine addressLine = await db.AddressLines.FindAsync(id);
            if (addressLine == null)
            {
                return HttpNotFound();
            }
            return View(addressLine);
        }

        // GET: AddressLines/Create
        public ActionResult Create()
        {
            ViewBag.AddressID = new SelectList(db.Addresses, "AddressId", "Code");
            return View();
        }

        // POST: AddressLines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AddressLineId,AddressLineText,AddressID,Order,Active")] AddressLine addressLine)
        {
            if (ModelState.IsValid)
            {
                db.AddressLines.Add(addressLine);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AddressID = new SelectList(db.Addresses, "AddressId", "Code", addressLine.AddressID);
            return View(addressLine);
        }

        // GET: AddressLines/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressLine addressLine = await db.AddressLines.FindAsync(id);
            if (addressLine == null)
            {
                return HttpNotFound();
            }
            ViewBag.AddressID = new SelectList(db.Addresses, "AddressId", "Code", addressLine.AddressID);
            return View(addressLine);
        }

        // POST: AddressLines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AddressLineId,AddressLineText,AddressID,Order,Active")] AddressLine addressLine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(addressLine).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AddressID = new SelectList(db.Addresses, "AddressId", "Code", addressLine.AddressID);
            return View(addressLine);
        }

        // GET: AddressLines/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressLine addressLine = await db.AddressLines.FindAsync(id);
            if (addressLine == null)
            {
                return HttpNotFound();
            }
            return View(addressLine);
        }

        // POST: AddressLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AddressLine addressLine = await db.AddressLines.FindAsync(id);
            db.AddressLines.Remove(addressLine);
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
