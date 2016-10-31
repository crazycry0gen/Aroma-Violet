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
    public class ClientSubscriptionsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: ClientSubscriptions
        public async Task<ActionResult> Index()
        {
            var clientSubscriptions = db.ClientSubscriptions.Include(c => c.Product);
            
            return View(await clientSubscriptions.OrderBy(m => m.ClientID).ThenBy(m=> m.Product.ProductName).ToListAsync());
        }

        // GET: ClientSubscriptions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientSubscription clientSubscription = await db.ClientSubscriptions.FindAsync(id);
            if (clientSubscription == null)
            {
                return HttpNotFound();
            }
            return View(clientSubscription);
        }

        // GET: ClientSubscriptions/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products.Where(m=>m.Active), "ProductID", "ProductName");
            return View();
        }

        // POST: ClientSubscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ClientSubscriptionId,ClientID,ProductID,Quantity,Active")] ClientSubscription clientSubscription)
        {
            if (ModelState.IsValid)
            {
                db.ClientSubscriptions.Add(clientSubscription);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProductID = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName", clientSubscription.ProductID);
            return View(clientSubscription);
        }

        // GET: ClientSubscriptions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientSubscription clientSubscription = await db.ClientSubscriptions.FindAsync(id);
            if (clientSubscription == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName", clientSubscription.ProductID);
            return View(clientSubscription);
        }

        // POST: ClientSubscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ClientSubscriptionId,ClientID,ProductID,Quantity,Active")] ClientSubscription clientSubscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientSubscription).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Manage", new { ClientId = clientSubscription.ClientID});
            }
            ViewBag.ProductID = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName", clientSubscription.ProductID);
            return View(clientSubscription);
        }

        // GET: ClientSubscriptions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientSubscription clientSubscription = await db.ClientSubscriptions.FindAsync(id);
            if (clientSubscription == null)
            {
                return HttpNotFound();
            }
            return View(clientSubscription);
        }

        // POST: ClientSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ClientSubscription clientSubscription = await db.ClientSubscriptions.FindAsync(id);
            db.ClientSubscriptions.Remove(clientSubscription);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Manage(int ClientID)
        {
            var bankingActive = (from item in db.BankingDetails
                                 where item.ClientID == ClientID
                                 && item.Active
                                 select 1).Count() > 0;
            if (!bankingActive)
            {
                ModelState.AddModelError("Banking not active", bankingNotActive);
            }

            var clientSubscriptions = db.ClientSubscriptions.Where(m => m.ClientID == ClientID).ToList();
            var ProductIDs = clientSubscriptions.Select(s => s.ProductID).ToArray();
            var productList = db.Products.Where(m => !ProductIDs.Contains(m.ProductID) && m.Active).ToList();
            ViewBag.ProductID = new SelectList(productList, "ProductID", "ProductName", null);
            var viewModel = new ClientSubscriptionViewModel() { ClientSubscriptions = clientSubscriptions, Subscription = new ClientSubscription() { ClientID = ClientID, Active = true, Quantity = 1 }, ProductCount = productList.Count };
            return View(viewModel);
        }

        private const string bankingNotActive = "Subscriptions are only allowed when the client has active banking detail";

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Manage([Bind(Include = "ClientSubscriptionId,ClientID,ProductID,Quantity")] int ClientSubscriptionId, int ClientID, int ProductID, int Quantity)
        {
            var bankingActive = (from item in db.BankingDetails
                                 where item.ClientID == ClientID
                                 && item.Active
                                 select 1).Count() > 0;
            if (!bankingActive)
            {
                ModelState.AddModelError("Banking not active", bankingNotActive);
            }

            if (ModelState.IsValid)
            {
                if (ClientSubscriptionId == 0)//check that the same product is not added twice
                {
                    ClientSubscriptionId = (from item in db.ClientSubscriptions
                                            where item.ClientID == ClientID
                                            && item.ProductID == ProductID
                                            select item.ClientSubscriptionId).FirstOrDefault();
                }
                    if (ClientSubscriptionId > 0 )
                {
                    ClientSubscription clientSubscription = db.ClientSubscriptions.FirstOrDefault(m => m.ClientSubscriptionId == ClientSubscriptionId);
                    clientSubscription.Active = !clientSubscription.Active;
                    await db.SaveChangesAsync();
                }
                else 
                {
                    var newItem = new ClientSubscription() {Active=true, ClientID = ClientID, ProductID=ProductID, Quantity=Quantity };
                    db.ClientSubscriptions.Add(newItem);
                    await db.SaveChangesAsync();
                }
            }

            var clientSubscriptions = db.ClientSubscriptions.Include(mbox=>mbox.Product).Where(m => m.ClientID == ClientID ).ToList();
            var ProductIDs = clientSubscriptions.Select(s => s.ProductID).ToArray();
            var productList = db.Products.Where(m => !ProductIDs.Contains(m.ProductID) && m.Active).ToList();
            ViewBag.ProductID = new SelectList(productList, "ProductID", "ProductName", null);
            var viewModel = new ClientSubscriptionViewModel() { ClientSubscriptions = clientSubscriptions, Subscription = new ClientSubscription() { ClientID = ClientID, Active = true, Quantity = 1 }, ProductCount= productList.Count };
            return View(viewModel);
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
