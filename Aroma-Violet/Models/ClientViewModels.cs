using System.Collections.Generic;
using System.Linq;

namespace Aroma_Violet.Models
{
    
    public class ClientSubscriptionViewModel
    {
        public List<ClientSubscription> ClientSubscriptions { get; set; }
        public int ProductCount { get; internal set; }
        public ClientSubscription Subscription { get; set; }
    }
    
    public class ClientContactView
    {
        public string ClientContactTypeName { get; set; }
        public string ClientContactValue { get; set; }
        public int ClientContactTypeId { get; set; } 
    }

    public class ClientViewModel : Client
    {
        private const int _lineCount = 4;

        public ClientViewModel()
        {
            SetUpObject();
        }

        private void SetUpObject()
        {
            this.PostalAddressLines = new List<string>();
            this.DeliveryAddressLines = new List<string>();
            for (int i = 0; i < _lineCount; i++)
            {
                this.PostalAddressLines.Add(GetAddressLine(PostalAddress, i));
                this.DeliveryAddressLines.Add(GetAddressLine(DeliveryAddress, i));
            }

        }

        public List<string> PostalAddressLines { get; set; }

        public List<string> DeliveryAddressLines { get; set; }

        public string TelWork { get; set; }
        public string Cell { get; set; }
        public string TelHome { get; set; }
        public string Email { get; set; }

        public int? ResellerID { get; set; }
        public ClientContactView[] Contacts { get; internal set; }

        private string GetAddressLine(Address add, int index)
        {
            if (add?.Lines?.Count > index)
            {
                var lines = add.Lines as AddressLine[];
                return lines[index].AddressLineText;
            }
            return string.Empty;
        }

        public Client GetBaseClient()
        {
             var myBase = this as Client;

            if (myBase != null)
            {
                if (myBase.PostalAddress != null)
                {
                    int index = 0;
                    myBase.PostalAddress.Lines = (from string item in PostalAddressLines
                                                  where item?.Length > 0
                                                  select new AddressLine() { Active = true, AddressLineText = item, Order = index++ }).ToList();
                }
                if (myBase.DeliveryAddress != null)
                {
                    int index = 0;
                    myBase.DeliveryAddress.Lines = (from string item in DeliveryAddressLines
                                                    where item?.Length > 0
                                                    select new AddressLine() { Active = true, AddressLineText = item, Order = index++ }).ToList();
                }
            }
            return myBase;
        }

    }
}