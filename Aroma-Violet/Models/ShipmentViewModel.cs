using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public class ShipmentViewModel
    {
        public ShipmentViewModel()
        {
            this.Shipments = new List<Shipment>();
        }
        public int ShippingTypeId { get; set; }
        public List<Shipment> Shipments { get; set; }

        public int Skip { get; set; }

        public void Add(PickingListHeader header, AromaContext db)
        {
            foreach (var detail in header.PickingListDetail)
            {
                var shipment = (from item in this.Shipments
                                where item.groupId.Equals(detail.GroupId)
                                select item).FirstOrDefault();
                if (shipment == null)
                {
                    if (detail.Address == null) detail.Address = detail.Client.DeliveryAddress;
                    var cell = db.GetContact(detail.ClientID, Generic.enumContactType.Cell);
                    //var contact = detail.Client.Contact?.Where(m => m.Active && m.ContactTypeID == 3).FirstOrDefault();
                    shipment = new Shipment()
                    {
                        ClientDescription = string.Format("{0} {1} ({2})", detail.Client.FullNames, detail.Client.ClientSurname, detail.ClientID),
                        PhoneNumber = cell,// contact==null?string.Empty:contact.ContactName,
                        Address = detail.Address,
                        ClientId = detail.ClientID,
                        groupId = detail.GroupId,
                        ItemCount = 1
                    };
                    this.Shipments.Add(shipment);
                }
                else
                {
                    shipment.ItemCount++;
                }
            }
        }
    }

    public class Shipment
    {
        public int ClientId { get; set; }
        public string ClientDescription {get;set;}
        public string PhoneNumber { get; set; }
        public Guid groupId { get; set; }
        public int ItemCount { get; set; }
        public Address Address {get;set;}
        public string TrackingNumber { get; set; }
    }
}