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
    public class PickingListDetailsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: PickingListDetails
        public async Task<ActionResult> Index()
        {
            var pickingListDetails = db.PickingListDetails.Include(p => p.Client).Include(p => p.Product);
            return View(await pickingListDetails.ToListAsync());
        }

        // GET: PickingListDetails/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickingListDetail pickingListDetail = await db.PickingListDetails.FindAsync(id);
            if (pickingListDetail == null)
            {
                return HttpNotFound();
            }
            return View(pickingListDetail);
        }
        [Authorize]
        // GET: PickingListDetails/Create
        public ActionResult Create()
        {
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: PickingListDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PickingListDetailId,ProductID,TotalItems,TransferQuantity,ClientID,OrderHeaderId,TrackingNumber,Active,Invoice")] PickingListDetail pickingListDetail)
        {
            if (ModelState.IsValid)
            {
                db.PickingListDetails.Add(pickingListDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", pickingListDetail.ClientID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", pickingListDetail.ProductID);
            return View(pickingListDetail);
        }

        // GET: PickingListDetails/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickingListDetail pickingListDetail = await db.PickingListDetails.FindAsync(id);
            if (pickingListDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", pickingListDetail.ClientID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", pickingListDetail.ProductID);
            return View(pickingListDetail);
        }

        // POST: PickingListDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PickingListDetailId,ProductID,TotalItems,TransferQuantity,ClientID,OrderHeaderId,TrackingNumber,Active")] PickingListDetail pickingListDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pickingListDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", pickingListDetail.ClientID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", pickingListDetail.ProductID);
            return View(pickingListDetail);
        }

        // GET: PickingListDetails/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickingListDetail pickingListDetail = await db.PickingListDetails.FindAsync(id);
            if (pickingListDetail == null)
            {
                return HttpNotFound();
            }
            return View(pickingListDetail);
        }

        // POST: PickingListDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PickingListDetail pickingListDetail = await db.PickingListDetails.FindAsync(id);
            db.PickingListDetails.Remove(pickingListDetail);
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
