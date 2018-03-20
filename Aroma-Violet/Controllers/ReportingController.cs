namespace Aroma_Violet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Configuration;
    using System.Data.Entity.SqlServer;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI;

    using Aroma_Violet.Models;

    using Microsoft.AspNet.Identity;

    using Postal;
    using GenericData;

    public class ReportingController : Controller
    {
        private int[] _cashSalesSaleTypes = new[] { 7, 8, 9, 11, 12 };

        private int[] _distrbutedSaleTypes = new[] { 13 };

        private AromaContext db = new AromaContext();

        public ActionResult Test(int x = 0)
        {
            this.Mail(10076, "Test");
            return this.RedirectToAction("Index");
        }

        public void Mail(int clientId, string view)
        {
            var emailAddress = (from item in this.db.Contacts
                         where item.ClientID == clientId && item.ContactTypeID == 6 && item.Active
                         select item.ContactName).FirstOrDefault();
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new Exception("No email address");
            }

            dynamic email = new Email(view);
            email.To = emailAddress;
            email.Send();
        }

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
        [HttpGet]
        public ActionResult DebitOrderExcelExport()
        {
            var fileCont = string.Empty;
            var header = "USER TRANSACTIONS,,,SC003_V1_10,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\r\nUSER'S CODE sending this spreadsheet,,,8321,,,,USER'S Name,, Novus Fragrances,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\r\n\"Green fields are compulsory. If you leave compulsory fields blank, your transaction will be discarded.\",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\r\nSTRATCOL,User's own,Surname,Initials,Mobile,Name of,Bank,Branch ,Branch ,ID or,Account,Account ,Credit,Credit,StratCol,Start,Amount,FUTURE ,Transaction,Day of,Continue,Number ,Final,Abbreviated,Escalation,Escalation,Publication,SMS,Batch,Future,User's own, User's own,Beneficiary 1,Ben. 1,Beneficiary 2,Ben. 2,Beneficiary 3,Ben. 3,Beneficiary 4,Ben. 4,Beneficiary 5,Ben. 5\r\nReference,Reference,,,number,account,,name,code,Passport or, number, type, card, card, User ID,date,,use,type,month / week,until cancel, of deductions,date,name,%,month,reference,reference,reference,use,sub,sub,benef.ID,R Value, benef. ID,R Value, benef. ID,R Value, benef. ID,R Value, benef. ID,R Value\r\n,,,,,,,,, Company reg.no.,,,ccv number, expiry date,,,,,,,,,,,,,,,,,reference 1,reference 2,,,,,,,,,,\r\nleave blank,, or business,, if SMS is ,,,,ABSA = 632005,ID compulsory, no spaces,1 - Current / Cheque,,,,,,,o - once off,1 - 31,,Either the, only if not,,,,message to be ,max 140 character,max 40 characters,,,,,,,,,,,,,\r\nfor new,, trading name,, required,,,, STD = 051001,for ahv -, 11 digits, 2 - Savings,,,,,,, m - monthly, or,, number or, marked,,,, printed on your client's,message to be,reference,,,,,,,,,,,,,\r\ninstructions,,,,,,,, FNB = 250655, account holder, 16 digits(credit card), 3 - Transmission,,,,,,, q - quarterly, L = last day,, \"\"\"continue\"\"\", \"\"\"continue\"\"\",,,, bank statement., sent by sms prior, to identify,,,,,,,,,,,,,\r\n,,,,,,,, CAPITEC = 470010, verification,, 4 - Credit Card,,,,,,, 6m - biannually, or,,if forever,,,,,\"If left blank, by default\",to the collection.,batch related,,,,,,,,,,,,,\r\n,,,,,,,, INVESTEC = 580105,,,,,,,, no spaces,,a - annually,,,until cancelled,,,,, we print our OTID,\"If left blank, by default\",transactions,,,,,,,,,,,,,\r\n,,,,,,,,POST = 460005,\"In case of ID,\",\"if less, 0 added\",,no spaces, no spaces,,,,,w - weekly,MON / TUE / WED,,,,,,,and your business name, we send a standard,,,,,,,,,,,,,,\r\n,,,,10 digits,,,,NEDBANK = 198765 ,\"add \"\":ID\"\" after number\",left,,3 digits,4 digits,,,no currency,,2w - biweekly,THU / FRI / SAT,yes  ,,,,,,or your registered,message.,,,,,,,,,,,,,,\r\n,,,,,,,,,to preserve formatting,\"if less, 0 added\",,,mmyy,,,indicator,,nad - naedo,,no,continue,,,,mm,abbreviated name and ,\"If no cell number provided,\",,,,,,,,,,,,,,\r\n,,,,,,,,6 digits,,left,,,,,,,,ahv - acc.holder verification,,,type = nad - naedo,,,,e.g. 02,our OTID, no sms will be sent.,,,,,,,,,,,,,,\r\n,,,,,,,,, For Company reg. no.,,,,,,,,,pmt - payment,,,then indicate,, max 10,,,max 10 characters,,,,,,,,,,,,,,,\r\n,,,,if less,,,,if less ,the format is ,In case of credit card,,\"if less,\",,,dd.mm.yy,0,,,,,tracking days, dd.mm.yy,characters,,if less,\"for EFT,\",,,,,,,,,,,,,,,\r\n,,,,\"\"\"0\"\" padded\",,,,\"\"\"0\"\" padded\",yyyy / CIPC no.(6 digits) /,\"add \"\":CC\"\" after number\",,0 added,,,,,,,,,0D no tracking,, e.g. ,,0 added,max 6 characters,,,,,,,,,,,,,,,\r\n,,,,left,,,,left,CIPC type (2 digits),to preserve formatting,,left,,,,,,,,,1D 2D 3D 4D 5D 6D 7D 8D,,STRATCOL,,left,for NAEDO,,,,,,,,,,,,,,,\r\n";
            var sql = "select  '' A,client.ClientId,Client.ClientSurname,	client.ClientInitials,	'','',	Bank.BankName,	Branch.BranchName,	Branch.BranchCode,	isnull(Client.IDNumber + ':ID',''), BankingDetail.AccountNumber,	AccountType.AccountTypeName,	'' M,	'' N,	'' O,	'' P,	sum(finJournal.Amount) from finClientAccount left join finJournal on finJournal.AccountID = finClientAccount.ClientAccountId left join Client on client.ClientId = finClientAccount.ClientID left join Contact on Client.ClientId = Contact.ClientID left join BankingDetail on BankingDetail.ClientID = client.ClientId left join Bank on BankingDetail.BankID = Bank.BankId left join Branch on BankingDetail.BranchID = Branch.BranchId left join AccountType on AccountType.AccountTypeId = BankingDetail.AccountTypeID where  finClientAccount.AccountId = '751FE521-3561-E511-80C3-2047477CE07A' and (ContactTypeID in (1,2,3) and Contact.Active = 1 ) group by Client.ClientSurname, 	client.ClientInitials, 	contact.ContactName, 	Bank.BankName, 	Branch.BranchName, Branch.BranchCode, Client.IDNumber, BankingDetail.AccountNumber, AccountType.AccountTypeName, Client.ClientId having sum(finJournal.Amount) <> 0 ";
            var model = GetGenericModel(sql);
            /*var row1 = new string[] {"USER TRANSACTIONS","","","SC003_V1_10","", "","", "", "", "", "", "", "", "", "", "", "", "" };
            var row2 = new string[] { "USER'S CODE sending this spreadsheet", "","","8321","", "", "","", "USER'S Name", "", "Novus Fragrances", "", "", "", "", "", "" };
            model.Rows.Insert(0, row1);
            model.Rows.Insert(1, row2);*/
            fileCont = header;
            foreach(var r in model.Rows)
            {
                for(int i = 0; i < r.Length; i++)
                {
                    r[i] = string.Format("=\"{0}\"", r[i].Replace(",","."));
                }
                fileCont += string.Join(",", r) + "\r\n";
            }

            Response.Clear();
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=Novus{0:yyyyMMdd}.csv",DateTime.Now));
            Response.ContentType = "text/csv";

            // Write all my data
            Response.Write(fileCont);
            Response.End();

            return this.View(model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult MemberAveragePurchases()
        {
            
            var sSql = "with Sales as (select oh.ClientID, sum(oh.Total) as Total from OrderHeader oh where oh.OrderStatusId in  (2,3,4) group by oh.ClientID) select c.ClientId as [Client ID], ct.ClientTypeName as [Client type], c.iDate as [Sign on date], GETDATE() as Today, datediff(d,c.iDate,getdate()) as [Period signed on days], datediff (MM,c.iDate,getdate()) as [Period signed on months], sales.Total as [Total purchases], \tsales.Total / datediff(d,c.iDate,getdate()) as [Average monthly purchases from sign on date] from Client c left join ClientType ct on ct.ClientTypeId = c.ClientTypeID left join Sales on sales.ClientID = c.ClientId order by \tsales.Total / datediff(d,c.iDate,getdate()) desc";
            var model = this.GetGenericModel(sSql, null, null, null, null, null, null, "{0:#,###,##0.00}", "{0:#,###,##0.00}");
            
            /*
            var validOrderStatuses = new int[] { 2, 3, 4 };
            var validOrders = (from item in this.db.OrderHeaders
                               where validOrderStatuses.Contains(item.OrderStatusId)
                               group item by item.ClientID
                               into sales
                               orderby sales.Sum(m => m.Total) descending 
                               select sales).ToArray();

            var model = new GenericGridReport();

            model.AddColumn("Client ID");
            model.AddColumn("Client Type");
            model.AddColumn("Sign on date");
            model.AddColumn("Today");
            model.AddColumn("Period signed on days");
            model.AddColumn("Period signed on months");
            model.AddColumn("Total purchases");
            model.AddColumn("Average monthly purchases from sign on date");

            foreach (var row in validOrders)
            {
                var client = this.db.Clients.Find(row.Key);
                var total = row.Sum(m => m.Total);

                const string dateFormat = "yyyy/MM/dd";
                const string moneyFormat = "#,###,###,##0.00";
                const string numberFormat = "#,###,##0.000";

                var span = DateTime.Now.Subtract(client.iDate);
                var months = span.TotalHours / 730;
                model.AddRow(
                    row.Key,
                    client.ClientType.ClientTypeName,
                    client.iDate.ToString(dateFormat),
                    DateTime.Now.ToString(dateFormat),
                    span.TotalDays.ToString(numberFormat),
                    months.ToString(numberFormat),
                    total.ToString(moneyFormat),
                    (total / (decimal)months).ToString(moneyFormat));
            }
            */
            return this.View(model);
        }

        private GenericGridReport GetGenericModel(string sSql, params string[] format)
        {
            using (var context = new AromaContext())
            {
                return GenericData.SqlToData(context, sSql, format);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult CashSales(DateTime fromDate, DateTime toDate)
        {
            var model = this.GetModel(fromDate, toDate, this._cashSalesSaleTypes);
            return this.View(model);
        }

        [Authorize]
        [LayoutInjecter("_LayoutNoLogo")]
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
                statement.Description = RebateClientTypesController.GetPeriodStart(periodIds[0]).ToString("MMM yyyy");
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
        public ActionResult StockMovement()
        {
            var startDate = DateTime.Today.AddMonths(-1);
            var toDate = DateTime.Today;
            var model = this.GetStockModel(startDate, toDate);
            ViewBag.StartDate = startDate;
            ViewBag.ToDate = toDate;
            return this.View(model);
        }

        [HttpPost]
        public ActionResult StockMovement(DateTime startDate, DateTime toDate)
        {
            var model = this.GetStockModel(startDate, toDate);
            ViewBag.StartDate = startDate;
            ViewBag.ToDate = toDate;
            return this.View(model);
        }

        //public ActionResult MemberSalesAnalysisReport()
        //{
        //    var model = (from item in this.db.Clients
        //                 orderby item.ClientId
        //                 select new MemberSalesAnalysisReportModel()
        //                                                    {
        //        No = item.ClientId,
        //        NickName = item.NickName,
        //        Surname = item.ClientSurname,
        //        ClientType = item.ClientType.ClientTypeName,
        //        SignOnDate= item.iDate,
        //        Today = DateTime.Today,
        //        DaysSinceSignOn = DateTime.Today.Subtract(item.iDate).TotalDays,
        //                  MonthsSinceSignOn           = Math.Floor( DateTime.Today.Subtract(item.iDate).TotalDays / 30),
        //                  Introducer

        //                 });
        //}

        private StockMovementResult[] GetStockModel(DateTime startDate, DateTime toDate)
        {
            var newToDate = toDate.AddDays(1);
            var orderLines = from item in this.db.OrderLines.Include(m => m.OrderHeader).Include(m => m.Product)
                             where item.OrderHeader.OrderDate >= startDate && item.OrderHeader.OrderDate <= newToDate
                             select item;
    
            var result = (from item in orderLines
                           where
                           item.Product != null && item.OrderHeader.SalesTypeId != 13 && (
                               Generic.ValidOrderStatuses.Contains(item.OrderHeader.OrderStatusId)
                               || item.OrderHeader.OrderStatusId == Generic.CreditNoted)
                           orderby item.Product.ProductName
                           group item by item.Product
                           into products
                           select
                               new StockMovementResult()
                                   {
                                       StartDate = startDate,
                                       ToDate = toDate,
                                       ProductCode = products.Key.ProductCode,
                                       ProductDescription = products.Key.ProductName,
                                       Sales = products.Sum(m => m.Quantity),
                                       CreditNotes = products.Any(m => m.OrderHeader.OrderStatusId == Generic.CreditNoted) ? products.Where(m => m.OrderHeader.OrderStatusId == Generic.CreditNoted).Sum(m => m.Quantity) : 0,
                                       NetQuantityMovement = products.Any(m => Generic.ValidOrderStatuses.Contains(m.OrderHeader.OrderStatusId)) ?
                                           products.Where(
                                               m => Generic.ValidOrderStatuses.Contains(m.OrderHeader.OrderStatusId))
                                           .Sum(m => m.Quantity) : 0,
                                       TotalNet = products.Sum(
                                                m => 
                                                    m.UnitCost * m.Quantity)
                                   }).ToArray();
            return result;
        }

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
        [LayoutInjecter("_LayoutNoLogo")]
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
            var newToDate = toDate.AddDays(1);
            if (lastSendMessageID.Trim().Length == 0) lastSendMessageID = null;
            var smses =
                this.db.SystemSMSes.Where(
                    m =>
                    (lastSendMessageID == "All" || m.LastSendMessage == lastSendMessageID)
                    && (m.iDate >= fromDate && m.iDate <= newToDate))
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
            var newToDate = toDate.AddDays(1);
            var userClients = this.db.UserClients.ToArray();
            var validUserIds = userClients.Select(m => m.UserId).ToArray();
            var orders = (from item in this.db.OrderHeaders
                          where
                              item.OrderDate >= fromDate && item.OrderDate <= newToDate
                              && validUserIds.Contains(item.UserId)
                          select item).ToArray();
            var model = new ReportLinkedUserSales()
                            {
                                FromDate = fromDate,
                                ToDate = newToDate,
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
                                                               .First(
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
                                                               .Sum(
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

            var orders = (from item in this.db.OrderHeaders/*.Where(m => m.OrderHeaderId.ToString() == "A1EBFFA3-68DF-E511-8118-2047477CE07A")*/
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
            
            var qry = string.Empty;
            foreach (var line in ReportViewModel.Lines.Select(m => m.OrderHeaderId).Distinct())
            {
                qry += $"'{line}',";
            }
            
            return ret;
        }
    }

    public class MemberSalesAnalysisReportModel
    {
    }

    public  class StockMovementResult
    {
        public DateTime StartDate { get; set; }

        public DateTime ToDate { get; set; }

        public string ProductCode { get; set; }

        public string ProductDescription { get; set; }

        public int Sales { get; set; }

        public int CreditNotes { get; set; }

        public int NetQuantityMovement { get; set; }

        public decimal TotalNet { get; set; }
    }
}