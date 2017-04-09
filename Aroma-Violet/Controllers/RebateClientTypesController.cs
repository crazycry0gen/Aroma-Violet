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

namespace Aroma_Violet.Controllers
{
    public class RebateClientTypesController : Controller
    {
        private AromaContext db = new AromaContext();

        // GET: RebateClientTypes
        public async Task<ActionResult> Index()
        {
            var rebateClientTypes = db.RebateClientTypes.Include(r => r.RebateLevelsTable);
            return View(await rebateClientTypes.ToListAsync());
        }

        // GET: RebateClientTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RebateClientType rebateClientType = await db.RebateClientTypes.FindAsync(id);
            if (rebateClientType == null)
            {
                return HttpNotFound();
            }
            return View(rebateClientType);
        }
        // GET: RebateClientTypes/Create
        public ActionResult Copy(int sourceRebateClientTypeID)
        {
           var source= (from item in db.RebateClientTypes where item.RebateClientTypeId == sourceRebateClientTypeID select item).First();
            ViewBag.ClientTypeName = source.ClientType.ClientTypeName;
            var clienttypes = db.ClientTypes.Where(m => m.ClientTypeId != source.ClientTypeID).ToArray();
            ViewBag.ClientTypeID = new SelectList(clienttypes.Where(m=>m.Active).OrderBy(m=>m.ClientTypeName), "ClientTypeID", "ClientTypeName");
            ViewBag.SourceClientTypeID = source.ClientTypeID;
            return View(new RebateClientType());
        }

        // POST: RebateClientTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Copy([Bind(Include = "RebateClientTypeId,ClientTypeID,RebateLevelsTable")] RebateClientType rebateClientType, int sourceClientTypeID)
        {
            if (ModelState.IsValid)
            {
                var existCheck = db.RebateClientTypes.Where(m => m.ClientTypeID == rebateClientType.ClientTypeID).FirstOrDefault();
                if (existCheck == null)
                {
                    var accountId = (from item in db.Accounts
                                     where item.Active
                                     select item.AccountId).First();
                    var newRebateLevelTabel = new RebateLevelsTable() { FromAccountId = accountId, ToAccountId = accountId };
                    db.RebateLevelsTables.Add(newRebateLevelTabel);
                    //await db.SaveChangesAsync();

                    rebateClientType.RebateLevelsTable = newRebateLevelTabel;

                    db.RebateClientTypes.Add(rebateClientType);
                    await db.SaveChangesAsync();

                    await CopyRebateClientType(sourceClientTypeID, rebateClientType.RebateClientTypeId);

                    return RedirectToAction("Edit", new { id = rebateClientType.RebateClientTypeId });
                }
                else
                {
                    await CopyRebateClientType(sourceClientTypeID, existCheck.RebateClientTypeId);

                    return RedirectToAction("Edit", new { id = existCheck.RebateClientTypeId });
                }
            }

            var source = (from item in db.ClientTypes where item.ClientTypeId == sourceClientTypeID select item).First();
            ViewBag.ClientTypeName = source.ClientTypeName;
            var clienttypes = db.ClientTypes.Where(m => m.ClientTypeId != source.ClientTypeId).ToArray();
            ViewBag.ClientTypeID = new SelectList(clienttypes, "ClientTypeID", "ClientTypeName");
            ViewBag.SourceClientTypeID = sourceClientTypeID;
            return View(rebateClientType);
        }

        private async Task CopyRebateClientType(int sourceClientTypeID, int destinationRebateClientTypeId)
        {
            var source =      await db.RebateClientTypes.Include(m=>m.RebateLevelsTable).FirstAsync(m=>m.ClientTypeID==sourceClientTypeID);
            var destination = await db.RebateClientTypes.Include(m => m.RebateLevelsTable).FirstAsync(m=>m.RebateClientTypeId==destinationRebateClientTypeId);
            db.SaveChanges();

            Generic.CopyObject(source.RebateLevelsTable, destination.RebateLevelsTable, "RebateClientType", "RebateClientTypeId", "RebateLevelRows");
            db.SaveChanges();

            if (destination != null && destination.RebateLevelsTable != null && destination.RebateLevelsTable.RebateLevelRows != null && destination.RebateLevelsTable.RebateLevelRows.Count() > 0)
            {
                db.RebateLevelsTableRows.RemoveRange(destination.RebateLevelsTable.RebateLevelRows);
                db.SaveChanges();
            }

            source.RebateLevelsTable.RebateLevelRows = new List<RebateLevelsTableRow>();
            destination.RebateLevelsTable.RebateLevelRows = new List<RebateLevelsTableRow>();

            foreach (var row in source.RebateLevelsTable.RebateLevelRows)
            {
                var nrow = new RebateLevelsTableRow() {RebateLevelsTableId=destination.RebateLevelsTableId };
                Generic.CopyObject(row, nrow, "RebateLevelsTableRowId", "RebateLevelsTableId", "RebateLevelsTable");
                destination.RebateLevelsTable.RebateLevelRows.Add(nrow);
                db.SaveChanges();

            }

            if (destination.SalesTables != null && destination.SalesTables.Count() > 0)
            {
                var ids = destination.SalesTables.Select(m => m.RebateSalesTableId).ToArray();
                var rebateranges = db.RebateRanges.Where(m => ids.Contains(m.RebateSalesTableId));
                db.RebateRanges.RemoveRange(rebateranges);
                db.SaveChanges();
                db.SalesTables.RemoveRange(destination.SalesTables);
                db.SaveChanges();
            }
            if (destination.SalesTables == null)
                destination.SalesTables = new List<SalesTable>();

            var sourceTables = source.SalesTables.ToArray();
            foreach (var tbl in sourceTables)
            {
                var ntbl = new SalesTable() {RebateClientTypeId = source.RebateClientTypeId, RebateRanges=new List<RebateRange>() };
                Generic.CopyObject(tbl, ntbl,"RebateSalesTableId", "RebateRanges", "RebateClientType", "RebateClientTypeId");
                destination.SalesTables.Add(ntbl);
                db.SaveChanges();

                var rows = tbl.RebateRanges.ToArray();
                foreach (var row in rows)
                {
                    var nrow = new RebateRange() {};
                    Generic.CopyObject(row, nrow,"RebateRangeId", "RebateSalesTable", "RebateSalesTableId");
                    ntbl.RebateRanges.Add(nrow);
                    db.SaveChanges();

                }

                
                db.SaveChanges();

            }
            db.SaveChanges();
        }

        [HttpGet]
        public async Task<ActionResult> MonthEndIndex()
        {
            //Calculate month end?
            var previousPeriodDate = DateTime.Today.AddMonths(-1);
            var previousPeriod = GetPeriodId(previousPeriodDate);
            if (db.MonthEndRows.Where(m => m.PeriodId == previousPeriod).Count() == 0)
            {
                var res = CalculateMonthEnd();
            }
            var monthEnds = await (from item in db.MonthEndRows
                             select item.PeriodId
                             ).Distinct().OrderByDescending(m=>m).ToArrayAsync();
            return View(monthEnds);
        }

