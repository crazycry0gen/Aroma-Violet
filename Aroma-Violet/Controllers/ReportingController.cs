﻿namespace Aroma_Violet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using Aroma_Violet.Models;

    using Microsoft.AspNet.Identity;

    public class ReportingController : Controller
    {
        private int[] _cashSalesSaleTypes = new[] { 7, 8, 9, 11, 12 };

        private int[] _distrbutedSaleTypes = new[] { 13 };

        private AromaContext db = new AromaContext();

        [Authorize]
        [HttpGet]
        [LayoutInjecter("_LayoutNoLogo")]
        public ActionResult AccountBalance()
        {
            var model = this.GetAccountBalances();
            return this.View(model);
        }

        // GET: Reporting
        [Authorize]
        [HttpGet]
        public ActionResult CashSales()
        {
            var model = this.GetModel(DateTime.Today, DateTime.Today, this._cashSalesSaleTypes);
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CashSales(DateTime fromDate, DateTime toDate)
        {
            var model = this.GetModel(fromDate, toDate, this._cashSalesSaleTypes);
            return this.View(model);
        }

        [Authorize]
        public ActionResult CommissionStatement(int? specificClientId, int? specificPeriodId)
        {
            var clients =
                (specificClientId.HasValue
                     ? new[] { specificClientId.Value }
                     : this.db.Clients.Where(m => m.Active).Select(m => m.ClientId).ToArray()).OrderBy(m => m).ToArray();
            var periodIds = specificPeriodId.HasValue
                                ? new[] { specificPeriodId.Value }
                                : new[] { this.db.MonthEndRows.Max(m => m.PeriodId) };
            var monthEnds = (from item in this.db.MonthEndRows.Include(m => m.Detail)
                             where clients.Contains(item.ClientId) && periodIds.Contains(item.PeriodId)
                             select item).ToArray();
            var model = new List<ReportCommisionStatementViewModel>();

            foreach (var clientId in clients)
            {
                var client = this.db.Clients.Find(clientId);
                var statement = new ReportCommisionStatementViewModel();
                statement.Name = string.Format("{0} {1}", client.FullNames, client.ClientSurname);
                statement.ClientId = clientId;
                statement.Description = DateTime.Now.ToString("MMM yyyy");
                statement.Periods = new List<ReportCommisionStatementPeriodViewModel>();
                foreach (var currentPeriodId in periodIds)
                {
                    var currentMonthends = (from item in monthEnds
                                            where item.ClientId == clientId && item.PeriodId == currentPeriodId
                                            orderby item.DownlineIndex
                                            select item).ToArray();
                    foreach (var currentMonthend in currentMonthends)
                    {
                        var newItem = new ReportCommisionStatementPeriodViewModel()
                                          {
                                              PeriodStart =
                                                  RebateClientTypesController
                                                  .GetPeriodStart(
                                                      currentPeriodId)
                                                  .ToString(
                                                      Generic.LongDateNoTime),
                                              PeriodEnd =
                                                  RebateClientTypesController
                                                  .GetPeriodEnd(
                                                      currentPeriodId)
                                                  .ToString(
                                                      Generic.LongDateNoTime),
                                              Level =
                                                  currentMonthend
                                                  .DownlineIndex,
                                              Amount = currentMonthend.Amount,
                                              AmountPer =
                                                  currentMonthend.AmountPer,
                                              AmountValue =
                                                  currentMonthend.AmountValue,
                                              SubscriptionFirstProduct =
                                                  currentMonthend
                                                  .SubscriptionFirstProduct,
                                              SubscriptionFirstProductPer =
                                                  currentMonthend
                                                  .SubscriptionFirstProductPer,
                                              SubscriptionFirstProductValue =
                                                  currentMonthend
                                                  .SubscriptionFirstProductValue,
                                              SubscriptionOtherProduct =
                                                  currentMonthend
                                                  .SubscriptionOtherProduct,
                                              SubscriptionOtherProductPer =
                                                  currentMonthend
                                                  .SubscriptionOtherProductPer,
                                              SubscriptionOtherProductValue =
                                                  currentMonthend
                                                  .SubscriptionOtherProductValue,
                                              Detail =
                                                  (from item in
                                                       currentMonthend.Detail
                                                   group item by
                                                       item.OrderHeader
                                                       .ClientID
                                                   into groupedSales
                                                   select
                                                       new ReportCommisionStatementPeriodDetailViewModel
                                                       ()
                                                           {
                                                               Client =
                                                                   this.db
                                                                   .Clients
                                                                   .Find(
                                                                       groupedSales
                                                                   .Key),
                                                               ClientDescription
                                                                   =
                                                                   this.db
                                                                   .GetDescription
                                                                   (
                                                                       groupedSales
                                                                   .Key),
                                                               Amount =
                                                                   groupedSales
                                                                   .Where(
                                                                       m =>
                                                                       m
                                                                           .OrderHeader
                                                                           .SaleSourceId
                                                                       == 0)
                                                                   .Select(
                                                                       m =>
                                                                       m
                                                                           .OrderHeader
                                                                           .Total)
                                                                   .Sum(),
                                                               SubscriptionAmount
                                                                   =
                                                                   groupedSales
                                                                   .Where(
                                                                       m =>
                                                                       m
                                                                           .OrderHeader
                                                                           .SaleSourceId
                                                                       == 1)
                                                                   .Select(
                                                                       m =>
                                                                       m
                                                                           .OrderHeader
                                                                           .Total)
                                                                   .Sum(),
                                                               Cell =
                                                                   this.db
                                                                   .GetContact(
                                                                       groupedSales
                                                                   .Key,
                                                                       Generic
                                                                   .enumContactType
                                                                   .Cell),
                                                               EMail =
                                                                   this.db
                                                                   .GetContact(
                                                                       groupedSales
                                                                   .Key,
                                                                       Generic
                                                                   .enumContactType
                                                                   .EMail),
                                                               Introducer =
                                                                   this.db
                                                                   .GetDescription
                                                                   (
                                                                       this.db
                                                                   .Clients
                                                                   .Find(
                                                                       groupedSales
                                                                   .Key)
                                                                   .ResellerID)
                                                           }).ToArray()
                                          };
                        statement.Periods.Add(newItem);
                    }

                    model.Add(statement);
                }
            }

            var periods =
                (from item in this.db.MonthEndRows select item.PeriodId).Distinct().OrderByDescending(m => m).ToArray();
            if (!specificPeriodId.HasValue && periods.Length > 0) specificPeriodId = periods[0];
            this.ViewBag.specificPeriodId = new SelectList(periods, specificPeriodId);
            return this.View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult CreditNote()
        {
            const int orderStatusCanceled = 5;
            var model = this.GetModel(DateTime.Today, DateTime.Today, orderStatusCanceled);
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreditNote(DateTime fromDate, DateTime toDate)
        {
            const int orderStatusCanceled = 5;
            var model = this.GetModel(fromDate, toDate, orderStatusCanceled);
            return this.View(model);
        }

        // GET: Reporting
        [Authorize]
        [HttpGet]
        public ActionResult CreditSales()
        {
            var model = this.GetModel(DateTime.Today, DateTime.Today, new[] { 10, 15 });
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreditSales(DateTime fromDate, DateTime toDate)
        {
            var model = this.GetModel(fromDate, toDate, new[] { 10, 15 });
            return this.View(model);
        }

        // GET: Reporting
        [Authorize]
        [HttpGet]
        public ActionResult DistrbutedSales()
        {
            var model = this.GetModel(DateTime.Today, DateTime.Today, this._distrbutedSaleTypes);
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DistrbutedSales(DateTime fromDate, DateTime toDate)
        {
            var model = this.GetModel(fromDate, toDate, this._distrbutedSaleTypes);
            return this.View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult DownlineStatement(int? clientId)
        {
            const int numberOfMonths = 3;
            var clientIds = clientId.HasValue
                                ? new[] { clientId.Value }
                                : this.db.Clients.Where(m => m.Active).Select(m => m.ClientId).ToArray();
            var parents = (from item in this.db.Clients where clientIds.Contains(item.ClientId) select item).ToArray();
            var model = (from item in parents select new ReportDownlineStatementViewModel(item)).ToArray();

            foreach (var parent in model)
            {
                List<OrderHeader> orders = new List<OrderHeader>();
                var parentIds = new[] { parent.ClientId };
                parent.Resellers = new List<ReportDownlineStatementResellerViewModel>();
                parent.Subscribers = new List<ReportDownlineStatementSubscriberViewModel>();

                for (int level = 1; level < 6; level++)
                {
                    var children =
                        (from item in this.db.Clients
                         where item.ResellerID.HasValue && parentIds.Contains(item.ResellerID.Value)
                         select item).ToArray();
                    foreach (var child in children)
                    {
                        orders.AddRange(this.db.GetLines(child.ClientId, numberOfMonths));
                    }

                    parent.Resellers.AddRange(
                        (from item in children
                         where item.ClientTypeID != 3
                         select
                             new ReportDownlineStatementResellerViewModel(
                             item,
                             level,
                             this.db.GetContact(item.ClientId, Generic.enumContactType.Cell),
                             this.db.GetContact(item.ClientId, Generic.enumContactType.EMail),
                             this.db.Clients.Find(item.ResellerID.Value))
                                 {
                                     AveragePurchases =
                                         this.db.GetAveragePurchases(
                                             item.ClientId,
                                             numberOfMonths),
                                     TotalPurchases =
                                         this.db.GetTotalPurchases(
                                             item.ClientId,
                                             numberOfMonths)
                                 }).ToArray());
                    parent.Subscribers.AddRange(
                        (from item in children
                         where item.ClientTypeID == 3
                         select
                             new ReportDownlineStatementSubscriberViewModel(
                             item,
                             level,
                             this.db.GetContact(item.ClientId, Generic.enumContactType.Cell),
                             this.db.GetContact(item.ClientId, Generic.enumContactType.EMail),
                             this.db.Clients.Find(item.ResellerID.Value))
                                 {
                                     SubscriptionAmount =
                                         this.db.GetSubscriptionAmount(
                                             item.ClientId)
                                 }).ToArray());
                    parentIds = (from item in children select item.ClientId).ToArray();
                }

                var glines = (from item in orders.OrderBy(m => m.OrderDate)
                              select
                                  new
                                      {
                                          Desc = item.OrderDate.ToString("MMM yyyy"),
                                          Total = item.SaleSourceId == 0 ? item.Total : 0,
                                          SubTotal = item.SaleSourceId != 0 ? item.Total : 0
                                      }).ToArray();

                parent.SalesValues = (from item in glines
                                      group item by item.Desc
                                      into months
                                      select
                                          new ReportDownlineStatementSalesValue()
                                              {
                                                  MonthDescription = months.Key,
                                                  Total = months.Sum(m => m.Total),
                                                  Subscription =
                                                      months.Sum(m => m.SubTotal),
                                                  Average =
                                                      glines.Sum(m => m.Total)
                                                      / numberOfMonths
                                              }).ToList();
            }

            return this.View(model);
        }

        public ActionResult DownlineStatementOwn()
        {
            var currentUserId = Guid.Parse(IdentityExtensions.GetUserId(this.User.Identity));
            var clientOwn = this.db.UserClients.FirstOrDefault(m => m.UserId.Equals(currentUserId));
            if (clientOwn != null)
            {
                return this.RedirectToAction("DownlineStatement", new { clientId = clientOwn.ClientId });
            }
            else
            {
                return this.HttpNotFound();
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetUserSales()
        {
            var toDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
            var fromDate = DateTime.Today.AddDays(-1);
            var model = this.GetLinkedUserModel(fromDate, toDate);
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetUserSales(DateTime fromDate, DateTime toDate)
        {
            var model = this.GetLinkedUserModel(fromDate, toDate);
            return this.View(model);
        }

        // GET: Reporting
        [Authorize]
        [HttpGet]
        public ActionResult Index(int showIndex = 0)
        {
            return this.View(showIndex);
        }

        // GET: Reporting
        [Authorize]
        [HttpGet]
        public ActionResult KioskSales()
        {
            var model = this.GetModel(DateTime.Today, DateTime.Today, new[] { 8 });
            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult KioskSales(DateTime fromDate, DateTime toDate)
        {
            var model = this.GetModel(fromDate, toDate, new[] { 8 });
            return this.View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Networkrelationship(int clientId = 0)
        {
            var networkClient = new NetworkClient();
            networkClient.LoadNetwork(this.db, clientId);
            return this.View(networkClient);
        }

        [Authorize]
        [HttpGet]
        public ActionResult SmsStatus()
        {
            var fromDate = DateTime.Today.AddDays(-1);
            var toDate = DateTime.Today.AddDays(1).AddSeconds(-1);
            var maxCount = 100;
            var smses =
                this.db.SystemSMSes.Where(m => (m.iDate >= fromDate && m.iDate <= toDate))
                    .OrderByDescending(m => m.iDate)
                    .Take(maxCount)
                    .ToList();

            var statuses = (from item in this.db.SystemSMSes select item.LastSendMessage).Distinct().ToList();
            statuses.Insert(0, "All");
            this.ViewBag.lastSendMessageID = new SelectList(statuses);
            this.ViewBag.FromDate = fromDate;
            this.ViewBag.ToDate = toDate;
            this.ViewBag.MaxCount = maxCount;
            return this.View(smses);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SmsStatus(DateTime fromDate, DateTime toDate, int maxCount, string lastSendMessageID)
        {
            if (lastSendMessageID.Trim().Length == 0) lastSendMessageID = null;
            var smses =
                this.db.SystemSMSes.Where(
                    m =>
                    (lastSendMessageID == "All" || m.LastSendMessage == lastSendMessageID)
                    && (m.iDate >= fromDate && m.iDate <= toDate))
                    .OrderByDescending(m => m.iDate)
                    .Take(maxCount)
                    .ToList();
            var statuses = (from item in this.db.SystemSMSes select item.LastSendMessage).Distinct().ToList();
            statuses.Insert(0, "All");
            this.ViewBag.lastSendMessageID = new SelectList(statuses, lastSendMessageID);
            this.ViewBag.FromDate = fromDate;
            this.ViewBag.ToDate = toDate;
            this.ViewBag.MaxCount = maxCount;

            return this.View(smses);
        }

        [Authorize]
        [HttpGet]
        [LayoutInjecter("_LayoutNoLogo")]
        public ActionResult StarterPackPurchaseReport()
        {
            var model =
                this.db.Clients.Include(m => m.Contact)
                    .ToArray()
                    .Where(m => !m.GetObligationProductMet(this.db))
                    .OrderBy(m => m.ClientId)
                    .ToArray();
            return this.View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }

            base.Dispose(disposing);
        }

        private ReportAccountBalanceViewModel GetAccountBalances()
        {
            const int xOffset = 3;
            const int yOffset = 1;
            var sql =
                "select ca.ClientID, ca.ClientAccountId, a.AccountName, sum(j.amount) Balance from finClientAccount ca             left join finJournal j  on ca.ClientAccountId = j.AccountID left join finAccount a on a.AccountId = ca.AccountId group by ca.ClientID, ca.ClientAccountId, a.AccountName having sum(j.Amount) <> 0 order by ClientID, AccountName";
            var model = new ReportAccountBalanceViewModel();
            var detail = this.db.Database.SqlQuery<ReportAccountBalanceViewModelDetail>(sql).ToArray();
            model.Detail.AddRange(detail);
            var clients = (from item in detail orderby item.ClientId select item.ClientId).Distinct().ToArray();
            var accounts = (from item in detail orderby item.AccountName select item.AccountName).Distinct().ToArray();
            var matrix = new string[clients.Length + yOffset, accounts.Length + xOffset];
            var links = new Guid?[clients.Length + yOffset, accounts.Length + xOffset];

            matrix[0, 0] = "No";
            matrix[0, 1] = "Client Name";
            matrix[0, 2] = "Client No";

            for (int x = xOffset; x < accounts.Length + xOffset; x++)
            {
                matrix[0, x] = accounts[x - xOffset];
            }

            for (int y = yOffset; y < clients.Length + yOffset; y++)
            {
                var client = this.db.Clients.Find(clients[y - yOffset]);
                matrix[y, 0] = (y - yOffset).ToString();
                matrix[y, 1] = string.Format("{0} {1}", client.FullNames, client.ClientSurname);
                matrix[y, 2] = client.ClientId.ToString();
                for (int x = xOffset; x < accounts.Length + xOffset; x++)
                {
                    var accountName = accounts[x - xOffset];
                    var value =
                        (from item in detail
                         where item.ClientId == client.ClientId && item.AccountName == accountName
                         select item.Balance).FirstOrDefault();
                    matrix[y, x] = string.Format("{0:#,###,##0.00}", value);
                    if (value != 0)
                    {
                        var clientAccountId = (from item in detail
                                               where item.ClientId == client.ClientId && item.AccountName == accountName
                                               select item.ClientAccountId).First();
                        links[y, x] = clientAccountId;
                    }
                }
            }

            model.Matrix = matrix;
            model.Links = links;
            return model;
        }

        private ReportLinkedUserSales GetLinkedUserModel(DateTime fromDate, DateTime toDate)
        {
            var userClients = this.db.UserClients.ToArray();
            var validUserIds = userClients.Select(m => m.UserId).ToArray();
            var orders = (from item in this.db.OrderHeaders
                          where
                              item.OrderDate >= fromDate && item.OrderDate <= toDate
                              && validUserIds.Contains(item.UserId)
                          select item).ToArray();
            var model = new ReportLinkedUserSales()
                            {
                                FromDate = fromDate,
                                ToDate = toDate,
                                ClientSales = (from item in orders
                                               group item by item.UserId
                                               into sales
                                               select
                                                   new ReportLinkedUserSalesViewModel()
                                                       {
                                                           UserId
                                                               =
                                                               sales
                                                               .Key,
                                                           UserClientId
                                                               =
                                                               userClients
                                                               .First
                                                               (
                                                                   m
                                                                   =>
                                                                   m
                                                                       .UserId
                                                                       .Equals
                                                                       (
                                                                           sales
                                                                       .Key))
                                                               .ClientId,
                                                           Total
                                                               =
                                                               sales
                                                               .Sum
                                                               (
                                                                   m
                                                                   =>
                                                                   m
                                                                       .Total)
                                                       }).ToArray
                                    ()
                            };
            return model;
        }

        private ReportViewModel GetModel(DateTime fromDate, DateTime toDate, int orderStatus)
        {
            const string currencyFormat = "R#,###,###,##0.00";

            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var orders = (from item in this.db.OrderHeaders
                          where
                              item.OrderStatusId == orderStatus && item.OrderDate >= fromDate
                              && item.OrderDate <= toDate
                          select item).ToArray();

            var ret = new ReportViewModel() { FromDate = fromDate, ToDate = toDate };

            var clientIds = (from item in orders select item.ClientID).Distinct().ToArray();

            ReportViewModel.Clients = (from item in this.db.Clients.Where(m => clientIds.Contains(m.ClientId)).ToArray()
                                       select
                                           new ReportViewClientModel()
                                               {
                                                   ClientID = item.ClientId,
                                                   Description =
                                                       string.Format(
                                                           "{0} {1}",
                                                           item.ClientInitials,
                                                           item.ClientSurname)
                                               }).Distinct()
                .OrderBy(m => m.ClientID)
                .ToList();

            ReportViewModel.Sales = (from item in orders
                                     select
                                         new ReportViewSaleModel()
                                             {
                                                 ClientID = item.ClientID,
                                                 Description =
                                                     string.Format(
                                                         "{0} {1}",
                                                         item.Client.ClientInitials,
                                                         item.Client.ClientSurname),
                                                 Source = item.SalesType.SalesTypeDescription,
                                                 Invoice =
                                                     this.db.Invoices.Where(
                                                         m => m.InvoiceId.Equals(item.OrderHeaderId))
                                                     .FirstOrDefault()?.Number,
                                                 ChildId =
                                                     this.db.SystemLinks.Where(
                                                         m => m.Parent.Equals(item.OrderHeaderId))
                                                     .FirstOrDefault()?.Child,
                                                 PaymentType = "Unknown",
                                                 OrderHeaderId = item.OrderHeaderId,
                                                 Total = item.Total.ToString(currencyFormat),
                                                 TotalValue = item.Total,
                                                 TotalExcl = item.VAT.ToString(currencyFormat),
                                                 Shipping = item.Shipping.ToString(currencyFormat),
                                                 ShippingValue = item.Shipping,
                                                 TotalIncShipping =
                                                     (item.Shipping + item.Total).ToString(
                                                         currencyFormat)
                                             }).Distinct()
                .OrderBy(m => m.ClientID)
                .ToList();

            foreach (var sale in ReportViewModel.Sales.Where(m => m.ChildId != null))
            {
                try
                {
                    sale.PaymentType = this.db.SystemNotes.Find(sale.ChildId).NoteText;
                }
                catch
                {
                }
            }

            var summary = (from item in ReportViewModel.Sales
                           group item.TotalValue by item.PaymentType
                           into g
                           select new { PaymentType = g.Key, Total = g.Sum() }).ToList();

            ReportViewModel.PaymentSummary =
                summary.Select(m => new KeyValuePair<string, string>(m.PaymentType, m.Total.ToString(currencyFormat)))
                    .ToList();

            ReportViewModel.Lines = new List<ReportViewLineItemModel>();
            foreach (var order in orders)
            {
                ReportViewModel.Lines.AddRange(
                    from item in order.OrderLines
                     orderby item.Product.ProductCode
                     select
                         new ReportViewLineItemModel()
                             {
                                 ClientID = order.ClientID,
                                 ProductCode = item.Product.ProductCode,
                                 ProductDescription = item.Product.ProductName,
                                 Quantity = item.Quantity.ToString(),
                                 QuantityValue = item.Quantity,
                                 UnitPrice = item.UnitCost.ToString(currencyFormat),
                                 Total =
                                     ((decimal)item.Quantity * item.UnitCost).ToString(
                                         currencyFormat),
                                 TotalValue = (decimal)item.Quantity * item.UnitCost,
                                 OrderHeaderId = item.OrderHeaderId
                             });
            }

            ReportViewModel.GroupedLines = new List<ReportViewGroupedLineItemModel>();
            foreach (var line in ReportViewModel.Lines)
            {
                var existing =
                    ReportViewModel.GroupedLines.Where(
                        m => m.ProductCode == line.ProductCode && m.UnitPrice == line.UnitPrice).FirstOrDefault();
                if (existing == null)
                {
                    existing = new ReportViewGroupedLineItemModel()
                                   {
                                       ProductCode = line.ProductCode,
                                       ProductDescription = line.ProductDescription,
                                       UnitPrice = line.UnitPrice
                                   };
                    ReportViewModel.GroupedLines.Add(existing);
                }

                existing.TotalValue += line.TotalValue;
                existing.QuantityValue += line.QuantityValue;
                existing.Total = existing.TotalValue.ToString(currencyFormat);
                existing.Quantity = existing.QuantityValue.ToString();
            }

            // ret.SalesTypeId = salesTypeId;
            ret.Total = ReportViewModel.Lines.Select(m => m.TotalValue).Sum().ToString(currencyFormat);
            ret.Shipping = ReportViewModel.Sales.Select(m => m.ShippingValue).Sum().ToString(currencyFormat);
            ret.TotalIncShipping =
                (ReportViewModel.Lines.Select(m => m.TotalValue).Sum()
                 + ReportViewModel.Sales.Select(m => m.ShippingValue).Sum()).ToString(currencyFormat);
            return ret;
        }

        private ReportViewModel GetModel(DateTime fromDate, DateTime toDate, int[] salesTypeId)
        {
            const string currencyFormat = "R#,###,###,##0.00";
            var validOrderStatuses = new[] { 2, 3, 4 };
            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var orders = (from item in this.db.OrderHeaders
                          where
                              salesTypeId.Contains(item.SalesTypeId) && validOrderStatuses.Contains(item.OrderStatusId)
                              && item.OrderDate >= fromDate && item.OrderDate <= toDate
                          select item).ToArray();

            var ret = new ReportViewModel() { FromDate = fromDate, ToDate = toDate };

            var clientIds = (from item in orders select item.ClientID).Distinct().ToArray();

            ReportViewModel.Clients = (from item in this.db.Clients.Where(m => clientIds.Contains(m.ClientId)).ToArray()
                                       select
                                           new ReportViewClientModel()
                                               {
                                                   ClientID = item.ClientId,
                                                   Description =
                                                       string.Format(
                                                           "{0} {1}",
                                                           item.ClientInitials,
                                                           item.ClientSurname)
                                               }).Distinct()
                .OrderBy(m => m.ClientID)
                .ToList();

            ReportViewModel.Sales = (from item in orders
                                     select
                                         new ReportViewSaleModel()
                                             {
                                                 ClientID = item.ClientID,
                                                 Description =
                                                     string.Format(
                                                         "{0} {1}",
                                                         item.Client.ClientInitials,
                                                         item.Client.ClientSurname),
                                                 Source = item.SalesType.SalesTypeDescription,
                                                 Invoice =
                                                     this.db.Invoices.Where(
                                                         m => m.InvoiceId.Equals(item.OrderHeaderId))
                                                     .FirstOrDefault()?.Number,
                                                 ChildId =
                                                     this.db.SystemLinks.Where(
                                                         m => m.Parent.Equals(item.OrderHeaderId))
                                                     .FirstOrDefault()?.Child,
                                                 PaymentType = "Unknown",
                                                 OrderHeaderId = item.OrderHeaderId,
                                                 Total = item.Total.ToString(currencyFormat),
                                                 TotalValue = item.Total,
                                                 TotalExcl = item.VAT.ToString(currencyFormat),
                                                 Shipping = item.Shipping.ToString(currencyFormat),
                                                 ShippingValue = item.Shipping,
                                                 TotalIncShipping =
                                                     (item.Shipping + item.Total).ToString(
                                                         currencyFormat)
                                             }).Distinct()
                .OrderBy(m => m.ClientID)
                .ToList();

            foreach (var sale in ReportViewModel.Sales.Where(m => m.ChildId != null))
            {
                try
                {
                    sale.PaymentType = this.db.SystemNotes.Find(sale.ChildId).NoteText;
                }
                catch
                {
                }
            }

            var summary = (from item in ReportViewModel.Sales
                           group item.TotalValue by item.PaymentType
                           into g
                           select new { PaymentType = g.Key, Total = g.Sum() }).ToList();

            ReportViewModel.PaymentSummary =
                summary.Select(m => new KeyValuePair<string, string>(m.PaymentType, m.Total.ToString(currencyFormat)))
                    .ToList();

            ReportViewModel.Lines = new List<ReportViewLineItemModel>();
            foreach (var order in orders)
            {
                ReportViewModel.Lines.AddRange(
                    from item in order.OrderLines
                     orderby item.Product.ProductCode
                     select
                         new ReportViewLineItemModel()
                             {
                                 ClientID = order.ClientID,
                                 ProductCode = item.Product.ProductCode,
                                 ProductDescription = item.Product.ProductName,
                                 Quantity = item.Quantity.ToString(),
                                 QuantityValue = item.Quantity,
                                 UnitPrice = item.UnitCost.ToString(currencyFormat),
                                 Total =
                                     ((decimal)item.Quantity * item.UnitCost).ToString(
                                         currencyFormat),
                                 TotalValue = (decimal)item.Quantity * item.UnitCost,
                                 OrderHeaderId = item.OrderHeaderId
                             });
            }

            ReportViewModel.GroupedLines = new List<ReportViewGroupedLineItemModel>();
            foreach (var line in ReportViewModel.Lines)
            {
                var existing =
                    ReportViewModel.GroupedLines.Where(
                        m => m.ProductCode == line.ProductCode && m.UnitPrice == line.UnitPrice).FirstOrDefault();
                if (existing == null)
                {
                    existing = new ReportViewGroupedLineItemModel()
                                   {
                                       ProductCode = line.ProductCode,
                                       ProductDescription = line.ProductDescription,
                                       UnitPrice = line.UnitPrice
                                   };
                    ReportViewModel.GroupedLines.Add(existing);
                }

                existing.TotalValue += line.TotalValue;
                existing.QuantityValue += line.QuantityValue;
                existing.Total = existing.TotalValue.ToString(currencyFormat);
                existing.Quantity = existing.QuantityValue.ToString();
            }

            ret.SalesTypeId = salesTypeId;
            ret.Total = ReportViewModel.Lines.Select(m => m.TotalValue).Sum().ToString(currencyFormat);
            ret.Shipping = ReportViewModel.Sales.Select(m => m.ShippingValue).Sum().ToString(currencyFormat);
            ret.TotalIncShipping =
                (ReportViewModel.Lines.Select(m => m.TotalValue).Sum()
                 + ReportViewModel.Sales.Select(m => m.ShippingValue).Sum()).ToString(currencyFormat);
            return ret;
        }
    }
}