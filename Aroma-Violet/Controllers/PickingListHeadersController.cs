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
    public class PickingListHeadersController : Controller
    {
        private AromaContext db = new AromaContext();

        private async Task<PickingListHeader> GetPickingListView(int shippingTypeId, int shippingMethodId)
        {
            var shippingType = db.ShippingTypes.Find(shippingTypeId);
            var codes = new string[0];
            if (shippingType.ShippingList)
            {
                codes = await (from item in db.ShippingMethodPostalCodes
                               where item.Active && item.ShippingMethodId == shippingMethodId
                               select item.PostalCode.PostalCodeName).Distinct().ToArrayAsync();
            }
            else
            {
                codes = await (from item in db.PostalCodes
                               select item.PostalCodeName).ToArrayAsync();
            }

            var codeCheck = codes.Where(m => m == "0300").FirstOrDefault();

            var ddd =  (from item in db.OrderHeaders.Include(mbox=>mbox.OrderLines).Include(m=>m.Address).ToArray()
                             where item.OrderHeaderId.Equals(Guid.Parse("18903cd2-9554-e611-814c-2047477ce07a"))
                             select item).ToArray();

            //var orders =  (from item in ddd
            //                    where item.Active
            //                    && codes.Contains(item.Address.Code)
            //                    select item).ToArray();

            //try and fix oreders without addesses
            var noAddressOrders = await (from item in db.OrderHeaders
                                         where item.Active
                                         && item.Address == null
                                         select item).ToArrayAsync();
            foreach (var order in noAddressOrders)
            {
                order.Address = (from item in db.Addresses
                                 where item.ClientID == order.ClientID
                                 && item.AddressTypeID == 2
                                 && item.Active
                                 select item).FirstOrDefault();
                if (order.Address == null)
                {
                    order.Address = (from item in db.Addresses
                                     where item.AddressId== order.Client.DeliveryAddress_AddressId
                                     select item).FirstOrDefault();
                }
                if (order.Address == null)
                {
                    order.Address = (from item in db.Addresses
                                     where item.ClientID == order.ClientID
                                     && item.AddressTypeID == 2
                                     select item).FirstOrDefault();
                }
                if (order.Address != null)
                {
                    db.SaveChanges();
                }
            }
            /////////////////////////////////////////

            var orders = await (from item in db.OrderHeaders
                                where item.Active
                                && codes.Contains(item.Address.Code)
                                select item).Include(m => m.OrderLines).ToArrayAsync();

            orders = (from item in orders
                      where item.ShippingTypeId == shippingTypeId
                      select item).ToArray();
            
            orders = (from item in orders
                      where item.OrderStatusId==2
                      select item).ToArray();


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
                              TotalItems = item.Quantity - (from PickingListDetail pld in db.PickingListDetails where pld.Active && pld.OrderLineId == item.OrderLineId select pld.TransferQuantity).DefaultIfEmpty(0).Sum(),
                              GroupId = item.OrderHeaderId,
                              Invoice = OrderHeadersController.GetInvoice(db,item.OrderHeaderId).Number,
                              TransferQuantity=item.Quantity - (from PickingListDetail pld in db.PickingListDetails where pld.Active && pld.OrderLineId==item.OrderLineId select pld.TransferQuantity).DefaultIfEmpty(0).Sum()
                          }).OrderBy(m=>m.ClientID).ThenBy(m=>m.Invoice).ToArray();

                detail.AddRange(dt.Where(m=>m.TransferQuantity>0));
                if (dt.Where(m => m.TransferQuantity > 0).Count() == 0)
                {
                    order.OrderStatusId = 4; //picked
                    db.SaveChanges();
                }    
                
            }

            ModelState.Remove("PickingListDetail");
            ModelState.Remove("ShippingMethodId");
            ModelState.Remove("ShippingMethodId");
            ModelState.Remove("PickingDate");
            ModelState.Remove("ClientId");
            var viewModel = new PickingListHeader()
            {
                PickingDate = DateTime.Now,
                ShippingMethodId = shippingMethodId,
                ShippingTypeId = shippingTypeId,
                PickingListDetail = detail.OrderBy(m=>m.ClientID)
                    .ThenBy(m => m.GroupId)
                    .ThenBy(m=>m.Product.ProductName).ToList()
            };

            return viewModel;
        }

        [HttpGet]
        public async Task<ActionResult> ShipmentList()
        {
            var shipment = new ShipmentViewModel();
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes.Where(m=>m.Active && m.ShippingList), "ShippingTypeId", "ShippingTypeName");
            var pickingListHeaders = await db.PickingListHeaders.Where(m => m.ShippingType.ShippingList).OrderByDescending(m => m.PickingDate).Where(m => m.ShippedDate == null).Include(p => p.ShippingMethod).Include(p => p.ShippingType).ToArrayAsync();

            foreach (var head in pickingListHeaders)
            {
                head.PickingListDetail = await db.PickingListDetails.Where(m => m.PickingListHeaderId == head.PickingListHeaderId).Include(m => m.Address).Include(m=>m.Client.Contact).ToListAsync();
                shipment.Add(head,db);
            }
            
            return View(shipment);
        }
        [HttpPost]

        public async Task<ActionResult> ShipmentList([Bind(Include ="Skip") ]ShipmentViewModel model, int? ShippingTypeId,bool prevShipment = false)
        {
            var shipment = new ShipmentViewModel() { Skip=model.Skip};
            //ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes.Where(m => m.Active && m.ShippingList), "ShippingTypeId", "ShippingTypeName", ShippingTypeId);

            var pickingListHeaders = await db.PickingListHeaders.Where(m=>m.ShippingType.ShippingList).OrderByDescending(m => m.PickingDate).Where(m => m.ShippedDate == null).Include(p => p.ShippingMethod).Include(p => p.ShippingType).ToArrayAsync();
            foreach (var head in pickingListHeaders)
            {
                head.PickingListDetail = await db.PickingListDetails.Where(m => m.PickingListHeaderId == head.PickingListHeaderId).Include(m => m.Address).ToListAsync();
                shipment.Add(head,db);
            }


            return View(shipment);
        }

        [HttpPost]
        public JsonResult MarkAsPrinted(Guid groupId, Guid batchId)
        {
            try
            {
                var lablePrintedGroup = new LabelPrintedGroup() {BatchId=batchId, GroupId = groupId, Printed=DateTime.Now };
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        [HttpPost]
        public JsonResult UpdatePickinglistDetail(Guid groupId, string trackingNumber)
        {
            try
            {
                var allDetail = db.PickingListDetails.Where(m=>m.GroupId.Equals(groupId)).ToArray();
                foreach (var detail in allDetail)
                {
                    var oldTrackingNo = db.PickingListDetails.Find(detail.PickingListDetailId).TrackingNumber;
                    detail.TrackingNumber = trackingNumber;
                    detail.PickingListHeader.ShippedDate = DateTime.Now;
                    

                    if (oldTrackingNo != detail.TrackingNumber)
                    {
                        var userId = Guid.Parse(User.Identity.GetUserId());
                        SystemSMSController.SendSMSEvent(db, 1, detail.ClientID,userId, detail.OrderLine.OrderHeaderId, new KeyValuePair<string, string>("EventTrackingNumber", detail.TrackingNumber));
                    }
                }
                db.SaveChanges();
                //TODO: run stored procedure to mark order as complete where shiped count >= order count
                db.Database.ExecuteSqlCommand("spCompleteOrders");

                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // GET: PickingListHeaders
        public async Task<ActionResult> Index()
        {
            var pickingListHeaders = await db.PickingListHeaders.OrderByDescending(m=>m.PickingDate).Take(50).Include(p => p.ShippingMethod).Include(p => p.ShippingType).ToListAsync();
            var clientIds = new List<KeyValuePair<int, int[]>>();
            foreach (var pickingHeader in pickingListHeaders)
            {
                var detail = db.PickingListDetails.Where(m => m.PickingListHeaderId == pickingHeader.PickingListHeaderId).ToArray();
                var ids = detail.Select(m => m.ClientID).Distinct().ToArray();
                var nitem = new KeyValuePair<int, int[]>(pickingHeader.PickingListHeaderId, ids);
                clientIds.Add(nitem);
            }
            ViewBag.ClientIds = clientIds.ToArray();

            return View( pickingListHeaders);
        }

        // GET: PickingListHeaders/Details/5
        [LayoutInjecter("_LayoutNoLogo")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickingListHeader pickingListHeader = await db.PickingListHeaders.FirstOrDefaultAsync(m=>m.PickingListHeaderId==id);
            if (pickingListHeader == null)
            {
                return HttpNotFound();
            }
            pickingListHeader.PickingListDetail = await db.PickingListDetails.Where(m=>m.PickingListHeaderId == id).OrderBy(m=>m.ClientID).ToListAsync();
            var dt = pickingListHeader.PickingDate.HasValue ? pickingListHeader.PickingDate.Value : DateTime.Now;
            var dts = dt.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.dts = dts;
            ViewBag.Invoices = (from item in pickingListHeader.PickingListDetail.Select(m=>m.OrderLine.OrderHeaderId).Distinct()
                                select new KeyValuePair<Guid, string> (item, OrderHeadersController.GetInvoice(db, item).Number)).ToArray();
            return View(pickingListHeader);
        }
        [Authorize]

        // GET: PickingListHeaders/Create
        public async Task<ActionResult> Create(int shippingTypeId =0, int shippingMethodId=0)
        {
            var methods = db.ShippingMethods.Where(m => m.Active);
            var types = db.ShippingTypes.Where(m => m.Active && m.PickingList);
            var method = shippingMethodId==0?methods.FirstOrDefault()?.ShippingMethodId:shippingMethodId;
            var type = shippingTypeId==0? types.FirstOrDefault()?.ShippingTypeId:shippingTypeId;
            ViewBag.ShippingMethodId = new SelectList(methods, "ShippingMethodId", "ShippingMethodName",method);
            ViewBag.ShippingTypeId = new SelectList(types, "ShippingTypeId", "ShippingTypeName",type);
            var shippingType = await db.ShippingTypes.FindAsync(type);
            ViewBag.ShowShippingMethod = (shippingType.ShippingList);
            PickingListHeader model = await GetPickingListView(type.HasValue?type.Value:0, method.HasValue ? method.Value : 0);
            return View(model);
        }

        // POST: PickingListHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PickingListHeaderId,ShippingTypeId,ShippingMethodId,PickingDate,ShippedDate,PickingListDetail")] PickingListHeader pickingListHeader, int refreshList)
        {
            if (refreshList == 1)
            {
                return RedirectToAction("Create", new { shippingTypeId = pickingListHeader.ShippingTypeId, shippingMethodId = pickingListHeader.ShippingMethodId });
            }
            if (refreshList==0)
            {
                if (ModelState.IsValid)
                {
                    db.PickingListHeaders.Add(pickingListHeader);
                    await db.SaveChangesAsync();

                    var detail = pickingListHeader.PickingListDetail?.Where(m => m.Active).ToArray();
                    Console.WriteLine("Detail item count {0}", detail.Length);
                    var clientIds = (from item in detail
                                     select item.ClientID).Distinct().ToArray();
                    foreach(var clientId in clientIds) Console.WriteLine(clientId);
                    var clientGuidList = (from item in clientIds
                                          select new KeyValuePair<int, Guid>(item,Guid.NewGuid())).ToArray();

                    foreach (var l in detail)
                    {

                        var clientGuid = (from item in clientGuidList
                                          where item.Key == l.ClientID
                                          select item.Value).First();

                        l.PickingListHeaderId = pickingListHeader.PickingListHeaderId;
                        l.GroupId = clientGuid;
                        l.Invoice = OrderHeadersController.GetInvoice(db, l.OrderLineId);
                        db.PickingListDetails.Add(l);
                    }
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details",new {id=pickingListHeader.PickingListHeaderId });
                }
            }
            PickingListHeader model = await GetPickingListView(pickingListHeader.ShippingTypeId, pickingListHeader.ShippingMethodId);
            model.PickingDate = pickingListHeader.PickingDate;
            ViewBag.ShippingMethodId = new SelectList(db.ShippingMethods.Where(m=>m.Active), "ShippingMethodId", "ShippingMethodName", pickingListHeader.ShippingMethodId);
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes.Where(m => m.Active && m.PickingList), "ShippingTypeId", "ShippingTypeName", pickingListHeader.ShippingTypeId);
            var shippingType = await db.ShippingTypes.FindAsync(pickingListHeader.ShippingTypeId);
            ViewBag.ShowShippingMethod = (shippingType.ShippingList);

            var det = model.PickingListDetail;
            ModelState.Remove("PickingListDetail");
            model.PickingListDetail = det;

            return View(model);
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
