using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;

namespace Aroma_Violet.Models
{
    public class AccountMovementViewModel
    {
        private const string spBalanceAtDate = "spBalanceAtDate '{0}','{1}'";
        private const string dtfmt = "dd MMM yyyy HH:mm:ss";
        public int? ClientId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<finJournal> Journals { get; set; }
        public List<AccountViewModel> Accounts { get; set; }

        

        public static AccountMovementViewModel LoadJournals(AromaContext db, DateTime fromDate, DateTime toDate, int? clientId)
        {
            var model = new AccountMovementViewModel();
            model.FromDate = fromDate;
            model.ToDate = toDate;
            model.ClientId = clientId;
            model.Journals = new List<finJournal>();

            var clientAccountIds = (from item in db.ClientAccounts
                                    where (clientId.HasValue && item.ClientID == clientId.Value) || !clientId.HasValue
                                    select item.ClientAccountId).ToArray();

            var finJournals = (from item in db.Journals
                               where item.EffectiveDate >= fromDate && item.EffectiveDate <= toDate
                               && clientAccountIds.Contains(item.AccountID)
                               && item.Index == 1
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
                              where RelevantAccounts.Count() == 0|| RelevantAccounts.Contains(item.AccountId)
                             select new AccountViewModel()
                             {
                                 AccountDescription = item.AccountName,
                                 AccountId = item.AccountId,
                                 IsClientAccount = false
                             }).ToList();

            model.Accounts.AddRange((from item in db.ClientAccounts.Where(m=>(!clientId.HasValue || (clientId.HasValue && m.ClientID == clientId.Value))).ToArray()
                                     where RelevantAccounts.Contains(item.ClientAccountId)
                                     select new AccountViewModel() {
                                        AccountDescription = db.Accounts.First(m=>m.AccountId.Equals(item.AccountId)).AccountName,
                                        AccountId = item.ClientAccountId,
                                        IsClientAccount = true
                                    }) );

            int index = 0;
            foreach (var acc in model.Accounts)
            {
                AccountBalance openBal = db.Database.SqlQuery<AccountBalance>(string.Format(spBalanceAtDate, fromDate.ToString(dtfmt), acc.AccountId)).First();
                AccountBalance closingBal = db.Database.SqlQuery<AccountBalance>(string.Format(spBalanceAtDate, toDate.ToString(dtfmt), acc.AccountId)).First(); 

                 /*
                 AccountBalance openBal = db.Database.SqlQuery<AccountBalance>(
                     "spBalanceAtDate", 
                     new SqlParameter("date", fromDate), 
                     new SqlParameter("accountId", acc.AccountId)
                     ).First();
                 AccountBalance closingBal = db.Database.SqlQuery<AccountBalance>("spBalanceAtDate", toDate, acc.AccountId).First();*/
                 acc.OpenBalance = openBal.Balance;
                acc.Balance = closingBal.Balance;
                acc.FutureBalance = closingBal.FutureBalance;
                acc.FutureBalanceDate = closingBal.FutureDate;
                acc.columnIndex = index;
                index++;
            }

            return model;
        }
    }

    public class AccountViewModel
    {
        public Guid AccountId { get; set; }
        public int columnIndex { get; set; }
        public string AccountDescription { get; set; }
        public bool IsClientAccount { get; set; }
        [DataType(DataType.Currency)]
        public decimal OpenBalance { get; set; }
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }
        [DataType(DataType.Currency)]
        public decimal FutureBalance { get; set; }
        public DateTime FutureBalanceDate { get; set; }
    }

    public class AccountBalance
    {
        public decimal Balance { get; set; }
        public decimal FutureBalance { get; set; }
        public DateTime FutureDate { get; set; }
    }
}