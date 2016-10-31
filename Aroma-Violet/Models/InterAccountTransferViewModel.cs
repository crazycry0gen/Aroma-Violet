using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Aroma_Violet.Models
{
    public class InterAccountTransferViewModel
    {
        [Key]
        public Guid Key { get; set; }
        [DisplayName("Client")]
        public int ClientId { get; set; }
        [DisplayName("From Account")]
        public Guid FromAccountID { get; set; }
        [DisplayName("To Account")]
        public Guid ToAccountID { get; set; }
        [DisplayName("Original Amount")]
        [DataType(DataType.Currency)]
        public decimal OriginalAmount { get; set; }
        [DisplayName("Available Amount")]
        [DataType(DataType.Currency)]
        public decimal AvailableAmount { get; set; }
        [DisplayName("Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [DisplayName("Transfer Date")]
        public DateTime FromEffectiveDate { get; set; }
        [DisplayName("Effective Date")]
        public DateTime ToEffectiveDate { get; set; }
        [DisplayName("Source")]
        public string MovementSource { get; set; }
        [DisplayName("Comment")]
        public string Comment { get; set; }
        [DisplayName("User")]
        public Guid UserID { get; set; }
    }
}