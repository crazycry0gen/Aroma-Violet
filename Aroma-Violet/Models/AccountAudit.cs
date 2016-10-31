using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public class AccountAuditList : List<AccountAudit>
    {
        const string fmt = "dd MMM yyyy HH:mm:ss";
        const string templateSQL = ";with f1 as (select JournalId, JournalDate, Effectivedate, Amount, Comment from finJournal f1 where AccountID='{0}' ) select JournalId, JournalDate, Effectivedate, Amount, Comment from f1 order by {1}";
        //const string templateSQL = ";with f1 as (select JournalId from finJournal f1 where AccountID='{0}' ) select JournalId, JournalDate, Effectivedate, Amount, comment from f1 order by {1}";
        public AccountAuditList(bool effectiveDate, DateTime startDateTime, DateTime endDateTime, Guid clientAccountId)
        {
            

            var sql = string.Format(templateSQL, clientAccountId.ToString(),
                effectiveDate ? "Effectivedate" : "JournalDate");
            using (var db = new AromaContext())
            {
                var items = db.Database.SqlQuery<AccountAuditQueryResult>(sql).ToList();
                for (var i = 0; i < items.Count;i++)
                {
                    var item = new AccountAudit()
                    {
                        JournalId = items[i].JournalId,
                        JournalDate =  items[i].JournalDate,
                        Effectivedate =   items[i].Effectivedate,
                        Amount = items[i].Amount,
                        Comment = items[i].Comment
                    };
                    item.Counter = i;
                    Add(item);
                    item.EffectiveTotal = this.Where(m=>m.Effectivedate<=item.JournalDate).Sum(m => m.Amount);
                    item.JournalTotal = this.Where(m => m.Effectivedate <= item.JournalDate).Sum(m => m.Amount);
                }

                var clientAccount = db.ClientAccounts.First(m => m.ClientAccountId.Equals(clientAccountId));
                this.AccountName = clientAccount.Account.AccountName;
                this.Client = clientAccount.Client;
            }
        }

        public string AccountName { get; set; }
        public Client Client { get; set; }
    }

    public class AccountAuditQueryResult
    {//JournalId, JournalDate, Effectivedate, Amount, Comment
        public Guid JournalId { get; set; }
        public DateTime JournalDate { get; set; }
        public DateTime Effectivedate { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }

    public class AccountAudit
    {
        public Guid JournalId { get; set; }
        public DateTime JournalDate { get; set; }
        public DateTime Effectivedate { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public int Counter { get; set; }
        public decimal JournalTotal { get; set; }
        public decimal EffectiveTotal { get; set; }
    }
}