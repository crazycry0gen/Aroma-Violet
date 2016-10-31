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
    public class AddressTypesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: AddressTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.AddressTypes.OrderBy(m=>m.AddressTypeName).ToListAsync());
        }

        // GET: AddressTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressType addressType = await db.AddressTypes.FindAsync(id);
            if (addressType == null)
            {
                return HttpNotFound();
            }
            return View(addressType);
        }

        // GET: AddressTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddressTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AddressTypeId,AddressTypeName,Active")] AddressType addressType)
        {
            if (ModelState.IsValid)
            {
                db.AddressTypes.Add(addressType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(addressType);
        }

        // GET: AddressTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressType addressType = await db.AddressTypes.FindAsync(id);
            if (addressType == null)
            {
                return HttpNotFound();
            }
            return View(addressType);
        }

        // POST: AddressTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AddressTypeId,AddressTypeName,Active")] AddressType addressType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(addressType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(addressType);
        }

        // GET: AddressTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AddressType addressType = await db.AddressTypes.FindAsync(id);
            if (addressType == null)
            {
                return HttpNotFound();
            }
            return View(addressType);
        }

        // POST: AddressTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AddressType addressType = await db.AddressTypes.FindAsync(id);
            db.AddressTypes.Remove(addressType);
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
