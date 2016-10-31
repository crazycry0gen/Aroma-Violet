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
            var bankingDetails = db.BankingDetails.Include(b => b.AccountHolder).Include(b => b.AccountType).Include(b => b.Bank).Include(b => b.Branch).Include(b => b.Client);
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
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeId", "AccountTypeName");
            ViewBag.BankID = new SelectList(db.Banks, "BankId", "BankName");
            ViewBag.BranchID = new SelectList(db.Branches, "BranchId", "BranchName");
            ViewBag.CellContactID = new SelectList(db.Contacts, "ContactId", "ContactName");
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials");
            ViewBag.EmailContactID = new SelectList(db.Contacts, "ContactId", "ContactName");
            ViewBag.HomeContactID = new SelectList(db.Contacts, "ContactId", "ContactName");
            ViewBag.WorkContactID = new SelectList(db.Contacts, "ContactId", "ContactName");
            return View();
        }

        // POST: BankingDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BankingDetailId,ClientID,AccountHolderID,AccountHolderOtherDetail,Initials,Surname,EmailContactID,WorkContactID,HomeContactID,CellContactID,AccountTypeID,CommencementDate,AccountNumber,BankID,BranchID,SalaryDate,Interval,Active")] BankingDetail bankingDetail)
        {
            if (ModelState.IsValid)
            {
                db.BankingDetails.Add(bankingDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName", bankingDetail.AccountHolderID);
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeId", "AccountTypeName", bankingDetail.AccountTypeID);
            ViewBag.BankID = new SelectList(db.Banks, "BankId", "BankName", bankingDetail.BankID);
            ViewBag.BranchID = new SelectList(db.Branches, "BranchId", "BranchName", bankingDetail.BranchID);
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", bankingDetail.ClientID);
            return View(bankingDetail);
        }


        public static void CreateClientBankingDetails(AromaContext db,int clientId, string initials, string surname, int cellContactId, int homeContactId, int workContactId, int emailContactId)
        {
            const string accountHolderText = "Self";
            const string accountTypeText = "Cheque";
            const string BankText = "ABSA";

            var accountHolder = db.AccountHolders.FirstOrDefault(m => m.AccountHolderName == accountHolderText);
            if (accountHolder == null)
            {
                string errorMessage = string.Format("Account holder \"{0}\" not defined in lookup", accountHolderText);
                throw new Exception(errorMessage);
            }
            var accountType = db.AccountTypes.FirstOrDefault(m => m.AccountTypeName == accountTypeText);
            if (accountType == null)
            {
                string errorMessage = string.Format("Account type \"{0}\" not defined in lookup", accountTypeText);
                throw new Exception(errorMessage);
            }
            var bank = db.Banks.FirstOrDefault(m => m.BankName == BankText);
            if (bank == null)
            {
                string errorMessage = string.Format("Bank name \"{0}\" not defined in lookup", BankText);
                throw new Exception(errorMessage);
            }

            var branch = db.Branches.FirstOrDefault(m => m.BankId == bank.BankId);
            if (branch == null)
            {
                string errorMessage = string.Format("No branch defined for \"{0}\" not defined in lookup", BankText);
                throw new Exception(errorMessage);
            }

            if (db.BankingDetails.Where(m => m.ClientID == clientId).Count() == 0)
            {
                var bankingDetail = new BankingDetail()
                {
                    Initials = initials,
                    Surname = surname,
                    AccountHolderID = accountHolder.AccountHolderId,
                    AccountTypeID = accountType.AccountTypeId,
                    BankID = bank.BankId,
                    ClientID = clientId,
                    CommencementDate = DateTime.Now.AddMonths(1),
                    SalaryDate = DateTime.Now,
                    AccountNumber = "0",
                    BranchID = branch.BranchId,
                    CellContact = db.Contacts.First(m => m.ContactId == cellContactId).ContactName,
                    HomeContact = db.Contacts.First(m => m.ContactId == homeContactId).ContactName,
                    WorkContact = db.Contacts.First(m => m.ContactId == workContactId).ContactName,
                    EmailContact = db.Contacts.First(m => m.ContactId == emailContactId).ContactName,
                     Interval=1,
                    Active = false
                };

                db.BankingDetails.Add(bankingDetail);
                db.SaveChanges();
            }
        }

        // GET: BankingDetails/Edit/5
        public async Task<ActionResult> Edit(int? id, int? ClientID)
        {
            if (!id.HasValue && ClientID.HasValue)
            {
                id = db.BankingDetails.FirstOrDefault(m => m.ClientID == ClientID.Value)?.BankingDetailId;
            }
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
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeId", "AccountTypeName", bankingDetail.AccountTypeID);
            ViewBag.BankID = new SelectList(db.Banks, "BankId", "BankName", bankingDetail.BankID);
            ViewBag.BranchID = new SelectList(db.Branches, "BranchId", "BranchName", bankingDetail.BranchID);
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", bankingDetail.ClientID);
            ViewBag.YearStart = DateTime.Now.Year;
            ViewBag.YearEnd = DateTime.Now.Year + 1;
            return View(bankingDetail);
        }

        // POST: BankingDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "BankingDetailId,ClientID,AccountHolderID,AccountHolderOtherDetail,Initials,Surname,EmailContact,WorkContact,HomeContact,CellContact,AccountTypeID,CommencementDate,AccountNumber,BankID,BranchID,SalaryDate,Interval,Active")] BankingDetail bankingDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankingDetail).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Edit","Clients", new {id=bankingDetail.ClientID });
            }
            ViewBag.AccountHolderID = new SelectList(db.AccountHolders, "AccountHolderId", "AccountHolderName", bankingDetail.AccountHolderID);
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "AccountTypeId", "AccountTypeName", bankingDetail.AccountTypeID);
            ViewBag.BankID = new SelectList(db.Banks, "BankId", "BankName", bankingDetail.BankID);
            ViewBag.BranchID = new SelectList(db.Branches, "BranchId", "BranchName", bankingDetail.BranchID);
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", bankingDetail.ClientID);
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
