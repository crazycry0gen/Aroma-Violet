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
    public class BankingDetailsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: BankingDetails
        public async Task<ActionResult> Index()
        {
            var bankingDetails = db.BankingDetails.Include(b => b.AccountHolder);
            return View(await bankingDetails.ToListAsync());
        }

        // GET: BankingDetails/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankingDetail bankingDetail = await db.BankingDetails.FindAsync(id);
            if (bankingDetail == null)
            {
                return HttpNotFound();
            }
            return View(bankingDetail);
        }

        // GET: BankingDetails/Create
        public ActionResult Create()
        {
            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName");
            return View();
        }

        // POST: BankingDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BankingDetailId,AccountHolderID,Active")] BankingDetail bankingDetail)
        {
            if (ModelState.IsValid)
            {
                db.BankingDetails.Add(bankingDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName", bankingDetail.AccountHolderID);
            return View(bankingDetail);
        }

        // GET: BankingDetails/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankingDetail bankingDetail = await db.BankingDetails.FindAsync(id);
            if (bankingDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName", bankingDetail.AccountHolderID);
            return View(bankingDetail);
        }

        // POST: BankingDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BankingDetailId,AccountHolderID,Active")] BankingDetail bankingDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankingDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName", bankingDetail.AccountHolderID);
            return View(bankingDetail);
        }

        // GET: BankingDetails/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankingDetail bankingDetail = await db.BankingDetails.FindAsync(id);
            if (bankingDetail == null)
            {
                return HttpNotFound();
            }
            return View(bankingDetail);
        }

        // POST: BankingDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            BankingDetail bankingDetail = await db.BankingDetails.FindAsync(id);
            db.BankingDetails.Remove(bankingDetail);
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
