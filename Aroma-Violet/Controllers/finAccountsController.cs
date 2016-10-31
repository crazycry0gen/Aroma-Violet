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
            return View(await db.Accounts.OrderBy(m=>m.AccountName).ToListAsync());
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
        public async Task<ActionResult> Create([Bind(Include = "AccountId,AccountName,IsSystemAccount,Active")] finAccount finAccount, string debitAmount,string creditAmount)
        {
            if (ModelState.IsValid)
            {
                finAccount.AccountId = Guid.NewGuid();
                db.Accounts.Add(finAccount);
                await db.SaveChangesAsync();
                InforceDebitCreditRule(finAccount.AccountId, debitAmount,creditAmount);
                return RedirectToAction("Index");
            }

            return View(finAccount);
        }

        private void InforceDebitCreditRule(Guid accountId, string debitAmount, string creditAmount)
        {
            decimal debit = 0;
            decimal credit = 0;

            var validDebit = (debitAmount != null && debitAmount.Length > 0 && decimal.TryParse(debitAmount, out debit));
            var validCredit = (creditAmount != null && creditAmount.Length > 0 && decimal.TryParse(creditAmount, out credit));
            var rule = db.DebitCreditRules.Find(accountId);
            if (validDebit || validCredit)
            {
                if (rule == null)
                {
                    rule = new DebitCreditRule() { AccountId = accountId };
                    db.DebitCreditRules.Add(rule);
                }
            }
            if (rule != null)
            {
                rule.DebitAmount = debit;
                rule.CreditAmount = credit;
                rule.ActiveCredit = validCredit;
                rule.ActiveDebit = validDebit;
                db.SaveChanges();
            }
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
            var rule = db.DebitCreditRules.Find(id);
            ViewBag.DebiCreditRule = rule;
            return View(finAccount);
        }

        // POST: finAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AccountId,AccountName,IsSystemAccount,Active")] finAccount finAccount, string debitAmount, string creditAmount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(finAccount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                InforceDebitCreditRule(finAccount.AccountId, debitAmount, creditAmount);

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

        internal static string Name(AromaContext db, Guid accountId)
        {
            var account = db.Accounts.Where(m => m.AccountId.Equals(accountId)).FirstOrDefault();
            if (account == null)
            {
                var clientAccount = db.ClientAccounts.Where(m => m.ClientAccountId.Equals(accountId)).FirstOrDefault();
                if (clientAccount != null) return clientAccount.Account.AccountName;
            }
            else
            {
                return account.AccountName;
            }
            return "Unknown";
        }
    }
}
