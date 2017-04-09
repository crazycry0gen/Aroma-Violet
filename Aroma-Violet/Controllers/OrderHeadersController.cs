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
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;

namespace Aroma_Violet.Controllers
{
    public class OrderHeadersController : Controller
    {
        private AromaContext db = new AromaContext();
        private const string dtfmt = "dd MMM yyyy HH:mm:ss";

        public async Task<ActionResult> UnpaidOrders()
        {
            /*
            var res = await db.Database.SqlQuery<OrderHeader>("spUnpaidOrders {0}", Generic.AccountTelemarked).ToListAsync();
            var ids = res.Select(m => m.OrderHeaderId).ToArray();
            var orders = db.OrderHeaders.Where(m=>ids.Contains( m.OrderHeaderId)).OrderBy(m => m.OrderDate).Include(m => m.OrderStatus).Include(m => m.ShippingType).ToList();
    */
            var salesAccounts = db.SalesTypes.Select(m => m.AccountId).Distinct().ToArray();
            var orders = await db.OrderHeaders.Where(m => m.OrderStatusId == 1).OrderBy(m => m.OrderDate).Include(m => m.OrderStatus).Include(m => m.ShippingType).ToListAsync();
            var balances = new List<decimal>();
            foreach(var acc in salesAccounts)
            balances.AddRange((from item in orders
                                select (item.Total - item.Balance(db, acc))).ToArray());
            ViewBag.Balances = balances.ToArray();
            return View(orders);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Pay(Guid id, decimal amount, int clientId)
        {
            var userId = Guid.Parse(User.Identity.GetUserId());

            var order = db.OrderHeaders.Find(id);
            var model = new PayOrderViewModel();

            var toAccount = await db.ClientAccounts.Where(m => m.AccountId.Equals(order.SalesType.AccountId) && m.ClientID == clientId).Select(m => m.ClientAccountId).FirstOrDefaultAsync();

            if (toAccount.Equals(Guid.Empty))
            {
                toAccount = Guid.NewGuid();
                db.ClientAccounts.Add(new finClientAccount()
                {
                    AccountId = order.SalesType.AccountId,
                    Active = true,
                    ClientAccountId = toAccount,
                    ClientID = order.ClientID
                });
                await db.SaveChangesAsync();
            }

            var accounts = await db.ClientAccounts.Where(m => m.ClientID == clientId).ToListAsync();
            model.OrderAmount = order.Total;
            model.OutstandingAmount = order.Total - order.Balance(db, toAccount);
            model.OrderHeaderId = id;
            model.AccountInfo = (from item in accounts
                                 select new PayOrderAccountInfo()
                                 {
                                     AccountId = item.AccountId,
                                     ClientAccountId = item.ClientAccountId,
                                     DestinationAccount = (item.AccountId.Equals(order.SalesType.AccountId)),
                                     EffectiveDate = DateTime.Now,
                                     Balance = Generic.Balance(item.ClientAccountId, db),
                                     AccountName = item.Account.AccountName
                                 }).ToList();

            return View(model);

            /*db.Database.ExecuteSqlCommand("spPayOrder {0}, {1}, {2}, {3}, {4} ", Generic.AccountTelemarked, id, amount, userId, clientId);*/
            /*return RedirectToAction("UnpaidOrders");*/
        }

        [Authorize]
        [HttpGet]
        [LayoutInjecter("_LayoutNoLogo")]
        public async Task<ActionResult> Invoice(Guid id)
        {
            var order = await db.OrderHeaders.Include(m => m.OrderLines)
                .Include(m => m.Client.DeliveryAddress)
                .FirstAsync(m => m.OrderHeaderId.Equals(id));

        
            ViewBag.Invoice = GetInvoice(id);

            return View(order);
        }

        private Invoice GetInvoice(Guid id)
        {
            return GetInvoice(db, id);
        }
        public static Invoice GetInvoice(AromaContext db, Guid id)
        {
            var invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                invoice = new Invoice() { InvoiceId = id, Number = string.Format("INB{0:000000}", db.Invoices.Count() + 1) };
                db.Invoices.Add(invoice);
                db.SaveChanges();
            }
            return invoice;
        }


