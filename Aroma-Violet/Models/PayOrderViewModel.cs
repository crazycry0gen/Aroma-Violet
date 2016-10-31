using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public class PayOrderViewModel
    {
        public PayOrderViewModel()
        {
            this.AccountInfo = new List<PayOrderAccountInfo>();
        }

        public Guid OrderHeaderId { get; set; }
        [DataType(DataType.Currency)]
        public decimal OrderAmount { get; set; }
        [DataType(DataType.Currency)]
        public decimal OutstandingAmount { get; set; }
        public List<PayOrderAccountInfo> AccountInfo {get;set;}
    }

    public class PayOrderAccountInfo
    {
        public Guid AccountId { get; set; }
        public Guid ClientAccountId { get; set; }
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }
        [DataType(DataType.Currency)]
        public decimal TransferAmount { get; set; }
        [DataType(DataType.Currency)]
        public bool DestinationAccount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string AccountName { get; set; }
    }
}