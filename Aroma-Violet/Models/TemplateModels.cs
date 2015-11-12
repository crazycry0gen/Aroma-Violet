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
                    if (property.Name == "TemplateBody") continue;
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
        public SystemSMSTemplateModel(string template, Client client) : base(template)
        {
            Populate(client);
        }

        public int ClientID { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Cell { get; private set; }
        public string ClientType { get; set; }
        public string Province { get; set; }

        private static int contactTypeId;
        internal void Populate(Client client)
        {
            var db = new AromaContext();
            if(contactTypeId==0)
                contactTypeId = db.ContactTypes.First(m => m.ContactTypeName == "Cell").ContactTypeId;
            this.ClientID = client.ClientId;
            this.Cell = db.Contacts.FirstOrDefault(m => m.ClientID == ClientID && m.ContactTypeID == contactTypeId)?.ContactName;
            this.Title = client.Title.TitleName;
            this.Name = client.FullNames;
            this.Surname = client.ClientSurname;
            this.ClientType = client.ClientType.ClientTypeName;
            this.Province = client.Province.ProvinceName;
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