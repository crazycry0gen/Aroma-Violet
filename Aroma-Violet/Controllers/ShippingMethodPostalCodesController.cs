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
    public class ShippingMethodPostalCodesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: ShippingMethodPostalCodes
        public async Task<ActionResult> Index()
        {
            var shippingMethodPostalCodes = db.ShippingMethodPostalCodes.Include(s => s.PostalCode).Include(s => s.ShippingMethod).Take(50);
            return View(await shippingMethodPostalCodes.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Index(string criteria)
        {
            @ViewBag.Criteria = criteria;
            var shippingMethodPostalCodes = db.ShippingMethodPostalCodes.Include(s => s.PostalCode).Include(s => s.ShippingMethod).Where(m => criteria == null || (criteria != null && m.PostalCode.PostalCodeName.Contains(criteria))).Take(50);
            return View(await shippingMethodPostalCodes.ToListAsync());
        }

        // GET: ShippingMethodPostalCodes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingMethodPostalCode shippingMethodPostalCode = await db.ShippingMethodPostalCodes.FindAsync(id);
            if (shippingMethodPostalCode == null)
            {
                return HttpNotFound();
            }
            return View(shippingMethodPostalCode);
        }

        // GET: ShippingMethodPostalCodes/Create
        public ActionResult Create()
        {
            ViewBag.PostalCodeId = new SelectList(db.PostalCodes, "PostalCodeId", "PostalCodeName");
            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods, "ShippingMethodId", "ShippingMethodName");
            return View();
        }

        // POST: ShippingMethodPostalCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ShippingMethodPostalCodeId,PostalCodeId,ShippingMethodId,Description,ExtraCost,Active")] ShippingMethodPostalCode shippingMethodPostalCode)
        {
            if (ModelState.IsValid)
            {
                db.ShippingMethodPostalCodes.Add(shippingMethodPostalCode);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PostalCodeId = new SelectList(db.PostalCodes, "PostalCodeId", "PostalCodeName", shippingMethodPostalCode.PostalCodeId);
            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods, "ShippingMethodId", "ShippingMethodName", shippingMethodPostalCode.ShippingMethodId);
            return View(shippingMethodPostalCode);
        }

        // GET: ShippingMethodPostalCodes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingMethodPostalCode shippingMethodPostalCode = await db.ShippingMethodPostalCodes.FindAsync(id);
            if (shippingMethodPostalCode == null)
            {
                return HttpNotFound();
            }
            ViewBag.PostalCodeId = new SelectList(db.PostalCodes, "PostalCodeId", "PostalCodeName", shippingMethodPostalCode.PostalCodeId);
            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods, "ShippingMethodId", "ShippingMethodName", shippingMethodPostalCode.ShippingMethodId);
            return View(shippingMethodPostalCode);
        }

        // POST: ShippingMethodPostalCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ShippingMethodPostalCodeId,PostalCodeId,ShippingMethodId,Description,ExtraCost,Active")] ShippingMethodPostalCode shippingMethodPostalCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shippingMethodPostalCode).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PostalCodeId = new SelectList(db.PostalCodes, "PostalCodeId", "PostalCodeName", shippingMethodPostalCode.PostalCodeId);
            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods, "ShippingMethodId", "ShippingMethodName", shippingMethodPostalCode.ShippingMethodId);
            return View(shippingMethodPostalCode);
        }

        // GET: ShippingMethodPostalCodes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingMethodPostalCode shippingMethodPostalCode = await db.ShippingMethodPostalCodes.FindAsync(id);
            if (shippingMethodPostalCode == null)
            {
                return HttpNotFound();
            }
            return View(shippingMethodPostalCode);
        }

        // POST: ShippingMethodPostalCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ShippingMethodPostalCode shippingMethodPostalCode = await db.ShippingMethodPostalCodes.FindAsync(id);
            db.ShippingMethodPostalCodes.Remove(shippingMethodPostalCode);
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
