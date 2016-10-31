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
    public class AccountHoldersController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: AccountHolders
        public async Task<ActionResult> Index()
        {
            return View(await db.AccountHolders.OrderBy(m=>m.AccountHolderName).ToListAsync());
        }

        // GET: AccountHolders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountHolder accountHolder = await db.AccountHolders.FindAsync(id);
            if (accountHolder == null)
            {
                return HttpNotFound();
            }
            return View(accountHolder);
        }

        // GET: AccountHolders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountHolders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AccountHolderId,AccountHolderName,Active")] AccountHolder accountHolder)
        {
            if (ModelState.IsValid)
            {
                db.AccountHolders.Add(accountHolder);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(accountHolder);
        }

        // GET: AccountHolders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountHolder accountHolder = await db.AccountHolders.FindAsync(id);
            if (accountHolder == null)
            {
                return HttpNotFound();
            }
            return View(accountHolder);
        }

        // POST: AccountHolders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AccountHolderId,AccountHolderName,Active")] AccountHolder accountHolder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountHolder).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(accountHolder);
        }

        // GET: AccountHolders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountHolder accountHolder = await db.AccountHolders.FindAsync(id);
            if (accountHolder == null)
            {
                return HttpNotFound();
            }
            return View(accountHolder);
        }

        // POST: AccountHolders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AccountHolder accountHolder = await db.AccountHolders.FindAsync(id);
            db.AccountHolders.Remove(accountHolder);
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