        [HttpGet]
        public async Task<ActionResult> ViewMonthEnd(int periodId)
        {
            var model = await (from item in db.MonthEndRows.Include(m=>m.Client)
                               where item.PeriodId == periodId //&& item.ClientId==10086
                               select item)
                         .OrderBy(m => m.PeriodId)
                         .ThenBy(m => m.ClientId)
                         .ThenBy(m=>m.DownlineIndex)
                         .ToArrayAsync();
            var dt = GetDate(periodId);
            ViewBag.Header = dt.ToString("MMM yyyy");
            ViewBag.ShowApprove = model.Where(m => m.ApprovedUserId.Equals(Guid.Empty)).Count() > 0;
            return View(model);
        }

        public async Task<ActionResult> DeleteMonthEnd(int periodId)
        {
            var model = await (from item in this.db.MonthEndRows.Include(m => m.Client)
                               where item.PeriodId == periodId && item.ApprovedUserId.Equals(Guid.Empty)
                               select item).ToArrayAsync();

            foreach (var row in model)
            {
                var details =
                    await
                    (from item in this.db.MonthEndDetail where item.MonthEndRowId == row.MonthEndRowId select item)
                        .ToArrayAsync();
                this.db.MonthEndDetail.RemoveRange(details);
            }

            this.db.MonthEndRows.RemoveRange(model);

            await this.db.SaveChangesAsync();

            return this.RedirectToAction("Index");
        }

        public async Task<ActionResult> ApproveMonthEnd(int periodId)
        {
            const string fmtMoney = "#,###,##0.00";
            var userId = Guid.Parse(User.Identity.GetUserId());
            var periodDate = GetDate(periodId);
            var model = await (from item in db.MonthEndRows.Include(m => m.Client)
                               where item.PeriodId == periodId
                               && item.ApprovedUserId.Equals(Guid.Empty)
                               select item)
                         .ToArrayAsync();

            foreach (var item in model)
            {
                
                var comment = string.Empty;
                var reason = string.Empty;
                if (item.AmountValue > 0 && item.AmountQualify)
                {
                    if (item.DownlineIndex == 0)
                    {
                        reason = "Volume discount";
                        SystemSMSController.SendSMSEvent(db, 7, item.ClientId, userId, item.Reference,
                            new KeyValuePair<string, string>("VolumeDiscount", item.AmountValue.ToString(fmtMoney)),
                            new KeyValuePair<string, string>("MEMonth", periodDate.ToString("MMMM")),
                            new KeyValuePair<string, string>("MEYear", periodDate.ToString("yyyy")),
                            new KeyValuePair<string, string>("AccountName", finAccountsController.Name(db,item.AmountToAccount))
                            );
                    }
                    else
                    {
                        reason = string.Format("Commision (Rebate) level {0} {1}%",item.DownlineIndex,item.AmountPer );
                        if (item.DownlineIndex == 1) //only do this once per client
                        {
                            var tot = (from entry in model
                                       where entry.ClientId == item.ClientId
                                       && entry.DownlineIndex > 0
                                       select entry.AmountValue).Sum();
                            SystemSMSController.SendSMSEvent(db, 4, item.ClientId, userId, item.Reference,
                                new KeyValuePair<string, string>("CommissionAmount", tot.ToString(fmtMoney)),
                                new KeyValuePair<string, string>("MEMonth", periodDate.ToString("MMMM")),
                                new KeyValuePair<string, string>("MEYear", periodDate.ToString("yyyy")),
                                new KeyValuePair<string, string>("AccountName", finAccountsController.Name(db, item.AmountToAccount))
                                );
                        }
                    }
                    comment = string.Format("{1} on purchases of R{0:#,###,###,#0.00}", item.Amount,reason);
                    db.Database.ExecuteSqlCommand(string.Format(Generic.sqlCreateJournalSingle,
                                item.ClientId,
                                item.AmountFromAccount,
                                item.AmountToAccount,
                                item.AmountValue.ToString().Replace(",", "."),
                                DateTime.Now.ToString(Generic.LongDate),
                                DateTime.Now.ToString(Generic.LongDate),
                                item.Reference,
                                comment,
                                userId
                                ));
                }
                var subscriptionTot = item.SubscriptionFirstProduct + item.SubscriptionOtherProduct;
                var subscriptionValue = item.SubscriptionFirstProductValue + item.SubscriptionOtherProductValue;
                if (subscriptionValue > 0)
                {
                    reason = "Subscription rebate";
                    comment = string.Format("{1} on purchases of R{0:#,###,###,#0.00}", subscriptionTot, reason);
                    db.Database.ExecuteSqlCommand(string.Format(Generic.sqlCreateJournalSingle,
                               item.ClientId,
                               item.SubscriptionFromAccount,
                               item.SubscriptionToAccount,
                               subscriptionValue.ToString().Replace(",", "."),
                               DateTime.Now.ToString(Generic.LongDate),
                               DateTime.Now.ToString(Generic.LongDate),
                               item.Reference,
                               comment,
                               userId
                               ));
                }
                item.ApprovedUserId = userId;
            }
            db.SaveChanges();

            return RedirectToAction("ViewMonthEnd", new { periodId = periodId });
        }

        private DateTime GetDate(int periodId)
        {
            var year = (int)Math.Floor((decimal)(periodId / 100));
            var month = periodId % 100;
            return new DateTime(year, month, 1);
        }