        private CreditNote GetCreditNote(Guid id)
        {
            var creditNote =  db.CreditNotes.Find(id);
            if (creditNote == null)
            {
                creditNote = new CreditNote() { CreditNoteId = id, Number = string.Format("ICB{0:000000}", db.CreditNotes.Count() + 1) };
                db.CreditNotes.Add(creditNote);
                db.SaveChanges();
            }
            creditNote.Invoice = GetInvoice(id);
            return creditNote;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> CreditNote(Guid id)
        {
            var order = await db.OrderHeaders.Include(m => m.OrderLines)
                .Include(m => m.Client.DeliveryAddress)
                .FirstAsync(m => m.OrderHeaderId.Equals(id));
            ViewBag.CreditNote = GetCreditNote(id);
            return View(order);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> IssueCreditNote(Guid id, int editLineItems=0)
        {
            
            var userId = Guid.Parse(User.Identity.GetUserId());
            var order = await db.OrderHeaders.Include(m => m.OrderLines)
                .FirstAsync(m => m.OrderHeaderId.Equals(id));
            if (order.OrderStatusId != 5)
            {
                var sql = string.Format("[spReverseJournals] '{0}', '{1}', '{2}'", id, userId, "Credit Note");
                db.Database.ExecuteSqlCommand(sql);

                order.OrderStatusId = 5;
                db.SaveChanges();

                if (editLineItems == 1)
                {
                    var newOrderId = CopyOrder(id,userId);
                    return RedirectToAction("Edit", new { id = newOrderId });
                }
            }
            return RedirectToAction("CreditNote", new {id=id });
            
        }

        private Guid CopyOrder(Guid id, Guid userId)
        {
            var oldHeader = db.OrderHeaders.Find(id);
            var newHeader = new OrderHeader()
            {
                Active = true,
                ClientID = oldHeader.ClientID,
                OrderDate = DateTime.Now,
                OrderHeaderId = Guid.NewGuid(),
                OrderStatusId = 1,
                SaleSourceId = oldHeader.SaleSourceId,
                SalesTypeId = oldHeader.SalesTypeId,
                Shipping=oldHeader.Shipping,
                ShippingTypeId=oldHeader.ShippingTypeId,
                Total = oldHeader.Total,
                UserId=userId,
                VAT = oldHeader.VAT
            };

            db.OrderHeaders.Add(newHeader);
            db.SaveChanges();

            foreach (var oldLine in oldHeader.OrderLines)
            {
                var newLine = new OrderLine()
                {
                    Active = true,
                    OrderHeaderId = newHeader.OrderHeaderId,
                    ProductID=oldLine.ProductID,
                    Quantity=oldLine.Quantity,
                    UnitCost=oldLine.UnitCost,
                    UnitCostExcl=oldLine.UnitCostExcl
                };
                db.OrderLines.Add(newLine);
            }
            db.SaveChanges();

            return newHeader.OrderHeaderId;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Pay(PayOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.Identity.GetUserId());
                var toAccount = model.AccountInfo.Where(m => m.DestinationAccount).Select(m => m.ClientAccountId).FirstOrDefault();
                var order = db.OrderHeaders.Find(model.OrderHeaderId);
                if (toAccount.Equals(Guid.Empty))
                {
                    toAccount = await db.ClientAccounts.Where(m => m.AccountId.Equals(order.SalesType.AccountId) && m.ClientID == order.ClientID).Select(m => m.ClientAccountId).FirstOrDefaultAsync();
                }
                if (toAccount.Equals(Guid.Empty))
                {
                    toAccount = Guid.NewGuid();
                    db.ClientAccounts.Add(new finClientAccount()
                    {
                        AccountId = order.SalesType.AccountId,
                        Active = true,
                        ClientAccountId = toAccount,
                        ClientID = order.ClientID
                    });
                    await db.SaveChangesAsync();
                }

                /////////////////////////////////////////////////////
                DataTable dt = new DataTable();
                dt.Columns.Add("FromAccountID");
                dt.Columns.Add("ToAccountID");
                dt.Columns.Add("Amount", typeof(decimal));
                dt.Columns.Add("FromEffectiveDate");
                dt.Columns.Add("ToEffectiveDate");
                dt.Columns.Add("JournalDateDate");
                dt.Columns.Add("MovementSource");
                dt.Columns.Add("Comment");
                dt.Columns.Add("UserID");

                foreach (var acc in model.AccountInfo.Where(m => !m.DestinationAccount))
                {
                    DataRow dr = dt.NewRow();
                    dr["FromAccountID"] = acc.ClientAccountId;
                    dr["ToAccountID"] = toAccount;
                    dr["Amount"] = acc.TransferAmount;
                    dr["FromEffectiveDate"] = DateTime.Now.ToString(Generic.LongDate);
                    dr["ToEffectiveDate"] = acc.EffectiveDate.ToString(Generic.LongDate);
                    dr["JournalDateDate"] = DateTime.Now.ToString(Generic.LongDate);
                    dr["MovementSource"] = model.OrderHeaderId;
                    dr["Comment"] = "Order payment";
                    dr["UserID"] = userId;
                    dt.Rows.Add(dr);
                }
                ////Use DbType.Structured for TVP
                var userdetails = new SqlParameter("JournalInput", SqlDbType.Structured);
                userdetails.Value = dt;
                userdetails.TypeName = "tpfinJournal_Base";


                db.Database.ExecuteSqlCommand("EXEC spfinCreateJournal_Base @JournalInput", userdetails);

                /////////////////////////////////////////////////////
            }
            return View(model);
        }

        // GET: OrderHeaders
        [Authorize]
        [HttpGet]
        [LayoutInjecter("_LayoutNoLogo")]
        public async Task<ActionResult> Index(int? clientId, Guid? specificUserId, DateTime? frd, DateTime? tod, bool getSeller = false)
        {
            DateTime bm = DateTime.Today.AddDays(DateTime.Today.Day * -1);
            DateTime fd =  (clientId.HasValue ? bm : DateTime.Today);
            DateTime td = DateTime.Today.AddDays(1);

            if (frd.HasValue) fd = frd.Value;
            if (tod.HasValue) td = tod.Value;

            var statusList = (from item in db.OrderStatuses.Where(m=>m.Active).ToArray()
                          select new KeyValuePair<int, string>( (int)Math.Pow(2, item.OrderStatusId),  item.OrderStatusName )).ToArray();

            ViewBag.FromDate = fd;
            ViewBag.ToDate = td;
            ViewBag.Statuses = statusList.Select(m=>m.Key).Sum();
            ViewBag.StatusList = statusList;

            var elegibleOrders = db.OrderHeaders.Where(m => m.OrderDate >= fd
                                                        && m.OrderDate <= td
                                                        && m.Active).Include(o => o.Client).Include(o => o.OrderStatus).Include(o => o.SalesType);
            var headerIds = elegibleOrders.Select(m=>m.OrderHeaderId).ToArray();
            var userId = Guid.Parse(User.Identity.GetUserId());
            if (specificUserId.HasValue) userId = specificUserId.Value;
            ViewBag.TrackingNumbers = GetTrackingNumbers(headerIds);
            if (clientId.HasValue)
            {
                ViewBag.ClientId = clientId.Value;
                var ordHeaders = elegibleOrders.OrderByDescending(m => m.OrderDate).Where(m => m.ClientID == clientId.Value);
                return View(await ordHeaders.ToListAsync());
            }
            else if (getSeller)
            {
                ViewBag.GetSeller = getSeller;
                var ordHeaders = elegibleOrders.OrderByDescending(m => m.OrderDate).Where(m => m.UserId.Equals(userId));
                return View(await ordHeaders.ToListAsync());
            }
            var orderHeaders = elegibleOrders.OrderByDescending(m => m.OrderDate);
            ViewBag.MyClientId = Generic.GetMyClientId(db,userId);
            ViewBag.SpecificUserId = specificUserId;

            return View(await orderHeaders.ToListAsync());
        }

        internal static string GetInvoice(AromaContext db, int orderLineId)
        {
            var orderHeaderId = db.OrderLines.Find(orderLineId).OrderHeaderId;
            return GetInvoice(db, orderHeaderId).Number;
        }

        private KeyValuePair<Guid, string[]>[] GetTrackingNumbers(Guid[] orderHeaderIds)
        {
            var ret = new List<KeyValuePair<Guid, string[]>>();
            foreach (var orderHeaderId in orderHeaderIds)
            {
                var lines = db.OrderLines.Where(m => m.OrderHeaderId.Equals(orderHeaderId)).Select(m=>m.OrderLineId).ToArray();
                var tracking = db.PickingListDetails.Where(m => lines.Contains(m.OrderLineId)).Select(m => m.TrackingNumber).Distinct().ToArray();
                var item = new KeyValuePair<Guid, string[]>(orderHeaderId, tracking);
                ret.Add(item);
            }
            return ret.ToArray();
        }

        [Authorize]
        [HttpPost]
        [LayoutInjecter("_LayoutNoLogo")]
        public async Task<ActionResult> Index(int? clientId, DateTime fromDate, DateTime toDate, int statuses,Guid? specificUserId, bool getSeller = false)
        {
            var newtoDate = toDate.AddDays(1);
            var statusList = (from item in db.OrderStatuses.Where(m => m.Active).ToArray()
                              select new KeyValuePair<int, string>((int)Math.Pow(2, item.OrderStatusId), item.OrderStatusName)).ToArray();

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Statuses = statuses;
            ViewBag.StatusList = statusList;

            var elegibleOrders = db.OrderHeaders.Where(m => m.OrderDate >= fromDate 
                                                        && m.OrderDate <= newtoDate
                                                        && ((int)Math.Pow(2, (m.OrderStatusId)) & statuses) == (int)Math.Pow(2, (m.OrderStatusId))
                                                        && m.Active).Include(o => o.Client).Include(o => o.OrderStatus).Include(o => o.SalesType);
            var headerIds = elegibleOrders.Select(m => m.OrderHeaderId).ToArray();
            
            ViewBag.TrackingNumbers = GetTrackingNumbers(headerIds);
            var userId = Guid.Parse(User.Identity.GetUserId());
            if (specificUserId.HasValue) userId = specificUserId.Value;
            if (clientId.HasValue)
            {
                ViewBag.ClientId = clientId.Value;
                var ordHeaders = elegibleOrders.OrderByDescending(m => m.OrderDate).Where(m => m.ClientID == clientId.Value);
                var lst1 = await ordHeaders.ToListAsync();
                return View(lst1);
            }
            else if (getSeller)
            {
                ViewBag.GetSeller = getSeller;
                var ordHeaders = elegibleOrders.OrderByDescending(m => m.OrderDate).Where(m => m.UserId.Equals(userId));
                var lst2 = await ordHeaders.ToListAsync();
                return View(lst2);

            }
            var orderHeaders = elegibleOrders.OrderByDescending(m => m.OrderDate);
            ViewBag.MyClientId = Generic.GetMyClientId(db, userId);
            ViewBag.SpecificUserId = specificUserId;
            var lst3 = await orderHeaders.ToListAsync();
            return View(lst3);

        }

        // GET: OrderHeaders/Details/5
        [Authorize]

        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return HttpNotFound();
            }

