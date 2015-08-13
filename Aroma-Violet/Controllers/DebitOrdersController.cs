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
    public class DebitOrdersController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: DebitOrders
        public async Task<ActionResult> Index()
        {
            var debitOrders = db.DebitOrders.Include(d => d.AccountHolder).Include(d => d.AccountType).Include(d => d.Bank).Include(d => d.Branch).Include(d => d.Client);
            return View(await debitOrders.ToListAsync());
        }

        // GET: DebitOrders/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebitOrder debitOrder = await db.DebitOrders.FindAsync(id);
            if (debitOrder == null)
            {
                return HttpNotFound();
            }
            return View(debitOrder);
        }

        // GET: DebitOrders/Create
        public ActionResult Create()
        {
            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName");
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeId", "AccountTypeName");
            ViewBag.BankID = new SelectList(db.Banks, "BankId", "BankName");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchId", "BranchName");
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientName");
            return View();
        }

        // POST: DebitOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DebitOrderId,ClientID,AccountHolderID,AccountHolderOtherDetail,Initials,Surname,AccountTypeID,DebitDate,AccountNumber,BankID,BranchID,SourceID,Active,Created,ProcessDate")] DebitOrder debitOrder)
        {
            if (ModelState.IsValid)
            {
                debitOrder.DebitOrderId = Guid.NewGuid();
                db.DebitOrders.Add(debitOrder);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName", debitOrder.AccountHolderID);
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeId", "AccountTypeName", debitOrder.AccountTypeID);
            ViewBag.BankID = new SelectList(db.Banks, "BankId", "BankName", debitOrder.BankID);
            ViewBag.BranchID = new SelectList(db.Branches, "BranchId", "BranchName", debitOrder.BranchID);
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientName", debitOrder.ClientID);
            return View(debitOrder);
        }

        // GET: DebitOrders/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebitOrder debitOrder = await db.DebitOrders.FindAsync(id);
            if (debitOrder == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName", debitOrder.AccountHolderID);
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeId", "AccountTypeName", debitOrder.AccountTypeID);
            ViewBag.BankID = new SelectList(db.Banks, "BankId", "BankName", debitOrder.BankID);
            ViewBag.BranchID = new SelectList(db.Branches, "BranchId", "BranchName", debitOrder.BranchID);
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientName", debitOrder.ClientID);
            return View(debitOrder);
        }

        // POST: DebitOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DebitOrderId,ClientID,AccountHolderID,AccountHolderOtherDetail,Initials,Surname,AccountTypeID,DebitDate,AccountNumber,BankID,BranchID,SourceID,Active,Created,ProcessDate")] DebitOrder debitOrder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(debitOrder).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName", debitOrder.AccountHolderID);
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeId", "AccountTypeName", debitOrder.AccountTypeID);
            ViewBag.BankID = new SelectList(db.Banks, "BankId", "BankName", debitOrder.BankID);
            ViewBag.BranchID = new SelectList(db.Branches, "BranchId", "BranchName", debitOrder.BranchID);
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientName", debitOrder.ClientID);
            return View(debitOrder);
        }

        // GET: DebitOrders/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DebitOrder debitOrder = await db.DebitOrders.FindAsync(id);
            if (debitOrder == null)
            {
                return HttpNotFound();
            }
            return View(debitOrder);
        }

        // POST: DebitOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            DebitOrder debitOrder = await db.DebitOrders.FindAsync(id);
            db.DebitOrders.Remove(debitOrder);
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
