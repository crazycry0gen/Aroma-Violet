using Aroma_Violet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aroma_Violet.Controllers
{
    public class AccountMovementController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: AccountMovement
        [Authorize]
        public ActionResult Index(int? clientId, Guid? clientAccountId, bool reverse=false)
        {
            var days = Generic.GetSetting<int>(db, "Default Statement Days");
            if (days == 0)
            {
                days = 30;
            }
            var fromDate = DateTime.Today.AddDays((Math.Abs(days) * -1));
            var model = AccountMovementViewModel.LoadJournals(db, fromDate, DateTime.Now,clientId,clientAccountId);
            ViewBag.Reverse = reverse;

            return View(model);
        }

        [Authorize]
        public ActionResult AuditAccount(bool effectiveDate, DateTime startDateTime, DateTime endDateTime, Guid clientAccountId)
        {
            var journalEntries = new AccountAuditList(effectiveDate,startDateTime,endDateTime,clientAccountId);
            return View(journalEntries);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Index([Bind(Include = "ClientId,FromDate,toDate, ClientAccountId")] AccountMovementViewModel model, bool reverse = false)
        {
            if (ModelState.IsValid)
            {
                model = AccountMovementViewModel.LoadJournals(db, model.FromDate, model.ToDate, model.ClientId, model.ClientAccountId);
            }
            ViewBag.Reverse = reverse;
            return View(model);
        }

        // GET: AccountMovement
        [Authorize]
        public ActionResult GlobalIndex(int? clientId)
        {
            var days = Generic.GetSetting<int>(db, "Default Statement Days");
            if (days == 0)
            {
                days = 30;
            }
            var fromDate = DateTime.Today.AddDays((Math.Abs(days) * -1));
            var model = AccountMovementViewModel.LoadGlobalJournals(db, fromDate, DateTime.Now);

            ViewBag.ClientAccounts = db.ClientAccounts.ToArray();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult GlobalIndex([Bind(Include = "ClientId,FromDate,toDate")] AccountMovementViewModel model)
        {
            if (ModelState.IsValid)
            {
                model = AccountMovementViewModel.LoadGlobalJournals(db, model.FromDate, model.ToDate);
            }
            ViewBag.ClientAccounts = db.ClientAccounts.ToArray();
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult InterAccountTransfer(int clientId, Guid? orderHeaderId)
        {
            var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
            var model = new InterAccountTransferViewModel()
            {
                Amount = 0,
                ClientId = clientId,
                FromEffectiveDate = DateTime.Now,
                ToEffectiveDate = DateTime.Now,
                MovementSource = "User generated transfer",
                UserID = currentUserId,
                FromAccountID = Generic.AccountControl
            };

            ViewBag.FromAccountID = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName",model.FromAccountID);

            if (orderHeaderId.HasValue)
            {
                var order = db.OrderHeaders.Find(orderHeaderId.Value);
                if (order.OnceOff)
                {
                    var client = db.Clients.Find(clientId);
                    var subscriptions = (from item in db.Subscriptions
                                         where item.ValidFromDate <= DateTime.Now
                                         && item.ClientTypeID == client.ClientTypeID
                                         && !item.InitialOnceOffFromAccountID.Equals(Guid.Empty)
                                         select item).ToArray();

                    var accountIds = subscriptions.Select(m => m.InitialOnceOffFromAccountID).Distinct().ToArray();
                    var accounts = db.Accounts.Where(m => m.Active && accountIds.Contains(m.AccountId)).ToArray();
                    if (accountIds.Length > 0)
                    {
                        model.FromAccountID = accountIds[0];
                        ViewBag.FromAccountID = new SelectList(accounts, "AccountId", "AccountName", accountIds[0]);
                    }
                }
                model.ToAccountID = order.SalesType.AccountId;
                ViewBag.ToAccountID = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName", model.ToAccountID);
                ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(m => m.Active), "PaymentTypeId", "PaymentTypeName");
                model.OriginalAmount = order.Total + order.Shipping;
                model.AvailableAmount = CalaculateBalance(order.SalesType.AccountId, model.ClientId);
                model.Amount = model.OriginalAmount < model.AvailableAmount ? 0 : model.OriginalAmount - model.AvailableAmount;
                model.FromEffectiveDate = order.OrderDate;
                model.ToEffectiveDate = order.OrderDate;
                var inv = OrderHeadersController.GetInvoice(db, order.OrderHeaderId);
                if (inv != null)
                {
                    model.Comment = string.Format("Payment received for {0}", inv.Number);
                }
                    ViewBag.OrderHeaderId = orderHeaderId.Value;
                
            }
            else
            {
                ViewBag.ToAccountID = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName");
                ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(m => m.Active), "PaymentTypeId", "PaymentTypeName");
            }
            return View(model);
        }
               
        public ActionResult Reverse(Guid JournalId, int clientId)
        {
            var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
            var journal1 = db.Journals.Find(JournalId);
            var journal2 = db.Journals.Find(journal1.CorrespondingJournalId);

            var journal3 = journal1.Reverse(currentUserId,1);
            var journal4 = journal2.Reverse(currentUserId,2);
            db.Journals.Add(journal3);
            db.Journals.Add(journal4);
            db.SaveChanges();

            journal3.CorrespondingJournalId = journal4.JournalId;
            journal4.CorrespondingJournalId = journal3.JournalId;

            db.SaveChanges();

            return RedirectToAction("Index", new { clientId = clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult InterAccountTransfer(InterAccountTransferViewModel model, int? paymentTypeId, Guid? orderHeaderId, bool splitPayment = false)
        {
            if (orderHeaderId.HasValue)
            {
                ViewBag.OrderHeaderId = orderHeaderId.Value;
                //check if payment already exists
                var orderHeader = db.OrderHeaders.Find(orderHeaderId);
                var payments = db.Journals.Where(m => m.MovementSource.Equals(orderHeaderId.Value) && m.Comment == model.Comment);
                var payment = false;
                if (payments.Count()>0) payment = payments.Sum(m=>m.Amount) >= orderHeader.Total + orderHeader.Shipping ;

                if (payment)
                {
                    ModelState.AddModelError(string.Empty, "A sufficient payment has already been made against this order");
                }
            }
                if (ModelState.IsValid)
            {
                try
                {
                    var paymentType = db.PaymentTypes.Find(paymentTypeId);
                    if (paymentType != null)
                    {
                        model.MovementSource = paymentType.PaymentTypeName;
                    }

                    if (!orderHeaderId.HasValue)
                    {

                        db.Database.ExecuteSqlCommand(string.Format(Generic.sqlCreateJournalSingle,
                            model.ClientId,
                            model.FromAccountID,
                            model.ToAccountID,
                            model.Amount.ToString().Replace(",", "."),
                            model.FromEffectiveDate.ToString(Generic.LongDate),
                            model.ToEffectiveDate.ToString(Generic.LongDate),
                            model.MovementSource,
                            model.Comment,
                            model.UserID
                            ));
                    }
                    else
                    {
                        if (model.Amount > 0)
                        {
                            db.Database.ExecuteSqlCommand(string.Format(Generic.sqlCreateJournalSingle_GS,
                                model.ClientId,
                                model.FromAccountID,
                                model.ToAccountID,
                                model.Amount.ToString().Replace(",", "."),
                                model.FromEffectiveDate.ToString(Generic.LongDate),
                                model.ToEffectiveDate.ToString(Generic.LongDate),
                                orderHeaderId.Value,
                                model.Comment,
                                model.UserID,
                                model.MovementSource
                                ));
                        }
                        if (splitPayment)
                        {
                            return RedirectToAction("InterAccountTransfer", new {clientId=model.ClientId, orderHeaderId = orderHeaderId });
                        }
                        db.Database.ExecuteSqlCommand("spCreateSalesWithAvailableFundsAll");
                        if ((User.IsInRole("Area Distributor")|| User.IsInRole("Distributor")))
                        {
                            if (orderHeaderId.HasValue)
                            {
                                return RedirectToAction("Invoice","OrderHeaders", new {id= orderHeaderId.Value });
                            }
                            else
                            {
                                return RedirectToAction("Index", "Clients");
                            }
                        }
                        else
                        {
                            return RedirectToAction("Details", "OrderHeaders", new { id = orderHeaderId.Value });
                        }
                    }

                    return RedirectToAction("Index", new { clientId = model.ClientId});
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("",ex);
                }
            }
            ViewBag.FromAccountID = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName");
            ViewBag.ToAccountID = new SelectList(db.Accounts.Where(m => m.Active), "AccountId", "AccountName");
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes.Where(m => m.Active), "PaymentTypeId", "PaymentTypeName");

            return View(model);
        }

        [HttpPost]
        public JsonResult GetBalance(Guid accountId, int clientId)
        {
            return Json(CalaculateBalance(accountId,clientId));
        }

        private decimal CalaculateBalance(Guid accountId, int clientId)
        {
            decimal ret = 0;
            var clientAccount = db.ClientAccounts.FirstOrDefault(m => m.AccountId.Equals(accountId) && m.ClientID == clientId);
            if (clientAccount != null)
            {
                var Journals = db.Journals.Where(m => m.AccountID.Equals(clientAccount.ClientAccountId));
                if (Journals.Count() > 0)
                {
                    ret = Journals.Sum(m => m.Amount);
                }
            }
            return ret;
        }

        [HttpPost]
        public JsonResult GetJournalExtraInfo(Guid journalId)
        {
            string extraInfo = string.Empty;
            const string template = "Effective Date:{0}<br/>Comment:{1}";

            var journal = db.Journals.First(m=>m.JournalId.Equals(journalId));
            extraInfo = string.Format(template,journal.EffectiveDate.ToString("dd MMM yy HH:mm"), journal.Comment);

            return Json(extraInfo);
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