            ViewBag.Journals = LoadJournals(orderHeader.OrderHeaderId);

            return View(orderHeader);
        }

        public AccountMovementViewModel LoadJournals(Guid movementSourceId)
        {

            var model = new AccountMovementViewModel();

            model.Journals = new List<finJournal>();

            var finJournals = (from item in db.Journals
                               where item.MovementSource.Equals(movementSourceId)
                               orderby item.JournalDate
                               select item).ToList();

            while (finJournals.Count > 0)
            {
                var leg1 = finJournals[0];
                var leg2 = (from finJournal item in finJournals
                            where leg1.CorrespondingJournalId.Equals(item.JournalId)
                            select item).FirstOrDefault();

                if (leg2 == null)
                {
                    leg2 = db.Journals.First(m => m.JournalId.Equals(leg1.CorrespondingJournalId));
                }
                else
                {
                    finJournals.Remove(leg2);
                }

                finJournals.Remove(leg1);
                model.Journals.Add(leg1);
                model.Journals.Add(leg2);
            }

            var RelevantAccounts = model.Journals.Select(m => m.AccountID).Distinct().ToArray();

            model.Accounts = (from item in db.Accounts.ToArray()
                              where RelevantAccounts.Count() == 0 || RelevantAccounts.Contains(item.AccountId)
                              select new AccountViewModel()
                              {
                                  AccountDescription = item.AccountName,
                                  AccountId = item.AccountId,
                                  IsClientAccount = false
                              }).ToList();

            model.Accounts.AddRange((from item in db.ClientAccounts.ToArray()
                                     where RelevantAccounts.Contains(item.ClientAccountId)
                                     select new AccountViewModel()
                                     {
                                         AccountDescription = string.Format("{0} ({1})", db.Accounts.First(m => m.AccountId.Equals(item.AccountId)).AccountName, item.ClientID),
                                         AccountId = item.ClientAccountId,
                                         IsClientAccount = true
                                     }));

            int index = 0;
            var openBalanceDate = DateTime.Now;
            if (model.Journals.Count > 0) openBalanceDate = model.Journals.Select(m => m.JournalDate).Min();
            foreach (var acc in model.Accounts)
            {

                acc.OpenBalance = 0;
                try
                {
                    acc.OpenBalance = db.Journals.Where(m => m.AccountID.Equals(acc.AccountId) && m.Active && m.EffectiveDate < openBalanceDate && m.JournalDate < openBalanceDate).Sum(m => m.Amount);
                }
                catch { }
                acc.Balance =  model.Journals.Where(m=>m.AccountID.Equals(acc.AccountId) && m.Active).Sum(m=>m.Amount);
                if (acc.IsClientAccount)
                {
                    acc.Balance += acc.OpenBalance;
                }
                acc.FutureBalance = 0;
                acc.FutureBalanceDate = DateTime.Now;
                acc.columnIndex = index;
                index++;
            }
            return model;
        }

