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
    public class PaymentTypeController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: PaymentType
        public async Task<ActionResult> Index()
        {
            return View(await db.PaymentTypes.OrderBy(m => m.PaymentTypeName).ToListAsync());
        }

        // GET: PaymentType/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentType PaymentType = await db.PaymentTypes.FindAsync(id);
            if (PaymentType == null)
            {
                return HttpNotFound();
            }
            return View(PaymentType);
        }

        // GET: PaymentType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaymentType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PaymentTypeId,PaymentTypeName")] PaymentType PaymentType)
        {
            if (ModelState.IsValid)
            {
                PaymentType.Active = true;
                db.PaymentTypes.Add(PaymentType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(PaymentType);
        }

        // GET: PaymentType/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentType PaymentType = await db.PaymentTypes.FindAsync(id);
            if (PaymentType == null)
            {
                return HttpNotFound();
            }
            return View(PaymentType);
        }

        // POST: PaymentType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PaymentTypeId,PaymentTypeName,Active")] PaymentType PaymentType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(PaymentType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(PaymentType);
        }

        // GET: PaymentType/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentType PaymentType = await db.PaymentTypes.FindAsync(id);
            if (PaymentType == null)
            {
                return HttpNotFound();
            }
            return View(PaymentType);
        }

        // POST: PaymentType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PaymentType PaymentType = await db.PaymentTypes.FindAsync(id);
            db.PaymentTypes.Remove(PaymentType);
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
