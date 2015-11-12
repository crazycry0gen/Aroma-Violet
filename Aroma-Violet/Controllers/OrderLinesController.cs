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
    public class OrderLinesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: OrderLines
        public async Task<ActionResult> Index()
        {
            var orderLines = db.OrderLines.Include(o => o.OrderHeader).Include(o => o.Product);
            return View(await orderLines.ToListAsync());
        }

        // GET: OrderLines/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLine orderLine = await db.OrderLines.FindAsync(id);
            if (orderLine == null)
            {
                return HttpNotFound();
            }
            return View(orderLine);
        }

        // GET: OrderLines/Create
        public ActionResult Create()
        {
            ViewBag.OrderHeaderId = new SelectList(db.OrderHeaders, "OrderHeaderId", "OrderHeaderId");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: OrderLines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderLineId,OrderHeaderId,ProductID,UnitCost,Quantity,Active")] OrderLine orderLine)
        {
            if (ModelState.IsValid)
            {
                var header = await db.OrderHeaders.FirstAsync(m=>m.OrderHeaderId.Equals(orderLine.OrderHeaderId));
                var client = await db.Clients.FirstAsync(m => m.ClientId == header.ClientID);
                var subscription = await db.Subscriptions.FirstAsync(m => m.ProductID == orderLine.ProductID && m.ClientTypeID == client.ClientTypeID);
                orderLine.UnitCost = subscription.Price;
                db.OrderLines.Add(orderLine);

                OrderHeadersController.UpdateHeader(ref header);

                await db.SaveChangesAsync();
                return RedirectToAction("Edit", "OrderHeaders", new { id = orderLine.OrderHeaderId });
            }

            ViewBag.OrderHeaderId = new SelectList(db.OrderHeaders, "OrderHeaderId", "OrderHeaderId", orderLine.OrderHeaderId);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", orderLine.ProductID);

            return View(orderLine);
        }

        // GET: OrderLines/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLine orderLine = await db.OrderLines.FindAsync(id);
            if (orderLine == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderHeaderId = new SelectList(db.OrderHeaders, "OrderHeaderId", "OrderHeaderId", orderLine.OrderHeaderId);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", orderLine.ProductID);
            return View(orderLine);
        }

        // POST: OrderLines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderLineId,OrderHeaderId,ProductID,UnitCost,Quantity,Active")] OrderLine orderLine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderLine).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Edit", "OrderHeaders", new { id=orderLine.OrderHeaderId });
            }
            ViewBag.OrderHeaderId = new SelectList(db.OrderHeaders, "OrderHeaderId", "OrderHeaderId", orderLine.OrderHeaderId);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", orderLine.ProductID);
            return View(orderLine);
        }

        // GET: OrderLines/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderLine orderLine = await db.OrderLines.FindAsync(id);
            if (orderLine == null)
            {
                return HttpNotFound();
            }
            return View(orderLine);
        }

        // POST: OrderLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OrderLine orderLine = await db.OrderLines.FindAsync(id);
            db.OrderLines.Remove(orderLine);
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
