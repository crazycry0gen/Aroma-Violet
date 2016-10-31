using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public static class Generic
    {
        //public static Guid AccountTelemarked = Guid.Parse("042edc37-3561-e511-80c3-2047477ce07a");
        public const string LongDate = "dd MMM yyyy HH:mm:ss";
        public const string LongDateNoTime = "dd MMM yyyy";
        //public static Guid AccountTelemarked = Guid.Parse("0c09a21a-8aa7-e511-82b0-ac9e179a4bc9");
        public static int[] ValidOrderStatuses = new int[] {2,3,4 };
        public static Guid AccountControl = Guid.Parse("f7a863cf-3361-e511-80c3-2047477ce07a");
        internal const string sqlBalanceAtDate = "spBalanceAtDate '{0}','{1}'";
        internal const string sqlGlobalBalanceAtDate = "spGlobalBalanceAtDate '{0}','{1}'";
        internal const string sqlCreateJournalSingle = "spfinCreateJournalSingle {0},'{1}','{2}',{3}, '{4}','{5}','{6}','{7}','{8}'";
        internal const string sqlCreateJournalSingle_GS = "spfinCreateJournalSingle_GS {0},'{1}','{2}',{3}, '{4}','{5}','{6}','{7}','{8}','{9}'";
        internal const string fmtCurrency = "#,###,###,##0.00";
        internal const string clientDescriptionFormat = "{1} {2}";
        public static decimal VATPercent = 14;
        public const int DistributorSalesTypeId = 13;
        public const int OwnOrderSalesTypeId = 15;

        public static Guid SystemEventOwnSale = Guid.Parse("3702203c-6b1b-412c-b48c-6410c8461da1");
        public static Guid SystemEventCheckPostal = Guid.Parse("271CCAA5-F250-4B53-9D33-CA16497368D7");
        public const  int SupportTicketTypeIdOrder = 3;
        public const  int SupportTicketTypeIdSystem = 2;
        public const int SupportTicketTypeIdOther = 1;

        public static t CopyObject<t>(t fromObject, t toObject, params string[] dontCopy)
        {
            var type = typeof(t);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (!dontCopy.Contains(property.Name))
                {
                    var value = property.GetValue(fromObject);
                    var defaultValue = default(t);
                    if (value != null && !value.Equals(defaultValue))
                    {
                        property.SetValue(toObject, value);
                    }
                }
            }
            return toObject;
        }

        public static t GetSetting<t>(this AromaContext context, string key)
        {
            var val = default(t);

            var settingEntry = (from item in context.SystemSettings
                                where item.SettingKey == key
                                select item).FirstOrDefault();
            if (settingEntry == null)
            {
                settingEntry = new SystemSetting() { SettingKey = key, SettingValue = val.ToString() };
                context.SystemSettings.Add(settingEntry);
                context.SaveChanges();
                return val;
            }
            return context.GetSetting<t>(settingEntry.SettingId);
        }

        internal static string GetDescription(this AromaContext context, int? clientId)
        {
            if (clientId.HasValue)
            {
                var client = context.Clients.Find(clientId.Value);
                if (client != null)
                {
                    return string.Format(clientDescriptionFormat, client.Title.TitleName, client.FullNames, client.ClientSurname);
                }
                return "unknown";
            }
            return string.Empty;
        }

        internal static bool GetObligationProductMet(this Client client, AromaContext context)
        {
            if (client.HasState(enumSpecialState.IgnoreProductPurchase))
            {
                return true;
            }

            var products = context.ClientTypeProductObligations
                .Where(m => m.ClientTypeId == client.ClientTypeID)
                .Select(m => m.ProductId).ToArray();

            if (products.Length == 0) return true;
            var product = (from item in context.OrderLines
                           where item.OrderHeader.ClientID == client.ClientId
                           && item.Active
                           && ValidOrderStatuses.Contains(item.OrderHeader.OrderStatusId)
                           && products.Contains(item.ProductID)
                           select item.Product).FirstOrDefault();

            return product!=null;
        }

        public static int GetSpecialState(enumSpecialState state)
        {
            var q = (int)state;
            var r = Math.Pow(2, q);
            return (int)r;
        }

        public static bool HasState(this Client client, enumSpecialState state)
        {
            var stateNo = GetSpecialState(state);
            var stateClient = client.SpecialState;
            return (stateNo & stateClient) == stateNo;
        }

        internal static string GetContact(this AromaContext context, int clientId, enumContactType type)
        {
            var contact = context.Contacts.Where(m => m.Active
            && m.ClientID == clientId
            && m.ContactTypeID == (int)type).FirstOrDefault();
            if (contact != null) return contact.ContactName;
            return "unknown";
        }

        internal static decimal GetTotalPurchases(this AromaContext context, int clientId, int months)
        {
            decimal total = 0;
            var totalLines = context.GetLines(clientId, months);
            if (totalLines.Length > 0)
            {
                total = totalLines.Sum(m=>m.Total);
            }
            return total;
        }

        internal static OrderHeader[] GetLines(this AromaContext context, int clientId, int months)
        {
            var startDate = DateTime.Now.AddMonths((months-1) * -1);
            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            var totalLines = (from item in context.OrderHeaders
                              where Generic.ValidOrderStatuses.Contains(item.OrderStatusId)
                              && item.OrderDate >= startDate
                              && item.ClientID == clientId
                              select item).ToArray();
            return totalLines;
        }

        internal static decimal GetAveragePurchases(this AromaContext context, int clientId, int months)
        {
            var total = context.GetTotalPurchases(clientId, months);
            return total / months;
        }
        internal static decimal GetSubscriptionAmount(this AromaContext context, int clientId)
        {
            var subscription = (from item in context.ClientSubscriptions
                                where item.Active
                                && item.ClientID == clientId
                                select new {
                                    Qty = item.Quantity,
                                    Sub=context.Subscriptions.FirstOrDefault(
                                        m => 
                                        m.ProductID == item.ProductID
                                        && m.ClientTypeID == item.Client.ClientTypeID)
                                } ).ToArray();
            var fin = (from item in subscription
                       where item.Sub != null
                       select item.Qty * item.Sub.Price).Sum();
            return fin;

        }
        public static t GetSetting<t>(this AromaContext context, int keyId)
        {
            var val = default(t);
            var type = typeof(t);
            var settingEntry = (from item in context.SystemSettings
                                where item.SettingId == keyId
                                select item).First();
            try
            {
                val = (t)(object)settingEntry.SettingValue;
            }
            catch  { }
            return val;
        }

        internal static decimal Balance(Guid accountId, AromaContext db)
        {
            decimal balance = 0;
           
            decimal? res = (from item in db.Journals
                       where item.AccountID.Equals(accountId)
                       select (decimal?)item.Amount).Sum();

            if (res.HasValue) balance = res.Value;

            return balance;
        }

        public static void Log<t>(this AromaContext context, enumLGActivity activity, t Id, string note)
        {
            if (context.GetSetting<bool>(1))
            {
                var newEntry = new LGActivityLog() { ActivityId = (int)activity, Note = note, iDate = DateTime.Now };
                if (typeof(t) == typeof(int)) newEntry.IntId = (int)(object)Id;
                if (typeof(t) == typeof(Guid)) newEntry.GuidId = (Guid)(object)Id;
                context.ActivityLogs.Add(newEntry);
                context.SaveChanges();
            }
        }

        public static decimal Balance(this OrderHeader header, AromaContext db, Guid account)
        {
            decimal balance = 0;
            var journals = (from item in db.Journals
                            where item.AccountID.Equals(account)
                            && item.MovementSource.Equals(header.OrderHeaderId)
                            select item);
            if (journals.Count() > 0)
            {
                balance = journals.Sum(m=>m.Amount);
            }
            return balance;
        }

        public static finJournal Reverse(this finJournal jrn, Guid userId, int index)
        {
            var newJrn = new finJournal()
            {
                AccountID = jrn.AccountID,
                Active = jrn.Active,
                Amount = jrn.Amount*-1,
                Comment="Journal reversal",
                CorrespondingJournalId = Guid.Empty,
                EffectiveDate=DateTime.Now,
                Index=index,
                JournalDate=DateTime.Now,
                JournalId = Guid.NewGuid(),
                MovementSource=jrn.JournalId,
                UserID=userId
            };
            return newJrn;
        }

        public enum enumLGActivity:int
        {
            CreateClient=1,
            UpdateClient=2
        }

        public enum enumSaleSource : int
        {
            Other =1,
            Subscription=2
        }

        public enum enumContactType : int
        {
            TelHome=1,
            TelWork=2,
            Cell=3,
            FaxHome=4,
            FaxWork=5,
            EMail
        }

        internal static Guid ClientAccount(finAccount account,int clientId, AromaContext db)
        {
            if (!account.IsSystemAccount)
            {
                var clientAccount = db.ClientAccounts.FirstOrDefault(m => m.AccountId.Equals(account.AccountId) && m.ClientID == clientId);
                if (clientAccount == null)
                {
                    clientAccount = new finClientAccount() {
                        AccountId = account.AccountId,
                        Active = true,
                        ClientID=clientId
                    };
                    db.ClientAccounts.Add(clientAccount);
                    db.SaveChanges();
                }
                return clientAccount.ClientAccountId;
            }
            else
            {
                return account.AccountId;
            }
        }

        internal static int? GetMyClientId(AromaContext db, Guid userId)
        {
            var userClient = db.UserClients.FirstOrDefault(m => m.UserId.Equals(userId));
            if (userClient != null)
            {
                return userClient.ClientId;
            }
            return null;
        }
    }


}