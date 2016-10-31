using System;

namespace StratCorp.CorpSMS.Entities
{
    public class DeliveryReportEntity
    {
        public int DeliveryReportId { get; set; }

        public int MessageId { get; set; }

        public long ChangeId { get; set; }

        public long SentId { get; set; }

        public string Status { get; set; }

        public DateTime StatusDate { get; set; }
    }
}
