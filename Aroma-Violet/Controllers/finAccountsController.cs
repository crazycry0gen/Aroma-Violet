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
    public class finAccountsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: finAccounts
        public async Task<ActionResult> Index()
        {
            return View(await db.Accounts.ToListAsync());
        }

        // GET: finAccounts/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            finAccount finAccount = await db.Accounts.FindAsync(id);
            if (finAccount == null)
            {
                return HttpNotFound();
            }
            return View(finAccount);
        }

        // GET: finAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: finAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AccountId,AccountName,IsSystemAccount,Active")] finAccount finAccount)
        {
            if (ModelState.IsValid)
            {
                finAccount.AccountId = Guid.NewGuid();
                db.Accounts.Add(finAccount);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(finAccount);
        }

        // GET: finAccounts/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            finAccount finAccount = await db.Accounts.FindAsync(id);
            if (finAccount == null)
            {
                return HttpNotFound();
            }
            return View(finAccount);
        }

        // POST: finAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AccountId,AccountName,IsSystemAccount,Active")] finAccount finAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(finAccount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(finAccount);
        }

        // GET: finAccounts/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            finAccount finAccount = await db.Accounts.FindAsync(id);
            if (finAccount == null)
            {
                return HttpNotFound();
            }
            return View(finAccount);
        }

        // POST: finAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            finAccount finAccount = await db.Accounts.FindAsync(id);
            db.Accounts.Remove(finAccount);
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
