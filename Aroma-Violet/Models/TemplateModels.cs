using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public class TemplateModel<t>
    {
        public TemplateModel(string template)
        {
            this.TemplateBody = template;
        }
        public static List<string> GetVariableList()
        {
            var type = typeof(t);
            var properties = type.GetProperties();
            var names = (from item in properties
                         select "{" + item.Name + "}").ToList();
            return names;
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return string.Empty;
        }

        private string TemplateBody;

        public string Generate()
        {
            var returnText = this.TemplateBody;
            var type = typeof(t);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    if (property.Name == "TemplateBody" || property.Name== "EventInfo") continue;
                    var keyNoFormat = "{" + property.Name + "}";
                    var startIndex = returnText.IndexOf(keyNoFormat);
                    if (startIndex > -1)
                    {
                        var propertyValue = property.GetValue(this, null);
                        if (propertyValue == null) propertyValue = GetDefault(property.GetType());
                        returnText = returnText.Replace(keyNoFormat, propertyValue.ToString());
                    }

                    do
                    {
                        var keyFormat = "{" + property.Name + ":";
                        startIndex = returnText.IndexOf(keyFormat);
                        if (startIndex > -1)
                        {
                            var formatStartIndex = startIndex + keyFormat.Length ;
                            var formatEndIndex = returnText.IndexOf("}", formatStartIndex) - formatStartIndex;
                            var format = returnText.Substring(formatStartIndex, formatEndIndex);
                            var fullKey = keyFormat + format + "}";
                            var propertyValue = property.GetValue(this, null);
                            if (propertyValue == null) propertyValue = GetDefault(property.PropertyType);

                            propertyValue = string.Format("{0:" + format + "}", Convert.ChangeType(propertyValue, property.PropertyType));
                            returnText = returnText.Replace(fullKey, propertyValue.ToString());
                        }
                    } while (startIndex > -1);
                }
                catch
                {
                    //on error try the next one
                }
            }
            return returnText;
        }
    }

    public class SystemSMSTemplateModel:TemplateModel<SystemSMSTemplateModel>
    {
        public SystemSMSTemplateModel(string template) : base(template)
        {

        }
        public SystemSMSTemplateModel(string template, Client client, AromaContext db) : base(template)
        {
            Populate(client, db);
        }

        public int ClientID { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Cell { get; private set; }
        public string ClientType { get; set; }
        public string Province { get; set; }
        public int ResellerID { get; set; }
        public string EventTrackingNumber { get; set; }
        public string Nickname { get; set; }
        public string ChildClientId { get; set; }
        public string ChildInitials { get; set; }
        public string ChildSurname { get; set; }
        public string VolumeDiscount { get; set; }
        public string MEMonth { get; set; }
        public string MEYear { get; set; }
        public string AccountName { get; set; }
        public string CommissionAmount { get; set; }

        public KeyValuePair<string, string>[] EventInfo
        {
            set {
                if (value != null)
                {
                    var tp = this.GetType();
                    foreach (var eI in value)
                    {
                        var par = tp.GetProperty(eI.Key);
                        if (par != null && par.CanWrite && par.CanRead)
                        {
                            par.SetValue(this, eI.Value);
                        }
                    }
                }
            }
        }

        public int Day { get; private set; }
        public string Month { get; private set; }
        public int Year { get; private set; }
        /*
        public string TotalPurchases { get; private set; }
        public string NextLevelVolumeDiscount { get; private set; }
        public string DifferenceTotalPurchasesNextQualify { get; private set; }
        */
        private static int contactTypeId;
        internal void Populate(Client client, AromaContext db)
        {
            
            
            if(contactTypeId==0)
                contactTypeId = db.ContactTypes.First(m => m.ContactTypeName == "Cell").ContactTypeId;
            this.ClientID = client.ClientId;
            this.Cell = db.Contacts.FirstOrDefault(m => m.ClientID == ClientID && m.ContactTypeID == contactTypeId && m.Active)?.ContactName;
            if (client.Title == null)
                client.Title = db.Titles.Find(client.TitleID);
            this.Title = client.Title.TitleName;
            this.Name = client.FullNames;
            this.Surname = client.ClientSurname;
            if (client.ClientType == null)
                client.ClientType = db.ClientTypes.Find(client.ClientTypeID);
            this.ClientType = client.ClientType.ClientTypeName;
            if (client.Province == null)
                client.Province = db.Provinces.Find(client.ProvinceID);
            this.Province = client.Province.ProvinceName;
            this.ResellerID = client.ResellerID.HasValue? client.ResellerID.Value:0;
            this.Nickname = client.NickName;

            var dt = DateTime.Today;
            this.Day = dt.Day;
            this.Month = dt.ToString("MMMM");
            this.Year = dt.Year;

            /*
            var volumeDiscountTable = (from item in db.RebateLevelsTables
                                        where item.RebateClientTypeId == client.ClientTypeID
                                        select item
                                        ).FirstOrDefault();
            var totPurch = (from item in db.OrderHeaders
                            where Generic.ValidOrderStatuses.Contains(item.OrderStatusId)
                            select item.Total).Sum();

            decimal volDiscount = 0;
            decimal nextLevelVolumeDicount = 0;
            decimal diffToQualify = 0;

            if (volumeDiscountTable != null)
            {
                var range = (from item in volumeDiscountTable.RebateLevelRows
                             where item.LevelRange.StartLevel <= totPurch
                             && item.LevelRange.EndLevel >= totPurch
                             select item.LevelRange).FirstOrDefault();
                if (range != null)
                {
                    var rangeVal = (from item in volumeDiscountTable.RebateLevelRows
                                    where item.LevelRange.StartLevel > range.EndLevel
                                    select item.LevelRange.StartLevel).Min();
                    var nextRange = (from item in volumeDiscountTable.RebateLevelRows
                                     where item.LevelRange.StartLevel >= rangeVal
                                     && item.LevelRange.EndLevel >= rangeVal
                                     select item.LevelRange).FirstOrDefault();
                }
            }

            this.TotalPurchases = totPurch.ToString(Generic.fmtCurrency);
            this.VolumeDiscount = volDiscount.ToString(Generic.fmtCurrency);
            this.NextLevelVolumeDiscount = nextLevelVolumeDicount.ToString(Generic.fmtCurrency);
            this.DifferenceTotalPurchasesNextQualify = diffToQualify.ToString(Generic.fmtCurrency);
            */
        }
    }



    public class SystemTicketTemplateModel:TemplateModel<SystemTicketTemplate>
    {
        public SystemTicketTemplateModel(string template) : base(template)
        {

        }
        public int TicketNumber { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        internal void Populate(Client client)
        {
            this.Title = client.Title.TitleName;
            this.Name = client.FullNames;
            this.Surname = client.ClientSurname;
        }
    }
}