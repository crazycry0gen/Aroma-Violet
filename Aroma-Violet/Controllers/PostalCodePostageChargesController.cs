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
    public class PostalCodePostageChargesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: PostalCodePostageCharges
        public async Task<ActionResult> Index()
        {
            var postalCodePostageCharges = db.PostalCodePostageCharges.Include(p => p.PostageCharge).Include(p => p.PostalCode);
            return View(await postalCodePostageCharges.ToListAsync());
        }

        // GET: PostalCodePostageCharges/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostalCodePostageCharge postalCodePostageCharge = await db.PostalCodePostageCharges.FindAsync(id);
            if (postalCodePostageCharge == null)
            {
                return HttpNotFound();
            }
            return View(postalCodePostageCharge);
        }

        // GET: PostalCodePostageCharges/Create
        public ActionResult Create()
        {
            ViewBag.PostageChargeId = new SelectList(db.PostageCharges, "PostageChargeId", "PostageChargeName");
            ViewBag.PostalCodeId = new SelectList(db.PostalCodes, "PostalCodeId", "PostalCodeName");
            return View();
        }

        // POST: PostalCodePostageCharges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PostalCodePostageChargeId,PostageChargeId,PostalCodeId")] PostalCodePostageCharge postalCodePostageCharge)
        {
            if (ModelState.IsValid)
            {
                db.PostalCodePostageCharges.Add(postalCodePostageCharge);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PostageChargeId = new SelectList(db.PostageCharges, "PostageChargeId", "PostageChargeName", postalCodePostageCharge.PostageChargeId);
            ViewBag.PostalCodeId = new SelectList(db.PostalCodes, "PostalCodeId", "PostalCodeName", postalCodePostageCharge.PostalCodeId);
            return View(postalCodePostageCharge);
        }

        // GET: PostalCodePostageCharges/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostalCodePostageCharge postalCodePostageCharge = await db.PostalCodePostageCharges.FindAsync(id);
            if (postalCodePostageCharge == null)
            {
                return HttpNotFound();
            }
            ViewBag.PostageChargeId = new SelectList(db.PostageCharges, "PostageChargeId", "PostageChargeName", postalCodePostageCharge.PostageChargeId);
            ViewBag.PostalCodeId = new SelectList(db.PostalCodes, "PostalCodeId", "PostalCodeName", postalCodePostageCharge.PostalCodeId);
            return View(postalCodePostageCharge);
        }

        // POST: PostalCodePostageCharges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PostalCodePostageChargeId,PostageChargeId,PostalCodeId")] PostalCodePostageCharge postalCodePostageCharge)
        {
            if (ModelState.IsValid)
            {
                db.Entry(postalCodePostageCharge).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PostageChargeId = new SelectList(db.PostageCharges, "PostageChargeId", "PostageChargeName", postalCodePostageCharge.PostageChargeId);
            ViewBag.PostalCodeId = new SelectList(db.PostalCodes, "PostalCodeId", "PostalCodeName", postalCodePostageCharge.PostalCodeId);
            return View(postalCodePostageCharge);
        }

        // GET: PostalCodePostageCharges/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostalCodePostageCharge postalCodePostageCharge = await db.PostalCodePostageCharges.FindAsync(id);
            if (postalCodePostageCharge == null)
            {
                return HttpNotFound();
            }
            return View(postalCodePostageCharge);
        }

        // POST: PostalCodePostageCharges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PostalCodePostageCharge postalCodePostageCharge = await db.PostalCodePostageCharges.FindAsync(id);
            db.PostalCodePostageCharges.Remove(postalCodePostageCharge);
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
