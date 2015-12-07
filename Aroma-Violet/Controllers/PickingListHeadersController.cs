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
    public class PickingListHeadersController : Controller
    {
        private AromaContext db = new AromaContext();

        private async Task<PickingListHeader> GetPickingListView(int shippingTypeId, int shippingMethodId)
        {
            var codes = await (from item in db.ShippingMethodPostalCodes
                               where item.Active && item.ShippingMethodId == shippingMethodId
                               select item.PostalCode.PostalCodeName).ToArrayAsync();
            var orders = await (from item in db.OrderHeaders
                                where item.ShippingTypeId == shippingTypeId
                                && codes.Contains(item.Address.Code)
                                && item.Active
                                && item.OrderStatusId == 2 //ready to ship         
                                select item).Include(m => m.OrderLines).ToArrayAsync();

            var detail = new List<PickingListDetail>();
            foreach (var order in orders)
            {
                var dt = (from item in order.OrderLines
                          select new PickingListDetail()
                          {
                              Active = false,
                              Address = order.Address,
                              Client = order.Client,
                              ClientID = order.ClientID,
                              OrderLineId = item.OrderLineId,
                              Product = item.Product,
                              ProductID = item.ProductID,
                              TotalItems = item.Quantity,
                              TransferQuantity=item.Quantity - (from PickingListDetail pld in db.PickingListDetails where pld.Active && pld.OrderLineId==item.OrderLineId select pld.TransferQuantity).DefaultIfEmpty(0).Sum()
                          }).ToArray();

                detail.AddRange(dt.Where(m=>m.TransferQuantity>0));
                if (dt.Where(m => m.TransferQuantity > 0).Count() == 0)
                {
                    order.OrderStatusId = 4; //picked
                    db.SaveChanges();
                }    
                
            }

            var viewModel = new PickingListHeader()
            {
                PickingDate = DateTime.Now,
                ShippingMethodId = shippingMethodId,
                ShippingTypeId = shippingTypeId,
                PickingListDetail = detail
            };

            return viewModel;
        }


        // GET: PickingListHeaders
        public async Task<ActionResult> Index()
        {
            var pickingListHeaders = db.PickingListHeaders.Include(p => p.ShippingMethod).Include(p => p.ShippingType);
            return View(await pickingListHeaders.ToListAsync());
        }

        // GET: PickingListHeaders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickingListHeader pickingListHeader = await db.PickingListHeaders.FindAsync(id);
            if (pickingListHeader == null)
            {
                return HttpNotFound();
            }
            return View(pickingListHeader);
        }
        [Authorize]

        // GET: PickingListHeaders/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods, "ShippingMethodId", "ShippingMethodName");
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes, "ShippingTypeId", "ShippingTypeName");
            PickingListHeader model = await GetPickingListView(2, 1);
            return View(model);
        }

        // POST: PickingListHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PickingListHeaderId,ShippingTypeId,ShippingMethodId,PickingDate,ShippedDate,PickingListDetail")] PickingListHeader pickingListHeader, bool refreshList)
        {
            if (ModelState.IsValid)
            {
                db.PickingListHeaders.Add(pickingListHeader);
                await db.SaveChangesAsync();
                foreach (var l in pickingListHeader.PickingListDetail.Where(m => m.Active))
                {
                    l.PickingListHeaderId = pickingListHeader.PickingListHeaderId;
                    db.PickingListDetails.Add(l);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Create");
            }

            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods, "ShippingMethodId", "ShippingMethodName", pickingListHeader.ShippingMethodId);
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes, "ShippingTypeId", "ShippingTypeName", pickingListHeader.ShippingTypeId);
            return View(pickingListHeader);
        }

        // GET: PickingListHeaders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickingListHeader pickingListHeader = await db.PickingListHeaders.FindAsync(id);
            if (pickingListHeader == null)
            {
                return HttpNotFound();
            }
            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods, "ShippingMethodId", "ShippingMethodName", pickingListHeader.ShippingMethodId);
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes, "ShippingTypeId", "ShippingTypeName", pickingListHeader.ShippingTypeId);
            return View(pickingListHeader);
        }

        // POST: PickingListHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PickingListHeaderId,ShippingTypeId,ShippingMethodId,PickingDate,ShippedDate,PickingListDetailId")] PickingListHeader pickingListHeader)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pickingListHeader).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods, "ShippingMethodId", "ShippingMethodName", pickingListHeader.ShippingMethodId);
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes, "ShippingTypeId", "ShippingTypeName", pickingListHeader.ShippingTypeId);
            return View(pickingListHeader);
        }

        // GET: PickingListHeaders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickingListHeader pickingListHeader = await db.PickingListHeaders.FindAsync(id);
            if (pickingListHeader == null)
            {
                return HttpNotFound();
            }
            return View(pickingListHeader);
        }

        // POST: PickingListHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PickingListHeader pickingListHeader = await db.PickingListHeaders.FindAsync(id);
            db.PickingListHeaders.Remove(pickingListHeader);
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
