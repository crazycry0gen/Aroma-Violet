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
    public class finClientAccountsController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: finClientAccounts
        public async Task<ActionResult> Index()
        {
            var clientAccounts = db.ClientAccounts.Include(f => f.Client);
            return View(await clientAccounts.ToListAsync());
        }

        // GET: finClientAccounts/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            finClientAccount finClientAccount = await db.ClientAccounts.FindAsync(id);
            if (finClientAccount == null)
            {
                return HttpNotFound();
            }
            return View(finClientAccount);
        }

        // GET: finClientAccounts/Create
        public ActionResult Create()
        {
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials");
            return View();
        }

        // POST: finClientAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ClientAccountId,ClientID,AccountID,Active")] finClientAccount finClientAccount)
        {
            if (ModelState.IsValid)
            {
                finClientAccount.ClientAccountId = Guid.NewGuid();
                db.ClientAccounts.Add(finClientAccount);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", finClientAccount.ClientID);
            return View(finClientAccount);
        }

        // GET: finClientAccounts/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            finClientAccount finClientAccount = await db.ClientAccounts.FindAsync(id);
            if (finClientAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", finClientAccount.ClientID);
            return View(finClientAccount);
        }

        // POST: finClientAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ClientAccountId,ClientID,AccountID,Active")] finClientAccount finClientAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(finClientAccount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", finClientAccount.ClientID);
            return View(finClientAccount);
        }

        // GET: finClientAccounts/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            finClientAccount finClientAccount = await db.ClientAccounts.FindAsync(id);
            if (finClientAccount == null)
            {
                return HttpNotFound();
            }
            return View(finClientAccount);
        }

        // POST: finClientAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            finClientAccount finClientAccount = await db.ClientAccounts.FindAsync(id);
            db.ClientAccounts.Remove(finClientAccount);
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
