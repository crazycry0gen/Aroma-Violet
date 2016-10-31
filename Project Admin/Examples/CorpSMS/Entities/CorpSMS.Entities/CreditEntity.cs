using System;

namespace StratCorp.CorpSMS.Entities
{
    public class CreditEntity
    {
        public int CreditId { get; set; }

        public int AccountId { get; set; }

        public int Amount { get; set; }

        public DateTime CreditDate { get; set; }
    }
}