        public async Task<ActionResult> ViewCommisionDetail(int clientId, int periodId)
        {

            var monthendRows = await (from item in db.MonthEndRows
                         .Include(m=>m.Client)
                         .Include(m=>m.Detail)
                         where item.PeriodId == periodId
                         && item.ClientId == clientId
                         select item).ToArrayAsync();
            var monthEndDetail = new List<MonthEndDetail>();
            foreach (var monthend in monthendRows)
            {
                monthEndDetail.AddRange(monthend.Detail);
            }

            var clientIds = (from item in monthEndDetail
                             select item.OrderHeader.ClientID).Distinct().ToArray();

            var model = new List<string[]>();
            foreach (var downClientId in clientIds)
            {
                var client = db.Clients.Find(downClientId);
                var orders = (from item in monthEndDetail
                              where item.OrderHeader.ClientID == downClientId
                              select item.OrderHeader).ToArray();
                var level = (from item in monthEndDetail
                             where item.OrderHeader.ClientID == downClientId
                             select item.MonthEnd.DownlineIndex).FirstOrDefault();
                var row = new string[] {
                    downClientId.ToString(),
                    client.ClientType.ClientTypeName,
                    orders.Sum(m=>m.Shipping).ToString(),
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                };
                row[level + 3] = orders.Sum(m => m.Total).ToString();
                model.Add(row);
            }

            var dt = GetDate(periodId);
            ViewBag.PeriodDescription = dt.ToString("MMM yyyy");
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> CalculateMonthEnd()
        {
            var breakAtClient = 10086;
            var periodDate = DateTime.Today.AddMonths(-1);
            var validOrderStatuses = new int[] { 2, 3, 4 };
            var periodId = GetPeriodId(periodDate);
            var periodStart = GetPeriodStart(periodDate);
            var periodEnd = GetPeriodEnd(periodDate);
            
            // TODO: Fix the sales source. This is a work arround for now
            var subClients = from item in this.db.Clients where item.ClientTypeID == 3 select item.ClientId;
            var sales =
                (from item in this.db.OrderHeaders
                 where subClients.Contains(item.ClientID) && item.SaleSourceId == 0
                 select item).ToArray();
            foreach (var item in sales)
            {
                item.SaleSourceId = 1;
            }
            this.db.SaveChanges();
            ///////////////////

            var result = Json(string.Empty);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            var orders = (from item in db.OrderHeaders
                          where item.Active
                          && validOrderStatuses.Contains(item.OrderStatusId)
                          && item.OrderDate >= periodStart
                          && item.OrderDate <= periodEnd
                          select item).ToArray();
            var clients = (from item in db.Clients
                           //where item.Active
                           //&& item.ClientId == 10016
                           select item).ToArray();
            var currentDownlineLevel = 0;

            foreach (var client in clients)
            {
                if (breakAtClient>0 && client.ClientId == breakAtClient )
                {

                }
                var monthend = (from item in db.MonthEndRows
                                where item.PeriodId == periodId
                                && item.ClientId == client.ClientId
                                && item.DownlineIndex == currentDownlineLevel
                                select item).FirstOrDefault();
                if (monthend == null)
                {
                    monthend = new MonthEnd();
                    monthend.PeriodId = periodId;
                    monthend.ClientId = client.ClientId;
                    monthend.ClientTypeId = client.ClientTypeID;
                    monthend.DownlineIndex = currentDownlineLevel;
                    monthend.Reference = Guid.NewGuid();
                    monthend.ApprovedUserId = Guid.Empty;
                    monthend.AmountQualify = true;
                    db.MonthEndRows.Add(monthend);
                }
                if (monthend.ApprovedUserId.Equals(Guid.Empty))
                {
                    var clientPurchases = (from order in orders
                                           where order.ClientID == client.ClientId
                                           select order);
                    monthend.Amount = (from item in clientPurchases
                                       where item.SaleSourceId == 0
                                       select item.Total).Sum();
                    var firstProducts = (from item in clientPurchases
                                         where item.SaleSourceId == 1
                                         select item.OrderLines.FirstOrDefault());
                    monthend.SubscriptionFirstProduct = (from item in firstProducts
                                                         select item.UnitCost).Sum();
                    monthend.SubscriptionOtherProduct = (from item in firstProducts
                                                         select (item.OrderHeader.Total - item.UnitCost)).Sum();
                    var salesTables = (from item in db.SalesTables
                                      where item.LevelRange.StartLevel <= currentDownlineLevel
                                      && item.LevelRange.EndLevel >= currentDownlineLevel
                                      && item.RebateClientType.ClientTypeID == client.ClientTypeID
                                      select item).ToArray();
                    if (salesTables.Length > 1)
                    {
                        //must check source client type
                    }

                    var salesTable = salesTables.FirstOrDefault();
                    if (salesTable != null)
                    {
                        monthend.AmountPer = (from item in salesTable.RebateRanges
                                              where item.RangeStart <= monthend.Amount
                                              && item.RangeEnd >= monthend.Amount
                                              select item.Rebate).FirstOrDefault();

                        monthend.AmountQualify = salesTable.MinOwnPurchToQualify <= monthend.Amount;
                        monthend.AmountValue = (monthend.Amount * monthend.AmountPer) / 100;
                        monthend.AmountFromAccount = salesTable.FromAccountId;
                        monthend.AmountToAccount = salesTable.ToAccountId;

                    }

                    var rebateLevelTable = (from item in db.RebateLevelsTables
                                            where item.RebateClientType.ClientTypeID == client.ClientTypeID
                                            select item).FirstOrDefault();
                    if (rebateLevelTable != null)
                    {
                        var row = (from item in rebateLevelTable.RebateLevelRows
                                   where item.LevelRange.StartLevel <= currentDownlineLevel
                                   && item.LevelRange.EndLevel >= currentDownlineLevel
                                   select item).FirstOrDefault();
                        if (row != null)
                        {
                            monthend.SubscriptionFirstProductPer = row.FirstProductRebate;
                            monthend.SubscriptionOtherProductPer = row.AdditionalProductsRebate;
                            monthend.SubscriptionFirstProductValue = (monthend.SubscriptionFirstProduct * monthend.SubscriptionFirstProductPer);
                            monthend.SubscriptionOtherProductValue = (monthend.SubscriptionOtherProduct * monthend.SubscriptionOtherProductPer);
                        }
                        monthend.SubscriptionFromAccount = rebateLevelTable.FromAccountId;
                        monthend.SubscriptionToAccount = rebateLevelTable.ToAccountId;
                    }


                    db.SaveChanges();
                    var existingDetail = (from item in db.MonthEndDetail
                                          where item.MonthEndRowId == monthend.MonthEndRowId
                                          select item.OrderHeaderId).ToArray();
                    var detail = (from item in clientPurchases.Where(m => !existingDetail.Contains(m.OrderHeaderId)).ToArray()
                                  select new MonthEndDetail()
                                  {
                                      MonthEndRowId = monthend.MonthEndRowId,
                                      OrderHeaderId = item.OrderHeaderId
                                  }).ToArray();

                    db.MonthEndDetail.AddRange(detail);
                    db.SaveChanges();
                }
            }

            var someoneHasDescendants = false;
            do
            {
                someoneHasDescendants = false;
                currentDownlineLevel++;
                foreach (var client in clients/*.Where(m=>m.ClientId==10016)*/)
                {
                    if (breakAtClient > 0 && client.ClientId == breakAtClient)
                    {

                    }

                    if (breakAtClient > 0 && client.ResellerID.HasValue && client.ResellerID.Value == breakAtClient)
                    {
                    }

                    var descendants = GetDescendants(client.ClientId, currentDownlineLevel);
                    
                    //get the posible sales tables
                    var salesTables = (from item in db.SalesTables
                                       where item.RebateClientType.ClientTypeID == client.ClientTypeID
                                       select item).ToArray();
                    salesTables =
                        salesTables.Where(
                            item =>
                            item.LevelRange.StartLevel <= currentDownlineLevel
                            && item.LevelRange.EndLevel >= currentDownlineLevel).ToArray();

                    foreach (var salesTable in salesTables)
                    {
                        var descendentValues = (from item in db.MonthEndRows
                                                where
                                                    item.PeriodId == periodId && item.DownlineIndex == 0
                                                    && descendants.Contains(item.ClientId)
                                                select item).ToArray();
                        if (descendentValues.Length > 0)
                        {

                            var downlineClientTypes = (from item in this.db.RebateSalesTableClientTypes
                                                       where item.RebateSalesTableId == salesTable.RebateSalesTableId
                                                       select item.ClientTypeId).ToList();

                            if (downlineClientTypes.Count > 0)
                            {
                                var clientTypeChkSm =
                                    (int)(from item in downlineClientTypes select Math.Pow(2, item)).Sum();

                                var monthend = (from item in db.MonthEndRows
                                                where
                                                    item.PeriodId == periodId
                                                    && item.DownlineIndex == currentDownlineLevel
                                                    && item.ClientTypeId == clientTypeChkSm
                                                    && item.ClientId == client.ClientId
                                                select item).FirstOrDefault();

                                var ownMonthend = (from item in db.MonthEndRows
                                                   where
                                                       item.PeriodId == periodId && item.ClientId == client.ClientId
                                                       && item.DownlineIndex == 0
                                                   select item).FirstOrDefault();

                                if (monthend == null)
                                {
                                    var qualify = true;

                                    

                                    if (ownMonthend != null) qualify = ownMonthend.AmountQualify;

                                    monthend = new MonthEnd();
                                    monthend.PeriodId = periodId;
                                    monthend.ClientId = client.ClientId;
                                    monthend.DownlineIndex = currentDownlineLevel;
                                    monthend.ApprovedUserId = Guid.Empty;
                                    monthend.Reference = Guid.NewGuid();
                                    monthend.AmountQualify = qualify;
                                    monthend.ClientTypeId = clientTypeChkSm;
                                    db.MonthEndRows.Add(monthend);
                                }

                                if (monthend.ApprovedUserId.Equals(Guid.Empty))
                                {
                                    monthend.Amount =
                                        (from item in descendentValues
                                         where downlineClientTypes.Contains(item.ClientTypeId)
                                         select item.Amount).Sum();
                                    monthend.SubscriptionFirstProduct =
                                        (from item in descendentValues
                                         where downlineClientTypes.Contains(item.ClientTypeId)
                                         select item.SubscriptionFirstProduct).Sum();
                                    monthend.SubscriptionOtherProduct =
                                        (from item in descendentValues
                                         where downlineClientTypes.Contains(item.ClientTypeId)
                                         select item.SubscriptionOtherProduct).Sum();
                                    monthend.SubscriptionFromAccount = ownMonthend.SubscriptionFromAccount;
                                    monthend.SubscriptionToAccount = ownMonthend.SubscriptionToAccount;
                                    monthend.ClientTypeId = clientTypeChkSm;

                                    /*
                                var validClientTypes = (from item in db.RebateSalesTableClientTypes
                                    where item.RebateSalesTableId == salesTable.RebateSalesTableId
                                    select item.ClientTypeId).ToArray();
                                    */

                                    var clientId = client.ClientId;
                                    var personalAmount = (from item in db.MonthEndRows
                                                          where
                                                              item.PeriodId == periodId && item.ClientId == clientId
                                                              && item.DownlineIndex == 0
                                                          //&& validClientTypes.Contains( item.Client.ClientTypeID)
                                                          select item.Amount).FirstOrDefault();
                                    var rebateRange = (from item in salesTable.RebateRanges
                                                       where
                                                           item.RangeStart <= monthend.Amount
                                                           && item.RangeEnd >= monthend.Amount
                                                       select item).FirstOrDefault();
                                    if (rebateRange != null) monthend.AmountPer = rebateRange.Rebate;
                                    monthend.AmountQualify = salesTable.MinOwnPurchToQualify <= personalAmount;
                                    monthend.AmountValue = (monthend.Amount * monthend.AmountPer) / 100;
                                    monthend.AmountFromAccount = salesTable.FromAccountId;
                                    monthend.AmountToAccount = salesTable.ToAccountId;
                                }

                                db.SaveChanges();
                                someoneHasDescendants = true;

                                var orderheaderIds = new List<Guid>();
                                foreach (var dec in descendentValues.Select(m => m.Detail))
                                {
                                    if (dec != null) orderheaderIds.AddRange(dec.Select(m => m.OrderHeaderId));
                                }
                                var existingDetail =
                                    (from item in db.MonthEndDetail
                                     where item.MonthEndRowId == monthend.MonthEndRowId
                                     select item.OrderHeaderId).ToArray();


                                var detail = (from item in orderheaderIds
                                              where !existingDetail.Contains(item)
                                              select
                                                  new MonthEndDetail()
                                                      {
                                                          MonthEndRowId = monthend.MonthEndRowId,
                                                          OrderHeaderId = item
                                                      }).ToArray();
                                db.MonthEndDetail.AddRange(detail);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            while (someoneHasDescendants);

            return result;
        }

        private int[] GetDescendants(int clientId, int deep)
        {
            var result = new int[] {clientId };
            for (int lvl = 0; lvl < deep; lvl++)
            {
                result = (from item in db.Clients
                          where result.Contains(item.ResellerID.Value)
                          && item.ResellerID.HasValue
                          select item.ClientId).ToArray();
            }
            return result;
        }

        private static int GetPeriodId(DateTime date)
        {
            var val = string.Format("{0:0000}{1:00}", date.Year, date.Month);
            return int.Parse(val);
        }
        private static DateTime GetPeriodStart(DateTime date)
        {
            date = new DateTime(date.Year, date.Month, 1);
            return date;
        }
        private static DateTime GetPeriodEnd(DateTime date)
        {
            date = GetPeriodStart(date.AddMonths(1)).AddMilliseconds(-1);
            return date;
        }
        public static DateTime GetPeriodStart(int periodId)
        {
            var speriodId = periodId.ToString();
            int year = int.Parse(speriodId.Substring(0, 4));
            int month = int.Parse(speriodId.Substring(4, 2));
            
            var date = new DateTime(year, month, 1);
            return date;
        }
        public static DateTime GetPeriodEnd(int periodId)
        {
            var date = GetPeriodStart(periodId).AddMonths(1).AddMilliseconds(-1);
            return date;
        }

        [HttpGet]
        public async Task<JsonResult> CalculateGroupSalesRebates()
        {
            var result = Json(string.Empty);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            const string getOrderHeaderIds = "select h.OrderHeaderId from GroupSaleRebateCalculationRevenue r right outer join OrderHeader h on r.OrderHeaderId = h.OrderHeaderId Where r.OrderHeaderId is null and h.SaleSourceId = 0 and OrderStatusId in (2,3,4) and h.Active = 1 and h.OrderDate < '1 Mar 2016' and h.OrderDate >= '1 Feb 2016'";
            //const string getOrderHeaderIds = "select orderheaderid from orderheader where orderheaderid in ('FBA76289-E9DE-E511-8118-2047477CE07A','593F3CF3-DFDE-E511-8117-2047477CE07A','28BF7027-E0DE-E511-8117-2047477CE07A','84A588E6-E0DE-E511-8117-2047477CE07A')";
            var orderHeaderIds = await db.Database.SqlQuery<Guid>(getOrderHeaderIds).ToArrayAsync();
            var period = GetCurrentPeriod();

            var headers = (from item in db.OrderHeaders
                           where orderHeaderIds.Contains(item.OrderHeaderId)
                           select item).ToArray();

            foreach (var header in headers)
            {
                if (header.OrderHeaderId.ToString() == "0A71C3A0-90DC-E511-8115-2047477CE07A")
                {
                }
                var clientHerarch = new List<Client>();
                var parent = header.Client;
                var uplineCount = 0;
                while (parent != null )
                {
                    if (parent.Active && !parent.IgnoreRebate)
                    {
                        var summary = (from item in db.GroupSaleRebateCalculationSummaries
                                       where item.ClientId == parent.ClientId
                                       && item.Lvl == uplineCount
                                       && item.CalculationPeriodId == period.CalculationPeriodId
                                       select item).FirstOrDefault();
                        if (summary == null)
                        {

                            summary = new GroupSaleRebateCalculationSummary()
                            {
                                CalculationPeriodId = period.CalculationPeriodId,
                                ClientId = parent.ClientId,
                                ClientTypeId = parent.ClientTypeID,
                                GroupSaleRebateCalculationSummaryId = Guid.NewGuid(),
                                LevelAmount = 0,
                                Lvl = uplineCount,
                                ReletiveLvl = 0,
                                FromAccountId = Guid.Empty,
                                ToAccountId = Guid.Empty
                            };
                            db.GroupSaleRebateCalculationSummaries.Add(summary);
                            db.SaveChanges();

                        }

                        var entry = (from item in db.GroupSaleRebateCalculationRevenues
                                     where item.OrderHeaderId.Equals(header.OrderHeaderId)
                                     select item).FirstOrDefault();

                        if (entry == null)
                        {
                            entry = new GroupSaleRebateCalculationRevenue()
                            {
                                ClientId = header.ClientID,
                                ClientTypeId = header.Client.ClientTypeID,
                                GroupSaleRebateCalculationRevenueId = Guid.NewGuid(),
                                OrderHeaderId = header.OrderHeaderId,
                                SaleAmount = header.Total
                            };
                            db.GroupSaleRebateCalculationRevenues.Add(entry);
                            db.SaveChanges();

                        }

                        var link = (from item in db.GroupRebateCalculationRevenueSummaries
                                    where item.GroupSaleRebateCalculationRevenueId.Equals(entry.GroupSaleRebateCalculationRevenueId)
                                    && item.GroupSaleRebateCalculationRevenueSummaryId.Equals(summary.GroupSaleRebateCalculationSummaryId)
                                    select item).FirstOrDefault();
                        if (link == null)
                        {
                            link = new GroupRebateCalculationRevenueSummary()
                            {
                                GroupSaleRebateCalculationRevenueId = entry.GroupSaleRebateCalculationRevenueId,
                                GroupSaleRebateCalculationSummaryId = summary.GroupSaleRebateCalculationSummaryId
                            };
                            db.GroupRebateCalculationRevenueSummaries.Add(link);
                            summary.LevelAmount += entry.SaleAmount;
                            db.SaveChanges();
                        }

                    }
                    parent = db.Clients.Find(parent.ResellerID);
                    uplineCount++;

                }
            }


            //Check previous period and calc if needed
            period = period.PreviousPeriod;
            if (!period.PeriodOpen && !period.GroupProcessed)
            {
                var newCalcs = await db.GroupSaleRebateCalculationSummaries.Where(m => m.CalculationPeriodId.Equals(period.CalculationPeriodId)).ToArrayAsync();
                var clients = newCalcs.Select(m => m.ClientId).Distinct();
                foreach (var clientId in clients)
                {

                    var maxLevel = (from item in newCalcs
                                    where item.ClientId == clientId
                                    select item.Lvl).Max();
                    var clientCalcs = (from item in newCalcs
                                       where item.ClientId == clientId
                                       select item);
                    foreach (var calc in clientCalcs)
                    {
                        if (calc.ClientTypeId == 2)
                        {



                            //var calcLvl = maxLevel - calc.Lvl;
                            var rebateRanges = (from item in db.RebateRanges
                                                where item.RebateSalesTable.RebateClientType.ClientTypeID == calc.ClientTypeId
                                                select item).Include(m => m.RebateSalesTable).ToArray();

                            rebateRanges = (from item in db.RebateRanges
                                            where item.RangeStart <= calc.LevelAmount
                                            && item.RangeEnd >= calc.LevelAmount
                                            select item).ToArray();


                            rebateRanges = (from item in rebateRanges
                                            where
                                             item.RebateSalesTable != null
                                             && item.RebateSalesTable.LevelRange != null
                                             && item.RebateSalesTable.LevelRange.StartLevel <= calc.Lvl
                                                 && item.RebateSalesTable.LevelRange.EndLevel >= calc.Lvl
                                            select item).ToArray();

                            var rebateRange = rebateRanges.FirstOrDefault();

                            if (rebateRange != null)
                            {
                                calc.ReletiveLvl = maxLevel - calc.Lvl;
                                calc.LevelAmountPer = rebateRange.Rebate;
                                calc.FromAccountId = rebateRange.RebateSalesTable.FromAccountId;
                                calc.ToAccountId = rebateRange.RebateSalesTable.ToAccountId;
                                calc.CalculatedAmount = (calc.LevelAmount * rebateRange.Rebate / 100);

                                var orders = (from item in db.GroupRebateCalculationRevenueSummaries
                                              where item.GroupSaleRebateCalculationSummaryId.Equals(calc.GroupSaleRebateCalculationSummaryId)
                                              select item.GroupSaleRebateCalculationRevenue).ToArray();
                                foreach (var order in orders)
                                {
                                    var amount = (decimal)(order.SaleAmount * rebateRange.Rebate / 100);

                                    if (amount > 0)
                                    {
                                        var sql = string.Format(Generic.sqlCreateJournalSingle,
                                            calc.ClientId,
                                            calc.FromAccountId,
                                            calc.ToAccountId,
                                            amount.ToString().Replace(",", "."),
                                            DateTime.Now.ToString(Generic.LongDate),
                                            DateTime.Now.ToString(Generic.LongDate),
                                            order.OrderHeaderId,
                                            string.Format("{1} Level {0}", calc.Lvl, rebateRange.RebateSalesTable.Description),
                                            Guid.Empty
                                            );
                                        db.Database.ExecuteSqlCommand(sql);
                                    }
                                }
                            }
                        }
                    }
                    period.GroupProcessed = true;
                    db.SaveChanges();
                }
            }
            return result;
        }
            //var result = Json(string.Empty);
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //const string getOrderHeaderIds = "select OrderHeader.OrderHeaderId from OrderHeader left join GroupSaleRebateCalculationOrderHeader on OrderHeader.OrderHeaderId = GroupSaleRebateCalculationOrderHeader.OrderHeaderId where GroupSaleRebateCalculationOrderHeader.OrderHeaderId is null and OrderHeader.SaleSourceId = 0 and OrderStatusId > 1";
            //var orderHeaderIds = await db.Database.SqlQuery<Guid>(getOrderHeaderIds).ToArrayAsync();
            //var period = GetCurrentPeriod();
            
            //var headers = (from item in db.OrderHeaders
            //               where orderHeaderIds.Contains(item.OrderHeaderId)
            //               select item).ToArray();

            //foreach (var header in headers)
            //{
            //    var groupRebateCalc = db.GroupSaleRebateCalculation.FirstOrDefault(m=>m.ClientId.Equals(header.ClientID)
            //                                                                        && m.CalculationPeriodId.Equals(period.CalculationPeriodId));
            //    if (groupRebateCalc == null)
            //    {
            //        groupRebateCalc = new GroupSaleRebateCalculation()
            //        {
            //            Amount = header.Total,
            //            CalculationPeriodId = period.CalculationPeriodId,
            //            ClientId = header.ClientID,
            //            ClientTypeId = header.Client.ClientTypeID,
            //            Level = 0,
            //            ResellerID = header.Client.ResellerID
            //        };
            //        db.GroupSaleRebateCalculation.Add(groupRebateCalc);
            //        await db.SaveChangesAsync();
                    
            //    }
            //    else
            //    {
            //        groupRebateCalc.Amount += header.Total; 
            //    }
            //    var link = new GroupSaleRebateCalculationOrderHeader() {
            //        GroupSaleRebateCalculationId = groupRebateCalc.GroupSaleRebateCalculationId,
            //        OrderHeaderId = header.OrderHeaderId
            //    };
            //    db.GroupSaleRebateCalculationOrderHeader.Add(link);
            //    //groupRebateCalc.Amount = await db.GroupSaleRebateCalculationOrderHeader.Where(m=>m.GroupSaleRebateCalculationId.Equals(groupRebateCalc.GroupSaleRebateCalculationId)).Select(m=>m.OrderHeader.Total)
            //    await db.SaveChangesAsync();
            //}

            //period = period.PreviousPeriod;
            //if (!period.PeriodOpen && !period.GroupProcessed)
            //{
            //    var newCalcs = await db.GroupSaleRebateCalculation.Where(m => m.CalculationPeriodId.Equals(period.CalculationPeriodId)).ToArrayAsync();
            //    var clients = newCalcs.Select(m => m.ClientId).Distinct();
            //    foreach (var client in clients)
            //    {
            //        var parents = new int[] { client };
            //        var decendents = new int[] { };
            //        int lvl = 1;
            //        do
            //        {
            //            decendents = (from item in db.Clients
            //                          where item.ResellerID.HasValue
            //                          && parents.Contains(item.ResellerID.Value)
            //                          select item.ClientId).ToArray();

            //            if (decendents.Length == 0) break;

            //            var groupRebateCalc = newCalcs.FirstOrDefault(m => m.ClientId.Equals(client)
            //                                                                            && m.Level == lvl);
            //            if (groupRebateCalc == null)
            //            {
            //                groupRebateCalc = new GroupSaleRebateCalculation()
            //                {
            //                    CalculationPeriodId = period.CalculationPeriodId,
            //                    ClientId = client,
            //                    ClientTypeId = db.Clients.Find(client).ClientTypeID,
            //                    Level = lvl,
            //                    ResellerID = null,
            //                    Amount = (from item in db.GroupSaleRebateCalculation
            //                              where decendents.Contains(item.ClientId)
            //                              select (decimal?)item.Amount).Sum() ?? default(decimal)
            //                };

            //                db.GroupSaleRebateCalculation.Add(groupRebateCalc);
            //                await db.SaveChangesAsync();

            //            }

            //            var rebateClientTypes = (from item in db.RebateClientTypes
            //                                     where item.ClientTypeID == groupRebateCalc.ClientTypeId
            //                                     select item).Include(m => m.SalesTables).ToArray();

            //            var finMoved = false;
            //            foreach (var rebateSalesType in rebateClientTypes)
            //            {
            //                var salesTables = (from item in rebateSalesType.SalesTables
            //                                   where item.LevelRange != null
            //                                   && item.LevelRange.StartLevel <= lvl
            //                                   && item.LevelRange.EndLevel >= lvl
            //                                   select item).ToArray();
            //                foreach (var table in salesTables)
            //                {
            //                    var range = (from item in table.RebateRanges
            //                                 where item.RangeStart <= groupRebateCalc.Amount
            //                                 && item.RangeEnd >= groupRebateCalc.Amount
            //                                 select item).FirstOrDefault();
            //                    if (range != null)
            //                    {
            //                        var fromAccount = Generic.ClientAccount(table.FromAccount, client, db);
            //                        var toAccount = Generic.ClientAccount(table.ToAccount, client, db);
            //                        var amount = groupRebateCalc.Amount * (range.Rebate / 100);
            //                        if (amount > 0)
            //                        {
            //                            /////////////////////////////////////////////////////
            //                            DataTable dt = new DataTable();
            //                            dt.Columns.Add("FromAccountID");
            //                            dt.Columns.Add("ToAccountID");
            //                            dt.Columns.Add("Amount", typeof(decimal));
            //                            dt.Columns.Add("FromEffectiveDate");
            //                            dt.Columns.Add("ToEffectiveDate");
            //                            dt.Columns.Add("JournalDateDate");
            //                            dt.Columns.Add("MovementSource");
            //                            dt.Columns.Add("Comment");
            //                            dt.Columns.Add("UserID");

            //                            DataRow dr = dt.NewRow();
            //                            dr["FromAccountID"] = fromAccount;
            //                            dr["ToAccountID"] = toAccount;
            //                            dr["Amount"] = amount;
            //                            dr["FromEffectiveDate"] = DateTime.Now.ToString(Generic.LongDate);
            //                            dr["ToEffectiveDate"] = DateTime.Now.ToString(Generic.LongDate);
            //                            dr["JournalDateDate"] = DateTime.Now.ToString(Generic.LongDate);
            //                            dr["MovementSource"] = groupRebateCalc.GroupSaleRebateCalculationId;
            //                            dr["Comment"] = "Group sales rebate";
            //                            dr["UserID"] = Guid.Empty;
            //                            dt.Rows.Add(dr);

            //                            var userdetails = new System.Data.SqlClient.SqlParameter("JournalInput", SqlDbType.Structured);
            //                            userdetails.Value = dt;
            //                            userdetails.TypeName = "tpfinJournal_Base";


            //                            db.Database.ExecuteSqlCommand("EXEC spfinCreateJournal_Base @JournalInput", userdetails);

            //                            /////////////////////////////////////////////////////
            //                            finMoved = true;
            //                            break;
            //                        }
            //                    }
            //                    if (finMoved) break;
            //                }
            //                if (finMoved) break;
            //            }


            //            parents = decendents;
            //            lvl++;

            //        }
            //        while (decendents.Length > 0);
            //    }
            //    period.GroupProcessed = true;
            //    db.SaveChanges();
            //}





            ////foreach (var calc in newCalcs)
            //{
            //    var rebateClientTypes = (from item in db.RebateClientTypes
            //                             where item.ClientTypeID == calc.ClientTypeId
            //                             select item).Include(m => m.SalesTables).ToArray();
            //    foreach (var rebateClientType in rebateClientTypes)
            //    {
            //        if (rebateClientType.SalesTables != null)
            //        {
            //            foreach (var salesTable in rebateClientType.SalesTables)
            //            {
            //                if (salesTable.LevelRange != null)
            //                {
            //                    var lvlStart = 0;// salesTable.LevelRange.StartLevel;
            //                    var lvlEnd = salesTable.LevelRange.EndLevel;
            //                    var desendants = new int[] { };
            //                    for (int lvl = lvlStart; lvl <= lvlEnd; lvl++)
            //                    {
            //                        var groupRebateCalc = newCalcs.FirstOrDefault(m => m.ClientId.Equals(calc.ClientId)
            //                                                                        && m.Level == lvl);
            //                        if (groupRebateCalc == null)
            //                        {
            //                            groupRebateCalc = new GroupSaleRebateCalculation()
            //                            {
            //                                CalculationPeriodId = period.CalculationPeriodId,
            //                                ClientId = calc.ClientId,
            //                                ClientTypeId = calc.ClientTypeId,
            //                                Level = 0,
            //                                ResellerID = calc.ResellerID,
            //                                Amount = (from item in db.GroupSaleRebateCalculation
            //                                          where desendants.Contains(item.ClientId)

            //                            };

            //                            db.GroupSaleRebateCalculation.Add(groupRebateCalc);
            //                            await db.SaveChangesAsync();

            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        ////        return result;
        ////}
        [HttpGet]
        
        public async Task<JsonResult> CalculateSubscriptionRebates()
        {
            var result = Json(string.Empty);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            const string getOrderHeaderIds = "select OrderHeader.OrderHeaderId from OrderHeader left join SubscriptionSaleRebateCalculation on OrderHeader.OrderHeaderId = SubscriptionSaleRebateCalculation.OrderHeaderId where SubscriptionSaleRebateCalculation.OrderHeaderId is null and OrderHeader.SaleSourceId = 1 and OrderStatusId in (2,3,4)";
            var orderHeaderIds =  db.Database.SqlQuery<Guid>(getOrderHeaderIds).ToArray();
            var period = GetCurrentPeriod();
            var headers =  (from item in db.OrderHeaders
                                 where orderHeaderIds.Contains(item.OrderHeaderId)
                                 select item).ToArray();
            foreach (var header in headers)
            {
                if (header.OrderHeaderId.Equals("0A71C3A0-90DC-E511-8115-2047477CE07A"))
                { }
                var rebateClientTypes =  (from item in db.RebateClientTypes
                                               where item.ClientTypeID == header.Client.ClientTypeID
                                               select item).Include(m=>m.RebateLevelsTable.RebateLevelRows).ToArray();
                foreach (var rebateClientType in rebateClientTypes)
                {
                    if (rebateClientType.RebateLevelsTable != null)
                    {
                        if (rebateClientType.RebateLevelsTable.RebateLevelRows != null)
                        {
                            if (rebateClientType.RebateLevelsTable.RebateLevelRows.Count > 0)
                            {
                                var lvlStart = (from item in rebateClientType.RebateLevelsTable.RebateLevelRows
                                                select item.LevelRange.StartLevel).Min();
                                var lvlEnd = (from item in rebateClientType.RebateLevelsTable.RebateLevelRows
                                              select item.LevelRange.EndLevel).Max();

                                var clientList = new List<Client>();
                                clientList.Add(header.Client);
                                
                                for(int lvl = 1;  lvl <= lvlEnd; lvl++)
                                {
                                    var id = clientList[lvl - 1].ResellerID;
                                    var parent = await db.Clients.FirstOrDefaultAsync(m => m.ClientId == id);
                                    if (parent != null && !parent.IgnoreRebate)
                                    {
                                        clientList.Add(parent);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                for (int lvl = lvlStart; lvl <= lvlEnd && lvl<clientList.Count; lvl++)
                                {
                                    var relevantRow = (from item in rebateClientType.RebateLevelsTable.RebateLevelRows
                                                       where item.LevelRange.StartLevel <= lvl
                                                       && item.LevelRange.EndLevel >= lvl
                                                       select item).FirstOrDefault();
                                    if (relevantRow != null)
                                    {
                                        var firstProductCost = header.OrderLines.First().UnitCost;
                                        var calcFirst = (relevantRow.FirstProductRebate > 0) ? firstProductCost * (relevantRow.FirstProductRebate /100) : 0;
                                        var calcOther = (relevantRow.AdditionalProductsRebate > 0) ? (header.Total - firstProductCost) * (relevantRow.AdditionalProductsRebate / 100) : 0;

                                        var fromAccount = Generic.ClientAccount(relevantRow.RebateLevelsTable.FromAccount, clientList[lvl].ClientId,db);
                                        var toAccount = Generic.ClientAccount(relevantRow.RebateLevelsTable.ToAccount, clientList[lvl].ClientId, db);

                                        var newcalc = new SubscriptionSaleRebateCalculation()
                                        {
                                            CalculationPeriodId = period.CalculationPeriodId,
                                            ClientId = clientList[lvl].ClientId,
                                            ClientTypeId = clientList[lvl].ClientTypeID,
                                            OrderHeaderId = header.OrderHeaderId,
                                            FirstProductAmount=firstProductCost,
                                            OtherProductsAmount=header.Total-firstProductCost,
                                            FirstProductPer=relevantRow.FirstProductRebate,
                                            OtherProductsPer=relevantRow.AdditionalProductsRebate,
                                            RebateLevelsTableRowId = relevantRow.RebateLevelsTableRowId,
                                            FirstProductCalculatedAmount= calcFirst,
                                            OtherProductsCalculatedAmount=calcOther,
                                            FromAccount = fromAccount,
                                            ToAccount = toAccount,
                                            Level = lvl,
                                            ReletiveClientId = header.ClientID
                                        };
                                        db.SubscriptionSaleRebateCalculation.Add(newcalc);
                                        await db.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return result;
        }

        public CalculationPeriod GetCurrentPeriod()
        {
            var period = db.CalculationPeriod.FirstOrDefault(m => m.PeriodOpen);
            if (period == null || !(period.PeriodMonth == DateTime.Today.Month && period.PeriodYear == DateTime.Today.Year) || period.ForceClose)
            {
                if (period != null) period.PeriodOpen = false;
                var newId = Guid.NewGuid();
                var prevId = period == null ? newId : period.CalculationPeriodId;
                var newPeriod = new CalculationPeriod()
                {
                    CalculationPeriodId = newId,
                    PeriodMonth = DateTime.Now.Month,
                    PeriodOpen = true,
                    PeriodStartDate = DateTime.Now,
                    PeriodYear = DateTime.Today.Year,
                    PreviousPeriodId = prevId,
                    ForceClose = false,
                    LevelProcessed = false
                };
                db.CalculationPeriod.Add(newPeriod);
                db.SaveChanges();
                return newPeriod;
            }
            else
            {
                return period;
            }
        }

        // GET: RebateClientTypes/Create
        public ActionResult Create()
        {
           
            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeID", "ClientTypeName");
            return View();
        }

        // POST: RebateClientTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RebateClientTypeId,ClientTypeID,RebateLevelsTable")] RebateClientType rebateClientType)
        {
            if (ModelState.IsValid)
            {
                var existCheck = db.RebateClientTypes.Where(m => m.RebateClientTypeId == rebateClientType.RebateClientTypeId).FirstOrDefault();
                if (existCheck == null)
                {
                    var accountId = (from item in db.Accounts
                                     where item.Active
                                     select item.AccountId).First();
                    var newRebateLevelTabel = new RebateLevelsTable() {FromAccountId=accountId, ToAccountId=accountId };
                    db.RebateLevelsTables.Add(newRebateLevelTabel);
                    //await db.SaveChangesAsync();

                    rebateClientType.RebateLevelsTable = newRebateLevelTabel;
                    
                    db.RebateClientTypes.Add(rebateClientType);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Edit", new { id = rebateClientType.RebateClientTypeId });
                }
                else
                {
                    return RedirectToAction("Edit", new { id = existCheck.RebateClientTypeId });
                }
            }

            ViewBag.ClientTypeID = new SelectList(db.ClientTypes, "ClientTypeID", "ClientTypeName");
            return View(rebateClientType);
        }

        public ActionResult DeleteRebateRange(int id)
        {
            var rebateRange = db.RebateRanges.Include(mbox=>mbox.RebateSalesTable).First(m=>m.RebateRangeId==id);
            var rebateClientTypeId = rebateRange.RebateSalesTable.RebateClientTypeId;
            db.RebateRanges.Remove(rebateRange);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = rebateClientTypeId });
        }

        public ActionResult DeleteSalesTable(int id)
        {
            throw new NotImplementedException();
            var salesTable = db.SalesTables.Find(id);
            /*db.SalesTables*/
                return RedirectToAction("Edit", new { id = id });
        }

        //DeleteRebateLevelsTableRow
        public ActionResult DeleteRebateLevelsTableRow(int id)
        {
            var rebateLevelsTableRow = db.RebateLevelsTableRows.Include(m=>m.RebateLevelsTable).First(m => m.RebateLevelsTableRowId == id);
            var rebateClientTypeId = rebateLevelsTableRow.RebateLevelsTable.RebateClientTypeId;
            db.RebateLevelsTableRows.Remove(rebateLevelsTableRow);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = rebateClientTypeId });
        }

        public JsonResult ProductRebateAccount(bool from, Guid accountId, int rebateClientTypeId)
        {
            try
            {
                var rebateLevelsTable = db.RebateLevelsTables.Find(rebateClientTypeId);
                if (from)
                {
                    rebateLevelsTable.FromAccountId = accountId;
                }
                else
                {
                    rebateLevelsTable.ToAccountId = accountId;
                }
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult UpdateRebateLevelRow(int selection, string value, int rowId)
        {
            try
            {
                var row = db.RebateLevelsTableRows.Find(rowId);
                switch (selection)
                {
                    case 0: //range start
                        row.LevelRange.StartLevel = int.Parse(value);
                        break;
                    case 1: //range end
                        row.LevelRange.EndLevel = int.Parse(value);
                        break;
                    case 2: //first product
                        row.FirstProductRebate = decimal.Parse(value);
                        break;
                    case 3: //additional product
                        row.AdditionalProductsRebate = decimal.Parse(value);
                        break;
                    default:
                        throw new Exception("Unknown selection");
                }
                        db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // GET: RebateClientTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RebateClientType rebateClientType = await db.RebateClientTypes.Include(m=>m.ClientType).Include(m => m.RebateLevelsTable.RebateLevelRows).Include(m => m.SalesTables).FirstAsync(m=>m.RebateClientTypeId==id);
            if (rebateClientType == null)
            {
                return HttpNotFound();
            }

            ViewBag.FromAccountID = new SelectList(db.Accounts, "AccountID", "AccountName",rebateClientType.RebateLevelsTable.FromAccount?.AccountId);
            ViewBag.ToAccountID = new SelectList(db.Accounts, "AccountID", "AccountName",rebateClientType.RebateLevelsTable.ToAccount?.AccountId);
            ViewBag.ClientTypes = db.ClientTypes.Where(m => m.Active).ToArray();
            var tables = rebateClientType.SalesTables.ToArray();
            foreach (var tbl in tables)
            {
                var key1 = string.Format("FromAccountID{0}", tbl.RebateSalesTableId);
                var key2 = string.Format("ToAccountID{0}", tbl.RebateSalesTableId);
                                
                ViewData.Add(key1, new SelectList(db.Accounts, key1, "AccountName", tbl.FromAccountId));
                ViewData.Add(key2, new SelectList(db.Accounts, key2, "AccountName", tbl.ToAccountId));

            }
            return View(rebateClientType);
        }

        public JsonResult SpecifyClientType(int salesTableId, int clientTypeId, bool add)
        {
            try
            {
                var rebateSalesTableClientType = (from item in db.RebateSalesTableClientTypes
                                                  where item.ClientTypeId == clientTypeId
                                                  && item.RebateSalesTableId == salesTableId
                                                  select item).FirstOrDefault();
                if (rebateSalesTableClientType == null && add)
                {
                    rebateSalesTableClientType = new RebateSalesTableClientType()
                    {
                        Active = true,
                        ClientTypeId = clientTypeId,
                        RebateSalesTableId = salesTableId
                    };
                    db.RebateSalesTableClientTypes.Add(rebateSalesTableClientType);
                }
                else if (rebateSalesTableClientType != null && rebateSalesTableClientType.Active != add)
                {
                    rebateSalesTableClientType.Active = add;
                }
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult EditSalesTable(int selection, string value, int tableId)
        {
            try
            {
                var table = db.SalesTables.Find(tableId);
                switch (selection)
                {
                    case 0:
                        table.Description = value;
                        break;
                    case 1:
                        table.FromAccountId = Guid.Parse(value);
                        break;
                    case 2:
                        table.ToAccountId = Guid.Parse(value);
                        break;
                    case 3:
                        table.LevelRange.StartLevel = int.Parse(value);
                        break;
                    case 4:
                        table.LevelRange.EndLevel = int.Parse(value);
                        break;
                    case 5:
                        table.MinOwnPurchToQualify = decimal.Parse(value);
                        break;
                    default:
                        throw new Exception("Unknown selection");

                }
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public ActionResult AddSalesTableRange(int id)
        {

            var tabel = db.SalesTables.Find(id);
            tabel.RebateRanges.Add(new RebateRange() { });
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = tabel.RebateClientTypeId });

        }


        public JsonResult EditSalesTableRange(int selection, string value, int rangeId)
        {
            try
            {
                var range = db.RebateRanges.Find(rangeId);
                switch (selection)
                {
                    case 0:
                        range.RangeStart = decimal.Parse( value);
                        break;
                    case 1:
                        range.RangeEnd = decimal.Parse(value);
                        break;
                    case 2:
                        range.Rebate = decimal.Parse(value);
                        break;
                    default:
                        throw new Exception("Unknown selection");

                }
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public JsonResult EditSalesTableRow(int selection, string value, int rowId)
        {
            try
            {
                var row = db.RebateRanges.Find(rowId);
                var dval = decimal.Parse(value);
                switch (selection)
                {
                    case 0:
                        row.RangeStart = dval;
                        break;
                    case 1:
                        row.RangeEnd = dval;
                        break;
                    case 2:
                        row.Rebate = dval;
                        break;
                    default:
                        throw new Exception("Unknown selection");
                }
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> AddSalesTable(int id)
        {
            var rebateClientType = db.RebateClientTypes.Find(id);
            var accountId = await (from item in db.Accounts
                                   where item.Active
                                   select item.AccountId).FirstAsync();
            rebateClientType.SalesTables.Add(new SalesTable() {LevelRange=new LevelRange(), FromAccountId=accountId, ToAccountId=accountId });

            db.SaveChanges();
            return RedirectToAction("Edit", new { id = id });
        }

        public async Task<ActionResult> AddRange(int id )
        {
            var start = 0;
            var table = await db.RebateLevelsTables.FindAsync(id);
            if (table.RebateLevelRows == null) table.RebateLevelRows = new List<RebateLevelsTableRow>();

            if(table.RebateLevelRows.Count > 0)
            {
                start = (from item in table.RebateLevelRows
                         select item.LevelRange.EndLevel).Max() + 1;
            }

            var level = new LevelRange() {StartLevel = start, EndLevel= start  };
            table.RebateLevelRows.Add(new RebateLevelsTableRow() {LevelRange=level });
            db.SaveChanges();
            return RedirectToAction("Edit", new {id=table.RebateClientTypeId });
        }
        
        // POST: RebateClientTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( RebateClientType rebateClientType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rebateClientType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClientTypes = db.ClientTypes.Where(m=>m.Active).ToArray();
            return View(rebateClientType);
        }

        // GET: RebateClientTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RebateClientType rebateClientType = await db.RebateClientTypes.FindAsync(id);
            if (rebateClientType == null)
            {
                return HttpNotFound();
            }
            return View(rebateClientType);
        }

        // POST: RebateClientTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RebateClientType rebateClientType = await db.RebateClientTypes.FindAsync(id);
            db.RebateClientTypes.Remove(rebateClientType);
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
