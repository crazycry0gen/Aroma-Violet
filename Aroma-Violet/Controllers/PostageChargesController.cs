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
    public class PostageChargesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: PostageCharges
        public async Task<ActionResult> Index()
        {
            return View(await db.PostageCharges.ToListAsync());
        }

        // GET: PostageCharges/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostageCharge postageCharge = await db.PostageCharges.FindAsync(id);
            if (postageCharge == null)
            {
                return HttpNotFound();
            }
            return View(postageCharge);
        }

        // GET: PostageCharges/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PostageCharges/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PostageChargeId,PostageChargeName,Charge,Active")] PostageCharge postageCharge)
        {
            if (ModelState.IsValid)
            {
                db.PostageCharges.Add(postageCharge);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(postageCharge);
        }

        // GET: PostageCharges/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostageCharge postageCharge = await db.PostageCharges.FindAsync(id);
            if (postageCharge == null)
            {
                return HttpNotFound();
            }
            return View(postageCharge);
        }

        // POST: PostageCharges/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PostageChargeId,PostageChargeName,Charge,Active")] PostageCharge postageCharge)
        {
            if (ModelState.IsValid)
            {
                db.Entry(postageCharge).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(postageCharge);
        }

        // GET: PostageCharges/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PostageCharge postageCharge = await db.PostageCharges.FindAsync(id);
            if (postageCharge == null)
            {
                return HttpNotFound();
            }
            return View(postageCharge);
        }

        // POST: PostageCharges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PostageCharge postageCharge = await db.PostageCharges.FindAsync(id);
            db.PostageCharges.Remove(postageCharge);
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
