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
    public class ShippingTypesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: ShippingTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.ShippingTypes.ToListAsync());
        }

        // GET: ShippingTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingType shippingType = await db.ShippingTypes.FindAsync(id);
            if (shippingType == null)
            {
                return HttpNotFound();
            }
            return View(shippingType);
        }

        // GET: ShippingTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShippingTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ShippingTypeId,ShippingTypeName")] ShippingType shippingType)
        {
            if (ModelState.IsValid)
            {
                db.ShippingTypes.Add(shippingType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(shippingType);
        }

        // GET: ShippingTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingType shippingType = await db.ShippingTypes.FindAsync(id);
            if (shippingType == null)
            {
                return HttpNotFound();
            }
            return View(shippingType);
        }

        // POST: ShippingTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ShippingTypeId,ShippingTypeName")] ShippingType shippingType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shippingType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(shippingType);
        }

        // GET: ShippingTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShippingType shippingType = await db.ShippingTypes.FindAsync(id);
            if (shippingType == null)
            {
                return HttpNotFound();
            }
            return View(shippingType);
        }

        // POST: ShippingTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ShippingType shippingType = await db.ShippingTypes.FindAsync(id);
            db.ShippingTypes.Remove(shippingType);
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
