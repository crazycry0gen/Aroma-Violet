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
    public class SalesTypesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: SalesTypes
        public async Task<ActionResult> Index()
        {
            var salesTypes = db.SalesTypes.OrderBy(m => m.SalesTypeDescription).Include(s => s.SourceAccount).Include(s=>s.DefaultShippingType);
            return View(await salesTypes.ToListAsync());
        }

        // GET: SalesTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesType salesType = await db.SalesTypes.FindAsync(id);
            if (salesType == null)
            {
                return HttpNotFound();
            }
            return View(salesType);
        }

        // GET: SalesTypes/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts.Where(m=>!m.IsSystemAccount && m.Active), "AccountId", "AccountName");
            ViewBag.DefaultShippingTypeId = new SelectList(db.ShippingTypes, "ShippingTypeId", "ShippingTypeName");
            var salesType = new SalesType() {Active=true };
            return View(salesType);
        }

        // POST: SalesTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SalesTypeId,SalesTypeDescription,Active,AccountId,DefaultShippingTypeId")] SalesType salesType)
        {
            if (ModelState.IsValid)
            {
                db.SalesTypes.Add(salesType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts.Where(m => !m.IsSystemAccount && m.Active), "AccountId", "AccountName", salesType.AccountId);
            ViewBag.DefaultShippingTypeId = new SelectList(db.ShippingTypes, "ShippingTypeId", "ShippingTypeName");

            return View(salesType);
        }

        // GET: SalesTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesType salesType = await db.SalesTypes.FindAsync(id);
            if (salesType == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "AccountId", "AccountName", salesType.AccountId);
            ViewBag.DefaultShippingTypeId = new SelectList(db.ShippingTypes, "ShippingTypeId", "ShippingTypeName");

            return View(salesType);
        }

        // POST: SalesTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "SalesTypeId,SalesTypeDescription,Active,AccountId,DefaultShippingTypeId")] SalesType salesType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salesType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "AccountId", "AccountName", salesType.AccountId);
            ViewBag.DefaultShippingTypeId = new SelectList(db.ShippingTypes, "ShippingTypeId", "ShippingTypeName");

            return View(salesType);
        }

        // GET: SalesTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesType salesType = await db.SalesTypes.FindAsync(id);
            if (salesType == null)
            {
                return HttpNotFound();
            }
            return View(salesType);
        }

        // POST: SalesTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            SalesType salesType = await db.SalesTypes.FindAsync(id);
            db.SalesTypes.Remove(salesType);
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
