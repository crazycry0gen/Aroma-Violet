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
    public class RebatesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: Rebates
        public async Task<ActionResult> Index()
        {
            var rebates = db.Rebates.Include(r => r.ClientType).Include(r => r.Product);
            return View(await rebates.ToListAsync());
        }

        // GET: Rebates/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rebate rebate = await db.Rebates.FindAsync(id);
            if (rebate == null)
            {
                return HttpNotFound();
            }
            return View(rebate);
        }

        // GET: Rebates/Create
        public ActionResult Create()
        {
            ViewBag.ClientTypeId = new SelectList(db.ClientTypes.Where(m => m.Active), "ClientTypeId", "ClientTypeName");
            ViewBag.ProductID = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName");
            ViewBag.AccountId = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName");
            return View();
        }

        // POST: Rebates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RebateId,ClientTypeId,ProductID,Amount,AccountId,MinOwnPurchToQualify")] Rebate rebate)
        {
            if (ModelState.IsValid)
            {
                db.Rebates.Add(rebate);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientTypeId = new SelectList(db.ClientTypes.Where(m => m.Active), "ClientTypeId", "ClientTypeName", rebate.ClientTypeId);
            ViewBag.ProductID = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName", rebate.ProductID);
            ViewBag.AccountId = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName", rebate.AccountId);

            return View(rebate);
        }

        // GET: Rebates/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rebate rebate = await db.Rebates.FindAsync(id);
            if (rebate == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientTypeId = new SelectList(db.ClientTypes.Where(m => m.Active), "ClientTypeId", "ClientTypeName", rebate.ClientTypeId);
            ViewBag.ProductID = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName", rebate.ProductID);
            ViewBag.AccountId = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName", rebate.AccountId);

            return View(rebate);
        }

        // POST: Rebates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RebateId,ClientTypeId,ProductID,Amount,AccountId,MinOwnPurchToQualify")] Rebate rebate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rebate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientTypeId = new SelectList(db.ClientTypes.Where(m => m.Active), "ClientTypeId", "ClientTypeName", rebate.ClientTypeId);
            ViewBag.ProductID = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName", rebate.ProductID);
            ViewBag.AccountId = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName", rebate.AccountId);

            return View(rebate);
        }

        // GET: Rebates/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rebate rebate = await db.Rebates.FindAsync(id);
            if (rebate == null)
            {
                return HttpNotFound();
            }
            return View(rebate);
        }

        // POST: Rebates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Rebate rebate = await db.Rebates.FindAsync(id);
            db.Rebates.Remove(rebate);
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
