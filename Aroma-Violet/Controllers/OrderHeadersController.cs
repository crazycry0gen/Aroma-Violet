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
using Microsoft.AspNet.Identity;

namespace Aroma_Violet.Controllers
{
    public class OrderHeadersController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: OrderHeaders
        [Authorize]
        public async Task<ActionResult> Index(int? clientId)
        {
            if (clientId.HasValue)
            {
                ViewBag.ClientId = clientId.Value;
                var ordHeaders = db.OrderHeaders.OrderByDescending(m=>m.OrderDate).Where(m => m.ClientID==clientId.Value && m.Active).Include(o => o.Client).Include(o => o.OrderStatus);
                return View(await ordHeaders.ToListAsync());
            }
            var orderHeaders = db.OrderHeaders.OrderByDescending(m => m.OrderDate).Where(m=>m.Active).Include(o => o.Client).Include(o => o.OrderStatus);
            return View(await orderHeaders.ToListAsync());
        }

        // GET: OrderHeaders/Details/5
        [Authorize]

        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return HttpNotFound();
            }
            return View(orderHeader);
        }

        // GET: OrderHeaders/Create
        [Authorize]
        public ActionResult Create(int? ClientId)
        {
            var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials");
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusName");
            var newOrder = new OrderHeader() {Active = true, OrderDate = DateTime.Now, OrderStatusId=1, UserId=currentUserId,ShippingTypeId=1 };
            ViewBag.ProductList = new SelectList(db.Products.Where(m=>m.Active), "ProductID", "ProductName");

            return View(newOrder);
        }

        // POST: OrderHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderHeaderId,ClientID,Total,OrderStatusId,UserId,OrderDate,ShippingTypeId")] OrderHeader orderHeader)
        {
            if (ModelState.IsValid)
            {
                orderHeader.OrderHeaderId = Guid.NewGuid();
                orderHeader.Active = true;
                db.OrderHeaders.Add(orderHeader);
                await db.SaveChangesAsync();
                return RedirectToAction("Edit", new {id=orderHeader.OrderHeaderId });
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", orderHeader.ClientID);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName");

            return View(orderHeader);
        }

        internal static void UpdateHeader(ref OrderHeader header)
        {
            header.Total = 0;
            foreach (var line in header.OrderLines)
            {
                header.Total += (line.Quantity * line.UnitCost);
            }
        }

        // GET: OrderHeaders/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", orderHeader.ClientID);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName");

            return View(orderHeader);
        }

        // POST: OrderHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderHeaderId,ClientID,Total,OrderStatusId,UserId,OrderDate,Active,ShippingTypeId")] OrderHeader orderHeader)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
                orderHeader.UserId = currentUserId;
                db.Entry(orderHeader).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index","Clients");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", orderHeader.ClientID);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName");
            return View(orderHeader);
        }

        // GET: OrderHeaders/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return HttpNotFound();
            }
            return View(orderHeader);
        }

        // POST: OrderHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            db.OrderHeaders.Remove(orderHeader);
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