        [Authorize]
        public ActionResult CreateOrder(int clientId, int salesTypeId)
        {
            var salesType = db.SalesTypes.Find(salesTypeId);
            var shippingType = db.ShippingTypes.Find(salesType.DefaultShippingTypeId);
            var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
            var newOrder = new OrderHeader() { Active = true, OrderDate = DateTime.Now, OrderStatusId = 8, UserId = currentUserId, ShippingTypeId = shippingType.ShippingTypeId, SalesTypeId = salesTypeId, OnceOff = false,ClientID=clientId };
                db.OrderHeaders.Add(newOrder);
                db.SaveChanges();
            return RedirectToAction("Edit", new { id = newOrder.OrderHeaderId });
        }

        // GET: OrderHeaders/Create
        [Authorize]
        public ActionResult Create(int? ClientId, bool onceOff=false)
        {
            Client client = null;
            var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
            bool ownOrder = IsOwnOrder(db,ClientId, currentUserId);
            if (ClientId.HasValue)
            {
                client = db.Clients.Find(ClientId.Value);
                
            }
                var pc = db.PostageCharges.Where(m => m.Active).ToArray();
            if (ClientId.HasValue)
            {
                var address = client.DeliveryAddress;
                if (address == null)
                {
                    address = (from item in db.Addresses
                               where item.ClientID == ClientId.Value
                               && item.AddressTypeID == 2
                               select item).FirstOrDefault();
                }
                if (address == null)
                {
                    ModelState.AddModelError("OrderDate", "This client does not have a valid shipping address");
                }
                else
                {
                    var code = db.PostalCodes.FirstOrDefault(m => m.Active && m.PostalCodeName == address.Code);
                    if (code != null)
                    {
                        var link = db.PostalCodePostageCharges.FirstOrDefault(m => m.PostalCodeId == code.PostalCodeId);
                        if (link != null)
                        {
                            ViewBag.PostageChargeId = new SelectList(pc, "PostageChargeId", "PostageChargeName", link.PostageChargeId);
                            ViewBag.PostageChargeSetUp = true;
                        }
                    }
                }
            }

            if (ViewBag.PostageCharge == null)
            {
                ViewBag.PostageChargeSetUp = false;
                ViewBag.PostageChargeId = new SelectList(pc, "PostageChargeId", "PostageChargeName");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials");
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusName");
            var salesTypeId = 1;
            if ( onceOff && !ownOrder)
            {
                var subscriptions = (from item in db.Subscriptions
                                    where item.ValidFromDate <= DateTime.Now
                                    && item.ClientTypeID == client.ClientTypeID
                                    && !item.InitialOnceOffFromAccountID.Equals(Guid.Empty)
                                    select item).ToArray();
                var salesTypeIds = subscriptions.Select(m => m.SalesTypeID).Distinct().ToArray();
                var productIds = subscriptions.Select(m => m.ProductID).Distinct().ToArray();
                
                ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active && salesTypeIds.Contains(m.SalesTypeId)).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
                ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m => m.ProductName), "ProductID", "ProductName");
            }
            else
            {
                var subscriptions = (from item in db.Subscriptions
                                     where item.ValidFromDate <= DateTime.Now
                                     && item.ClientTypeID == client.ClientTypeID
                                     select item.ProductID).Distinct().ToArray();
                ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active && (!ownOrder || (ownOrder && m.SalesTypeId == Generic.OwnOrderSalesTypeId))).OrderBy(m=>m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
                ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && subscriptions.Contains(m.ProductID)).OrderBy(m=>m.ProductName), "ProductID", "ProductName");
            }
            
            var newOrder = new OrderHeader() {Active = true, OrderDate = DateTime.Now, OrderStatusId=1, UserId=currentUserId,ShippingTypeId=1, SalesTypeId=salesTypeId, OnceOff = onceOff };

            ViewBag.ShippingCost = new KeyValuePair<string, string>[0];

            if (ClientId.HasValue)
            {
                newOrder.Client = client;
                ViewBag.ContactInfo = (from item in db.ContactTypes.Where(m => m.Active).ToArray()
                                       select new string[] { item.ContactTypeName, db.Contacts.Where(m=>m.Active
                                                                                                && m.ContactTypeID == item.ContactTypeId
                                                                                                && m.ClientID==ClientId.Value).Select(m=>m.ContactName).FirstOrDefault()
                                                        }).ToArray();
                var shipping = (from item in db.ShippingMethodPostalCodes
                                        where item.PostalCode.PostalCodeName == newOrder.Client.DeliveryAddress.Code
                                        select item).ToArray();
               // ViewBag.ShippingCost = shipping.Select(m => new KeyValuePair<string, string>(string.Format("{0}:{1}", m.ShippingMethod.ShippingMethodName, m.Description), m.ExtraCost.ToString("R#,###,##0.00"))).Take(5).ToArray();
                

            }
            return View(newOrder);
        }
        
        private static bool IsOwnOrder(AromaContext db, int? clientId, Guid userId)
        {
            if (clientId.HasValue)
            {
                var userClient = db.UserClients.FirstOrDefault(m => m.UserId.Equals(userId));
                if (userClient != null)
                {
                    return (userClient.ClientId == clientId.Value);
                }
            }
            return false;
        }

        // POST: OrderHeaders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderHeaderId,ClientID,Total,OrderStatusId,UserId,OrderDate,ShippingTypeId,SalesTypeId,OnceOff")] OrderHeader orderHeader)
        {
            var address = db.Clients.Find(orderHeader.ClientID).DeliveryAddress;
            if (address == null)
            {
                address = (from item in db.Addresses
                           where item.ClientID == orderHeader.ClientID
                           && item.AddressTypeID == 2
                           select item).FirstOrDefault();
            }
                if (address == null)
            {
                ModelState.AddModelError("OrderDate", "This client does not have a valid shipping address");
            }
            if (ModelState.IsValid)
            {
                
                orderHeader.OrderHeaderId = Guid.NewGuid();
                orderHeader.Active = true;
                orderHeader.Address = address;
                orderHeader.ShippingTypeId = db.SalesTypes.Find(orderHeader.SalesTypeId).DefaultShippingTypeId;
                orderHeader.OrderStatusId = 8;
                db.OrderHeaders.Add(orderHeader);
                await db.SaveChangesAsync();
                //Persist Invoice
                var inv = GetInvoice(orderHeader.OrderHeaderId);

                return RedirectToAction("Edit", new {id=orderHeader.OrderHeaderId});
            }
            ViewBag.ShippingCost = new KeyValuePair<string, string>[0];
            var client = db.Clients.Find(orderHeader.ClientID);
            orderHeader.Client = client;
                ViewBag.ContactInfo = (from item in db.ContactTypes.Where(m => m.Active).ToArray()
                                       select new string[] { item.ContactTypeName, db.Contacts.Where(m=>m.Active
                                                                                                && m.ContactTypeID == item.ContactTypeId
                                                                                                && m.ClientID==orderHeader.ClientID).Select(m=>m.ContactName).FirstOrDefault()
                                                        }).ToArray();
                var shipping = (from item in db.ShippingMethodPostalCodes
                                where item.PostalCode.PostalCodeName == orderHeader.Client.DeliveryAddress.Code
                                select item).ToArray();
            //ViewBag.ShippingCost = shipping.Select(m => new KeyValuePair<string, string>(string.Format("{0}:{1}", m.ShippingMethod.ShippingMethodName, m.Description), m.ExtraCost.ToString("R#,###,##0.00"))).ToArray();
            if (orderHeader.OnceOff)
            {
                var subscriptions = (from item in db.Subscriptions
                                     where item.ValidFromDate <= DateTime.Now
                                     && item.ClientTypeID == client.ClientTypeID
                                     && !item.InitialOnceOffFromAccountID.Equals(Guid.Empty)
                                     select item).ToArray();
                var salesTypeIds = subscriptions.Select(m => m.SalesTypeID).Distinct().ToArray();
                var productIds = subscriptions.Select(m => m.ProductID).Distinct().ToArray();

                ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active && salesTypeIds.Contains(m.SalesTypeId)).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
                ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m => m.ProductName), "ProductID", "ProductName");
            }
            else
            {
                ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
                ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active).OrderBy(m => m.ProductName), "ProductID", "ProductName");
            }
            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", orderHeader.ClientID);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active), "ProductID", "ProductName");


            return View(orderHeader);
        }

        internal static void UpdateHeader(ref OrderHeader header)
        {
            header.Total = 0;
            foreach (var line in header.OrderLines)
            {
                header.Total += (line.Quantity * line.UnitCost);
            }
        }

        [Authorize]
        public async Task<ActionResult> AdminEdit(Guid? id)
        {
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses.Where(m => m.Active), "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            //ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m=>m.ProductName), "ProductID", "ProductName");
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes.Where(m => m.Active), "ShippingTypeId", "ShippingTypeName", orderHeader.ShippingTypeId);
            ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription"); return View(orderHeader);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminEdit(
            [Bind(
                Include =
                    "OrderHeaderId,ClientID,Total,OrderStatusId,UserId,OrderDate,Active,ShippingTypeId,SalesTypeId,OnceOff,VAT,Shipping,SaleSourceID"
                )] OrderHeader orderHeader)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
                orderHeader.UserId = currentUserId;
                db.Entry(orderHeader).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = orderHeader.OrderHeaderId });
            }
            ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses.Where(m => m.Active), "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            //ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m=>m.ProductName), "ProductID", "ProductName");
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes.Where(m => m.Active), "ShippingTypeId", "ShippingTypeName", orderHeader.ShippingTypeId);
            return View(orderHeader);
        }

        // GET: OrderHeaders/Edit/5
        [Authorize]
       public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            var invalidStatuses = new int[] {2,3,4,5 };
            if (invalidStatuses.Contains( orderHeader.OrderStatusId))
            {
                if(User.IsInRole("Administrator"))
                    return RedirectToAction("AdminEdit", new { id = orderHeader.OrderHeaderId });


                return RedirectToAction("Details", new {id=orderHeader.OrderHeaderId });
            }
            if (orderHeader == null)
            {
                return HttpNotFound();
            }
            var userId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
            var ownOrder = IsOwnOrder(db, orderHeader.ClientID, userId);
            if(ownOrder )
            {
                orderHeader.SaleSourceId = Generic.OwnOrderSalesTypeId;
            }
            //ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", orderHeader.ClientID);
            var client = await db.Clients.FirstAsync(m => m.ClientId == orderHeader.ClientID);
            int[] productIds = new int[0];
            if (orderHeader.OnceOff && !ownOrder)
            {
                var subscriptions = (from item in db.Subscriptions
                                     where item.ValidFromDate <= DateTime.Now
                                     && item.ClientTypeID == client.ClientTypeID
                                     && !item.InitialOnceOffFromAccountID.Equals(Guid.Empty)
                                     select item).ToArray();
                var salesTypeIds = subscriptions.Select(m => m.SalesTypeID).Distinct().ToArray();
                productIds = subscriptions.Where(m=>m.ClientTypeID==client.ClientTypeID).Select(m => m.ProductID).Distinct().ToArray();

                ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active && salesTypeIds.Contains(m.SalesTypeId)).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
                ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m => m.ProductName), "ProductID", "ProductName");
            }
            else
            {
                productIds = (from item in db.Subscriptions where item.ClientTypeID == client.ClientTypeID && item.Active select item.ProductID).ToArray();
                ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
                ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m => m.ProductName), "ProductID", "ProductName");
            }

            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses.Where(m => m.Active), "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            //ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m=>m.ProductName), "ProductID", "ProductName");
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes.Where(m => m.Active ), "ShippingTypeId", "ShippingTypeName", orderHeader.ShippingTypeId);

            orderHeader.Total = 0;
            foreach (var line in orderHeader.OrderLines.Where(m=>m.Active))
            {
                line.UnitCost = db.Subscriptions.Where(m => m.ProductID == line.ProductID && m.ClientTypeID == client.ClientTypeID).First().Price;
                orderHeader.Total += line.UnitCost * line.Quantity;
            }
            db.SaveChanges();

            if (orderHeader.ShippingType.ShippingList)
            {
                var pc = db.PostageCharges.Where(m => m.Active).ToArray();
                ViewBag.ShowPostage = orderHeader.ShippingType.ShippingList;
                var postageChargeSetUp = false;
                if (client.DeliveryAddress != null)
                {
                    var code = db.PostalCodes.FirstOrDefault(m => m.Active && m.PostalCodeName == client.DeliveryAddress.Code);
                    if (code != null)
                    {
                        var shippingMethodPostalCode = (from item in db.ShippingMethodPostalCodes
                                                        where item.PostalCodeId == code.PostalCodeId
                                                        && item.Active
                                                        select item).FirstOrDefault();
                        if (shippingMethodPostalCode != null)
                        {
                            var postageChargeId = (from item in db.PostageCharges
                                                   where item.Charge == shippingMethodPostalCode.ExtraCost
                                                   select item.PostageChargeId).FirstOrDefault();
                            ViewBag.PostageChargeId = new SelectList(pc, "PostageChargeId", "PostageChargeName", postageChargeId);
                            postageChargeSetUp = true;
                            if (orderHeader.Client.ClientType.AddShipping)
                            {
                                orderHeader.Shipping = shippingMethodPostalCode.ExtraCost;
                            }
                        }
                        else
                        {
                            var link = db.PostalCodePostageCharges.FirstOrDefault(m => m.PostalCodeId == code.PostalCodeId);
                            if (link != null)
                            {

                                ViewBag.PostageChargeId = new SelectList(pc, "PostageChargeId", "PostageChargeName", link.PostageChargeId);
                                postageChargeSetUp = true;
                                if (link.PostageCharge == null) link.PostageCharge = db.PostageCharges.Find(link.PostageChargeId);
                                if (orderHeader.Client.ClientType.AddShipping)
                                {
                                    orderHeader.Shipping = link.PostageCharge.Charge;
                                }
                            }
                        }
                        orderHeader.VAT = (orderHeader.Total + orderHeader.Shipping) / ((100 + Generic.VATPercent)/100);
                    }
                }
                if (!postageChargeSetUp)
                {
                    ViewBag.Warning = "Charges has not been set up for this delivery address. Please be sure to select the correct shipping charges.";
                    ViewBag.PostageChargeId = new SelectList(pc, "PostageChargeId", "PostageChargeName");
                }

                ViewBag.PostageChargeSetUp = postageChargeSetUp;
            }
            ViewBag.ShowPostage = orderHeader.ShippingType.ShippingList;
            ViewBag.OwnOrder = ownOrder;
            return View(orderHeader);
        }

        // POST: OrderHeaders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderHeaderId,ClientID,Total,OrderStatusId,UserId,OrderDate,Active,ShippingTypeId,SalesTypeId,OnceOff")] OrderHeader orderHeader, int Navigate, int PostageChargeId=1, int WavePostage =0, int JustSave=0)
        {
            var client = await db.Clients.FirstAsync(m => m.ClientId == orderHeader.ClientID);
            int[] productIds = new int[0];
            var userId = Guid.Parse(User.Identity.GetUserId());
            var ownOrder = IsOwnOrder(db, client.ClientId, userId);
            
            if (ModelState.IsValid)
            {
                orderHeader.OrderStatusId = 1;
                if (client.DeliveryAddress != null)
                {
                    var code = db.PostalCodes.FirstOrDefault(m => m.Active && m.PostalCodeName == client.DeliveryAddress.Code);
                    if (code != null)
                    {
                        var link = db.PostalCodePostageCharges.FirstOrDefault(m => m.PostalCodeId == code.PostalCodeId);
                        if (link == null)
                        {
                            link = new PostalCodePostageCharge() { PostageChargeId = PostageChargeId, PostalCodeId = code.PostalCodeId };
                            db.PostalCodePostageCharges.Add(link);
                        }
                        else if(link.PostageChargeId != PostageChargeId)
                        {
                            link.PostageChargeId = PostageChargeId;
                            db.SaveChanges();
                            link.PostageCharge = db.PostageCharges.Find(link.PostageChargeId);
                           
                        }

                        if (link.PostageCharge == null)
                            link.PostageCharge = db.PostageCharges.Find(link.PostageChargeId);

                        if (orderHeader.ShippingType == null)
                            orderHeader.ShippingType = db.ShippingTypes.Find(orderHeader.ShippingTypeId);

                        if (orderHeader.Client == null)
                            orderHeader.Client = db.Clients.Find(orderHeader.ClientID);

                        if (orderHeader.ShippingType.ShippingList && orderHeader.Client.ClientType.AddShipping && WavePostage == 0)
                        {
                            orderHeader.Shipping = link.PostageCharge.Charge;
                        }
                        else
                        {
                            orderHeader.Shipping = 0;
                        }
                    }
                }

                //if (orderHeader.SalesType == null) orderHeader.SalesType = db.SalesTypes.Find(orderHeader.SalesTypeId);
                //var account = orderHeader.SalesType.AccountId;
                //var bal = (from item in db.Journals
                //           where item.MovementSource.Equals(orderHeader.OrderHeaderId)
                //           select item.Amount)?.Sum();
                //if (bal == null) bal = 0;
                //if (bal < orderHeader.Total + orderHeader.Shipping && orderHeader.OrderStatusId == 1) orderHeader.OrderStatusId = 1;

                if (orderHeader.OnceOff)
                {
                    var subscriptions = (from item in db.Subscriptions
                                         where item.ValidFromDate <= DateTime.Now
                                         && item.ClientTypeID == client.ClientTypeID
                                         && !item.InitialOnceOffFromAccountID.Equals(Guid.Empty)
                                         select item).ToArray();
                    var salesTypeIds = subscriptions.Select(m => m.SalesTypeID).Distinct().ToArray();
                    productIds = subscriptions.Select(m => m.ProductID).Distinct().ToArray();

                    ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active && salesTypeIds.Contains(m.SalesTypeId)).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
                    ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m => m.ProductName), "ProductID", "ProductName");
                }
                else
                {
                    productIds = (from item in db.Subscriptions where item.ClientTypeID == client.ClientTypeID && item.Active select item.ProductID).ToArray();
                    ViewBag.SalesTypeId = new SelectList(db.SalesTypes.Where(m => m.Active).OrderBy(m => m.SalesTypeDescription), "SalesTypeId", "SalesTypeDescription");
                    ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active).OrderBy(m => m.ProductName), "ProductID", "ProductName");
                }
                if (orderHeader.OrderStatusId > 1 || orderHeader.Total == 0) orderHeader.OrderStatusId = 1;


                orderHeader.VAT = (orderHeader.Total + orderHeader.Shipping) / ((100 + Generic.VATPercent) / 100);
                var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(User.Identity));
                orderHeader.UserId = currentUserId;
                db.Entry(orderHeader).State = EntityState.Modified;
                await db.SaveChangesAsync();

                

                if (Navigate == 1)
                {
                    if (ownOrder)
                    {
                        var ticketText = string.Format("{0} {1} has created an order", client.FullNames, client.ClientSurname);
                        SystemTicketTemplatesController.PersistTicket(db, Generic.SystemEventOwnSale, orderHeader.OrderHeaderId, client.ClientId, Generic.SupportTicketTypeIdOrder, ticketText, userId);
                    }
                    if ((User.IsInRole("Finance") || (User.IsInRole("Area Distributor")|| User.IsInRole("Distributor"))) && orderHeader.OrderStatusId==1 && JustSave==0)
                    {
                        //http://localhost:60768/AccountMovement/InterAccountTransfer?clientId=10012
                        return RedirectToAction("InterAccountTransfer", "AccountMovement", new { clientId = orderHeader.ClientID, orderHeaderId = orderHeader.OrderHeaderId });
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("spCreateSalesWithAvailableFundsAll");
                        return RedirectToAction("Details", "OrderHeaders", new { id = orderHeader.OrderHeaderId });
                    }
                }
            }

            orderHeader = await db.OrderHeaders.Include(mbox=>mbox.OrderLines).Include(m=>m.ShippingType).FirstAsync(m=>m.OrderHeaderId.Equals(orderHeader.OrderHeaderId));

            ViewBag.ClientID = new SelectList(db.Clients, "ClientId", "ClientInitials", orderHeader.ClientID);
            //var productIds = (from item in db.Subscriptions where item.ClientTypeID == client.ClientTypeID && item.Active select item.ProductID).ToArray();
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)).OrderBy(m => m.ProductName), "ProductID", "ProductName");
            ViewBag.ShippingTypeId = new SelectList(db.ShippingTypes.Where(m => m.Active), "ShippingTypeId", "ShippingTypeName", orderHeader.ShippingTypeId);


            if (orderHeader.ShippingType == null) orderHeader.ShippingType = await db.ShippingTypes.FindAsync(orderHeader.ShippingTypeId);
            if (orderHeader.ShippingType.ShippingList)
            {
                var pc = db.PostageCharges.Where(m => m.Active).ToArray();
                ViewBag.ShowPostage = orderHeader.ShippingType.ShippingList;
                var postageChargeSetUp = false;
                if (client.DeliveryAddress != null)
                {
                    var code = db.PostalCodes.FirstOrDefault(m => m.Active && m.PostalCodeName == client.DeliveryAddress.Code);
                    if (code != null)
                    {
                        var link = db.PostalCodePostageCharges.FirstOrDefault(m => m.PostalCodeId == code.PostalCodeId);
                        if (link != null)
                        {

                            ViewBag.PostageChargeId = new SelectList(pc, "PostageChargeId", "PostageChargeName", link.PostageChargeId);
                            postageChargeSetUp = true;
                        }
                    }
                }
                if (!postageChargeSetUp)
                {
                    ViewBag.Warning = "Charges has not been set up for this delivery address. Please be sure to select the correct shipping charges.";
                    ViewBag.PostageChargeId = new SelectList(pc, "PostageChargeId", "PostageChargeName");
                }

                ViewBag.PostageChargeSetUp = postageChargeSetUp;
            }
            ViewBag.ShowPostage = orderHeader.ShippingType.ShippingList;

            ViewBag.OwnOrder = ownOrder;
            return View(orderHeader);
        }

        // GET: OrderHeaders/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return HttpNotFound();
            }
           
            var client = await db.Clients.FirstAsync(m => m.ClientId == orderHeader.ClientID);
            var productIds = (from item in db.Subscriptions where item.ClientTypeID == client.ClientTypeID && item.Active select item.ProductID).ToArray();
            ViewBag.OrderStatusId = new SelectList(db.OrderStatuses, "OrderStatusId", "OrderStatusName", orderHeader.OrderStatusId);
            ViewBag.ProductList = new SelectList(db.Products.Where(m => m.Active && productIds.Contains(m.ProductID)), "ProductID", "ProductName");
            var journals = (from item in db.Journals.Where(m => m.MovementSource.Equals(orderHeader.OrderHeaderId))
                            select item).ToArray();



            return View(orderHeader);
        }

        // POST: OrderHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            OrderHeader orderHeader = await db.OrderHeaders.FindAsync(id);
            db.OrderHeaders.Remove(orderHeader);
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

        public static void FixScewedTotals(AromaContext db)
        {
            var incorrectTotals =
                (from item in db.OrderHeaders
                 where 
                 item.Active
                 && item.Total != item.OrderLines.Where(m => m.Active).Sum(m => m.Quantity * m.UnitCost)
                 && item.Total != 0
                 && item.OrderLines.Sum(m => m.Quantity * m.UnitCost) != 0
                 select item).ToArray();
            foreach (var incorrectTotal in incorrectTotals)
            {
                incorrectTotal.Total = incorrectTotal.OrderLines.Sum(m => m.Quantity * m.UnitCost);
            }

            db.SaveChanges();
        }
    }
}
